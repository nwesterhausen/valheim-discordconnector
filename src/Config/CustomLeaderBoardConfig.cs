using BepInEx.Configuration;

namespace DiscordConnector.Config
{
    internal class LeaderBoardConfigValues
    {
        // Each leader board has these values to configure
        public ConfigEntry<LeaderBoards.Ordering> type;
        public ConfigEntry<LeaderBoards.TimeRange> timeRange;
        public ConfigEntry<int> numberListings;
        public ConfigEntry<bool> enabled;
        public ConfigEntry<int> periodInMinutes;
        public ConfigEntry<bool> deaths;
        public ConfigEntry<bool> sessions;
        public ConfigEntry<bool> shouts;
        public ConfigEntry<bool> pings;
        public ConfigEntry<bool> timeOnline;
        public ConfigEntry<string> displayedHeading;

        public const string EnabledTitle = "Enabled";
        public const bool EnabledDefault = false;
        public const string EnableDescription = "Enable or disable this leader board.";

        public const string TimeRangeTitle = "Leader Board Time Range";
        public const LeaderBoards.TimeRange TimeRangeDefault = LeaderBoards.TimeRange.AllTime;
        public const string TimeRangeDescription = "A more restrictive filter of time can be applied to the leader board. This restricts it to tally up statistics within the range specified.";
        public static readonly string TimeRangeDescription1 = $"{LeaderBoards.TimeRange.AllTime}: Apply no time restriction to the leader board, use all available records.";
        public static readonly string TimeRangeDescription2 = $"{LeaderBoards.TimeRange.Today}: Restrict leader board to recorded events from today.";
        public static readonly string TimeRangeDescription3 = $"{LeaderBoards.TimeRange.Yesterday}: Restrict leader board to recorded events from yesterday.";
        public static readonly string TimeRangeDescription4 = $"{LeaderBoards.TimeRange.PastWeek}: Restrict leader board to recorded events from the past week (including today).";
        public static readonly string TimeRangeDescription5 = $"{LeaderBoards.TimeRange.WeekSundayToSaturday}: Restrict leader board to recorded events from the current week, beginning on Sunday and ending Saturday.";
        public static readonly string TimeRangeDescription6 = $"{LeaderBoards.TimeRange.WeekMondayToSunday}: Restrict leader board to recorded events from the current week, beginning on Monday and ending Sunday.";

        public const string NumberListingsTitle = "Number of Rankings";
        public const int NumberListingsDefault = 3;
        public const string NumberListingsDescription = "Specify a number of places in the leader board. Setting this can help prevent a very long leader board in the case of active servers.";
        public static readonly string NumberListingsDescription1 = $"Setting to 0 (zero) results in limiting to the hard-coded maximum of {LeaderBoard.MAX_LEADER_BOARD_SIZE}.";

        public const string TypeTitle = "Type";
        public const LeaderBoards.Ordering TypeDefault = LeaderBoards.Ordering.Descending;
        public const string TypeDescription = "Choose what type of leader board this should be. There are 2 options:";
        public static readonly string TypeDescription1 = $"{LeaderBoards.Ordering.Descending}:\"Number of Rankings\" players (with at least 1 record) are listed in descending order";
        public static readonly string TypeDescription2 = $"{LeaderBoards.Ordering.Ascending}:  \"Number of Rankings\" players (with at least 1 record) are listed in ascending order";

        public const string PeriodTitle = "Sending Period";
        public const int PeriodDefault = 600;
        public const string PeriodDescription = "Set the number of minutes between a leader board announcement sent to discord.";
        public const string PeriodDescription1 = "This timer starts when the server is started. Default is set to 10 hours (600 minutes).";

        public const string DeathsTitle = "Death Statistics";
        public const bool DeathsDefault = true;
        public const string DeathsDescription = "If enabled, player death statistics will be part of the leader board.";

        public const string SessionsTitle = "Session Statistics";
        public const bool SessionsDefault = false;
        public const string SessionsDescription = "If enabled, player session statistics will be part of the leader board.";

        public const string ShoutsTitle = "Shout Statistics";
        public const bool ShoutsDefault = false;
        public const string ShoutsDescription = "If enabled, player shout statistics will be part of the leader board.";

        public const string PingsTitle = "Ping Statistics";
        public const bool PingsDefault = false;
        public const string PingsDescription = "If enabled, player ping statistics will be part of the leader board.";

        public const string TimeOnlineTitle = "Time Online Statistics";
        public const bool TimeOnlineDefault = false;
        public const string TimeOnlineDescription = "If enabled, player online time statistics will be part of the leader board.";

        public const string DisplayedHeadingTitle = "Leader Board Heading";
        public const string DisplayedHeadingDescription = "Define the heading message to display with this leader board.";
        public const string DisplayedHeadingDescription1 = "Include %N% to dynamically reference the value in \"Number of Rankings\"";

        public LeaderBoardConfigValues(ConfigFile config, string header)
        {
            enabled = config.Bind<bool>(header,
                EnabledTitle,
                EnabledDefault,
                EnableDescription);

            displayedHeading = config.Bind<string>(header,
                DisplayedHeadingTitle,
                $"{header} Statistic Leader Board",
                DisplayedHeadingDescription + System.Environment.NewLine +
                DisplayedHeadingDescription1);

            timeRange = config.Bind<LeaderBoards.TimeRange>(header,
                TimeRangeTitle,
                TimeRangeDefault,
                TimeRangeDescription + System.Environment.NewLine +
                TimeRangeDescription1 + System.Environment.NewLine +
                TimeRangeDescription2 + System.Environment.NewLine +
                TimeRangeDescription3 + System.Environment.NewLine +
                TimeRangeDescription4 + System.Environment.NewLine +
                TimeRangeDescription5 + System.Environment.NewLine +
                TimeRangeDescription6
                );

            periodInMinutes = config.Bind<int>(header,
                PeriodTitle,
                PeriodDefault,
                PeriodDescription + System.Environment.NewLine +
                PeriodDescription1
            );

            type = config.Bind<LeaderBoards.Ordering>(header,
                TypeTitle,
                TypeDefault,
                TypeDescription + System.Environment.NewLine +
                TypeDescription1 + System.Environment.NewLine +
                TypeDescription2
            );

            numberListings = config.Bind<int>(header,
                NumberListingsTitle,
                NumberListingsDefault,
                new ConfigDescription(
                    NumberListingsDescription + System.Environment.NewLine +
                    NumberListingsDescription1,
                new AcceptableValueRange<int>(0, LeaderBoard.MAX_LEADER_BOARD_SIZE * 3)
            ));

            deaths = config.Bind<bool>(header,
                DeathsTitle,
                DeathsDefault,
                DeathsDescription);

            sessions = config.Bind<bool>(header,
                SessionsTitle,
                SessionsDefault,
                SessionsDescription);

            shouts = config.Bind<bool>(header,
                ShoutsTitle,
                ShoutsDefault,
                ShoutsDescription);

            pings = config.Bind<bool>(header,
                PingsTitle,
                PingsDefault,
                PingsDescription);

            timeOnline = config.Bind<bool>(header,
                TimeOnlineTitle,
                TimeOnlineDefault,
                TimeOnlineDescription);
        }

        public string ConfigAsJson()
        {
            string jsonString = "{";
            jsonString += $"\"enabled\":\"{enabled.Value}\",";
            jsonString += $"\"periodInMinutes\":{periodInMinutes.Value},";
            jsonString += $"\"displayedHeading\":\"{displayedHeading.Value}\",";
            jsonString += $"\"type\":\"{type.Value}\",";
            jsonString += $"\"timeRange\":\"{timeRange.Value}\",";
            jsonString += $"\"numberListings\":{numberListings.Value},";
            jsonString += $"\"deaths\":\"{deaths.Value}\",";
            jsonString += $"\"sessions\":\"{sessions.Value}\",";
            jsonString += $"\"shouts\":\"{shouts.Value}\",";
            jsonString += $"\"pings\":\"{pings.Value}\",";
            jsonString += $"\"timeOnline\":\"{timeOnline.Value}\"";
            jsonString += "}";
            return jsonString;
        }
    }

    public class LeaderBoardConfigReference
    {
        public LeaderBoards.Ordering Type;
        public LeaderBoards.TimeRange TimeRange;
        public int NumberListings;
        public bool Enabled;
        public int PeriodInMinutes;
        public bool Deaths;
        public bool Sessions;
        public bool Shouts;
        public bool Pings;
        public bool TimeOnline;
        public string DisplayedHeading;
    }
}
