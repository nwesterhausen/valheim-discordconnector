using System;
using System.Collections;
using System.Collections.Generic;

using DiscordConnector.Config;
using DiscordConnector.Leaderboards;
using DiscordConnector.Records;

using UnityEngine;

namespace DiscordConnector.Tests;

/// <summary>
///     The test cases. Each test makes its own pretend players (names end with a per-run suffix), so tests
///     don't depend on each other or on leftover data.
///     Leaderboard tests rely on the leaderboard config written by tests/run-tests.ps1.
/// </summary>
internal static class Suite
{
    private static readonly string Run = DateTime.Now.ToString("HHmmss");

    private static ZNetPeer Player(string name)
    {
        return FakePeer.Create(name + Run, "Steam_" + Math.Abs((name + Run).GetHashCode()));
    }

    public static IEnumerable<TestCase> All()
    {
        yield return new TestCase { Name = "server started message was sent", Body = ServerStartedSent };
        yield return new TestCase { Name = "event watcher is active (raids)", Body = EventWatcherActive };
        yield return new TestCase { Name = "join is recorded and announced", Body = JoinIsRecordedAndAnnounced };
        yield return new TestCase { Name = "OnDeath RPC counts as a death", Body = DeathRpcCountsAsDeath };
        yield return new TestCase { Name = "respawn without dying is not a death", Body = RespawnIsNotDeath };
        yield return new TestCase { Name = "OnDeath for another object is ignored", Body = OnDeathForOtherObjectIgnored };
        yield return new TestCase { Name = "first death uses first-death message", Body = FirstDeathMessage };
        yield return new TestCase { Name = "leave is recorded and announced", Body = LeaveIsRecorded };
        yield return new TestCase { Name = "rejoin after crash is not a death", Body = RejoinAfterCrashIsNotDeath };
        yield return new TestCase { Name = "shout and ping are recorded", Body = ShoutAndPing };
        yield return new TestCase { Name = "burst of 20 messages all delivered in order", Body = BurstInOrder };
        yield return new TestCase { Name = "date range inside one month", Body = DateRangeSameMonth };
        yield return new TestCase { Name = "date range across a month boundary", Body = DateRangeAcrossMonth };
        yield return new TestCase { Name = "date range across a year boundary", Body = DateRangeAcrossYear };
        yield return new TestCase { Name = "date range excludes records outside it", Body = DateRangeExcludes };
        yield return new TestCase { Name = "time online adds up sessions", Body = TimeOnline };
        yield return new TestCase { Name = "leaderboard 1 ranks deaths (all time)", Body = LeaderboardAllTime };
        yield return new TestCase { Name = "leaderboard with bad index does not crash", Body = LeaderboardBadIndex };
        yield return new TestCase
        {
            Name = "leaderboard 5 timer sends leaderboard 5", Body = Leaderboard5Timer, Slow = true
        };
    }

    // ---- Server lifecycle ------------------------------------------------------------------------

    private static IEnumerator ServerStartedSent()
    {
        // Sent when the world finishes loading, which happened before the tests started.
        foreach (object y in Check.Eventually(() => Capture.Count("Server has started!") >= 1,
                     () => "'Server has started!' was never sent (world-load hook did not run?)", 3f))
            yield return y;
    }

    private static IEnumerator EventWatcherActive()
    {
        // On dedicated servers raid/event messages come from the EventWatcher, which is started on world load.
        Check.True(DiscordConnectorPlugin.StaticEventWatcher.IsActive,
            "EventWatcher was never activated, so raid/event messages will not be sent");
        yield break;
    }

    // ---- Player events -------------------------------------------------------------------------

    private static IEnumerator JoinIsRecordedAndAnnounced()
    {
        ZNetPeer p = Player("Alice");
        Handlers.Join(p);

        foreach (object y in Check.Eventually(() => Db.Count(Categories.Join, p.m_playerName) == 1,
                     () => $"join records = {Db.Count(Categories.Join, p.m_playerName)}"))
            yield return y;
        foreach (object y in Check.Eventually(() => Capture.Count(p.m_playerName) >= 1,
                     () => "no webhook mentioning the player"))
            yield return y;
    }

    /// <summary>
    ///     Sends the "OnDeath" broadcast that a dying player's client sends (Player.OnDeath), through the server's
    ///     real routed-RPC handler, so whatever DiscordConnector hooks gets exercised.
    /// </summary>
    private static void SendOnDeath(ZNetPeer peer, ZDOID? target = null)
    {
        ZRoutedRpc.RoutedRPCData data = new()
        {
            m_msgID = DateTime.Now.Ticks,
            m_senderPeerID = peer.m_uid,
            m_targetPeerID = ZRoutedRpc.Everybody,
            m_targetZDO = target ?? peer.m_characterID,
            m_methodHash = "OnDeath".GetStableHashCode(),
            m_parameters = new ZPackage()
        };
        ZRoutedRpc.instance.HandleRoutedRPC(data);
    }

    /// <summary>Adds the pretend player to the server's peer list so ZNet.GetPeer can find them.</summary>
    private static ZNetPeer Connect(string name)
    {
        ZNetPeer p = Player(name);
        ZNet.instance.m_peers.Add(p);
        Handlers.Join(p);
        return p;
    }

    private static void Drop(ZNetPeer p)
    {
        ZNet.instance.m_peers.Remove(p);
        Handlers.joinedPlayers.Remove(p.m_socket.GetHostName());
    }

    private static IEnumerator DeathRpcCountsAsDeath()
    {
        ZNetPeer p = Connect("Bob");
        try
        {
            SendOnDeath(p);
            Check.Equal(1, Db.Count(Categories.Death, p.m_playerName), "deaths after one OnDeath");
            SendOnDeath(p);
            Check.Equal(2, Db.Count(Categories.Death, p.m_playerName), "deaths after two OnDeath");
            Check.Equal(1, Db.Count(Categories.Join, p.m_playerName), "join records");
        }
        finally
        {
            Drop(p);
        }

        yield break;
    }

    /// <summary>Players also respawn without dying (e.g. TeleportHome). That must not count as a death.</summary>
    private static IEnumerator RespawnIsNotDeath()
    {
        ZNetPeer p = Connect("Bea");
        try
        {
            Handlers.Join(p); // the server sees the character ID again after a respawn
            Handlers.Join(p);
            Check.Equal(0, Db.Count(Categories.Death, p.m_playerName), "deaths after two respawns");
            Check.Equal(1, Db.Count(Categories.Join, p.m_playerName), "joins after two respawns");
        }
        finally
        {
            Drop(p);
        }

        yield break;
    }

    private static IEnumerator OnDeathForOtherObjectIgnored()
    {
        ZNetPeer p = Connect("Ozzy");
        try
        {
            // e.g. a creature the player's client owns dying
            SendOnDeath(p, new ZDOID(p.m_uid, 424242));
            Check.Equal(0, Db.Count(Categories.Death, p.m_playerName), "deaths");
        }
        finally
        {
            Drop(p);
        }

        yield break;
    }

    private static IEnumerator FirstDeathMessage()
    {
        ZNetPeer p = Connect("Cora");
        try
        {
            SendOnDeath(p);
            SendOnDeath(p);
        }
        finally
        {
            Drop(p);
        }

        foreach (object y in Check.Eventually(
                     () => Capture.Count($"{p.m_playerName} has died for the first time") == 1 &&
                           Capture.Count($"{p.m_playerName} has died.") == 1,
                     () => "expected one first-death and one normal death message, got: " +
                           Capture.Search(p.m_playerName)))
            yield return y;
    }

    private static IEnumerator LeaveIsRecorded()
    {
        ZNetPeer p = Player("Dana");
        string host = p.m_socket.GetHostName();
        Handlers.Join(p);
        foreach (object y in Check.Eventually(() => Db.Count(Categories.Join, p.m_playerName) == 1,
                     () => "join not recorded"))
            yield return y;

        Handlers.Leave(p);
        foreach (object y in Check.Eventually(() => Db.Count(Categories.Leave, p.m_playerName) == 1,
                     () => $"leave records = {Db.Count(Categories.Leave, p.m_playerName)}"))
            yield return y;

        Check.True(!Handlers.joinedPlayers.Contains(host), "player still in joined list after leaving");
    }

    /// <summary>
    ///     If a player crashes or times out, the game never sends the Disconnect RPC that DiscordConnector listens to.
    ///     When they reconnect, their first spawn must count as a join, not a death.
    /// </summary>
    private static IEnumerator RejoinAfterCrashIsNotDeath()
    {
        ZNetPeer first = Player("Evan");
        Handlers.Join(first);
        foreach (object y in Check.Eventually(() => Db.Count(Categories.Join, first.m_playerName) == 1,
                     () => "join not recorded"))
            yield return y;

        // Simulate the server dropping the connection without the Disconnect RPC (crash/timeout).
        SimulateConnectionLost(first);

        // Same player connects again on a new connection.
        ZNetPeer second = FakePeer.Create(first.m_playerName, first.m_socket.GetHostName());
        Handlers.Join(second);
        yield return new WaitForSeconds(2f);

        Check.Equal(0, Db.Count(Categories.Death, first.m_playerName), "death records after reconnect");
        Check.Equal(2, Db.Count(Categories.Join, first.m_playerName), "join records after reconnect");
    }

    /// <summary>
    ///     Runs the game's own code path for a dropped connection, so the test exercises whatever the mod hooks.
    /// </summary>
    private static void SimulateConnectionLost(ZNetPeer peer)
    {
        ZNet.instance.m_peers.Add(peer);
        ZNet.instance.Disconnect(peer);
    }

    private static IEnumerator ShoutAndPing()
    {
        ZNetPeer p = Player("Finn");
        Handlers.Join(p);
        string text = "test shout " + Run;
        Handlers.Shout(p, new Vector3(1, 2, 3), text);
        Handlers.Ping(p, new Vector3(4, 5, 6));

        foreach (object y in Check.Eventually(() => Db.Count(Categories.Shout, p.m_playerName) == 1,
                     () => "shout not recorded"))
            yield return y;
        foreach (object y in Check.Eventually(() => Db.Count(Categories.Ping, p.m_playerName) == 1,
                     () => "ping not recorded"))
            yield return y;
        foreach (object y in Check.Eventually(() => Capture.Count(text) >= 1, () => "shout text not sent"))
            yield return y;
    }

    /// <summary>
    ///     A raid can kill several players within a second. Every message should arrive, in the order sent.
    /// </summary>
    private static IEnumerator BurstInOrder()
    {
        ZNetPeer p = Player("Gus");
        Handlers.Join(p);
        string tag = "burst" + Run + "-";
        for (int i = 0; i < 20; i++)
        {
            Handlers.Shout(p, Vector3.zero, tag + i.ToString("00"));
        }

        foreach (object y in Check.Eventually(() => Capture.Count(tag) == 20,
                     () => $"primary webhook got {Capture.Count(tag)} of 20 messages", 15f))
            yield return y;

        List<string> lines = Capture.Lines(tag).FindAll(l => l.Contains("\"webhook\":\"primary\""));
        for (int i = 0; i < lines.Count; i++)
        {
            Check.True(lines[i].Contains(tag + i.ToString("00")),
                $"message {i} arrived out of order: {lines[i].Substring(0, Math.Min(160, lines[i].Length))}");
        }
    }

    // ---- Database date ranges (used by Today / PastWeek / weekly leaderboards) ------------------

    private static IEnumerator DateRangeSameMonth()
    {
        return DateRangeCase("RangeA", new DateTime(2024, 3, 12, 12, 0, 0),
            new DateTime(2024, 3, 10), new DateTime(2024, 3, 16), 1);
    }

    private static IEnumerator DateRangeAcrossMonth()
    {
        return DateRangeCase("RangeB", new DateTime(2024, 4, 29, 12, 0, 0),
            new DateTime(2024, 4, 27), new DateTime(2024, 5, 3), 1);
    }

    private static IEnumerator DateRangeAcrossYear()
    {
        return DateRangeCase("RangeC", new DateTime(2023, 12, 30, 12, 0, 0),
            new DateTime(2023, 12, 28), new DateTime(2024, 1, 3), 1);
    }

    private static IEnumerator DateRangeExcludes()
    {
        return DateRangeCase("RangeD", new DateTime(2024, 2, 20, 12, 0, 0),
            new DateTime(2024, 3, 10), new DateTime(2024, 3, 16), 0);
    }

    private static IEnumerator DateRangeCase(string baseName, DateTime recordDate, DateTime start, DateTime end,
        int expected)
    {
        string name = baseName + Run;
        string host = "Steam_" + Math.Abs(name.GetHashCode());
        Db.Instance.InsertSimpleStatRecordAt(Categories.Join, name, host, new DateTime(2019, 1, 1));
        Db.Instance.InsertSimpleStatRecordAt(Categories.Death, name, host, recordDate);

        List<CountResult> results = Db.Instance.CountRecordsBetweenDatesGrouped(Categories.Death, start, end);
        int got = Db.Lookup(results, name);
        Check.Equal(expected == 0 ? -1 : expected, got,
            $"deaths for {name} dated {recordDate:yyyy-MM-dd} in range {start:yyyy-MM-dd}..{end:yyyy-MM-dd} " +
            $"(results: {Db.Describe(results)})");
        yield break;
    }

    private static IEnumerator TimeOnline()
    {
        string name = "Tim" + Run;
        string host = "Steam_" + Math.Abs(name.GetHashCode());
        DateTime t = new(2021, 6, 1, 10, 0, 0);
        Db.Instance.InsertSimpleStatRecordAt(Categories.Join, name, host, t);
        Db.Instance.InsertSimpleStatRecordAt(Categories.Leave, name, host, t.AddHours(1));
        Db.Instance.InsertSimpleStatRecordAt(Categories.Join, name, host, t.AddHours(2));
        Db.Instance.InsertSimpleStatRecordAt(Categories.Leave, name, host, t.AddHours(2.5));

        List<CountResult> results = Db.Instance.CountAllRecordsGrouped(Categories.TimeOnline);
        Check.Equal(5400, Db.Lookup(results, name), "seconds online (1h + 30m)");

        // Same data through the date-range path used by Today/PastWeek leaderboards.
        List<CountResult> ranged = Db.Instance.CountRecordsBetweenDatesGrouped(Categories.TimeOnline,
            new DateTime(2021, 5, 30), new DateTime(2021, 6, 2));
        Check.Equal(5400, Db.Lookup(ranged, name), "seconds online within 2021-05-30..2021-06-02");
        yield break;
    }

    // ---- Leaderboards ----------------------------------------------------------------------------

    private static IEnumerator LeaderboardAllTime()
    {
        // High counts so these three are the top 3 no matter what other tests recorded.
        string a = "LbAce" + Run, b = "LbBee" + Run, c = "LbCat" + Run;
        Seed(a, 9);
        Seed(b, 5);
        Seed(c, 7);

        new Composer(0).SendLeaderBoard();

        string heading = "DCTEST-LB1";
        foreach (object y in Check.Eventually(() => Capture.Count(heading) >= 1, () => "leaderboard 1 not sent"))
            yield return y;

        string sent = Capture.Search(heading);
        int ia = sent.IndexOf(a, StringComparison.Ordinal);
        int ib = sent.IndexOf(b, StringComparison.Ordinal);
        int ic = sent.IndexOf(c, StringComparison.Ordinal);
        Check.True(ia >= 0 && ib >= 0 && ic >= 0, "leaderboard is missing a player: " + sent);
        Check.True(ia < ic && ic < ib, $"wrong order (expected {a}, {c}, {b}): " + sent);
    }

    private static void Seed(string name, int deaths)
    {
        string host = "Steam_" + Math.Abs(name.GetHashCode());
        Db.Instance.InsertSimpleStatRecordAt(Categories.Join, name, host, new DateTime(2019, 1, 1));
        for (int i = 0; i < deaths; i++)
        {
            Db.Instance.InsertSimpleStatRecordAt(Categories.Death, name, host, new DateTime(2019, 1, 2));
        }
    }

    private static IEnumerator LeaderboardBadIndex()
    {
        int length = DiscordConnectorPlugin.StaticConfig.LeaderBoards.Length;
        // SendLeaderBoard swallows its own exceptions, so look for the error it would log instead.
        bool threw = false;
        try
        {
            LeaderBoardConfigReference _ = DiscordConnectorPlugin.StaticConfig.LeaderBoards[length];
        }
        catch (IndexOutOfRangeException)
        {
            threw = true;
        }

        Check.True(threw, "sanity: indexing past the end should throw");
        Check.True(new Composer(length).IsIndexValid() == false,
            $"Composer({length}) accepted an index past the end of {length} leaderboards");
        yield break;
    }

    private static IEnumerator Leaderboard5Timer()
    {
        // run-tests.ps1 enables leaderboard 5 with a 1 minute period and heading DCTEST-LB5, and disables 4.
        foreach (object y in Check.Eventually(() => Capture.Count("DCTEST-LB5") >= 1,
                     () => "leaderboard 5 never sent (timer wired to the wrong leaderboard?)", 90f))
            yield return y;
    }
}
