using System;
using System.Collections.Generic;
using System.Timers;

namespace DiscordConnector
{
    internal class LeaderBoard
    {
        private LeaderBoards.Base leaderBoard1;
        private LeaderBoards.Base leaderBoard2;
        private LeaderBoards.Base leaderBoard3;
        private LeaderBoards.Base leaderBoard4;
        private LeaderBoards.Base leaderBoard5;
        public static readonly int MAX_LEADER_BOARD_SIZE = 16;

        public LeaderBoard()
        {
            leaderBoard1 = new LeaderBoards.Composer(0);
            leaderBoard2 = new LeaderBoards.Composer(1);
            leaderBoard3 = new LeaderBoards.Composer(2);
            leaderBoard4 = new LeaderBoards.Composer(3);
            leaderBoard5 = new LeaderBoards.Composer(4);
        }

        public LeaderBoards.Base LeaderBoard1 => leaderBoard1;
        public LeaderBoards.Base LeaderBoard2 => leaderBoard2;
        public LeaderBoards.Base LeaderBoard3 => leaderBoard3;
        public LeaderBoards.Base LeaderBoard4 => leaderBoard4;
        public LeaderBoards.Base LeaderBoard5 => leaderBoard5;

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

namespace DiscordConnector.LeaderBoards
{
    /// <summary>
    /// A base class for leaderboards to inherit from. It includes a method that lets the leader board be sent on a timer
    /// and an abstract method which sends the leader board.
    /// </summary>
    internal abstract class Base
    {
        /// <summary>
        /// An interface for sending the leader board as a timer event.
        /// </summary>
        public void SendLeaderBoardOnTimer(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                this.SendLeaderBoard();
            });
        }

        /// <summary>
        /// Send the leader board to the DiscordAPI
        /// </summary>
        public abstract void SendLeaderBoard();
    }
    /// <summary>
    /// Time ranges that are supported for querying from the database using a "where" clause on the date.
    /// </summary>
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
    /// <summary>
    /// Available options for sorting the results gathered from the database. This is used when defining the custom leader boards.
    /// </summary>
    public enum Ordering
    {
        [System.ComponentModel.Description("Most to Least (Descending)")]
        Descending,
        [System.ComponentModel.Description("Least to Most (Ascending)")]
        Ascending,
    }
    /// <summary>
    /// Tracked statistics which can be stored in the records database. The <see cref="TimeOnline"/> value is calculated dynamically.
    /// </summary>
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
        /// <summary>
        /// A "dummy" date time, set to 20 years ago. This is used internally as both the start and end date to indicate all records.
        /// </summary>
        public static readonly System.DateTime DummyDateTime = System.DateTime.Now.AddYears(-20);

        /// <summary>
        /// Get a tuple with the start and end date for the specified <paramref name="timeRange"/>
        /// </summary>
        /// <param name="timeRange">TimeRange that you want the actual start and end date for</param>
        /// <returns>A tuple with two dates for the time range, where the earlier date is <code>Item1</code></returns>
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
                        monday = today3.AddDays(-6); // Sunday - 6 = previous monday
                        sunday = today3; // Sunday is today
                    }

                    return new Tuple<DateTime, DateTime>(monday, sunday1);

                case TimeRange.AllTime:
                    return new Tuple<DateTime, DateTime>(DummyDateTime, DummyDateTime);

                default:
                    Plugin.StaticLogger.LogWarning("DateHelper fell through, probably not wanted!");
                    return new Tuple<DateTime, DateTime>(DummyDateTime, DummyDateTime);
            }
        }
    }
}
