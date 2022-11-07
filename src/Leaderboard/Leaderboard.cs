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
        [System.ComponentModel.Description("All Time")]
        AllTime,
        [System.ComponentModel.Description("Today")]
        Today,
        [System.ComponentModel.Description("Yesterday")]
        Yesterday,
        [System.ComponentModel.Description("Past 7 Days")]
        PastWeek,
        [System.ComponentModel.Description("Current Week, Sunday to Saturday")]
        WeekSundayToSaturday,
        [System.ComponentModel.Description("Current Week, Monday to Sunday")]
        WeekMondayToSunday,
    }
    public enum Ordering
    {
        [System.ComponentModel.Description("Most to Least (Descending)")]
        Descending,
        [System.ComponentModel.Description("Least to Most (Ascending)")]
        Ascending,
    }
    public enum Statistic
    {
        Death,
        Session,
        Shout,
        Ping,
        TimeOnline,
    }
    public static class DateHelper
    {
        public static readonly System.DateTime DummyDateTime = System.DateTime.Now.AddYears(-20);
        public static Tuple<System.DateTime, System.DateTime> StartEndDatesForTimeRange(TimeRange timeRange)
        {
            switch (timeRange)
            {
                case TimeRange.Today:
                    System.DateTime today = System.DateTime.Today;
                    return new Tuple<DateTime, DateTime>(today, today);

                case TimeRange.Yesterday:
                    System.DateTime yesterday = System.DateTime.Today.AddDays(-1.0);
                    return new Tuple<DateTime, DateTime>(yesterday, yesterday);

                case TimeRange.PastWeek:
                    System.DateTime weekAgo = System.DateTime.Today.AddDays(-7.0);
                    System.DateTime today1 = System.DateTime.Today;
                    return new Tuple<DateTime, DateTime>(weekAgo, today1);

                case TimeRange.WeekSundayToSaturday:
                    System.DateTime today2 = System.DateTime.Today;
                    int dow = (int)today2.DayOfWeek;

                    System.DateTime sunday = today2.AddDays(-dow);
                    System.DateTime saturday = today2.AddDays(6 - dow);
                    // If we are on sunday, show for the current week 
                    if (today2.DayOfWeek == System.DayOfWeek.Sunday)
                    {
                        sunday = today2;
                        saturday = today2.AddDays(6);
                    }

                    return new Tuple<DateTime, DateTime>(sunday, saturday);

                case TimeRange.WeekMondayToSunday:
                    System.DateTime today3 = System.DateTime.Today;
                    int dow1 = (int)today3.DayOfWeek;

                    System.DateTime monday = today3.AddDays(1 - dow1); // Monday - day of week = goes backward to previous monday until we are in Sunday
                    System.DateTime sunday1 = today3.AddDays(7 - dow1); // (Next monday) - day of week = goes to next monday until we are in Sunday then shows next Sunday

                    // If we are on sunday, fix to show "current" week still 
                    if (today3.DayOfWeek == System.DayOfWeek.Sunday)
                    {
                        monday = today3.AddDays(-6); // Sunday - 6 = previuos monday
                        sunday = today3; // Sunday is today
                    }

                    return new Tuple<DateTime, DateTime>(monday, sunday1);

                default:
                    Plugin.StaticLogger.LogWarning("DateHelper fell through, probably not wanted!");
                    return new Tuple<DateTime, DateTime>(DummyDateTime, DummyDateTime);
            }
        }
    }
}
