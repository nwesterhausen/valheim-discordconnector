using UnityEngine;

namespace DiscordConnector.Tests;

/// <summary>
///     A socket that never sends or receives anything. Only GetHostName matters to DiscordConnector,
///     since that's the player ID it records (e.g. Steam_7656...).
/// </summary>
internal class FakeSocket : ISocket
{
    private readonly string _hostName;

    public FakeSocket(string hostName)
    {
        _hostName = hostName;
    }

    public bool IsConnected() => true;
    public void Send(ZPackage pkg) { }
    public ZPackage Recv() => null;
    public int GetSendQueueSize() => 0;
    public int GetCurrentSendRate() => 0;
    public bool IsHost() => false;
    public void Dispose() { }
    public bool GotNewData() => false;
    public void Close() { }
    public string GetEndPointString() => "127.0.0.1";

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

    public ISocket Accept() => null;
    public int GetHostPort() => 0;
    public bool Flush() => true;
    public string GetHostName() => _hostName;
    public void VersionMatch() { }
}

internal static class FakePeer
{
    private static long s_nextUid = 900000;

    /// <summary>
    ///     Builds a server-side peer for a pretend player.
    ///     <paramref name="spawned" /> sets a non-zero character ID, which is what the server sees once the player's
    ///     character exists in the world.
    /// </summary>
    public static ZNetPeer Create(string playerName, string hostName, bool spawned = true)
    {
        ZNetPeer peer = new(new FakeSocket(hostName), true)
        {
            m_playerName = playerName,
            m_uid = s_nextUid++,
            m_refPos = new Vector3(100, 30, -200)
        };
        peer.m_characterID = spawned ? new ZDOID(peer.m_uid, (uint)s_nextUid++) : ZDOID.None;
        return peer;
    }
}
