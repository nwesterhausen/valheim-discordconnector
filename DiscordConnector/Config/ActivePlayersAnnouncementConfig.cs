using BepInEx.Configuration;

namespace DiscordConnector.Config;
internal class ActivePlayersAnnouncementConfig
{
    private ConfigEntry<bool> enabled;
    private ConfigEntry<bool> disableWhenNooneOnline;
    private ConfigEntry<int> periodInMinutes;
    private ConfigEntry<bool> includeCurrentlyOnline;
    private ConfigEntry<bool> includePlayersToday;
    private ConfigEntry<bool> includePlayersPastWeek;
    private ConfigEntry<bool> includePlayersAllTime;

    private const string EnabledTitle = "Enabled";
    private const bool EnabledDefault = false;
    private const string EnableDescription = "Enable or disable the active players announcement being sent to Discord";

    private const string DisabledOfflineTitle = "Disable When No One Online";
    private const bool DisabledOfflineDefault = false;
    private const string DisabledOfflineDescription = "Enable or disable the active players announcement when no one is online";

    public const string PeriodTitle = "Sending Period";
    public const int PeriodDefault = 360;
    public const string PeriodDescription = "Set the number of minutes between a leader board announcement sent to discord. (Default period is 6 hours.)";

    private const string IncludeCurrentlyOnlineTitle = "Include Currently Online Players";
    private const bool IncludeCurrentlyOnlineDefault = true;
    private const string IncludeCurrentlyOnlineDescription = "Enable or disable currently online players as part of the active players announcement";

    private const string IncludePlayersTodayTitle = "Include Unique Players for Today";
    private const bool IncludePlayersTodayDefault = true;
    private const string IncludePlayersTodayDescription = "Enable or disable unique online players for today as part of the active players announcement";

    private const string IncludePlayersPastWeekTitle = "Include Unique Players for the Past Week";
    private const bool IncludePlayersPastWeekDefault = true;
    private const string IncludePlayersPastWeekDescription = "Enable or disable unique online players for the past week (including today) as part of the active players announcement";

    private const string IncludePlayersAllTimeTitle = "Include Unique Players from All Time";
    private const bool IncludePlayersAllTimeDefault = true;
    private const string IncludePlayersAllTimeDescription = "Enable or disable unique online players from all time as part of the active players announcement";


    public ActivePlayersAnnouncementConfig(ConfigFile config, string header)
    {
        enabled = config.Bind<bool>(header,
            EnabledTitle,
            EnabledDefault,
            EnableDescription);

        disableWhenNooneOnline = config.Bind<bool>(header,
            DisabledOfflineTitle,
            DisabledOfflineDefault,
            DisabledOfflineDescription);

        periodInMinutes = config.Bind<int>(header,
            PeriodTitle,
            PeriodDefault,
            PeriodDescription
        );

        includeCurrentlyOnline = config.Bind<bool>(header,
            IncludeCurrentlyOnlineTitle,
            IncludeCurrentlyOnlineDefault,
            IncludeCurrentlyOnlineDescription);

        includePlayersToday = config.Bind<bool>(header,
            IncludePlayersTodayTitle,
            IncludePlayersTodayDefault,
            IncludePlayersTodayDescription);

        includePlayersPastWeek = config.Bind<bool>(header,
            IncludePlayersPastWeekTitle,
            IncludePlayersPastWeekDefault,
            IncludePlayersPastWeekDescription);

        includePlayersAllTime = config.Bind<bool>(header,
            IncludePlayersAllTimeTitle,
            IncludePlayersAllTimeDefault,
            IncludePlayersAllTimeDescription);
    }

    public string ConfigAsJson()
    {
        string jsonString = "{";
        jsonString += $"\"enabled\":\"{enabled.Value}\",";
        jsonString += $"\"disableWhenNooneOnline\":\"{disableWhenNooneOnline.Value}\",";
        jsonString += $"\"periodInMinutes\":{periodInMinutes.Value},";
        jsonString += $"\"includeCurrentlyOnline\":\"{includeCurrentlyOnline.Value}\",";
        jsonString += $"\"includePlayersToday\":\"{includePlayersToday.Value}\",";
        jsonString += $"\"includePlayersPastWeek\":\"{includePlayersPastWeek.Value}\",";
        jsonString += $"\"includePlayersAllTime\":\"{includePlayersAllTime.Value}\"";
        jsonString += "}";
        return jsonString;
    }

    public ActivePlayersAnnouncementConfigValues Value =>
     new ActivePlayersAnnouncementConfigValues
     {
         Enabled = enabled.Value,
         DisabledWhenNooneOnline = disableWhenNooneOnline.Value,
         PeriodInMinutes = periodInMinutes.Value,
         IncludeCurrentlyOnline = includeCurrentlyOnline.Value,
         IncludeTotalToday = includePlayersToday.Value,
         IncludeTotalPastWeek = includePlayersPastWeek.Value,
         IncludeTotalAllTime = includePlayersAllTime.Value
     };

}

public class ActivePlayersAnnouncementConfigValues
{
    public bool Enabled;
    public bool DisabledWhenNooneOnline;
    public int PeriodInMinutes;
    public bool IncludeCurrentlyOnline;
    public bool IncludeTotalToday;
    public bool IncludeTotalPastWeek;
    public bool IncludeTotalAllTime;
}
