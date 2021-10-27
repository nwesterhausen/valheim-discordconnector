using System;
using System.Collections.Generic;

namespace DiscordConnector.Leaderboards
{
    internal class OverallHighest : Base
    {
        public override void SendLeaderboard()
        {
            var deathLeader = Records.Helper.TopResultForCategory(Records.Categories.Death);
            var joinLeader = Records.Helper.TopResultForCategory(Records.Categories.Join);
            var shoutLeader = Records.Helper.TopResultForCategory(Records.Categories.Shout);
            var pingLeader = Records.Helper.TopResultForCategory(Records.Categories.Ping);

            List<Tuple<string, string>> leaderFields = new List<Tuple<string, string>>();
            if (Plugin.StaticConfig.MostDeathLeaderboardEnabled && deathLeader.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Most Deaths", $"{deathLeader.Name} ({deathLeader.Count})"));
            }
            if (Plugin.StaticConfig.MostSessionLeaderboardEnabled && joinLeader.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Most Sessions", $"{joinLeader.Name} ({joinLeader.Count})"));
            }
            if (Plugin.StaticConfig.MostShoutLeaderboardEnabled && shoutLeader.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Most Shouts", $"{shoutLeader.Name} ({shoutLeader.Count})"));
            }
            if (Plugin.StaticConfig.MostPingLeaderboardEnabled && pingLeader.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Most Pings", $"{pingLeader.Name} ({pingLeader.Count})"));
            }
            if (leaderFields.Count > 0)
            {
                DiscordApi.SendMessageWithFields("Current Highest stat leader board:", leaderFields);
            }
            else
            {
                Plugin.StaticLogger.LogInfo("Not sending a leaderboard because theirs either no leaders, or nothing allowed.");
            }
        }

    }
}
