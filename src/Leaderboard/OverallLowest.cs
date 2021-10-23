using System;
using System.Collections.Generic;

namespace DiscordConnector.Leaderboards
{
    internal class OverallLowest : Base
    {
        public override void SendLeaderboard()
        {
            var deathLeader = Plugin.StaticRecords.RetrieveLowest(RecordCategories.Death);
            var joinLeader = Plugin.StaticRecords.RetrieveLowest(RecordCategories.Join);
            var shoutLeader = Plugin.StaticRecords.RetrieveLowest(RecordCategories.Shout);
            var pingLeader = Plugin.StaticRecords.RetrieveLowest(RecordCategories.Ping);

            List<Tuple<string, string>> leaderFields = new List<Tuple<string, string>>();
            if (Plugin.StaticConfig.LeastDeathLeaderboardEnabled && deathLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Least Deaths", $"{deathLeader.Item1} ({deathLeader.Item2})"));
            }
            if (Plugin.StaticConfig.LeastSessionLeaderboardEnabled && joinLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Least Sessions", $"{joinLeader.Item1} ({joinLeader.Item2})"));
            }
            if (Plugin.StaticConfig.LeastShoutLeaderboardEnabled && shoutLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Least Shouts", $"{shoutLeader.Item1} ({shoutLeader.Item2})"));
            }
            if (Plugin.StaticConfig.LeastPingLeaderboardEnabled && pingLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Least Pings", $"{pingLeader.Item1} ({pingLeader.Item2})"));
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
