// MAIN CONFIG
using System;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace DiscordConnector
{
    internal class MainConfig
    {
        private ConfigFile config;
        private static List<String> mutedPlayers;
        private const string MAIN_SETTINGS = "Main Settings";

        private ConfigEntry<string> webhookUrl;
        private ConfigEntry<bool> discordEmbedMessagesToggle;
        private ConfigEntry<string> mutedDiscordUserlist;
        private ConfigEntry<bool> statsAnnouncementToggle;
        private ConfigEntry<int> statsAnnouncementPeriod;
        private ConfigEntry<bool> colectStatsToggle;
        private ConfigEntry<bool> sendPositionsToggle;
        private ConfigEntry<bool> announcePlayerFirsts;


        public MainConfig(ConfigFile configFile)
        {
            config = configFile;

            LoadConfig();

            mutedPlayers = new List<string>(mutedDiscordUserlist.Value.Split(';'));
        }

        private void LoadConfig()
        {

            webhookUrl = config.Bind<string>(MAIN_SETTINGS,
                "Webhook URL",
                "",
                "Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of " + Environment.NewLine +
                "Discord's documentation: https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook");

            discordEmbedMessagesToggle = config.Bind<bool>(MAIN_SETTINGS,
                "Use fancier discord messages",
                false,
                "Enable this setting to use embeds in the messages sent to Discord." + Environment.NewLine +
                "NOTE: Some things may not work as expected with this enabled. Report any weirdness!");

            mutedDiscordUserlist = config.Bind<string>(MAIN_SETTINGS,
                "Ignored Players",
                "",
                "It may be that you have some players that you never want to send Discord messages for. Adding a player name to this list will ignore them." + Environment.NewLine +
                "Format should be a semicolon-separated list: Stuart;John McJohnny;Weird-name1");

            sendPositionsToggle = config.Bind<bool>(MAIN_SETTINGS,
                "Send Positions with Messages",
                true,
                "Disable this setting to disable any positions/coordinates being sent with messages (e.g. players deaths or players joining/leaving). (Overwrites any individual setting.)");

            colectStatsToggle = config.Bind<bool>(MAIN_SETTINGS,
                "Collect Player Stats",
                true,
                "Disable this setting to disable all stat collections and notifications. (Overwrites any individual setting.)");

            statsAnnouncementToggle = config.Bind<bool>(MAIN_SETTINGS,
                "Periodic Player Stats Notifications",
                false,
                "If enabled, periodically send a leaderboard or of top player stats to Discord." + Environment.NewLine +
                "EX: Top Player Deaths: etc etc Top Player Joins: etc etc");

            statsAnnouncementPeriod = config.Bind<int>(MAIN_SETTINGS,
                "Player Stats Notifications Period",
                600,
                "Set the number of minutes between a leaderboard announcement sent to discord." + Environment.NewLine +
                "This time starts when the server is started. Default is set to 10 hours (600 mintues).");

            announcePlayerFirsts = config.Bind<bool>(MAIN_SETTINGS,
                "Announce Player Firsts",
                true,
                "Disable this setting to disable all extra announcements the first time each player does something. (Overwrites any individual setting.)");

            config.Save();
        }

        public string ConfigAsJson()
        {
            string jsonString = "{";
            jsonString += "\"discord\":{";
            jsonString += $"\"webhook\":\"{(string.IsNullOrEmpty(WebHookURL) ? "unset" : "REDACTED")}\",";
            jsonString += $"\"fancierMessages\":\"{DiscordEmbedsEnabled}\",";
            jsonString += $"\"ignoredPlayers\":\"{mutedDiscordUserlist.Value}\"";
            jsonString += "},";
            jsonString += $"\"periodicLeaderboardEnabled\":\"{StatsAnnouncementEnabled}\",";
            jsonString += $"\"periodicLeaderabordPeriodSeconds\":{StatsAnnouncementPeriod},";
            jsonString += $"\"colectStatsEnabled\":\"{CollectStatsEnabled}\",";
            jsonString += $"\"sendPositionsEnabled\":\"{SendPositionsEnabled}\",";
            jsonString += $"\"announcePlayerFirsts\":\"{AnnouncePlayerFirsts}\"";
            jsonString += "}";
            return jsonString;
        }

        public string WebHookURL => webhookUrl.Value;
        public bool StatsAnnouncementEnabled => statsAnnouncementToggle.Value;
        public int StatsAnnouncementPeriod => statsAnnouncementPeriod.Value;
        public bool CollectStatsEnabled => colectStatsToggle.Value;
        public bool DiscordEmbedsEnabled => discordEmbedMessagesToggle.Value;
        public bool SendPositionsEnabled => sendPositionsToggle.Value;
        public List<string> MutedPlayers => mutedPlayers;
        public bool AnnouncePlayerFirsts => announcePlayerFirsts.Value;
    }
}