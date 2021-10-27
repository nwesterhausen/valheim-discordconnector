using System;
using System.Collections.Generic;

namespace DiscordConnector.Leaderboards
{
    internal class OverallLowest : Base
    {
        public override void SendLeaderboard()
        {
            var deathLeader = Records.Helper.BottomResultForCategory(Records.Categories.Death);
            var joinLeader = Records.Helper.BottomResultForCategory(Records.Categories.Join);
            var shoutLeader = Records.Helper.BottomResultForCategory(Records.Categories.Shout);
            var pingLeader = Records.Helper.BottomResultForCategory(Records.Categories.Ping);

            List<Tuple<string, string>> leaderFields = new List<Tuple<string, string>>();
            if (Plugin.StaticConfig.LeastDeathLeaderboardEnabled && deathLeader.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Least Deaths", $"{deathLeader.Name} ({deathLeader.Count})"));
            }
            if (Plugin.StaticConfig.LeastSessionLeaderboardEnabled && joinLeader.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Least Sessions", $"{joinLeader.Name} ({joinLeader.Count})"));
            }
            if (Plugin.StaticConfig.LeastShoutLeaderboardEnabled && shoutLeader.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Least Shouts", $"{shoutLeader.Name} ({shoutLeader.Count})"));
            }
            if (Plugin.StaticConfig.LeastPingLeaderboardEnabled && pingLeader.Count > 0)
            {
                leaderFields.Add(Tuple.Create("Least Pings", $"{pingLeader.Name} ({pingLeader.Count})"));
            }
            if (leaderFields.Count > 0)
            {
                DiscordApi.SendMessageWithFields("Current leaderboard least stats:", leaderFields);
            }
            else
            {
                Plugin.StaticLogger.LogInfo("Not sending a leaderboard because theirs either no leaders, or nothing allowed.");
            }
        }
    }
}
