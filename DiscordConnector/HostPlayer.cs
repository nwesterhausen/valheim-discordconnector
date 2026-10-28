using HarmonyLib;

using UnityEngine;

namespace DiscordConnector;

/// <summary>
///     On a non-dedicated server (a game "opened to LAN" or hosted from the game client) the host plays on the
///     server itself and has no <see cref="ZNetPeer" />, so <c>ZNet.GetPeer</c> returns null for them and their
///     joins, deaths and shouts were never counted. This builds a stand-in peer for the host so the normal
///     <see cref="Handlers" /> work unchanged.
/// </summary>
internal static class HostPlayer
{
    private static ZNetPeer? s_peer;

    /// <summary>True on a server that has a local player (not a dedicated server).</summary>
    internal static bool IsListenServer =>
        ZNet.instance != null && ZNet.instance.IsServer() && !ZNet.instance.IsDedicated();

    /// <summary>The host as a peer, or null when there is no host player.</summary>
    internal static ZNetPeer? Peer()
    {
        if (!IsListenServer || Player.m_localPlayer == null)
        {
            return null;
        }

        string id = UserInfo.GetLocalUser().UserId.ToString();
        if (s_peer == null || s_peer.m_socket.GetHostName() != id)
        {
            s_peer = new ZNetPeer(new LocalSocket(id), true) { m_uid = ZNet.GetUID() };
        }

        s_peer.m_playerName = Player.m_localPlayer.GetPlayerName();
        s_peer.m_characterID = Player.m_localPlayer.GetZDOID();
        s_peer.m_refPos = Player.m_localPlayer.transform.position;
        return s_peer;
    }

    /// <summary>The host's character spawned (first spawn counts as a join; later ones are respawns).</summary>
    [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
    internal static class Spawned
    {
        private static void Postfix(Player __instance)
        {
            if (__instance != Player.m_localPlayer || !IsListenServer)
            {
                return;
            }

            ZNetPeer? peer = Peer();
            if (peer != null)
            {
                Handlers.Join(peer);
            }
        }
    }

    /// <summary>The host logs out to the main menu (also runs when they quit the game from the menu).</summary>
    [HarmonyPatch(typeof(Game), nameof(Game.Logout))]
    internal static class Logout
    {
        private static void Prefix()
        {
            if (IsListenServer)
            {
                ZNetPeer? peer = Peer();
                if (peer != null)
                {
                    Handlers.Leave(peer);
                }
            }
        }
    }

    /// <summary>A socket that only knows the host's platform id, which is all DiscordConnector reads.</summary>
    private class LocalSocket : ISocket
    {
        private readonly string _hostName;

        public LocalSocket(string hostName)
        {
            _hostName = hostName;
        }

        public bool IsConnected() => true;
        public void Send(ZPackage pkg) { }
        public ZPackage? Recv() => null;
        public int GetSendQueueSize() => 0;
        public int GetCurrentSendRate() => 0;
        public bool IsHost() => true;
        public void Dispose() { }
        public bool GotNewData() => false;
        public void Close() { }
        public string GetEndPointString() => "localhost";

        public void GetAndResetStats(out int totalSent, out int totalRecv)
        {
            totalSent = 0;
            totalRecv = 0;
        }

        public void GetConnectionQuality(out float localQuality, out float remoteQuality, out int ping,
            out float outByteSec, out float inByteSec)
        {
            localQuality = remoteQuality = outByteSec = inByteSec = 0;
            ping = 0;
        }

        public ISocket? Accept() => null;
        public int GetHostPort() => 0;
        public bool Flush() => true;
        public string GetHostName() => _hostName;
        public void VersionMatch() { }
    }
}
