using System;
using BepInEx.Configuration;

namespace DiscordConnector.Config
{
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

        public static readonly string EnabledTitle = "Enabled";
        public static readonly bool EnabledDefault = false;
        public static readonly string EnableDescription = "Enable or disable this leader board.";

        public static readonly string TimeRangeTitle = "Leader Board Time Range";
        public static readonly LeaderBoards.TimeRange TimeRangeDefault = LeaderBoards.TimeRange.AllTime;
        public static readonly string TimeRangeDescription = "A more restrictive filter of time can be applied to the leader board. This restricts it to tally up statistics within the range specified.";
        public static readonly string TimeRangeDescription1 = $"{LeaderBoards.TimeRange.AllTime}: Apply no time restriction to the leader board, use all available records.";
        public static readonly string TimeRangeDescription2 = $"{LeaderBoards.TimeRange.Today}: Restrict leader board to recorded events from today.";
        public static readonly string TimeRangeDescription3 = $"{LeaderBoards.TimeRange.Yesterday}: Restrict leader board to recorded events from yesterday.";
        public static readonly string TimeRangeDescription4 = $"{LeaderBoards.TimeRange.PastWeek}: Restrict leader board to recorded events from the past week (including today).";
        public static readonly string TimeRangeDescription5 = $"{LeaderBoards.TimeRange.WeekSundayToSaturday}: Restrict leader board to recorded events from the current week, beginning on Sunday and ending Saturday.";
        public static readonly string TimeRangeDescription6 = $"{LeaderBoards.TimeRange.WeekMondayToSunday}: Restrict leader board to recorded events from the current week, beginning on Monday and ending Sunday.";

        public static readonly string NumberListingsTitle = "Number of Rankings";
        public static readonly int NumberListingsDefault = 3;
        public static readonly string NumberListingsDescription = "Specify a number of places in the leader board. Setting this can help prevent a very long leader board in the case of active servers.";
        public static readonly string NumberListingsDescription1 = $"Setting to 0 (zero) results in limiting to the hard-coded maximum of {LeaderBoard.MAX_LEADER_BOARD_SIZE}.";

        public static readonly string TypeTitle = "Type";
        public static readonly LeaderBoards.Ordering TypeDefault = LeaderBoards.Ordering.Descending;
        public static readonly string TypeDescription = "Choose what type of leader board this should be. There are 2 options:";
        public static readonly string TypeDescription1 = $"{LeaderBoards.Ordering.Descending}:\"Number of Rankings\" players (with at least 1 record) are listed in descending order";
        public static readonly string TypeDescription2 = $"{LeaderBoards.Ordering.Ascending}:  \"Number of Rankings\" players (with at least 1 record) are listed in ascending order";

        public static readonly string PeriodTitle = "Sending Period";
        public static readonly int PeriodDefault = 600;
        public static readonly string PeriodDescription = "Set the number of minutes between a leader board announcement sent to discord.";
        public static readonly string PeriodDescription1 = "This timer starts when the server is started. Default is set to 10 hours (600 minutes).";

        public static readonly string DeathsTitle = "Death Statistics";
        public static readonly bool DeathsDefault = true;
        public static readonly string DeathsDescription = "If enabled, player death statistics will be part of the leader board.";

        public static readonly string SessionsTitle = "Session Statistics";
        public static readonly bool SessionsDefault = false;
        public static readonly string SessionsDescription = "If enabled, player session statistics will be part of the leader board.";

        public static readonly string ShoutsTitle = "Shout Statistics";
        public static readonly bool ShoutsDefault = false;
        public static readonly string ShoutsDescription = "If enabled, player shout statistics will be part of the leader board.";

        public static readonly string PingsTitle = "Ping Statistics";
        public static readonly bool PingsDefault = false;
        public static readonly string PingsDescription = "If enabled, player ping statistics will be part of the leader board.";

        public static readonly string TimeOnlineTitle = "Time Online Statistics";
        public static readonly bool TimeOnlineDefault = false;
        public static readonly string TimeOnlineDescription = "If enabled, player online time statistics will be part of the leader board.";

        public static readonly string DisplayedHeadingTitle = "Leader Board Heading";
        public static readonly string DisplayedHeadingDescription = "Define the heading message to display with this leader board.";
        public static readonly string DisplayedHeadingDescription1 = "Include %N% to dynamically reference the value in \"Number of Rankings\"";

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
    internal class LeaderBoardConfig
    {
        private static ConfigFile config;

        public static string ConfigExtension = "leaderBoards";

        // config header strings
        private const string LEADER_BOARD_1 = "LeaderBoard.1";
        private const string LEADER_BOARD_2 = "LeaderBoard.2";
        private const string LEADER_BOARD_3 = "LeaderBoard.3";
        private const string LEADER_BOARD_4 = "LeaderBoard.4";
        private const string LEADER_BOARD_5 = "LeaderBoard.5";

        // Config Definitions
        private LeaderBoardConfigValues leaderBoard1;
        private LeaderBoardConfigValues leaderBoard2;
        private LeaderBoardConfigValues leaderBoard3;
        private LeaderBoardConfigValues leaderBoard4;
        private LeaderBoardConfigValues leaderBoard5;
        private LeaderBoardConfigReference[] _leaderBoards;

        public LeaderBoardConfig(ConfigFile configFile)
        {
            config = configFile;
            LoadConfig();
        }

        public void ReloadConfig()
        {
            config.Reload();
            config.Save();
        }
        private void LoadConfig()
        {
            leaderBoard1 = new LeaderBoardConfigValues(config, LEADER_BOARD_1);
            leaderBoard2 = new LeaderBoardConfigValues(config, LEADER_BOARD_2);
            leaderBoard3 = new LeaderBoardConfigValues(config, LEADER_BOARD_3);
            leaderBoard4 = new LeaderBoardConfigValues(config, LEADER_BOARD_4);
            leaderBoard5 = new LeaderBoardConfigValues(config, LEADER_BOARD_5);

            config.Save();
            _leaderBoards = new LeaderBoardConfigReference[]{
            new LeaderBoardConfigReference
            {
                Type = leaderBoard1.type.Value,
                TimeRange = leaderBoard1.timeRange.Value,
                DisplayedHeading = leaderBoard1.displayedHeading.Value,
                NumberListings = leaderBoard1.numberListings.Value == 0 ? LeaderBoard.MAX_LEADER_BOARD_SIZE : leaderBoard1.numberListings.Value,
                Enabled = leaderBoard1.enabled.Value,
                PeriodInMinutes = leaderBoard1.periodInMinutes.Value,
                Deaths = leaderBoard1.deaths.Value,
                Sessions = leaderBoard1.sessions.Value,
                Shouts = leaderBoard1.shouts.Value,
                Pings = leaderBoard1.pings.Value,
                TimeOnline = leaderBoard1.timeOnline.Value,
            },
            new LeaderBoardConfigReference
            {
                Type = leaderBoard2.type.Value,
                TimeRange = leaderBoard2.timeRange.Value,
                DisplayedHeading = leaderBoard2.displayedHeading.Value,
                NumberListings = leaderBoard2.numberListings.Value == 0 ? LeaderBoard.MAX_LEADER_BOARD_SIZE : leaderBoard2.numberListings.Value,
                Enabled = leaderBoard2.enabled.Value,
                PeriodInMinutes = leaderBoard2.periodInMinutes.Value,
                Deaths = leaderBoard2.deaths.Value,
                Sessions = leaderBoard2.sessions.Value,
                Shouts = leaderBoard2.shouts.Value,
                Pings = leaderBoard2.pings.Value,
                TimeOnline = leaderBoard2.timeOnline.Value,
            },
            new LeaderBoardConfigReference
            {
                Type = leaderBoard3.type.Value,
                TimeRange = leaderBoard3.timeRange.Value,
                DisplayedHeading = leaderBoard3.displayedHeading.Value,
                NumberListings = leaderBoard3.numberListings.Value == 0 ? LeaderBoard.MAX_LEADER_BOARD_SIZE : leaderBoard3.numberListings.Value,
                Enabled = leaderBoard3.enabled.Value,
                PeriodInMinutes = leaderBoard3.periodInMinutes.Value,
                Deaths = leaderBoard3.deaths.Value,
                Sessions = leaderBoard3.sessions.Value,
                Shouts = leaderBoard3.shouts.Value,
                Pings = leaderBoard3.pings.Value,
                TimeOnline = leaderBoard3.timeOnline.Value,
            },
            new LeaderBoardConfigReference
            {
                Type = leaderBoard4.type.Value,
                TimeRange = leaderBoard4.timeRange.Value,
                DisplayedHeading = leaderBoard4.displayedHeading.Value,
                NumberListings = leaderBoard4.numberListings.Value == 0 ? LeaderBoard.MAX_LEADER_BOARD_SIZE : leaderBoard4.numberListings.Value,
                Enabled = leaderBoard4.enabled.Value,
                PeriodInMinutes = leaderBoard4.periodInMinutes.Value,
                Deaths = leaderBoard4.deaths.Value,
                Sessions = leaderBoard4.sessions.Value,
                Shouts = leaderBoard4.shouts.Value,
                Pings = leaderBoard4.pings.Value,
                TimeOnline = leaderBoard4.timeOnline.Value,
            },
            new LeaderBoardConfigReference
            {
                Type = leaderBoard5.type.Value,
                TimeRange = leaderBoard5.timeRange.Value,
                DisplayedHeading = leaderBoard5.displayedHeading.Value,
                NumberListings = leaderBoard5.numberListings.Value == 0 ? LeaderBoard.MAX_LEADER_BOARD_SIZE : leaderBoard5.numberListings.Value,
                Enabled = leaderBoard5.enabled.Value,
                PeriodInMinutes = leaderBoard5.periodInMinutes.Value,
                Deaths = leaderBoard5.deaths.Value,
                Sessions = leaderBoard5.sessions.Value,
                Shouts = leaderBoard5.shouts.Value,
                Pings = leaderBoard5.pings.Value,
                TimeOnline = leaderBoard5.timeOnline.Value,
            }};

        }

        public string ConfigAsJson()
        {
            string jsonString = "{";
            jsonString += $"\"leaderBoard1\":{leaderBoard1.ConfigAsJson()},";
            jsonString += $"\"leaderBoard2\":{leaderBoard2.ConfigAsJson()},";
            jsonString += $"\"leaderBoard3\":{leaderBoard3.ConfigAsJson()},";
            jsonString += $"\"leaderBoard4\":{leaderBoard4.ConfigAsJson()},";
            jsonString += $"\"leaderBoard5\":{leaderBoard5.ConfigAsJson()}";
            jsonString += "}";
            return jsonString;
        }
        // Variables
        public LeaderBoardConfigReference[] LeaderBoards => _leaderBoards;
    }
}
