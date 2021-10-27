using System;
using System.Collections.Generic;

namespace DiscordConnector.Leaderboards
{
    internal class TopPlayers : Base
    {
        public override void SendLeaderboard()
        {
            List<Tuple<string, string>> leaderFields = new List<Tuple<string, string>>();

            var deaths = Records.Helper.TopNResultForCategory(Records.Categories.Death, Plugin.StaticConfig.IncludedNumberOfRankings);
            var sessions = Records.Helper.TopNResultForCategory(Records.Categories.Join, Plugin.StaticConfig.IncludedNumberOfRankings);
            var shouts = Records.Helper.TopNResultForCategory(Records.Categories.Shout, Plugin.StaticConfig.IncludedNumberOfRankings);
            var pings = Records.Helper.TopNResultForCategory(Records.Categories.Ping, Plugin.StaticConfig.IncludedNumberOfRankings);



            if (Plugin.StaticConfig.RankedDeathLeaderboardEnabled && deaths.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Top Deaths", Leaderboard.RankedCountResultToString(deaths)));
            }
            if (Plugin.StaticConfig.RankedSessionLeaderboardEnabled && sessions.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Top Sessions", Leaderboard.RankedCountResultToString(sessions)));
            }
            if (Plugin.StaticConfig.RankedShoutLeaderboardEnabled && shouts.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Top Shouts", Leaderboard.RankedCountResultToString(shouts)));
            }
            if (Plugin.StaticConfig.RankedPingLeaderboardEnabled && pings.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Top Pings", Leaderboard.RankedCountResultToString(pings)));
            }
            if (leaderFields.Count > 0)
            {
                DiscordApi.SendMessageWithFields("Current top player leaderboard:", leaderFields);
            }
            else
            {
                Plugin.StaticLogger.LogInfo("Not sending a leaderboard because theirs either no leaders, or nothing allowed.");
            }
        }
    }
}
