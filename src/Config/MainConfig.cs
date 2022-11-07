using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BepInEx.Configuration;

namespace DiscordConnector.Config
{
    public static class RetrievalDiscernmentMethods
    {
        public static readonly string BySteamID = "Treat each SteamID as a separate player";
        public static readonly string ByNameAndSteamID = "Treat each SteamID:PlayerName combo as a separate player";
        public static readonly string ByName = "Treat each PlayerName as a separate player";

    }
    internal class MainConfig
    {
        private ConfigFile config;
        private static List<String> mutedPlayers;
        private static Regex mutedPlayersRegex;
        private const string MAIN_SETTINGS = "Main Settings";

        // Main Settings
        private ConfigEntry<string> webhookUrl;
        private ConfigEntry<bool> discordEmbedMessagesToggle;
        private ConfigEntry<string> mutedDiscordUserlist;
        private ConfigEntry<string> mutedDiscordUserlistRegex;
        private ConfigEntry<bool> colectStatsToggle;
        private ConfigEntry<bool> sendPositionsToggle;
        private ConfigEntry<bool> announcePlayerFirsts;
        private ConfigEntry<string> playerLookupPreference;

        public MainConfig(ConfigFile configFile)
        {
            config = configFile;
            LoadConfig();
            mutedPlayers = new List<string>(mutedDiscordUserlist.Value.Split(';'));
            if (String.IsNullOrEmpty(@mutedDiscordUserlistRegex.Value))
            {
                mutedPlayersRegex = new Regex(@"a^");
            }
            else
            {
                mutedPlayersRegex = new Regex(@mutedDiscordUserlistRegex.Value);
            }
        }

        public void ReloadConfig()
        {
            config.Reload();
            config.Save();

            mutedPlayers = new List<string>(mutedDiscordUserlist.Value.Split(';'));
            if (String.IsNullOrEmpty(@mutedDiscordUserlistRegex.Value))
            {
                mutedPlayersRegex = new Regex(@"a^");
            }
            else
            {
                mutedPlayersRegex = new Regex(@mutedDiscordUserlistRegex.Value);
            }
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
                "Enable this setting to use embeds in the messages sent to Discord. Currently this will affect the position details for the messages.");

            mutedDiscordUserlist = config.Bind<string>(MAIN_SETTINGS,
                "Ignored Players",
                "",
                "It may be that you have some players that you never want to send Discord messages for. Adding a player name to this list will ignore them." + Environment.NewLine +
                "Format should be a semicolon-separated list: Stuart;John McJohnny;Weird-name1");

            mutedDiscordUserlistRegex = config.Bind<string>(MAIN_SETTINGS,
                "Ignored Players (Regex)",
                "",
                "It may be that you have some players that you never want to send Discord messages for. This option lets you provide a regular expression to filter out players if their name matches." + Environment.NewLine +
                "Format should be a valid string for a .NET Regex (reference: https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference)");

            sendPositionsToggle = config.Bind<bool>(MAIN_SETTINGS,
                "Send Positions with Messages",
                true,
                "Disable this setting to disable any positions/coordinates being sent with messages (e.g. players deaths or players joining/leaving). (Overwrites any individual setting.)");

            colectStatsToggle = config.Bind<bool>(MAIN_SETTINGS,
                "Collect Player Stats",
                true,
                "Disable this setting to disable all stat collection. (Overwrites any individual setting.)");

            announcePlayerFirsts = config.Bind<bool>(MAIN_SETTINGS,
                "Announce Player Firsts",
                true,
                "Disable this setting to disable all extra announcements the first time each player does something. (Overwrites any individual setting.)");

            playerLookupPreference = config.Bind<string>(MAIN_SETTINGS,
                "How to discern players in Record Retrieval",
                RetrievalDiscernmentMethods.BySteamID,
                new ConfigDescription("Choose a method for how players will be separated from the results of a record query.",
                new AcceptableValueList<string>(new string[] {
                    RetrievalDiscernmentMethods.BySteamID,
                    RetrievalDiscernmentMethods.ByName,
                    RetrievalDiscernmentMethods.ByNameAndSteamID
                })));


            config.Save();
        }

        public string ConfigAsJson()
        {
            string jsonString = "{";
            jsonString += "\"discord\":{";
            jsonString += $"\"webhook\":\"{(string.IsNullOrEmpty(WebHookURL) ? "unset" : "REDACTED")}\",";
            jsonString += $"\"fancierMessages\":\"{DiscordEmbedsEnabled}\",";
            jsonString += $"\"ignoredPlayers\":\"{mutedDiscordUserlist.Value}\",";
            jsonString += $"\"ignoredPlayersRegex\":\"{mutedDiscordUserlistRegex.Value}\"";
            jsonString += "},";
            jsonString += $"\"colectStatsEnabled\":\"{CollectStatsEnabled}\",";
            jsonString += $"\"sendPositionsEnabled\":\"{SendPositionsEnabled}\",";
            jsonString += $"\"announcePlayerFirsts\":\"{AnnouncePlayerFirsts}\",";
            jsonString += $"\"playerLookupPreference\":\"{RecordRetrievalDiscernmentMethod}\"";
            jsonString += "}";
            return jsonString;
        }

        public string WebHookURL => webhookUrl.Value;
        public bool CollectStatsEnabled => colectStatsToggle.Value;
        public bool DiscordEmbedsEnabled => discordEmbedMessagesToggle.Value;
        public bool SendPositionsEnabled => sendPositionsToggle.Value;
        public List<string> MutedPlayers => mutedPlayers;
        public Regex MutedPlayersRegex => mutedPlayersRegex;
        public bool AnnouncePlayerFirsts => announcePlayerFirsts.Value;
        public string RecordRetrievalDiscernmentMethod => playerLookupPreference.Value;

    }
}
