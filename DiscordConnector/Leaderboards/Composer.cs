using System;
using System.Collections.Generic;

using DiscordConnector.Config;
using DiscordConnector.Records;

namespace DiscordConnector.Leaderboards;

internal class Composer : Base
{
    private readonly int leaderBoardIdx;

    public Composer(int leaderBoard)
    {
        leaderBoardIdx = leaderBoard;
    }

    public override void SendLeaderBoard()
    {
        if (leaderBoardIdx > DiscordConnectorPlugin.StaticConfig.LeaderBoards.Length || leaderBoardIdx < 0)
        {
            DiscordConnectorPlugin.StaticLogger.LogWarning(
                $"Tried to get leader board out of index bounds (index:{leaderBoardIdx}, length:{DiscordConnectorPlugin.StaticConfig.LeaderBoards.Length})");
            return;
        }

        LeaderBoardConfigReference settings = DiscordConnectorPlugin.StaticConfig.LeaderBoards[leaderBoardIdx];
        Webhook.Event ev = DiscordConnectorPlugin.StaticConfig.LeaderBoards[leaderBoardIdx].WebhookEvent;

        if (!settings.Enabled)
        {
            return;
        }

        // Build standings
        Dictionary<Statistic, List<CountResult>> rankings = makeRankings(settings);

        // Build leader board for discord
        List<Tuple<string, string>> leaderFields = new();

        // Check if there are rankings for each statistic. If there are, we want to build and add it.
        if (rankings.ContainsKey(Statistic.Death))
        {
            List<CountResult> deathRankings;
            if (rankings.TryGetValue(Statistic.Death, out deathRankings))
            {
                if (deathRankings.Count > 0)
                {
                    leaderFields.Add(Tuple.Create("Deaths", LeaderbBoard.RankedCountResultToString(deathRankings)));
                }
            }
        }

        if (rankings.ContainsKey(Statistic.Session))
        {
            List<CountResult> sessionRankings;
            if (rankings.TryGetValue(Statistic.Session, out sessionRankings))
            {
                if (sessionRankings.Count > 0)
                {
                    leaderFields.Add(Tuple.Create("Sessions", LeaderbBoard.RankedCountResultToString(sessionRankings)));
                }
            }
        }

        if (rankings.ContainsKey(Statistic.Shout))
        {
            List<CountResult> shoutRankings;
            if (rankings.TryGetValue(Statistic.Shout, out shoutRankings))
            {
                if (shoutRankings.Count > 0)
                {
                    leaderFields.Add(Tuple.Create("Shouts", LeaderbBoard.RankedCountResultToString(shoutRankings)));
                }
            }
        }

        if (rankings.ContainsKey(Statistic.Ping))
        {
            List<CountResult> pingRankings;
            if (rankings.TryGetValue(Statistic.Ping, out pingRankings))
            {
                if (pingRankings.Count > 0)
                {
                    leaderFields.Add(Tuple.Create("Pings", LeaderbBoard.RankedCountResultToString(pingRankings)));
                }
            }
        }

        if (rankings.ContainsKey(Statistic.TimeOnline))
        {
            List<CountResult> timeOnlineRankings;
            if (rankings.TryGetValue(Statistic.TimeOnline, out timeOnlineRankings))
            {
                if (timeOnlineRankings.Count > 0)
                {
                    leaderFields.Add(
                        Tuple.Create("Time Online", LeaderbBoard.RankedSecondsToString(timeOnlineRankings)));
                }
            }
        }

        string discordContent = MessageTransformer.FormatLeaderBoardHeader(
            settings.DisplayedHeading, settings.NumberListings
        );

        DiscordApi.SendMessageWithFields(ev, discordContent, leaderFields);

        // string json = JsonConvert.SerializeObject(rankings, Formatting.Indented);
        // DiscordApi.SendMessage($"```json\n{json}\n```");
    }

    private Dictionary<Statistic, List<CountResult>> makeRankings(LeaderBoardConfigReference settings)
    {
        if (settings.TimeRange == TimeRange.AllTime)
        {
            return AllRankings(settings);
        }

        Tuple<DateTime, DateTime>? BeginEndDate = DateHelper.StartEndDatesForTimeRange(settings.TimeRange);
        return TimeBasedRankings(settings, BeginEndDate.Item1, BeginEndDate.Item2);
    }

    private Dictionary<Statistic, List<CountResult>> AllRankings(LeaderBoardConfigReference settings)
    {
        Dictionary<Statistic, List<CountResult>> Dict = new();
        if (settings.Type == Ordering.Descending)
        {
            if (settings.Deaths)
            {
                Dict.Add(Statistic.Death, Helper.TopNResultForCategory(Categories.Death, settings.NumberListings));
            }

            if (settings.Sessions)
            {
                Dict.Add(Statistic.Session, Helper.TopNResultForCategory(Categories.Join, settings.NumberListings));
            }

            if (settings.Shouts)
            {
                Dict.Add(Statistic.Shout, Helper.TopNResultForCategory(Categories.Shout, settings.NumberListings));
            }

            if (settings.Pings)
            {
                Dict.Add(Statistic.Ping, Helper.TopNResultForCategory(Categories.Ping, settings.NumberListings));
            }

            if (settings.TimeOnline)
            {
                Dict.Add(Statistic.TimeOnline,
                    Helper.TopNResultForCategory(Categories.TimeOnline, settings.NumberListings));
            }
        }

        if (settings.Type == Ordering.Ascending)
        {
            if (settings.Deaths)
            {
                Dict.Add(Statistic.Death, Helper.BottomNResultForCategory(Categories.Death, settings.NumberListings));
            }

            if (settings.Sessions)
            {
                Dict.Add(Statistic.Session, Helper.BottomNResultForCategory(Categories.Join, settings.NumberListings));
            }

            if (settings.Shouts)
            {
                Dict.Add(Statistic.Shout, Helper.BottomNResultForCategory(Categories.Shout, settings.NumberListings));
            }

            if (settings.Pings)
            {
                Dict.Add(Statistic.Ping, Helper.BottomNResultForCategory(Categories.Ping, settings.NumberListings));
            }

            if (settings.TimeOnline)
            {
                Dict.Add(Statistic.TimeOnline,
                    Helper.BottomNResultForCategory(Categories.TimeOnline, settings.NumberListings));
            }
        }

        DiscordConnectorPlugin.StaticLogger.LogDebug($"Prepared to send leaderboard for {Dict.Keys.Count} values");
        printDict(Dict);

        return Dict;
    }


    private Dictionary<Statistic, List<CountResult>> TimeBasedRankings(LeaderBoardConfigReference settings,
        DateTime startDate, DateTime endDate)
    {
        Dictionary<Statistic, List<CountResult>> Dict = new();
        if (settings.Type == Ordering.Descending)
        {
            if (settings.Deaths)
            {
                Dict.Add(Statistic.Death,
                    Helper.TopNResultForCategory(Categories.Death, settings.NumberListings, startDate, endDate));
            }

            if (settings.Sessions)
            {
                Dict.Add(Statistic.Session,
                    Helper.TopNResultForCategory(Categories.Join, settings.NumberListings, startDate, endDate));
            }

            if (settings.Shouts)
            {
                Dict.Add(Statistic.Shout,
                    Helper.TopNResultForCategory(Categories.Shout, settings.NumberListings, startDate, endDate));
            }

            if (settings.Pings)
            {
                Dict.Add(Statistic.Ping,
                    Helper.TopNResultForCategory(Categories.Ping, settings.NumberListings, startDate, endDate));
            }

            if (settings.TimeOnline)
            {
                Dict.Add(Statistic.TimeOnline,
                    Helper.TopNResultForCategory(Categories.TimeOnline, settings.NumberListings, startDate, endDate));
            }
        }

        if (settings.Type == Ordering.Ascending)
        {
            if (settings.Deaths)
            {
                Dict.Add(Statistic.Death,
                    Helper.BottomNResultForCategory(Categories.Death, settings.NumberListings, startDate, endDate));
            }

            if (settings.Sessions)
            {
                Dict.Add(Statistic.Session,
                    Helper.BottomNResultForCategory(Categories.Join, settings.NumberListings, startDate, endDate));
            }

            if (settings.Shouts)
            {
                Dict.Add(Statistic.Shout,
                    Helper.BottomNResultForCategory(Categories.Shout, settings.NumberListings, startDate, endDate));
            }

            if (settings.Pings)
            {
                Dict.Add(Statistic.Ping,
                    Helper.BottomNResultForCategory(Categories.Ping, settings.NumberListings, startDate, endDate));
            }

            if (settings.TimeOnline)
            {
                Dict.Add(Statistic.TimeOnline,
                    Helper.BottomNResultForCategory(Categories.Ping, settings.NumberListings, startDate, endDate));
            }
        }

        DiscordConnectorPlugin.StaticLogger.LogDebug($"Prepared to send leaderboard for {Dict.Keys.Count} values");
        printDict(Dict);

        return Dict;
    }

    private static void printDict(Dictionary<Statistic, List<CountResult>> dict)
    {
        foreach (KeyValuePair<Statistic, List<CountResult>> pair in dict)
        {
            string outStr = "";
            foreach (CountResult? x in pair.Value)
            {
                outStr += x + ",";
            }

            DiscordConnectorPlugin.StaticLogger.LogDebug($"{pair.Key}: {outStr}");
        }
    }
}
