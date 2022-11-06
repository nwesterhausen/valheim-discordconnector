using System;
using System.Collections.Generic;
using System.Timers;

namespace DiscordConnector
{
    internal class Leaderboard
    {
        private Leaderboards.Base overallHighest;
        private Leaderboards.Base overallLowest;
        private Leaderboards.Base topPlayers;
        private Leaderboards.Base bottomPlayers;

        public Leaderboard()
        {
            overallHighest = new Leaderboards.OverallHighest();
            overallLowest = new Leaderboards.OverallLowest();
            topPlayers = new Leaderboards.TopPlayers();
            bottomPlayers = new Leaderboards.BottomPlayers();
        }

        public Leaderboards.Base OverallHighest => overallHighest;
        public Leaderboards.Base OverallLowest => overallLowest;
        public Leaderboards.Base TopPlayers => topPlayers;
        public Leaderboards.Base BottomPlayers => bottomPlayers;



        /// <summary>
        /// Takes a sorted list <paramref name="rankings"/> and returns a string listing each member on a line prepended with 1, 2, 3, etc.
        /// </summary>
        /// <param name="rankings">A pre-sorted list of CountResults.</param>
        /// <returns>String ready to send to discord listing each player and their value.</returns>
        public static string RankedCountResultToString(List<Records.CountResult> rankings)
        {
            string res = "";
            for (int i = 0; i < rankings.Count; i++)
            {
                res += $"{i + 1}: {rankings[i].Name}: {rankings[i].Count}{Environment.NewLine}";
            }
            return res;
        }
    }
}

namespace DiscordConnector.Leaderboards
{
    internal abstract class Base
    {
        /// <summary>
        /// An interface for sending the leaderboard as a timer event.
        /// </summary>
        public void SendLeaderboardOnTimer(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            this.SendLeaderboard();
        }

        /// <summary>
        /// Send the leaderboard to the DiscordAPI
        /// </summary>
        public abstract void SendLeaderboard();
    }

    internal enum LeaderboardRange
    {
        AllTime,
        Today,
        Yesterday,
        PastWeek,
        WeekSundayToSaturday,
        WeekMondayToSunday,
    }
}