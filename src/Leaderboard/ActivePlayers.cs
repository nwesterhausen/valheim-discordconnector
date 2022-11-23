
using System;
namespace DiscordConnector.LeaderBoards
{
    /// <summary>
    /// <para>
    ///     A board that posts periodically the number of unique players and other stats.
    /// </para>
    /// <para>
    ///     Provided example from github issue:
    /// <code>
    ///     SERVER PLAYERS
    ///     Online now: 4
    ///     Today: 8
    ///     Week: 29
    ///     Month: 31
    ///     Total Unique: 42
    ///     Most at once: 17
    /// </code>
    /// </para>
    /// <para>
    ///     What is currently supported:
    /// <code>
    ///     SERVER PLAYERS
    ///     Online now: 4
    ///     (unique) Players today: 5
    ///     (unique) Players this week: 6
    ///     (unique) Players all time: 12
    /// </code>
    /// </para>
    /// </summary>
    internal class ActivePlayersBoard
    {
        /// <summary>
        /// Return the number of currently online players. This is grabbed from the GetAllCharacterZDOS method.
        /// </summary>
        /// <returns>Count of character ZDOS</returns>
        private static int CurrentOnlinePlayers()
        {
            return ZNet.instance.GetAllCharacterZDOS().Count;
        }

        /// <summary>
        /// Send an announcement leader board to Discord with the current active players and total unique players for some values.
        /// To count unique players we do a trick by getting the total number of Joins for each player for each time range we care
        /// about and then doing an meta count of how many records come back (see <see cref="Records.Helper.CountUniquePlayers"/>).
        /// </summary>
        public static void SendActivePlayersBoard()
        {
            int currentlyOnline = CurrentOnlinePlayers();
            int uniqueToday = Records.Helper.CountUniquePlayers(Records.Categories.Join, TimeRange.Today);
            int uniqueThisWeek = Records.Helper.CountUniquePlayers(Records.Categories.Join, TimeRange.PastWeek);
            int uniqueAllTime = Records.Helper.CountUniquePlayers(Records.Categories.Join, TimeRange.AllTime);

            string formattedAnnouncement = $"**Active Players**\n\nOnline now: {currentlyOnline}\n" +
                $"Players today: {uniqueToday}\n" +
                $"This week: {uniqueThisWeek}\n" +
                $"All time: {uniqueAllTime}";

            DiscordApi.SendMessage(formattedAnnouncement);
        }
    }
}