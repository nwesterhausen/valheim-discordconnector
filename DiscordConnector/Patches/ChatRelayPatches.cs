using System;
using System.Collections.Generic;

using HarmonyLib;

using UnityEngine;

namespace DiscordConnector.Patches;

/// <summary>
///     Sees players' shouts and pings as they pass through the server.
/// </summary>
/// <remarks>
///     <para>
///         Since Valheim 0.219 a shout is no longer broadcast. The client sends one copy of the "ChatMessage" RPC to
///         each other player (after a platform permission check), and the server only relays those copies, so
///         <c>Chat.OnNewChatMessage</c> never runs on a dedicated server and shouts stopped reaching Discord
///         (issues #338, #319). Pings are still broadcast and are handled by the server directly.
///     </para>
///     <para>
///         This patch looks at every ChatMessage RPC the server handles or relays, and reports each shout once.
///         A shout from a player who is alone on the server is never sent to the server at all; the optional
///         client plugin forwards those (see <see cref="RPC.Server" />), and both paths go through
///         <see cref="ChatRelay.Report" /> so nothing is announced twice.
///     </para>
/// </remarks>
internal static class ChatRelayPatches
{
    internal static readonly int ChatMessageHash = "ChatMessage".GetStableHashCode();

    /// <summary>Messages addressed to the server itself, or broadcast to everybody (pings).</summary>
    [HarmonyPatch(typeof(ZRoutedRpc), nameof(ZRoutedRpc.HandleRoutedRPC))]
    internal static class Handle
    {
        private static void Prefix(ZRoutedRpc.RoutedRPCData data)
        {
            ChatRelay.Observe(data);
        }
    }

    /// <summary>Messages passing through the server on their way to another player (shouts).</summary>
    [HarmonyPatch(typeof(ZRoutedRpc), "RouteRPC")]
    internal static class Route
    {
        private static void Prefix(ZRoutedRpc.RoutedRPCData rpcData)
        {
            // Harmony matches parameters by name: this must be "rpcData", as in the game.
            ChatRelay.Observe(rpcData);
        }
    }
}

internal static class ChatRelay
{
    /// <summary>Copies of one shout arrive within this window (they're sent in the same moment).</summary>
    private static readonly TimeSpan s_copyWindow = TimeSpan.FromSeconds(3);

    /// <summary>The shout every client sends on first spawn is treated as an arrival, not a shout.</summary>
    private static readonly TimeSpan s_arrivalWindow = TimeSpan.FromSeconds(15);

    /// <summary>Target used for messages forwarded by the client plugin (not a real peer id).</summary>
    internal const long ClientPluginTarget = -1;

    private static readonly Dictionary<string, Seen> s_recent = new();
    private static readonly Dictionary<long, DateTime> s_awaitingArrival = new();

    private class Seen
    {
        public DateTime First;
        public readonly HashSet<long> Targets = new();
        public readonly HashSet<long> MessageIds = new();
    }

    /// <summary>Called when a player joins; their next shout within a few seconds is the automatic arrival shout.</summary>
    internal static void ExpectArrival(ZNetPeer peer)
    {
        s_awaitingArrival[peer.m_uid] = DateTime.UtcNow;
    }

    internal static void Observe(ZRoutedRpc.RoutedRPCData data)
    {
        if (data == null || data.m_methodHash != ChatRelayPatches.ChatMessageHash || ZNet.instance == null ||
            !ZNet.instance.IsServer())
        {
            return;
        }

        try
        {
            // Parameters of Chat.RPC_ChatMessage: Vector3 position, int type, UserInfo sender, string text.
            // Read from a copy so the game's own handler still sees the package from the start.
            ZPackage pkg = new(data.m_parameters.GetArray());
            Vector3 pos = pkg.ReadVector3();
            Talker.Type type = (Talker.Type)pkg.ReadInt();
            pkg.ReadString(); // UserInfo.Name
            pkg.ReadString(); // UserInfo.UserId
            string text = pkg.ReadString();

            ZNetPeer? peer = ZNet.instance.GetPeer(data.m_senderPeerID);
            if (peer == null && data.m_senderPeerID == ZNet.GetUID())
            {
                // The host of a non-dedicated server. On a dedicated server this stays null: messages "from" the
                // server come from other mods' server chat, which Chat.OnNewChatMessage handles.
                peer = HostPlayer.Peer();
            }

            if (peer == null)
            {
                return;
            }

            Report(peer, type, pos, text, data.m_targetPeerID, data.m_msgID);
        }
        catch (Exception e)
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Could not read chat message: {e.Message}");
        }
    }

    /// <summary>
    ///     Report a chat message from <paramref name="peer" />, unless it's another copy of one already reported.
    /// </summary>
    /// <param name="target">Who this copy was addressed to. Each player gets their own copy of a shout.</param>
    /// <param name="messageId">Routed RPC message id, so the same copy seen twice is ignored.</param>
    internal static void Report(ZNetPeer peer, Talker.Type type, Vector3 pos, string text, long target,
        long messageId)
    {
        if (type != Talker.Type.Shout && type != Talker.Type.Ping)
        {
            return;
        }

        if (!IsNewMessage(peer.m_uid, type, text, target, messageId))
        {
            return;
        }

        string name = peer.m_playerName ?? "";
        if (DiscordConnectorPlugin.StaticConfig.MutedPlayers.IndexOf(name) >= 0 ||
            DiscordConnectorPlugin.StaticConfig.MutedPlayersRegex.IsMatch(name))
        {
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Ignored {type} from muted player {name}.");
            return;
        }

        if (type == Talker.Type.Ping)
        {
            Handlers.Ping(peer, pos);
            return;
        }

        if (IsArrivalShout(peer, text))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Ignored arrival shout from {name}: '{text}'");
            return;
        }

        Handlers.Shout(peer, pos, text);
    }

    private static bool IsNewMessage(long sender, Talker.Type type, string text, long target, long messageId)
    {
        DateTime now = DateTime.UtcNow;
        if (s_recent.Count > 200)
        {
            List<string> old = new();
            foreach (KeyValuePair<string, Seen> kv in s_recent)
            {
                if (now - kv.Value.First > s_copyWindow) old.Add(kv.Key);
            }

            foreach (string k in old) s_recent.Remove(k);
        }

        string key = $"{sender}|{(int)type}|{text}";
        if (s_recent.TryGetValue(key, out Seen seen) && now - seen.First <= s_copyWindow)
        {
            // The same copy can pass both the handle and the route hooks (broadcasts do).
            if (messageId != 0 && seen.MessageIds.Contains(messageId))
            {
                return false;
            }

            // A copy addressed to a player we haven't seen yet belongs to the same shout. A second copy to the
            // same player means the player said the same thing again.
            if (!seen.Targets.Contains(target) && target != 0)
            {
                seen.Targets.Add(target);
                seen.MessageIds.Add(messageId);
                return false;
            }
        }

        Seen fresh = new() { First = now };
        fresh.Targets.Add(target);
        fresh.MessageIds.Add(messageId);
        s_recent[key] = fresh;
        return true;
    }

    private static bool IsArrivalShout(ZNetPeer peer, string text)
    {
        if (text == Patches.ChatPatches.ArrivalShout)
        {
            s_awaitingArrival.Remove(peer.m_uid);
            return true;
        }

        // The arrival shout is translated by the player's game, so other languages can't be matched by text.
        // It's the first thing a player shouts, right after spawning.
        if (s_awaitingArrival.TryGetValue(peer.m_uid, out DateTime joined))
        {
            s_awaitingArrival.Remove(peer.m_uid);
            return DateTime.UtcNow - joined <= s_arrivalWindow;
        }

        return false;
    }
}
