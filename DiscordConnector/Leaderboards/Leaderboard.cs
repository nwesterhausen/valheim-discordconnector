using System;
using System.Collections.Generic;
using System.Timers;

namespace DiscordConnector
{
    internal class LeaderbBoard
    {
        private Leaderboards.Base leaderBoard1;
        private Leaderboards.Base leaderBoard2;
        private Leaderboards.Base leaderBoard3;
        private Leaderboards.Base leaderBoard4;
        private Leaderboards.Base leaderBoard5;
        public static readonly int MAX_LEADER_BOARD_SIZE = 16;

        public LeaderbBoard()
        {
            leaderBoard1 = new Leaderboards.Composer(0);
            leaderBoard2 = new Leaderboards.Composer(1);
            leaderBoard3 = new Leaderboards.Composer(2);
            leaderBoard4 = new Leaderboards.Composer(3);
            leaderBoard5 = new Leaderboards.Composer(4);
        }

        public Leaderboards.Base LeaderBoard1 => leaderBoard1;
        public Leaderboards.Base LeaderBoard2 => leaderBoard2;
        public Leaderboards.Base LeaderBoard3 => leaderBoard3;
        public Leaderboards.Base LeaderBoard4 => leaderBoard4;
        public Leaderboards.Base LeaderBoard5 => leaderBoard5;

        /// <summary>
        /// Takes a sorted list <paramref name="rankings"/> and returns a string listing each member on a line prepended with 1, 2, 3, etc.
        /// </summary>
        /// <param name="rankings">A pre-sorted list of CountResults.</param>
        /// <returns>String ready to send to discord listing each player and their value.</returns>
        public static string RankedCountResultToString(List<Database.CountResult> rankings)
        {
            string res = "";
            for (int i = 0; i < rankings.Count; i++)
            {
                res += $"{i + 1}: {rankings[i].Name}: {rankings[i].Count}{Environment.NewLine}";
            }
            return res;
        }

        // https://stackoverflow.com/a/4423615/624900
        private static string ToReadableString(TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? string.Empty : "s") : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? string.Empty : "s") : string.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? string.Empty : "s") : string.Empty,
                span.Duration().Seconds > 0 ? string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? string.Empty : "s") : string.Empty);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

            return formatted;
        }

        /// <summary>
        /// Same as RankedCountResultToString but formats as a time duration, assuming integer seconds.
        /// </summary>
        /// <param name="rankings">A pre-sorted list of CountResults.</param>
        /// <returns>String ready to send to discord listing each player and their duration.</returns>
        public static string RankedSecondsToString(List<Database.CountResult> rankings)
        {
            string res = "";
            for (int i = 0; i < rankings.Count; i++)
            {
                string formattedDuration = ToReadableString(TimeSpan.FromSeconds(rankings[i].Count));
                res += $"{i + 1}: {rankings[i].Name}: {formattedDuration}{Environment.NewLine}";
            }
            return res;
        }
    }
}

namespace DiscordConnector.Leaderboards
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
}
