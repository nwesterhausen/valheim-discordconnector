using System;
using System.Collections.Generic;
using DiscordConnector.Config;
using DiscordConnector.Records;
using Newtonsoft.Json;

namespace DiscordConnector.Leaderboards
{
    internal class Composer : Base
    {
        private int leaderboardIdx;
        public Composer(int leaderboard)
        {
            leaderboardIdx = leaderboard;
        }
        public override void SendLeaderboard()
        {
            if (leaderboardIdx > Plugin.StaticConfig.LeaderboardConfigs.Length || leaderboardIdx < 0)
            {
                Plugin.StaticLogger.LogWarning($"Tried to get leaderboard out of index bounds (index:{leaderboardIdx}, length:{Plugin.StaticConfig.LeaderboardConfigs.Length})");
                return;
            }

            LeaderboardConfigReference settings = Plugin.StaticConfig.LeaderboardConfigs[leaderboardIdx];

            if (!settings.Enabled)
            {
                return;
            }

            // Build standings
            var rankings = makeRankings(settings);

            // Build leaderboard for discord
            List<Tuple<string, string>> leaderFields = new List<Tuple<string, string>>();
            if (rankings[Statistic.Death].Count > 0)
            {
                leaderFields.Add(Tuple.Create("Deaths", Leaderboard.RankedCountResultToString(rankings[Statistic.Death])));
            }
            if (rankings[Statistic.Session].Count > 0)
            {
                leaderFields.Add(Tuple.Create("Sessions", Leaderboard.RankedCountResultToString(rankings[Statistic.Session])));
            }
            if (rankings[Statistic.Shout].Count > 0)
            {
                leaderFields.Add(Tuple.Create("Shouts", Leaderboard.RankedCountResultToString(rankings[Statistic.Shout])));
            }
            if (rankings[Statistic.Ping].Count > 0)
            {
                leaderFields.Add(Tuple.Create("Pings", Leaderboard.RankedCountResultToString(rankings[Statistic.Ping])));
            }

            string discordContent = MessageTransformer.FormatLeaderboardHeader(
                settings.DisplayedHeading, settings.NumberListings
            );

            DiscordApi.SendMessageWithFields(discordContent, leaderFields);

            // string json = JsonConvert.SerializeObject(rankings, Formatting.Indented);
            // DiscordApi.SendMessage($"```json\n{json}\n```");
        }

        private Dictionary<Statistic, List<CountResult>> makeRankings(LeaderboardConfigReference settings)
        {
            if (settings.TimeRange == TimeRange.AllTime)
            {
                return AllRankings(settings);
            }

            var BeginEndDate = DateHelper.StartEndDatesForTimeRange(settings.TimeRange);
            return TimeBasedRankings(settings, BeginEndDate.Item1, BeginEndDate.Item2);
        }

        private Dictionary<Statistic, List<CountResult>> AllRankings(LeaderboardConfigReference settings)
        {
            Dictionary<Statistic, List<CountResult>> Dict = new Dictionary<Statistic, List<CountResult>>();
            if (settings.Type == Leaderboards.Ordering.Descending)
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
            if (settings.Type == Leaderboards.Ordering.Ascending)
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


        private Dictionary<Statistic, List<CountResult>> TimeBasedRankings(LeaderboardConfigReference settings, System.DateTime startDate, System.DateTime endDate)
        {
            Dictionary<Statistic, List<CountResult>> Dict = new Dictionary<Statistic, List<CountResult>>();
            if (settings.Type == Leaderboards.Ordering.Descending)
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
            if (settings.Type == Leaderboards.Ordering.Ascending)
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
