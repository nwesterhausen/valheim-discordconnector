using System;
using BepInEx.Configuration;

namespace DiscordConnector.Config
{
    public class LeaderboardConfigReference
    {
        public Leaderboards.Ordering Type;
        public Leaderboards.TimeRange TimeRange;
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
    internal class LeaderboardConfigValues
    {
        // Each leaderboard has these values to configure
        public ConfigEntry<Leaderboards.Ordering> type;
        public ConfigEntry<Leaderboards.TimeRange> timeRange;
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
        public static readonly string EnableDescription = "Enable or disable this leaderboard.";

        public static readonly string TimeRangeTitle = "Leaderboard Time Range";
        public static readonly Leaderboards.TimeRange TimeRangeDefault = Leaderboards.TimeRange.AllTime;
        public static readonly string TimeRangeDescription = "A more restrictive filter of time can be applied to the leaderboard. This restricts it to tally up statistics within the range specified.";
        public static readonly string TimeRangeDescription1 = $"{Leaderboards.TimeRange.AllTime}: Apply no time restriction to the leaderboard, use all available records.";
        public static readonly string TimeRangeDescription2 = $"{Leaderboards.TimeRange.Today}: Restrict leaderboard to recorded events from today.";
        public static readonly string TimeRangeDescription3 = $"{Leaderboards.TimeRange.Yesterday}: Restrict leaderboard to recorded events from yesterday.";
        public static readonly string TimeRangeDescription4 = $"{Leaderboards.TimeRange.PastWeek}: Restrict leaderboard to recorded events from the past week (including today).";
        public static readonly string TimeRangeDescription5 = $"{Leaderboards.TimeRange.WeekSundayToSaturday}: Restrict leaderboard to recorded events from the current week, beginning on Sunday and ending Saturday.";
        public static readonly string TimeRangeDescription6 = $"{Leaderboards.TimeRange.WeekMondayToSunday}: Restrict leaderboard to recorded events from the current week, beginning on Monday and ending Sunday.";

        public static readonly string NumberListingsTitle = "Number of Rankings";
        public static readonly int NumberListingsDefault = 3;
        public static readonly string NumberListingsDescription = "Specify a number of places in the leaderboard. Setting this can help prevent a very long leaderboard in the case of active servers.";
        public static readonly string NumberListingsDescription1 = $"Setting to 0 (zero) results in limiting to the hard-coded maximum of {Leaderboard.MAX_LEADERBOARD_SIZE}.";

        public static readonly string TypeTitle = "Type";
        public static readonly Leaderboards.Ordering TypeDefault = Leaderboards.Ordering.Descending;
        public static readonly string TypeDescription = "Choose what type of leaderboard this should be. There are 2 options:";
        public static readonly string TypeDescription1 = $"{Leaderboards.Ordering.Descending}:\"Number of Rankings\" players (with at least 1 record) are listed in descending order";
        public static readonly string TypeDescription2 = $"{Leaderboards.Ordering.Ascending}:  \"Number of Rankings\" players (with at least 1 record) are listed in ascending order";

        public static readonly string PeriodTitle = "Sending Period";
        public static readonly int PeriodDefault = 600;
        public static readonly string PeriodDescription = "Set the number of minutes between a leaderboard announcement sent to discord.";
        public static readonly string PeriodDescription1 = "This timer starts when the server is started. Default is set to 10 hours (600 mintues).";

        public static readonly string DeathsTitle = "Death Statistics";
        public static readonly bool DeathsDefault = true;
        public static readonly string DeathsDescription = "If enabled, player death statistics will be part of the leaderboard.";

        public static readonly string SessionsTitle = "Session Statistics";
        public static readonly bool SessionsDefault = false;
        public static readonly string SessionsDescription = "If enabled, player session statistics will be part of the leaderboard.";

        public static readonly string ShoutsTitle = "Shout Statistics";
        public static readonly bool ShoutsDefault = false;
        public static readonly string ShoutsDescription = "If enabled, player shout statistics will be part of the leaderboard.";

        public static readonly string PingsTitle = "Ping Statistics";
        public static readonly bool PingsDefault = false;
        public static readonly string PingsDescription = "If enabled, player ping statistics will be part of the leaderboard.";

        public static readonly string TimeOnlineTitle = "Time Online Statistics";
        public static readonly bool TimeOnlineDefault = false;
        public static readonly string TimeOnlineDescription = "If enabled, player online time statistics will be part of the leaderboard.";

        public static readonly string DisplayedHeadingTitle = "Leaderboard Heading";
        public static readonly string DisplayedHeadingDescription = "Define the heading message to display with this leaderboard.";
        public static readonly string DisplayedHeadingDescription1 = "Include %N% to dynamically reference the value in \"Number of Rankings\"";

        public LeaderboardConfigValues(ConfigFile config, string header)
        {
            enabled = config.Bind<bool>(header,
                EnabledTitle,
                EnabledDefault,
                EnableDescription);

            displayedHeading = config.Bind<string>(header,
                DisplayedHeadingTitle,
                $"{header} Statistic Leaderboard",
                DisplayedHeadingDescription + System.Environment.NewLine +
                DisplayedHeadingDescription1);

            timeRange = config.Bind<Leaderboards.TimeRange>(header,
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

            type = config.Bind<Leaderboards.Ordering>(header,
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
                new AcceptableValueRange<int>(0, Leaderboard.MAX_LEADERBOARD_SIZE * 3)
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
    internal class LeaderboardConfig
    {
        private static ConfigFile config;

        public static string ConfigExtension = "leaderboards";

        // config header strings
        private const string LEADERBOARD_1 = "Leaderboard.1";
        private const string LEADERBOARD_2 = "Leaderboard.2";
        private const string LEADERBOARD_3 = "Leaderboard.3";
        private const string LEADERBOARD_4 = "Leaderboard.4";
        private const string LEADERBOARD_5 = "Leaderboard.5";

        // Config Definitions
        private LeaderboardConfigValues leaderboard1;
        private LeaderboardConfigValues leaderboard2;
        private LeaderboardConfigValues leaderboard3;
        private LeaderboardConfigValues leaderboard4;
        private LeaderboardConfigValues leaderboard5;
        private LeaderboardConfigReference[] _leaderboards;

        public LeaderboardConfig(ConfigFile configFile)
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
            leaderboard1 = new LeaderboardConfigValues(config, LEADERBOARD_1);
            leaderboard2 = new LeaderboardConfigValues(config, LEADERBOARD_2);
            leaderboard3 = new LeaderboardConfigValues(config, LEADERBOARD_3);
            leaderboard4 = new LeaderboardConfigValues(config, LEADERBOARD_4);
            leaderboard5 = new LeaderboardConfigValues(config, LEADERBOARD_5);

            config.Save();
            _leaderboards = new LeaderboardConfigReference[]{
            new LeaderboardConfigReference
            {
                Type = leaderboard1.type.Value,
                TimeRange = leaderboard1.timeRange.Value,
                DisplayedHeading = leaderboard1.displayedHeading.Value,
                NumberListings = leaderboard1.numberListings.Value == 0 ? Leaderboard.MAX_LEADERBOARD_SIZE : leaderboard1.numberListings.Value,
                Enabled = leaderboard1.enabled.Value,
                PeriodInMinutes = leaderboard1.periodInMinutes.Value,
                Deaths = leaderboard1.deaths.Value,
                Sessions = leaderboard1.sessions.Value,
                Shouts = leaderboard1.shouts.Value,
                Pings = leaderboard1.pings.Value,
                TimeOnline = leaderboard1.timeOnline.Value,
            },
            new LeaderboardConfigReference
            {
                Type = leaderboard2.type.Value,
                TimeRange = leaderboard2.timeRange.Value,
                DisplayedHeading = leaderboard2.displayedHeading.Value,
                NumberListings = leaderboard2.numberListings.Value == 0 ? Leaderboard.MAX_LEADERBOARD_SIZE : leaderboard2.numberListings.Value,
                Enabled = leaderboard2.enabled.Value,
                PeriodInMinutes = leaderboard2.periodInMinutes.Value,
                Deaths = leaderboard2.deaths.Value,
                Sessions = leaderboard2.sessions.Value,
                Shouts = leaderboard2.shouts.Value,
                Pings = leaderboard2.pings.Value,
                TimeOnline = leaderboard2.timeOnline.Value,
            },
            new LeaderboardConfigReference
            {
                Type = leaderboard3.type.Value,
                TimeRange = leaderboard3.timeRange.Value,
                DisplayedHeading = leaderboard3.displayedHeading.Value,
                NumberListings = leaderboard3.numberListings.Value == 0 ? Leaderboard.MAX_LEADERBOARD_SIZE : leaderboard3.numberListings.Value,
                Enabled = leaderboard3.enabled.Value,
                PeriodInMinutes = leaderboard3.periodInMinutes.Value,
                Deaths = leaderboard3.deaths.Value,
                Sessions = leaderboard3.sessions.Value,
                Shouts = leaderboard3.shouts.Value,
                Pings = leaderboard3.pings.Value,
                TimeOnline = leaderboard3.timeOnline.Value,
            },
            new LeaderboardConfigReference
            {
                Type = leaderboard4.type.Value,
                TimeRange = leaderboard4.timeRange.Value,
                DisplayedHeading = leaderboard4.displayedHeading.Value,
                NumberListings = leaderboard4.numberListings.Value == 0 ? Leaderboard.MAX_LEADERBOARD_SIZE : leaderboard4.numberListings.Value,
                Enabled = leaderboard4.enabled.Value,
                PeriodInMinutes = leaderboard4.periodInMinutes.Value,
                Deaths = leaderboard4.deaths.Value,
                Sessions = leaderboard4.sessions.Value,
                Shouts = leaderboard4.shouts.Value,
                Pings = leaderboard4.pings.Value,
                TimeOnline = leaderboard4.timeOnline.Value,
            },
            new LeaderboardConfigReference
            {
                Type = leaderboard5.type.Value,
                TimeRange = leaderboard5.timeRange.Value,
                DisplayedHeading = leaderboard5.displayedHeading.Value,
                NumberListings = leaderboard5.numberListings.Value == 0 ? Leaderboard.MAX_LEADERBOARD_SIZE : leaderboard5.numberListings.Value,
                Enabled = leaderboard5.enabled.Value,
                PeriodInMinutes = leaderboard5.periodInMinutes.Value,
                Deaths = leaderboard5.deaths.Value,
                Sessions = leaderboard5.sessions.Value,
                Shouts = leaderboard5.shouts.Value,
                Pings = leaderboard5.pings.Value,
                TimeOnline = leaderboard5.timeOnline.Value,
            }};

        }

        public string ConfigAsJson()
        {
            string jsonString = "{";
            jsonString += $"\"leaderboard1\":{leaderboard1.ConfigAsJson()},";
            jsonString += $"\"leaderboard2\":{leaderboard2.ConfigAsJson()},";
            jsonString += $"\"leaderboard3\":{leaderboard3.ConfigAsJson()},";
            jsonString += $"\"leaderboard4\":{leaderboard4.ConfigAsJson()},";
            jsonString += $"\"leaderboard5\":{leaderboard5.ConfigAsJson()}";
            jsonString += "}";
            return jsonString;
        }
        // Variables
        public LeaderboardConfigReference[] Leaderboards => _leaderboards;
    }
}
