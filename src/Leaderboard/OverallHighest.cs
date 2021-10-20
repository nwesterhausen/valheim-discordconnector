using System;
using System.Collections.Generic;

namespace DiscordConnector.Leaderboards
{
    internal class OverallHighest : Base
    {
        public override void SendLeaderboard()
        {
            var deathLeader = Plugin.StaticRecords.RetrieveHighest(RecordCategories.Death);
            var joinLeader = Plugin.StaticRecords.RetrieveHighest(RecordCategories.Join);
            var shoutLeader = Plugin.StaticRecords.RetrieveHighest(RecordCategories.Shout);
            var pingLeader = Plugin.StaticRecords.RetrieveHighest(RecordCategories.Ping);

            List<Tuple<string, string>> leaderFields = new List<Tuple<string, string>>();
            if (Plugin.StaticConfig.MostDeathLeaderboardEnabled && deathLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Most Deaths", $"{deathLeader.Item1} ({deathLeader.Item2})"));
            }
            if (Plugin.StaticConfig.MostSessionLeaderboardEnabled && joinLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Most Sessions", $"{joinLeader.Item1} ({joinLeader.Item2})"));
            }
            if (Plugin.StaticConfig.MostShoutLeaderboardEnabled && shoutLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Most Shouts", $"{shoutLeader.Item1} ({shoutLeader.Item2})"));
            }
            if (Plugin.StaticConfig.MostPingLeaderboardEnabled && pingLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Most Pings", $"{pingLeader.Item1} ({pingLeader.Item2})"));
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
