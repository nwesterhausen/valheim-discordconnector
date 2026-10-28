using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Timers;

using DiscordConnector.Leaderboards;
using DiscordConnector.Records;

namespace DiscordConnector
{
    internal class LeaderbBoard
    {
        public static readonly int MAX_LEADER_BOARD_SIZE = 16;

        public LeaderbBoard()
        {
            LeaderBoard1 = new Composer(0);
            LeaderBoard2 = new Composer(1);
            LeaderBoard3 = new Composer(2);
            LeaderBoard4 = new Composer(3);
            LeaderBoard5 = new Composer(4);
        }

        public Base LeaderBoard1 { get; }

        public Base LeaderBoard2 { get; }

        public Base LeaderBoard3 { get; }

        public Base LeaderBoard4 { get; }

        public Base LeaderBoard5 { get; }

        /// <summary>
        ///     Takes a sorted list <paramref name="rankings" /> and returns a string listing each member on a line prepended with
        ///     1, 2, 3, etc.
        /// </summary>
        /// <param name="rankings">A pre-sorted list of CountResults.</param>
        /// <returns>String ready to send to discord listing each player and their value.</returns>
        public static string RankedCountResultToString(List<CountResult> rankings)
        {
            List<string> lines = new();
            for (int i = 0; i < rankings.Count; i++)
            {
                lines.Add($"{i + 1}: {rankings[i].Name}: {rankings[i].Count}");
            }

            return JoinWithinFieldLimit(lines);
        }

        /// <summary>Discord rejects the whole message if any embed field value is longer than this.</summary>
        internal const int MaxFieldLength = 1024;

        /// <summary>
        ///     Joins ranking lines, dropping lines from the end so the text fits in one Discord field. Up to 48 rankings
        ///     can be configured, and with long names or durations that went over the limit, so the leaderboard was
        ///     never posted.
        /// </summary>
        internal static string JoinWithinFieldLimit(List<string> lines)
        {
            string newLine = Environment.NewLine;
            System.Text.StringBuilder res = new();
            foreach (string line in lines)
            {
                if (res.Length + line.Length + newLine.Length > MaxFieldLength)
                {
                    break;
                }

                res.Append(line).Append(newLine);
            }

            return res.ToString();
        }

        // https://stackoverflow.com/a/4423615/624900
        private static string ToReadableString(TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0
                    ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? string.Empty : "s")
                    : string.Empty,
                span.Duration().Hours > 0
                    ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? string.Empty : "s")
                    : string.Empty,
                span.Duration().Minutes > 0
                    ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? string.Empty : "s")
                    : string.Empty,
                span.Duration().Seconds > 0
                    ? string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? string.Empty : "s")
                    : string.Empty);

            if (formatted.EndsWith(", "))
            {
                formatted = formatted.Substring(0, formatted.Length - 2);
            }

            if (string.IsNullOrEmpty(formatted))
            {
                formatted = "0 seconds";
            }

            return formatted;
        }

        /// <summary>
        ///     Same as RankedCountResultToString but formats as a time duration, assuming integer seconds.
        /// </summary>
        /// <param name="rankings">A pre-sorted list of CountResults.</param>
        /// <returns>String ready to send to discord listing each player and their duration.</returns>
        public static string RankedSecondsToString(List<CountResult> rankings)
        {
            List<string> lines = new();
            for (int i = 0; i < rankings.Count; i++)
            {
                string formattedDuration = ToReadableString(TimeSpan.FromSeconds(rankings[i].Count));
                lines.Add($"{i + 1}: {rankings[i].Name}: {formattedDuration}");
            }

            return JoinWithinFieldLimit(lines);
        }
    }
}

namespace DiscordConnector.Leaderboards
{
    /// <summary>
    ///     A base class for leaderboards to inherit from. It includes a method that lets the leader board be sent on a timer
    ///     and an abstract method which sends the leader board.
    /// </summary>
    internal abstract class Base
    {
        /// <summary>
        ///     An interface for sending the leader board as a timer event.
        /// </summary>
        public void SendLeaderBoardOnTimer(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Task.Run(() =>
            {
                SendLeaderBoard();
            });
        }

        /// <summary>
        ///     Send the leader board to the DiscordAPI
        /// </summary>
        public abstract void SendLeaderBoard();
    }

    /// <summary>
    ///     Time ranges that are supported for querying from the database using a "where" clause on the date.
    /// </summary>
    public enum TimeRange
    {
        [Description("All Time")] AllTime,
        [Description("Today")] Today,
        [Description("Yesterday")] Yesterday,
        [Description("Past 7 Days")] PastWeek,

        [Description("Current Week, Sunday to Saturday")]
        WeekSundayToSaturday,

        [Description("Current Week, Monday to Sunday")]
        WeekMondayToSunday
    }

    /// <summary>
    ///     Available options for sorting the results gathered from the database. This is used when defining the custom leader
    ///     boards.
    /// </summary>
    public enum Ordering
    {
        [Description("Most to Least (Descending)")]
        Descending,

        [Description("Least to Most (Ascending)")]
        Ascending
    }

    /// <summary>
    ///     Tracked statistics which can be stored in the records database. The <see cref="TimeOnline" /> value is calculated
    ///     dynamically.
    /// </summary>
    public enum Statistic
    {
        Death,
        Session,
        Shout,
        Ping,
        TimeOnline
    }

    public static class DateHelper
    {
        /// <summary>
        ///     A "dummy" date time, set to 20 years ago. This is used internally as both the start and end date to indicate all
        ///     records.
        /// </summary>
        public static readonly DateTime DummyDateTime = DateTime.Now.AddYears(-20);

        /// <summary>
        ///     Get a tuple with the start and end date for the specified <paramref name="timeRange" />
        /// </summary>
        /// <param name="timeRange">TimeRange that you want the actual start and end date for</param>
        /// <returns>A tuple with two dates for the time range, where the earlier date is <code>Item1</code></returns>
        public static Tuple<DateTime, DateTime> StartEndDatesForTimeRange(TimeRange timeRange)
        {
            return StartEndDatesForTimeRange(timeRange, DateTime.Today);
        }

        /// <summary>
        ///     Same as <see cref="StartEndDatesForTimeRange(TimeRange)" />, as if <paramref name="today" /> were today.
        /// </summary>
        internal static Tuple<DateTime, DateTime> StartEndDatesForTimeRange(TimeRange timeRange, DateTime today)
        {
            today = today.Date;
            switch (timeRange)
            {
                case TimeRange.Today:
                    return new Tuple<DateTime, DateTime>(today, today);

                case TimeRange.Yesterday:
                    DateTime yesterday = today.AddDays(-1.0);
                    return new Tuple<DateTime, DateTime>(yesterday, yesterday);

                case TimeRange.PastWeek:
                    // Seven days including today (the old range started 7 days back, covering 8 days).
                    return new Tuple<DateTime, DateTime>(today.AddDays(-6.0), today);

                case TimeRange.WeekSundayToSaturday:
                    int daysSinceSunday = (int)today.DayOfWeek;
                    DateTime sunday = today.AddDays(-daysSinceSunday);
                    return new Tuple<DateTime, DateTime>(sunday, sunday.AddDays(6));

                case TimeRange.WeekMondayToSunday:
                    // On a Sunday this used to set the wrong variable, giving Monday to *next* Sunday (13 days).
                    int daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;
                    DateTime monday = today.AddDays(-daysSinceMonday);
                    return new Tuple<DateTime, DateTime>(monday, monday.AddDays(6));

                case TimeRange.AllTime:
                    return new Tuple<DateTime, DateTime>(DummyDateTime, DummyDateTime);

                default:
                    DiscordConnectorPlugin.StaticLogger.LogWarning("DateHelper fell through, probably not wanted!");
                    return new Tuple<DateTime, DateTime>(DummyDateTime, DummyDateTime);
            }
        }
    }
}
