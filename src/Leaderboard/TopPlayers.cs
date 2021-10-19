using System;
using System.Collections.Generic;

namespace DiscordConnector.Leaderboards
{
    internal class TopPlayers : Base
    {
        public override void SendLeaderboard()
        {
            var deaths = Plugin.StaticRecords.RetrieveAll(RecordCategories.Death);
            var sessions = Plugin.StaticRecords.RetrieveAll(RecordCategories.Join);
            var shouts = Plugin.StaticRecords.RetrieveAll(RecordCategories.Shout);
            var pings = Plugin.StaticRecords.RetrieveAll(RecordCategories.Ping);

            List<Tuple<string, string>> leaderFields = new List<Tuple<string, string>>();
            if (Plugin.StaticConfig.RankedDeathLeaderboardEnabled && deaths.Count > 0)
            {
                deaths.Sort(Plugin.StaticRecords.HighToLowSort);
                leaderFields.Add(Tuple.Create("Top Deaths", TopPlayersFormater(deaths.ToArray())));
            }
            if (Plugin.StaticConfig.RankedSessionLeaderboardEnabled && sessions.Count > 0)
            {
                sessions.Sort(Plugin.StaticRecords.HighToLowSort);
                leaderFields.Add(Tuple.Create("Top Sessions", TopPlayersFormater(sessions.ToArray())));
            }
            if (Plugin.StaticConfig.RankedShoutLeaderboardEnabled && shouts.Count > 0)
            {
                shouts.Sort(Plugin.StaticRecords.HighToLowSort);
                leaderFields.Add(Tuple.Create("Top Shouts", TopPlayersFormater(shouts.ToArray())));
            }
            if (Plugin.StaticConfig.RankedPingLeaderboardEnabled && pings.Count > 0)
            {
                pings.Sort(Plugin.StaticRecords.HighToLowSort);
                leaderFields.Add(Tuple.Create("Top Pings", TopPlayersFormater(pings.ToArray())));
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

        /// <summary>
        /// Takes a sorted array <paramref name="sortedTopPlayers"/> and returns a string combining the top n results (n as defined in config).
        /// </summary>
        /// <param name="sortedTopPlayers">A pre-sorted array of (playername, value) Tuples.</param>
        /// <returns>String ready to send to discord listing each player and their value.</returns>
        private string TopPlayersFormater(Tuple<string, int>[] sortedTopPlayers)
        {
            string result = "";
            for (int i = 1; i < Plugin.StaticConfig.IncludedNumberOfRankings; i++)
            {
                if (i - 1 < sortedTopPlayers.Length)
                {
                    Tuple<string, int> player = sortedTopPlayers[i - 1];
                    result += $"{i}: {player.Item1}: {player.Item2}{Environment.NewLine}";
                }
            }
            return result;
        }
    }
}
