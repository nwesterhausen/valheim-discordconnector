using System;
using System.Collections.Generic;
using DiscordConnector.Config;
using DiscordConnector.Records;
using Newtonsoft.Json;

namespace DiscordConnector.LeaderBoards
{
    internal class Composer : Base
    {
        private int leaderBoardIdx;
        public Composer(int leaderBoard)
        {
            leaderBoardIdx = leaderBoard;
        }
        public override void SendLeaderBoard()
        {
            if (leaderBoardIdx > Plugin.StaticConfig.LeaderBoards.Length || leaderBoardIdx < 0)
            {
                Plugin.StaticLogger.LogWarning($"Tried to get leader board out of index bounds (index:{leaderBoardIdx}, length:{Plugin.StaticConfig.LeaderBoards.Length})");
                return;
            }

            LeaderBoardConfigReference settings = Plugin.StaticConfig.LeaderBoards[leaderBoardIdx];

            if (!settings.Enabled)
            {
                return;
            }

            // Build standings
            var rankings = makeRankings(settings);

            // Build leader board for discord
            List<Tuple<string, string>> leaderFields = new List<Tuple<string, string>>();

            // Check if there are rankings for each statistic. If there are, we want to build and add it.
            if (rankings.ContainsKey(Statistic.Death))
            {
                List<CountResult> deathRankings;
                if (rankings.TryGetValue(Statistic.Death, out deathRankings))
                {
                    if (deathRankings.Count > 0)
                    {
                        leaderFields.Add(Tuple.Create("Deaths", LeaderBoard.RankedCountResultToString(deathRankings)));
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
                        leaderFields.Add(Tuple.Create("Sessions", LeaderBoard.RankedCountResultToString(sessionRankings)));
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
                        leaderFields.Add(Tuple.Create("Shouts", LeaderBoard.RankedCountResultToString(shoutRankings)));
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
                        leaderFields.Add(Tuple.Create("Pings", LeaderBoard.RankedCountResultToString(pingRankings)));
                    }
                }
            }

            string discordContent = MessageTransformer.FormatLeaderBoardHeader(
                settings.DisplayedHeading, settings.NumberListings
            );

            DiscordApi.SendMessageWithFields(discordContent, leaderFields);

            // string json = JsonConvert.SerializeObject(rankings, Formatting.Indented);
            // DiscordApi.SendMessage($"```json\n{json}\n```");
        }

        private Dictionary<Statistic, List<CountResult>> makeRankings(LeaderBoardConfigReference settings)
        {
            if (settings.TimeRange == TimeRange.AllTime)
            {
                return AllRankings(settings);
            }

            var BeginEndDate = DateHelper.StartEndDatesForTimeRange(settings.TimeRange);
            return TimeBasedRankings(settings, BeginEndDate.Item1, BeginEndDate.Item2);
        }

        private Dictionary<Statistic, List<CountResult>> AllRankings(LeaderBoardConfigReference settings)
        {
            Dictionary<Statistic, List<CountResult>> Dict = new Dictionary<Statistic, List<CountResult>>();
            if (settings.Type == LeaderBoards.Ordering.Descending)
            {
                if (settings.Deaths)
                {
                    Dict.Add(Statistic.Death, Records.Helper.TopNResultForCategory(Categories.Death, settings.NumberListings));
                }
                if (settings.Sessions)
                {
                    Dict.Add(Statistic.Session, Records.Helper.TopNResultForCategory(Categories.Join, settings.NumberListings));
                }
                if (settings.Shouts)
                {
                    Dict.Add(Statistic.Shout, Records.Helper.TopNResultForCategory(Categories.Shout, settings.NumberListings));
                }
                if (settings.Pings)
                {
                    Dict.Add(Statistic.Ping, Records.Helper.TopNResultForCategory(Categories.Ping, settings.NumberListings));
                }
                if (settings.TimeOnline)
                {
                    Dict.Add(Statistic.TimeOnline, Records.Helper.TopNResultForCategory(Categories.TimeOnline, settings.NumberListings));
                }
            }
            if (settings.Type == LeaderBoards.Ordering.Ascending)
            {
                if (settings.Deaths)
                {
                    Dict.Add(Statistic.Death, Records.Helper.BottomNResultForCategory(Categories.Death, settings.NumberListings));
                }
                if (settings.Sessions)
                {
                    Dict.Add(Statistic.Session, Records.Helper.BottomNResultForCategory(Categories.Join, settings.NumberListings));
                }
                if (settings.Shouts)
                {
                    Dict.Add(Statistic.Shout, Records.Helper.BottomNResultForCategory(Categories.Shout, settings.NumberListings));
                }
                if (settings.Pings)
                {
                    Dict.Add(Statistic.Ping, Records.Helper.BottomNResultForCategory(Categories.Ping, settings.NumberListings));
                }
                if (settings.TimeOnline)
                {
                    Dict.Add(Statistic.TimeOnline, Records.Helper.BottomNResultForCategory(Categories.TimeOnline, settings.NumberListings));
                }
            }

            return Dict;

        }


        private Dictionary<Statistic, List<CountResult>> TimeBasedRankings(LeaderBoardConfigReference settings, System.DateTime startDate, System.DateTime endDate)
        {
            Dictionary<Statistic, List<CountResult>> Dict = new Dictionary<Statistic, List<CountResult>>();
            if (settings.Type == LeaderBoards.Ordering.Descending)
            {
                if (settings.Deaths)
                {
                    Dict.Add(Statistic.Death, Records.Helper.TopNResultForCategory(Categories.Death, settings.NumberListings, startDate, endDate));
                }
                if (settings.Sessions)
                {
                    Dict.Add(Statistic.Session, Records.Helper.TopNResultForCategory(Categories.Join, settings.NumberListings, startDate, endDate));
                }
                if (settings.Shouts)
                {
                    Dict.Add(Statistic.Shout, Records.Helper.TopNResultForCategory(Categories.Shout, settings.NumberListings, startDate, endDate));
                }
                if (settings.Pings)
                {
                    Dict.Add(Statistic.Ping, Records.Helper.TopNResultForCategory(Categories.Ping, settings.NumberListings, startDate, endDate));
                }
                if (settings.TimeOnline)
                {
                    Dict.Add(Statistic.TimeOnline, Records.Helper.TopNResultForCategory(Categories.TimeOnline, settings.NumberListings, startDate, endDate));
                }
            }
            if (settings.Type == LeaderBoards.Ordering.Ascending)
            {
                if (settings.Deaths)
                {
                    Dict.Add(Statistic.Death, Records.Helper.BottomNResultForCategory(Categories.Death, settings.NumberListings, startDate, endDate));
                }
                if (settings.Sessions)
                {
                    Dict.Add(Statistic.Session, Records.Helper.BottomNResultForCategory(Categories.Join, settings.NumberListings, startDate, endDate));
                }
                if (settings.Shouts)
                {
                    Dict.Add(Statistic.Shout, Records.Helper.BottomNResultForCategory(Categories.Shout, settings.NumberListings, startDate, endDate));
                }
                if (settings.Pings)
                {
                    Dict.Add(Statistic.Ping, Records.Helper.BottomNResultForCategory(Categories.Ping, settings.NumberListings, startDate, endDate));
                }
                if (settings.TimeOnline)
                {
                    Dict.Add(Statistic.TimeOnline, Records.Helper.BottomNResultForCategory(Categories.Ping, settings.NumberListings, startDate, endDate));
                }
            }

            return Dict;

        }

    }
}
