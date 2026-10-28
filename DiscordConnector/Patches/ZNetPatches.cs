using System.Reflection;

using HarmonyLib;

namespace DiscordConnector.Patches;

internal class ZNetPatches
{
    /// <summary>
    ///     Runs once the server's world is loaded.
    ///     Valheim 1.0 routes world loading through ZNet.ServerLoadWorld, which only calls ZNet.LoadWorld for worlds
    ///     in the new chunked save format. New worlds and worlds saved before 1.0 go through LoadOldWorld instead,
    ///     so patching LoadWorld alone missed them. Older game versions don't have ServerLoadWorld, so fall back.
    /// </summary>
    [HarmonyPatch]
    internal class LoadWorld
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(ZNet), "ServerLoadWorld") ??
                   AccessTools.Method(typeof(ZNet), nameof(ZNet.LoadWorld));
        }

        private static void Postfix()
        {
            if (DiscordConnectorPlugin.StaticConfig.LoadedMessageEnabled)
            {
                DiscordApi.SendMessage(
                    Webhook.Event.ServerStart,
                    MessageTransformer.FormatServerMessage(DiscordConnectorPlugin.StaticConfig.LoadedMessage)
                );
            }

            if (DiscordConnectorPlugin.IsHeadless())
            {
                DiscordConnectorPlugin.StaticEventWatcher.Activate();
            }
        }
    }

    [HarmonyPatch(typeof(ZNet), nameof(ZNet.SaveWorld))]
    internal class SaveWorld
    {
        private static void Postfix(ref bool sync)
        {
            if (DiscordConnectorPlugin.StaticConfig.WorldSaveMessageEnabled)
            {
                DiscordApi.SendMessage(
                    Webhook.Event.ServerSave,
                    MessageTransformer.FormatServerMessage(DiscordConnectorPlugin.StaticConfig.SaveMessage)
                );
            }
        }
    }

    /// <summary>
    ///     A client sends its character ID when its character spawns: on first join, and again after every respawn
    ///     (death, or anything else that respawns the player). Only the first one counts, as a join.
    ///     Deaths are detected separately, see <see cref="DeathRpc" />.
    /// </summary>
    [HarmonyPatch(typeof(ZNet), nameof(ZNet.RPC_CharacterID))]
    internal class RPC_CharacterID
    {
        private static void Postfix(ZRpc rpc, ZDOID characterID)
        {
            ZNetPeer peer = ZNet.instance.GetPeer(rpc);
            if (peer == null || characterID.IsNone())
            {
                return;
            }

            Handlers.Join(peer);
        }
    }

    /// <summary>
    ///     Runs for every way a peer leaves: a clean quit (RPC_Disconnect calls this), a timeout, a crash, or a kick.
    ///     Previously only RPC_Disconnect was patched, so players who crashed or timed out stayed "joined" and their
    ///     next spawn was mistaken for a death.
    /// </summary>
    [HarmonyPatch(typeof(ZNet), nameof(ZNet.Disconnect))]
    internal class Disconnect
    {
        private static void Prefix(ZNetPeer peer)
        {
            if (peer != null && peer.m_uid != 0 && ZNet.instance != null && ZNet.instance.IsServer())
            {
                Handlers.Leave(peer);
            }
        }
    }

    /// <summary>
    ///     When a player dies, their client broadcasts the "OnDeath" RPC on their own character (Player.OnDeath), and
    ///     every broadcast passes through the server. This is a direct signal, unlike guessing from respawns.
    /// </summary>
    [HarmonyPatch(typeof(ZRoutedRpc), nameof(ZRoutedRpc.HandleRoutedRPC))]
    internal class DeathRpc
    {
        internal static readonly int OnDeathHash = "OnDeath".GetStableHashCode();

        private static void Prefix(ZRoutedRpc.RoutedRPCData data)
        {
            if (data == null || data.m_methodHash != OnDeathHash || ZNet.instance == null || !ZNet.instance.IsServer())
            {
                return;
            }

            ZNetPeer peer = ZNet.instance.GetPeer(data.m_senderPeerID);
            // Only count it when the RPC targets the sender's own character.
            if (peer == null || peer.m_characterID.IsNone() || data.m_targetZDO != peer.m_characterID)
            {
                return;
            }

            Handlers.Death(peer);
        }
    }
}
