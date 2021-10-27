using System;
using System.Collections.Generic;

namespace DiscordConnector.Leaderboards
{
    internal class BottomPlayers : Base
    {
        public override void SendLeaderboard()
        {
            List<Tuple<string, string>> leaderFields = new List<Tuple<string, string>>();

            var deaths = Records.Helper.BottomNResultForCategory(Records.Categories.Death, Plugin.StaticConfig.IncludedNumberOfRankings);
            var sessions = Records.Helper.BottomNResultForCategory(Records.Categories.Join, Plugin.StaticConfig.IncludedNumberOfRankings);
            var shouts = Records.Helper.BottomNResultForCategory(Records.Categories.Shout, Plugin.StaticConfig.IncludedNumberOfRankings);
            var pings = Records.Helper.BottomNResultForCategory(Records.Categories.Ping, Plugin.StaticConfig.IncludedNumberOfRankings);



            if (Plugin.StaticConfig.RankedDeathLeaderboardEnabled && deaths.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Bottom Deaths", Leaderboard.RankedCountResultToString(deaths)));
            }
            if (Plugin.StaticConfig.RankedSessionLeaderboardEnabled && sessions.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Bottom Sessions", Leaderboard.RankedCountResultToString(sessions)));
            }
            if (Plugin.StaticConfig.RankedShoutLeaderboardEnabled && shouts.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Bottom Shouts", Leaderboard.RankedCountResultToString(shouts)));
            }
            if (Plugin.StaticConfig.RankedPingLeaderboardEnabled && pings.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Bottom Pings", Leaderboard.RankedCountResultToString(pings)));
            }
            if (leaderFields.Count > 0)
            {
                string discordContent = MessageTransformer.FormatLeaderboardHeader(Plugin.StaticConfig.LeaderboardBottomPlayersHeading, Plugin.StaticConfig.IncludedNumberOfRankings);
                DiscordApi.SendMessageWithFields("Current Bottom player leaderboard:", leaderFields);
            }
            else
            {
                Plugin.StaticLogger.LogInfo("Not sending a leaderboard because theirs either no leaders, or nothing allowed.");
            }
        }
    }
}
