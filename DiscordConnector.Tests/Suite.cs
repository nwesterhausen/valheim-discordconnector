using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using HarmonyLib;

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
        yield return new TestCase { Name = "chat relay hooks are patched", Body = ChatRelayHooksPatched };
        yield return new TestCase { Name = "shout copies to several players count once", Body = ShoutCopiesCountOnce };
        yield return new TestCase { Name = "arrival shout is not reported", Body = ArrivalShoutIgnored };
        yield return new TestCase { Name = "translated arrival shout is not reported", Body = TranslatedArrivalIgnored };
        yield return new TestCase { Name = "relayed ping counts once", Body = RelayedPingCountsOnce };
        yield return new TestCase { Name = "muted player's shout is not reported", Body = MutedPlayerIgnored };
        yield return new TestCase { Name = "webhook event names parse loosely", Body = WebhookEventParsing };
        yield return new TestCase { Name = "positions read back from database", Body = PositionsReadBack };
        yield return new TestCase { Name = "names with trailing spaces match", Body = TrailingSpaceName };
        yield return new TestCase { Name = "active players board is sent", Body = ActivePlayersSent };
        yield return new TestCase { Name = "no host player on dedicated server", Body = NoHostOnDedicated };
        yield return new TestCase { Name = "database checkpoint keeps records", Body = DatabaseCheckpoint };
        yield return new TestCase { Name = "flush waits for queued webhooks", Body = FlushWaits };
        yield return new TestCase { Name = "week ranges on every weekday", Body = WeekRanges };
        yield return new TestCase { Name = "today, yesterday and past 7 days ranges", Body = DayRanges };
        yield return new TestCase { Name = "tied players listed in name order", Body = TiesInNameOrder };
        yield return new TestCase { Name = "ascending ranking lists least first", Body = AscendingOrder };
        yield return new TestCase { Name = "ranking text fits in a Discord field", Body = RankingFitsField };
        yield return new TestCase { Name = "time online skips players with no time", Body = TimeOnlineSkipsZero };
        yield return new TestCase { Name = "time online clips sessions to the range", Body = TimeOnlineClipsRange };
        yield return new TestCase
        {
            Name = "time online combines characters by player id", Body = TimeOnlineByPlayerId
        };
        yield return new TestCase { Name = "time online combines players by name", Body = TimeOnlineByName };
        yield return new TestCase
        {
            Name = "time online separates name+id combos", Body = TimeOnlineByNameAndPlayerId
        };
        yield return new TestCase { Name = "deaths combine characters by player id", Body = DeathsByPlayerId };
        yield return new TestCase { Name = "today leaderboard sends today's deaths", Body = TodayLeaderboard };
        yield return new TestCase { Name = "time online leaderboard is sent", Body = TimeOnlineLeaderboard };
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

    // ---- Chat relay (shouts/pings seen as they pass through the server, issues #338, #319) ---------

    /// <summary>A pretend player the server knows about (in ZNet's peer list) but who hasn't spawned yet.</summary>
    private static ZNetPeer AddPeer(string name)
    {
        ZNetPeer p = Player(name);
        ZNet.instance.m_peers.Add(p);
        return p;
    }

    /// <summary>
    ///     Builds the "ChatMessage" routed RPC a client sends, laid out like Chat.RPC_ChatMessage expects:
    ///     position, talker type, UserInfo (name, platform id), text.
    /// </summary>
    private static ZRoutedRpc.RoutedRPCData ChatRpc(ZNetPeer from, Talker.Type type, string text, long target,
        long messageId)
    {
        ZPackage pkg = new();
        pkg.Write(new Vector3(10, 20, 30));
        pkg.Write((int)type);
        pkg.Write(from.m_playerName);
        pkg.Write(from.m_socket.GetHostName());
        pkg.Write(text);
        pkg.SetPos(0);
        return new ZRoutedRpc.RoutedRPCData
        {
            m_msgID = messageId,
            m_senderPeerID = from.m_uid,
            m_targetPeerID = target,
            m_methodHash = "ChatMessage".GetStableHashCode(),
            m_parameters = pkg
        };
    }

    private static long s_msgId = DateTime.Now.Ticks;

    private static IEnumerator ChatRelayHooksPatched()
    {
        string owner = DiscordConnectorPlugin.ModGuid;
        foreach (string method in new[] { "HandleRoutedRPC", "RouteRPC" })
        {
            MethodInfo target = AccessTools.Method(typeof(ZRoutedRpc), method);
            HarmonyLib.Patches info = Harmony.GetPatchInfo(target);
            Check.True(info != null && info.Prefixes.Any(p => p.owner == owner),
                $"ZRoutedRpc.{method} has no DiscordConnector prefix, so shouts are never seen");
        }

        yield break;
    }

    private static IEnumerator ShoutCopiesCountOnce()
    {
        ZNetPeer p = AddPeer("Hal");
        string text = "hello all " + Run;
        try
        {
            // The sender's client sends one copy per other player; one copy can pass both server hooks.
            long id1 = ++s_msgId, id2 = ++s_msgId;
            Patches.ChatRelay.Observe(ChatRpc(p, Talker.Type.Shout, text, 111, id1));
            Patches.ChatRelay.Observe(ChatRpc(p, Talker.Type.Shout, text, 111, id1));
            Patches.ChatRelay.Observe(ChatRpc(p, Talker.Type.Shout, text, 222, id2));
            Check.Equal(1, Db.Count(Categories.Shout, p.m_playerName), "shouts after three copies of one shout");

            // Saying the same thing again (a new copy to a player who already got one) is a second shout.
            Patches.ChatRelay.Observe(ChatRpc(p, Talker.Type.Shout, text, 111, ++s_msgId));
            Check.Equal(2, Db.Count(Categories.Shout, p.m_playerName), "shouts after repeating it");
        }
        finally
        {
            Drop(p);
        }

        foreach (object y in Check.Eventually(() => Capture.Count(text) == 2,
                     () => $"expected 2 shout webhooks, got {Capture.Count(text)}"))
            yield return y;
    }

    private static IEnumerator ArrivalShoutIgnored()
    {
        ZNetPeer p = Connect("Ida");
        try
        {
            Patches.ChatRelay.Observe(ChatRpc(p, Talker.Type.Shout, Patches.ChatPatches.ArrivalShout, 111,
                ++s_msgId));
            Check.Equal(0, Db.Count(Categories.Shout, p.m_playerName), "shouts after the arrival shout");

            // A real shout afterwards still counts.
            Patches.ChatRelay.Observe(ChatRpc(p, Talker.Type.Shout, "after arrival " + Run, 111, ++s_msgId));
            Check.Equal(1, Db.Count(Categories.Shout, p.m_playerName), "shouts after a real shout");
        }
        finally
        {
            Drop(p);
        }

        yield break;
    }

    /// <summary>The arrival shout is translated by the client, so it's recognised by timing, not text.</summary>
    private static IEnumerator TranslatedArrivalIgnored()
    {
        ZNetPeer p = Connect("Jon");
        try
        {
            Patches.ChatRelay.Observe(ChatRpc(p, Talker.Type.Shout, "Ich bin angekommen!", 111, ++s_msgId));
            Check.Equal(0, Db.Count(Categories.Shout, p.m_playerName), "shouts after a German arrival shout");
        }
        finally
        {
            Drop(p);
        }

        yield break;
    }

    private static IEnumerator RelayedPingCountsOnce()
    {
        ZNetPeer p = AddPeer("Kim");
        try
        {
            // Pings are broadcast (target Everybody); the server both handles and routes the same message.
            long id = ++s_msgId;
            Patches.ChatRelay.Observe(ChatRpc(p, Talker.Type.Ping, "", ZRoutedRpc.Everybody, id));
            Patches.ChatRelay.Observe(ChatRpc(p, Talker.Type.Ping, "", ZRoutedRpc.Everybody, id));
            Check.Equal(1, Db.Count(Categories.Ping, p.m_playerName), "pings after one broadcast ping");
        }
        finally
        {
            Drop(p);
        }

        yield break;
    }

    private static IEnumerator MutedPlayerIgnored()
    {
        ZNetPeer p = AddPeer("Mute");
        List<string> muted = DiscordConnectorPlugin.StaticConfig.MutedPlayers;
        muted.Add(p.m_playerName);
        try
        {
            Patches.ChatRelay.Observe(ChatRpc(p, Talker.Type.Shout, "muted " + Run, 111, ++s_msgId));
            Check.Equal(0, Db.Count(Categories.Shout, p.m_playerName), "shouts from a muted player");
        }
        finally
        {
            muted.Remove(p.m_playerName);
            Drop(p);
        }

        yield break;
    }

    // ---- Config parsing --------------------------------------------------------------------------

    private static string Names(List<Webhook.Event> events)
    {
        return string.Join(",", events.Select(e => e.ToString()).ToArray());
    }

    private static IEnumerator WebhookEventParsing()
    {
        Check.Equal(Webhook.Event.PlayerDeath, Webhook.StringToEvent("playerdeath"), "lower case");
        Check.Equal(Webhook.Event.PlayerDeath, Webhook.StringToEvent(" PLAYERDEATH "), "upper case with spaces");
        Check.Equal(Webhook.Event.Other, Webhook.StringToEvent("other"), "'other' (issue #382)");
        Check.Equal(Webhook.Event.None, Webhook.StringToEvent("notAnEvent"), "unknown name");

        Check.Equal("PlayerJoin,PlayerLeave,PlayerDeath",
            Names(Webhook.StringToEventList("playerJoin, playerLeave;playerDeath playerJoin")),
            "mixed separators, duplicates removed");
        Check.Equal("PlayerJoin", Names(Webhook.StringToEventList("playerJoin;typo;;")),
            "unknown and empty names dropped");
        Check.Equal("ALL", Names(Webhook.StringToEventList("all")), "'all' in lower case");
        yield break;
    }

    // ---- Database --------------------------------------------------------------------------------

    private static IEnumerator PositionsReadBack()
    {
        string name = "Pos" + Run;
        string host = "Steam_" + Math.Abs(name.GetHashCode());
        Db.Instance.InsertSimpleStatRecord(Categories.Ping, name, host, new Vector3(12.5f, -3f, 400f));

        List<SimpleStat> records = Db.Instance.RecordsFor(Categories.Ping, name);
        Check.Equal(1, records.Count, "ping records");
        Position pos = records[0].Pos;
        Check.True(Math.Abs(pos.x - 12.5f) < 0.01f && Math.Abs(pos.y + 3f) < 0.01f && Math.Abs(pos.z - 400f) < 0.01f,
            $"position read back as ({pos.x}, {pos.y}, {pos.z}), expected (12.5, -3, 400)");
        yield break;
    }

    /// <summary>LiteDB trims stored strings, so lookups must trim too (issue #190).</summary>
    private static IEnumerator TrailingSpaceName()
    {
        ZNetPeer p = FakePeer.Create("Spacey" + Run + "  ", "Steam_" + Math.Abs(("Spacey" + Run).GetHashCode()));
        Handlers.Join(p);
        Handlers.joinedPlayers.Remove(p.m_socket.GetHostName());
        Check.Equal(1, Db.Count(Categories.Join, p.m_playerName), "joins looked up with the spaces");
        Check.Equal(1, Db.Count(Categories.Join, p.m_playerName.Trim()), "joins looked up without the spaces");
        yield break;
    }

    private static IEnumerator DatabaseCheckpoint()
    {
        string name = "Ckpt" + Run;
        string host = "Steam_" + Math.Abs(name.GetHashCode());
        Db.Instance.InsertSimpleStatRecord(Categories.Death, name, host);
        Db.Instance.Checkpoint();
        Check.Equal(1, Db.Count(Categories.Death, name), "deaths after checkpoint");
        yield break;
    }

    // ---- Misc ------------------------------------------------------------------------------------

    private static IEnumerator ActivePlayersSent()
    {
        ActivePlayersAnnouncement.SendActivePlayersBoard();
        foreach (object y in Check.Eventually(() => Capture.Count("Active Players") >= 1,
                     () => "active players announcement not sent"))
            yield return y;
        Check.True(Capture.Search("Active Players").Contains("Online now"),
            "announcement is missing 'Online now': " + Capture.Search("Active Players"));
    }

    private static IEnumerator NoHostOnDedicated()
    {
        Check.True(!HostPlayer.IsListenServer, "dedicated server reported as having a host player");
        Check.True(HostPlayer.Peer() == null, "dedicated server returned a host peer");
        yield break;
    }

    private static IEnumerator FlushWaits()
    {
        string text = "flush " + Run;
        DiscordApi.SendMessage(Webhook.Event.ServerSave, text);
        Check.True(DiscordApi.Flush(TimeSpan.FromSeconds(10)), "Flush timed out");
        // Flush returns once the HTTP request has completed, so the capture should already have it.
        foreach (object y in Check.Eventually(() => Capture.Count(text) == 1, () => "flushed message not captured",
                     2f))
            yield return y;
    }

    // ---- Leaderboard date ranges -----------------------------------------------------------------

    private static string Range(TimeRange range, DateTime today)
    {
        Tuple<DateTime, DateTime> r = DateHelper.StartEndDatesForTimeRange(range, today);
        return $"{r.Item1:yyyy-MM-dd}..{r.Item2:yyyy-MM-dd}";
    }

    private static IEnumerator WeekRanges()
    {
        // 2024-03-04 is a Monday, 2024-03-10 a Sunday.
        for (int d = 4; d <= 10; d++)
        {
            DateTime day = new(2024, 3, d, 15, 30, 0);
            Check.Equal("2024-03-04..2024-03-10", Range(TimeRange.WeekMondayToSunday, day),
                $"Monday-Sunday week on {day:dddd}");
            string sundayWeek = d == 10 ? "2024-03-10..2024-03-16" : "2024-03-03..2024-03-09";
            Check.Equal(sundayWeek, Range(TimeRange.WeekSundayToSaturday, day), $"Sunday-Saturday week on {day:dddd}");
        }

        yield break;
    }

    private static IEnumerator DayRanges()
    {
        DateTime day = new(2024, 3, 1, 0, 30, 0);
        Check.Equal("2024-03-01..2024-03-01", Range(TimeRange.Today, day), "today");
        Check.Equal("2024-02-29..2024-02-29", Range(TimeRange.Yesterday, day), "yesterday (leap year)");
        Check.Equal("2024-02-24..2024-03-01", Range(TimeRange.PastWeek, day), "past 7 days, including today");
        yield break;
    }

    // ---- Ranking order and formatting ------------------------------------------------------------

    private static void SeedDeaths(string name, string host, int deaths, DateTime when)
    {
        Db.Instance.InsertSimpleStatRecordAt(Categories.Join, name, host, new DateTime(2019, 1, 1));
        for (int i = 0; i < deaths; i++)
        {
            Db.Instance.InsertSimpleStatRecordAt(Categories.Death, name, host, when);
        }
    }

    private static string Order(List<CountResult> results, params string[] names)
    {
        return string.Join(",", results.Where(r => names.Contains(r.Name)).Select(r => r.Name).ToArray());
    }

    private static IEnumerator TiesInNameOrder()
    {
        string a = "TieA" + Run, b = "TieB" + Run, c = "TieC" + Run;
        // Inserted out of order on purpose.
        foreach (string n in new[] { c, a, b })
        {
            SeedDeaths(n, "Steam_" + Math.Abs(n.GetHashCode()), 4, new DateTime(2019, 1, 2));
        }

        for (int i = 0; i < 3; i++)
        {
            Check.Equal($"{a},{b},{c}", Order(Helper.TopNResultForCategory(Categories.Death, 1000), a, b, c),
                "tied players (descending)");
            Check.Equal($"{a},{b},{c}", Order(Helper.BottomNResultForCategory(Categories.Death, 1000), a, b, c),
                "tied players (ascending)");
        }

        yield break;
    }

    private static IEnumerator AscendingOrder()
    {
        string few = "AscFew" + Run, mid = "AscMid" + Run, lots = "AscLots" + Run;
        SeedDeaths(lots, "Steam_" + Math.Abs(lots.GetHashCode()), 3, new DateTime(2019, 1, 2));
        SeedDeaths(few, "Steam_" + Math.Abs(few.GetHashCode()), 1, new DateTime(2019, 1, 2));
        SeedDeaths(mid, "Steam_" + Math.Abs(mid.GetHashCode()), 2, new DateTime(2019, 1, 2));
        Check.Equal($"{few},{mid},{lots}",
            Order(Helper.BottomNResultForCategory(Categories.Death, 1000), few, mid, lots), "ascending order");
        Check.Equal($"{lots},{mid},{few}",
            Order(Helper.TopNResultForCategory(Categories.Death, 1000), few, mid, lots), "descending order");
        yield break;
    }

    private static IEnumerator RankingFitsField()
    {
        // The config allows up to 48 rankings. Long names with long durations went over Discord's 1024 limit.
        List<CountResult> rows = new();
        for (int i = 0; i < 48; i++)
        {
            rows.Add(new CountResult("AVeryLongCharacterName" + i, 3 * 86400 + 23 * 3600 + 59 * 60 + 59));
        }

        foreach (string text in new[]
                 {
                     LeaderbBoard.RankedSecondsToString(rows), LeaderbBoard.RankedCountResultToString(rows)
                 })
        {
            Check.True(text.Length <= 1024, $"ranking text is {text.Length} characters, Discord allows 1024");
            Check.True(text.StartsWith("1: AVeryLongCharacterName0: "), "first ranking missing: " + text);
            Check.True(text.EndsWith(Environment.NewLine), "last line was cut off mid-line");
        }

        List<CountResult> three = rows.GetRange(0, 3);
        Check.Equal(3, LeaderbBoard.RankedCountResultToString(three).Split('\n').Length - 1, "short list lines");
        yield break;
    }

    // ---- Time online -----------------------------------------------------------------------------

    private static void Session(string name, string host, DateTime join, DateTime leave)
    {
        Db.Instance.InsertSimpleStatRecordAt(Categories.Join, name, host, join);
        Db.Instance.InsertSimpleStatRecordAt(Categories.Leave, name, host, leave);
    }

    private static IEnumerator TimeOnlineSkipsZero()
    {
        string name = "NoTime" + Run;
        string host = "Steam_" + Math.Abs(name.GetHashCode());
        Db.Instance.InsertSimpleStatRecordAt(Categories.Join, name, host, new DateTime(2021, 6, 1, 10, 0, 0));

        Check.Equal(-1, Db.Lookup(Db.Instance.CountAllRecordsGrouped(Categories.TimeOnline), name),
            "player who never left (0 seconds) in all-time results");

        string other = "Tim2" + Run;
        string otherHost = "Steam_" + Math.Abs(other.GetHashCode());
        Session(other, otherHost, new DateTime(2021, 6, 1, 10, 0, 0), new DateTime(2021, 6, 1, 11, 0, 0));
        Check.Equal(-1, Db.Lookup(Db.Instance.CountRecordsBetweenDatesGrouped(Categories.TimeOnline,
                new DateTime(2022, 1, 1), new DateTime(2022, 1, 1)), other),
            "player with no sessions in the range");
        yield break;
    }

    private static IEnumerator TimeOnlineClipsRange()
    {
        string name = "Night" + Run;
        string host = "Steam_" + Math.Abs(name.GetHashCode());
        // 23:00 on the 1st to 01:00 on the 2nd.
        Session(name, host, new DateTime(2021, 7, 1, 23, 0, 0), new DateTime(2021, 7, 2, 1, 0, 0));

        Check.Equal(3600, Db.Lookup(Db.Instance.CountRecordsBetweenDatesGrouped(Categories.TimeOnline,
            new DateTime(2021, 7, 1), new DateTime(2021, 7, 1)), name), "seconds on the 1st");
        Check.Equal(3600, Db.Lookup(Db.Instance.CountRecordsBetweenDatesGrouped(Categories.TimeOnline,
            new DateTime(2021, 7, 2), new DateTime(2021, 7, 2)), name), "seconds on the 2nd");
        Check.Equal(7200, Db.Lookup(Db.Instance.CountRecordsBetweenDatesGrouped(Categories.TimeOnline,
            new DateTime(2021, 7, 1), new DateTime(2021, 7, 2)), name), "seconds over both days");
        yield break;
    }

    /// <summary>Runs <paramref name="body" /> with the "How to discern players" setting changed.</summary>
    private static void WithDiscernment(MainConfig.RetrievalDiscernmentMethods method, Action body)
    {
        object main = AccessTools.Field(typeof(PluginConfig), "mainConfig").GetValue(DiscordConnectorPlugin.StaticConfig);
        BepInEx.Configuration.ConfigEntry<MainConfig.RetrievalDiscernmentMethods> entry =
            (BepInEx.Configuration.ConfigEntry<MainConfig.RetrievalDiscernmentMethods>)AccessTools
                .Field(typeof(MainConfig), "_playerLookupPreference").GetValue(main);
        MainConfig.RetrievalDiscernmentMethods old = entry.Value;
        entry.Value = method;
        try
        {
            body();
        }
        finally
        {
            entry.Value = old;
        }
    }

    private static IEnumerator TimeOnlineByPlayerId()
    {
        string host = "Steam_Alt" + Run;
        string first = "AltOne" + Run, second = "AltTwo" + Run;
        Session(first, host, new DateTime(2021, 8, 1, 10, 0, 0), new DateTime(2021, 8, 1, 11, 0, 0));
        Session(second, host, new DateTime(2021, 8, 2, 10, 0, 0), new DateTime(2021, 8, 2, 11, 0, 0));

        WithDiscernment(MainConfig.RetrievalDiscernmentMethods.PlayerId, () =>
        {
            List<CountResult> results = Db.Instance.CountAllRecordsGrouped(Categories.TimeOnline);
            List<CountResult> mine = results.Where(r => r.Name == first || r.Name == second).ToList();
            Check.Equal(1, mine.Count, "rows for one player with two characters: " + Db.Describe(mine));
            Check.Equal(7200, mine[0].Count, "seconds for both characters together");
        });
        yield break;
    }

    private static IEnumerator TimeOnlineByName()
    {
        string name = "Twin" + Run;
        Session(name, "Steam_TwinA" + Run, new DateTime(2021, 9, 1, 10, 0, 0), new DateTime(2021, 9, 1, 11, 0, 0));
        Session(name, "Steam_TwinB" + Run, new DateTime(2021, 9, 1, 12, 0, 0), new DateTime(2021, 9, 1, 12, 30, 0));

        WithDiscernment(MainConfig.RetrievalDiscernmentMethods.Name, () =>
        {
            List<CountResult> mine = Db.Instance.CountAllRecordsGrouped(Categories.TimeOnline)
                .Where(r => r.Name == name).ToList();
            Check.Equal(1, mine.Count, "rows for two players sharing a name: " + Db.Describe(mine));
            Check.Equal(5400, mine[0].Count, "seconds for both players together");
        });
        yield break;
    }

    private static IEnumerator TimeOnlineByNameAndPlayerId()
    {
        string name = "Combo" + Run;
        Session(name, "Steam_ComboA" + Run, new DateTime(2021, 10, 1, 10, 0, 0), new DateTime(2021, 10, 1, 11, 0, 0));
        Session(name, "Steam_ComboB" + Run, new DateTime(2021, 10, 1, 12, 0, 0),
            new DateTime(2021, 10, 1, 12, 30, 0));

        WithDiscernment(MainConfig.RetrievalDiscernmentMethods.NameAndPlayerId, () =>
        {
            List<int> counts = Db.Instance.CountAllRecordsGrouped(Categories.TimeOnline)
                .Where(r => r.Name == name).Select(r => r.Count).OrderBy(c => c).ToList();
            Check.Equal("1800,3600", string.Join(",", counts.Select(c => c.ToString()).ToArray()),
                "separate rows for the two players");
        });
        yield break;
    }

    private static IEnumerator DeathsByPlayerId()
    {
        string host = "Steam_DAlt" + Run;
        string first = "DAltOne" + Run, second = "DAltTwo" + Run;
        SeedDeaths(first, host, 2, new DateTime(2019, 1, 2));
        SeedDeaths(second, host, 3, new DateTime(2019, 1, 2));

        WithDiscernment(MainConfig.RetrievalDiscernmentMethods.PlayerId, () =>
        {
            List<CountResult> mine = Db.Instance.CountAllRecordsGrouped(Categories.Death)
                .Where(r => r.Name == first || r.Name == second).ToList();
            Check.Equal(1, mine.Count, "rows for one player with two characters: " + Db.Describe(mine));
            Check.Equal(5, mine[0].Count, "deaths for both characters together");
        });
        WithDiscernment(MainConfig.RetrievalDiscernmentMethods.Name, () =>
        {
            List<CountResult> mine = Db.Instance.CountAllRecordsGrouped(Categories.Death)
                .Where(r => r.Name == first || r.Name == second).ToList();
            Check.Equal(2, mine.Count, "rows by character name: " + Db.Describe(mine));
        });
        yield break;
    }

    // ---- Leaderboards with other settings --------------------------------------------------------

    /// <summary>Runs leaderboard 2 once with the given settings, then puts its settings back.</summary>
    private static void SendLeaderboard2(Action<LeaderBoardConfigReference> configure)
    {
        LeaderBoardConfigReference lb = DiscordConnectorPlugin.StaticConfig.LeaderBoards[1];
        LeaderBoardConfigReference saved = (LeaderBoardConfigReference)AccessTools
            .Method(typeof(object), "MemberwiseClone").Invoke(lb, null);
        try
        {
            lb.Enabled = true;
            lb.Deaths = lb.Sessions = lb.Shouts = lb.Pings = lb.TimeOnline = false;
            configure(lb);
            new Composer(1).SendLeaderBoard();
        }
        finally
        {
            foreach (FieldInfo f in typeof(LeaderBoardConfigReference).GetFields())
            {
                f.SetValue(lb, f.GetValue(saved));
            }
        }
    }

    private static IEnumerator TodayLeaderboard()
    {
        string today = "TodayDie" + Run, old = "OldDie" + Run;
        SeedDeaths(today, "Steam_" + Math.Abs(today.GetHashCode()), 1, DateTime.Now);
        SeedDeaths(old, "Steam_" + Math.Abs(old.GetHashCode()), 40, DateTime.Now.AddDays(-3));

        string heading = "DCTEST-TODAY" + Run;
        SendLeaderboard2(lb =>
        {
            lb.DisplayedHeading = heading;
            lb.TimeRange = TimeRange.Today;
            lb.Deaths = true;
            lb.NumberListings = 16;
        });

        foreach (object y in Check.Eventually(() => Capture.Count(heading) >= 1, () => "today leaderboard not sent"))
            yield return y;
        string sent = Capture.Search(heading);
        Check.True(sent.Contains(today), "today's death missing: " + sent);
        Check.True(!sent.Contains(old), "a death from 3 days ago is on today's board: " + sent);
    }

    private static IEnumerator TimeOnlineLeaderboard()
    {
        string name = "LbTime" + Run;
        Session(name, "Steam_" + Math.Abs(name.GetHashCode()), DateTime.Now.AddHours(-2), DateTime.Now.AddHours(-1));

        string heading = "DCTEST-TIME" + Run;
        SendLeaderboard2(lb =>
        {
            lb.DisplayedHeading = heading;
            lb.TimeRange = TimeRange.PastWeek;
            lb.TimeOnline = true;
            lb.NumberListings = 16;
        });

        foreach (object y in Check.Eventually(() => Capture.Count(heading) >= 1,
                     () => "time online leaderboard not sent"))
            yield return y;
        string sent = Capture.Search(heading);
        Check.True(sent.Contains(name + ": 1 hour"), "expected '" + name + ": 1 hour' in: " + sent);
        Check.True(!sent.Contains("0 seconds"), "board lists players with no time online: " + sent);
    }
}
