using System.Threading.Tasks;
using System.Timers;
using DiscordConnector.Records;

namespace DiscordConnector.Leaderboards;

/// <summary>
///     <para>
///         A board that posts periodically the number of unique players and other stats.
///     </para>
///     <para>
///         Provided example from github issue:
///         <code>
///     SERVER PLAYERS
///     Online now: 4
///     Today: 8
///     Week: 29
///     Month: 31
///     Total Unique: 42
///     Most at once: 17
/// </code>
///     </para>
///     <para>
///         What is currently supported:
///         <code>
///     Active Players
///     Online now: 1
///     Players today: 2
///     This week: 2
///     All time: 0
/// </code>
///     </para>
/// </summary>
internal static class ActivePlayersAnnouncement
{
    /// <summary>
    ///     Return the number of currently online players. This is grabbed from the GetAllCharacterZDOS method.
    /// </summary>
    /// <returns>Count of character ZDOS</returns>
    private static int CurrentOnlinePlayers()
    {
        if (ZNet.instance == null)
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug(
                "ActivePlayersAnnouncement: ZNet instance is null, cannot get online players");
            return 0;
        }

        return ZNet.instance.GetNrOfPlayers();
    }

    /// <summary>
    ///     Send an announcement leader board to Discord with the current active players and total unique players for some
    ///     values.
    ///     To count unique players we do a trick by getting the total number of Joins for each player for each time range we
    ///     care
    ///     about and then doing an meta count of how many records come back (see
    ///     <see cref="Records.Helper.CountUniquePlayers" />).
    /// </summary>
    private static void SendActivePlayersBoard()
    {
        if (DiscordConnectorPlugin.StaticConfig.ActivePlayersAnnouncement.DisabledWhenNooneOnline &&
            CurrentOnlinePlayers() == 0)
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug(
                "ActivePlayersAnnouncement: No one is online and the config is set to disable when no one is online. Not sending announcement.");
            return;
        }

        Webhook.Event ev = Webhook.Event.ActivePlayers;
        string statsAnnouncement = "";
        if (DiscordConnectorPlugin.StaticConfig.ActivePlayersAnnouncement.IncludeCurrentlyOnline)
        {
            int currentlyOnline = CurrentOnlinePlayers();
            statsAnnouncement += $"Online now: {currentlyOnline}\n";
        }

        if (DiscordConnectorPlugin.StaticConfig.ActivePlayersAnnouncement.IncludeTotalToday)
        {
            int uniqueToday = Helper.CountUniquePlayers(Categories.Join, TimeRange.Today);
            statsAnnouncement += $"Players today: {uniqueToday}\n";
        }

        if (DiscordConnectorPlugin.StaticConfig.ActivePlayersAnnouncement.IncludeTotalPastWeek)
        {
            int uniqueThisWeek = Helper.CountUniquePlayers(Categories.Join, TimeRange.PastWeek);
            statsAnnouncement += $"This week: {uniqueThisWeek}\n";
        }

        if (DiscordConnectorPlugin.StaticConfig.ActivePlayersAnnouncement.IncludeTotalAllTime)
        {
            int uniqueAllTime = Helper.CountUniquePlayers(Categories.Join, TimeRange.AllTime);
            statsAnnouncement += $"All time: {uniqueAllTime}";
        }

        // Get world name from ZNet if available
        string worldName = "";
        if (ZNet.instance != null)
        {
            worldName = ZNet.instance.GetWorldName();
        }
        
        // If embeds are enabled, use the new embed template
        if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled)
        {
            var embedBuilder = EmbedTemplates.ActivePlayersAnnouncement(statsAnnouncement, worldName);
            DiscordApi.SendEmbed(ev, embedBuilder);
        }
        else
        {
            // Fallback to plain text message if embeds are disabled
            string formattedAnnouncement = "**Active Players**\n" + statsAnnouncement;
            DiscordApi.SendMessage(ev, formattedAnnouncement);
        }
    }

    /// <summary>
    ///     An interface for sending the leader board as a timer event.
    /// </summary>
    public static void SendOnTimer(object sender, ElapsedEventArgs elapsedEventArgs)
    {
        Task.Run(() =>
        {
            SendActivePlayersBoard();
        });
    }
}
