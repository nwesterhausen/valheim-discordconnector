using System.Timers;

namespace DiscordConnector.LeaderBoards;

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
///     Active Players
///     Online now: 1
///     Players today: 2
///     This week: 2
///     All time: 0
/// </code>
/// </para>
/// </summary>
internal static class ActivePlayersAnnouncement
{
    /// <summary>
    /// Return the number of currently online players. This is grabbed from the GetAllCharacterZDOS method.
    /// </summary>
    /// <returns>Count of character ZDOS</returns>
    private static int CurrentOnlinePlayers()
    {
        if (ZNet.instance == null)
        {
            Plugin.StaticLogger.LogDebug("ActivePlayersAnnouncement: ZNet instance is null, cannot get online players");
            return 0;
        }
        return ZNet.instance.GetNrOfPlayers();
    }

    /// <summary>
    /// Send an announcement leader board to Discord with the current active players and total unique players for some values.
    /// To count unique players we do a trick by getting the total number of Joins for each player for each time range we care
    /// about and then doing an meta count of how many records come back (see <see cref="Records.Helper.CountUniquePlayers"/>).
    /// </summary>
    private static void SendActivePlayersBoard()
    {
        if (Plugin.StaticConfig.ActivePlayersAnnouncement.DisabledWhenNooneOnline && CurrentOnlinePlayers() == 0)
        {
            Plugin.StaticLogger.LogDebug("ActivePlayersAnnouncement: No one is online and the config is set to disable when no one is online. Not sending announcement.");
            return;
        }

        Webhook.Event ev = Webhook.Event.ActivePlayers;
        string formattedAnnouncement = $"**Active Players**\n";
        if (Plugin.StaticConfig.ActivePlayersAnnouncement.IncludeCurrentlyOnline)
        {
            int currentlyOnline = CurrentOnlinePlayers();
            formattedAnnouncement += $"Online now: {currentlyOnline}\n";
        }

        if (Plugin.StaticConfig.ActivePlayersAnnouncement.IncludeTotalToday)
        {
            int uniqueToday = Records.Helper.CountUniquePlayers(Records.Categories.Join, TimeRange.Today);
            formattedAnnouncement += $"Players today: {uniqueToday}\n";
        }

        if (Plugin.StaticConfig.ActivePlayersAnnouncement.IncludeTotalPastWeek)
        {
            int uniqueThisWeek = Records.Helper.CountUniquePlayers(Records.Categories.Join, TimeRange.PastWeek);
            formattedAnnouncement += $"This week: {uniqueThisWeek}\n";
        }

        if (Plugin.StaticConfig.ActivePlayersAnnouncement.IncludeTotalAllTime)
        {
            int uniqueAllTime = Records.Helper.CountUniquePlayers(Records.Categories.Join, TimeRange.AllTime);
            formattedAnnouncement += $"All time: {uniqueAllTime}";
        }


        DiscordApi.SendMessage(ev, formattedAnnouncement);
    }

    /// <summary>
    /// An interface for sending the leader board as a timer event.
    /// </summary>
    public static void SendOnTimer(object sender, ElapsedEventArgs elapsedEventArgs)
    {
        System.Threading.Tasks.Task.Run(() =>
        {
            SendActivePlayersBoard();
        });
    }
}
