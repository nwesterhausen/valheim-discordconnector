using System;
using System.Collections.Generic;
using System.Timers;

namespace DiscordConnector
{
    internal class Leaderboard
    {
        private Leaderboards.Base leaderboard1;
        private Leaderboards.Base leaderboard2;
        private Leaderboards.Base leaderboard3;
        private Leaderboards.Base leaderboard4;
        public static readonly int MAX_LEADERBOARD_SIZE = 16;

        public Leaderboard()
        {
            leaderboard1 = new Leaderboards.Composer(0);
            leaderboard2 = new Leaderboards.Composer(1);
            leaderboard3 = new Leaderboards.Composer(2);
            leaderboard4 = new Leaderboards.Composer(3);
        }

        public Leaderboards.Base Leaderboard1 => leaderboard1;
        public Leaderboards.Base Leaderboard2 => leaderboard2;
        public Leaderboards.Base Leaderboard3 => leaderboard3;
        public Leaderboards.Base Leaderboard4 => leaderboard4;

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
            System.Threading.Tasks.Task.Run(() =>
            {
                this.SendLeaderboard();
            });
        }

        /// <summary>
        /// Send the leaderboard to the DiscordAPI
        /// </summary>
        public abstract void SendLeaderboard();
    }

    public enum TimeRange
    {
        AllTime,
        Today,
        Yesterday,
        PastWeek,
        WeekSundayToSaturday,
        WeekMondayToSunday,
    }
    public enum Statistic
    {
        Death,
        Session,
        Shout,
        Ping,
        TimeOnline,
    }
}
