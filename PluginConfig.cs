using System;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace DiscordConnector
{
    internal class PluginConfig
    {
        public static ConfigFile config;

        private static List<String> mutedPlayers;

        // config header strings
        private const string DISCORD_SETTINGS = "Discord Settings";
        private const string NOTIFICATION_SETTINGS = "Notification Settings";
        private const string NOTIFICATION_CONTENT_SETTINGS = "Notification Content Settings";
        private const string STATISTIC_COLLECTION_SETTINGS = "Statistics Collection Settings and Opt-Outs";

        // Discord Settings
        private ConfigEntry<string> webhookUrl;
        private ConfigEntry<bool> discordEmbedMessagesToggle;
        private ConfigEntry<string> mutedDiscordUserlist;

        // Logged Information Toggles
        private ConfigEntry<bool> serverLaunchToggle;
        private ConfigEntry<bool> serverLoadedToggle;
        private ConfigEntry<bool> serverStopToggle;
        private ConfigEntry<bool> chatToggle;
        private ConfigEntry<bool> chatShoutToggle;
        private ConfigEntry<bool> chatShoutPosToggle;
        private ConfigEntry<bool> chatPingToggle;
        private ConfigEntry<bool> chatPingPosToggle;
        private ConfigEntry<bool> playerJoinToggle;
        private ConfigEntry<bool> playerJoinPosToggle;
        private ConfigEntry<bool> playerDeathToggle;
        private ConfigEntry<bool> playerDeathPosToggle;
        private ConfigEntry<bool> playerLeaveToggle;
        private ConfigEntry<bool> playerLeavePosToggle;
        private ConfigEntry<bool> statsAnnouncementToggle;
        private ConfigEntry<int> statsAnnouncementPeriod;

        // Logged Information Messages
        private ConfigEntry<string> serverLaunchMessage;
        private ConfigEntry<string> serverLoadedMessage;
        private ConfigEntry<string> serverStopMessage;
        private ConfigEntry<string> playerJoinMessage;
        private ConfigEntry<string> playerLeaveMessage;
        private ConfigEntry<string> playerDeathMessage;

        // Statistic collection settings
        private ConfigEntry<bool> collectStatsEnable;
        private ConfigEntry<bool> collectStatsJoins;
        private ConfigEntry<bool> collectStatsLeaves;
        private ConfigEntry<bool> collectStatsDeaths;
        private ConfigEntry<bool> collectStatsShouts;
        private ConfigEntry<bool> collectStatsPings;

        public PluginConfig(ConfigFile config)
        {
            PluginConfig.config = config;
            LoadConfig();

            mutedPlayers = new List<string>(mutedDiscordUserlist.Value.Split(';'));
            Plugin.StaticLogger.LogDebug("Configuration Loaded");
            Plugin.StaticLogger.LogDebug(ConfigAsJson());
        }

        private void LoadConfig()
        {
            webhookUrl = config.Bind<string>(DISCORD_SETTINGS,
                "Webhook URL",
                "",
                "Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of " + Environment.NewLine +
                "Discord's documentation: https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook");

            discordEmbedMessagesToggle = config.Bind<bool>(DISCORD_SETTINGS,
                "Use fancier discord messages",
                false,
                "Enable this setting to use embeds in the messages sent to Discord." + Environment.NewLine +
                "NOTE: Some things may not work as expected with this enabled. Report any weirdness!");

            mutedDiscordUserlist = config.Bind<string>(DISCORD_SETTINGS,
                "Ignored Players",
                "",
                "It may be that you have some players that you never want to send Discord messages for. Adding a player name to this list will ignore them." + Environment.NewLine +
                "Format should be a semicolon-separated list: Stuart;John McJohnny;Weird-name1");

            // Message Toggles

            serverLaunchToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Server Launch Notifications",
                true,
                "If enabled, this will send a message to Discord when the server launches (and the plugin is loaded)." + Environment.NewLine +
                "EX: Server has started. | Server has stopped.");

            serverLoadedToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Server Loaded Notifications",
                true,
                "If enabled, this will send a message to Discord when the server has loaded the map and is ready for connections." + Environment.NewLine +
                "EX: Server has started. | Server has stopped.");

            serverStopToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Server Status Notifications",
                true,
                "If enabled, this will send a message to Discord when the server shuts down." + Environment.NewLine +
                "EX: Server has started. | Server has stopped.");

            chatToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Chat Messages Notifications",
                true,
                "If enabled, this will send a message to Discord the server chat has new messages." + Environment.NewLine +
                "If this is false, no messages will be sent based on the server chat, even if specific notifications are enabled.");

            chatShoutToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Chat Shout Messages Notifications",
                true,
                "If enabled, this will send a message to Discord when a player joins the server." + Environment.NewLine +
                "EX: Nick shouted \"Hey you!\"");

            chatShoutPosToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Chat Shout Messages Position Notifications",
                true,
                "If enabled, include a position with the arrival message." + Environment.NewLine +
                "EX: Nick shouted \"Hey you!\" (at -124, 81.4, -198.9)");

            chatPingToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Ping Notifications",
                true,
                "If enabled, include a position with the arrival message." + Environment.NewLine +
                "If the top-level chat notifications are disabled, that will disable these messages." + Environment.NewLine +
                "EX: Nick pinged the map!");

            chatPingPosToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Ping Notificiations Include Position",
                true,
                "If enabled, includes the coordinates of the ping.");

            playerJoinToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Player Join Notifications",
                true,
                "If enabled, this will send a message to Discord when a player joins the server." + Environment.NewLine +
                "EX: Player has joined");

            playerJoinPosToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Include POS With Player Join",
                true,
                "If enabled, this will include the coordinates of the player when they join.");

            playerDeathToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Player Death Notifications",
                true,
                "If enabled, this will send a message to Discord when a player dies on the server." + Environment.NewLine +
                "EX: Player has died");

            playerDeathPosToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Include POS With Player Death",
                true,
                "If enabled, this will include the coordinates of the player when they die.");

            playerLeaveToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Player Leave Notifications",
                true,
                "If enabled, this will send a message to Discord when a player leaves the server." + Environment.NewLine +
                "EX: Player has left.");

            playerLeavePosToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Include POS With Player Leave",
                true,
                "If enabled, this will include the coordinates of the player when they leave.");

            statsAnnouncementToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Periodic Player Stats Notifications",
                false,
                "If enabled, periodically send a leaderboard or of top player stats to Discord." + Environment.NewLine +
                "EX: Top Player Deaths: etc etc Top Player Joins: etc etc");

            statsAnnouncementPeriod = config.Bind<int>(NOTIFICATION_SETTINGS,
                "Player Stats Notifications Period",
                600,
                "Set the number of minutes between a leaderboard announcement sent to discord." + Environment.NewLine +
                "This time starts when the server is started. Default is set to 10 hours (600 mintues).");

            // Message Settings

            serverLaunchMessage = config.Bind<string>(NOTIFICATION_CONTENT_SETTINGS,
                "Server Launch Message",
                "Server is starting up.",
                "Set the message that will be sent when the server starts up." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: 'Server is starting;Server beginning to load'");

            serverLoadedMessage = config.Bind<string>(NOTIFICATION_CONTENT_SETTINGS,
                "Server Started Message",
                "Server has started!",
                "Set the message that will be sent when the server has loaded the map and is ready for connections." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: 'Server has started;Server ready!'");

            serverStopMessage = config.Bind<string>(NOTIFICATION_CONTENT_SETTINGS,
                "Server Stop Message",
                "Server is stopping.",
                "Set the message that will be sent when the server shuts down." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: 'Server stopping;Valheim signing off!'");

            playerJoinMessage = config.Bind<string>(NOTIFICATION_CONTENT_SETTINGS,
                "Player Join Message",
                "%PLAYER_NAME% has joined.",
                "Set the message that will be sent when a player joins the server" + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: 'has joined;awakens;arrives'");

            playerDeathMessage = config.Bind<string>(NOTIFICATION_CONTENT_SETTINGS,
                "Player Death Message",
                "has died.",
                "Set the message that will be sent when a player dies." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: 'has died;was yeeted'");

            playerLeaveMessage = config.Bind<string>(NOTIFICATION_CONTENT_SETTINGS,
                "Player Leave Message",
                "%PLAYER_NAME% has left.",
                "Set the message that will be sent when a player leaves the server." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: 'has left;has moved on;returns to dreams'");

            // Statistic Settings
            collectStatsEnable = config.Bind<bool>(STATISTIC_COLLECTION_SETTINGS,
                "Collect Player Stats",
                true,
                "Disable this setting to disable all stat collections and notifications. (Overwrites any individual setting.)");
            collectStatsDeaths = config.Bind<bool>(STATISTIC_COLLECTION_SETTINGS,
                "Collect and Send Player Death Stats",
                true,
                "If enabled, will collect and enable sending player death statistics.");
            collectStatsJoins = config.Bind<bool>(STATISTIC_COLLECTION_SETTINGS,
                "Collect and Send Player Join Stats",
                true,
                "If enabled, will collect and enable sending stat announcements for how many times a player has joined the game.");
            collectStatsLeaves = config.Bind<bool>(STATISTIC_COLLECTION_SETTINGS,
                "Collect and Send Player Leave Stats",
                true,
                "If enabled, will collect and enable sending stat announcements for how many times a player has left the game.");
            collectStatsPings = config.Bind<bool>(STATISTIC_COLLECTION_SETTINGS,
                "Collect and Send Player Ping Stats",
                true,
                "If enabled, will collect and enable sending stat announcements for number of pings made by a player.");
            collectStatsShouts = config.Bind<bool>(STATISTIC_COLLECTION_SETTINGS,
                "Collect and Send Player Shout Stats",
                true,
                "If enabled, will collect and enable sending stat announcements for number of times a player has shouted.");

            config.Save();
        }

        // Exposed Config Values

        public string WebHookURL => webhookUrl.Value;

        // Toggles
        public bool DiscordEmbedsEnabled => discordEmbedMessagesToggle.Value;
        public bool LaunchMessageEnabled => serverLaunchToggle.Value;
        public bool LoadedMessageEnabled => serverLaunchToggle.Value;
        public bool StopMessageEnabled => serverLaunchToggle.Value;
        public bool ChatMessageEnabled => chatToggle.Value;
        public bool ChatShoutEnabled => chatShoutToggle.Value;
        public bool ChatShoutPosEnabled => chatShoutPosToggle.Value;
        public bool ChatPingEnabled => chatPingToggle.Value;
        public bool ChatPingPosEnabled => chatPingPosToggle.Value;
        public bool PlayerJoinMessageEnabled => playerJoinToggle.Value;
        public bool PlayerJoinPosEnabled => playerJoinPosToggle.Value;
        public bool PlayerDeathMessageEnabled => playerJoinToggle.Value;
        public bool PlayerDeathPosEnabled => playerJoinPosToggle.Value;
        public bool PlayerLeaveMessageEnabled => playerLeaveToggle.Value;
        public bool PlayerLeavePosEnabled => playerLeavePosToggle.Value;
        public bool CollectStatsEnabled => collectStatsEnable.Value;
        public bool StatsDeathEnabled => collectStatsDeaths.Value;
        public bool StatsJoinEnabled => collectStatsJoins.Value;
        public bool StatsLeaveEnabled => collectStatsLeaves.Value;
        public bool StatsPingEnabled => collectStatsPings.Value;
        public bool StatsShoutEnabled => collectStatsShouts.Value;
        public bool StatsAnnouncementEnabled => statsAnnouncementToggle.Value;
        public int StatsAnnouncementPeriod => statsAnnouncementPeriod.Value;

        // Messages
        public string LaunchMessage
        {
            get
            {
                if (!serverLaunchMessage.Value.Contains(";"))
                {
                    return serverLaunchMessage.Value;
                }
                string[] choices = serverLaunchMessage.Value.Split(';');
                int selection = (new Random()).Next(choices.Length);
                return choices[selection];
            }
        }
        public string LoadedMessage
        {
            get
            {
                if (!serverLoadedMessage.Value.Contains(";"))
                {
                    return serverLoadedMessage.Value;
                }
                string[] choices = serverLoadedMessage.Value.Split(';');
                int selection = (new Random()).Next(choices.Length);
                return choices[selection];
            }
        }
        public string StopMessage
        {
            get
            {
                if (!serverStopMessage.Value.Contains(";"))
                {
                    return serverStopMessage.Value;
                }
                string[] choices = serverStopMessage.Value.Split(';');
                int selection = (new Random()).Next(choices.Length);
                return choices[selection];
            }
        }
        public string JoinMessage
        {
            get
            {
                if (!playerJoinMessage.Value.Contains(";"))
                {
                    return playerJoinMessage.Value;
                }
                string[] choices = playerJoinMessage.Value.Split(';');
                int selection = (new Random()).Next(choices.Length);
                return choices[selection];
            }
        }
        public string LeaveMessage
        {
            get
            {
                if (!playerLeaveMessage.Value.Contains(";"))
                {
                    return playerLeaveMessage.Value;
                }
                string[] choices = playerLeaveMessage.Value.Split(';');
                int selection = (new Random()).Next(choices.Length);
                return choices[selection];
            }
        }
        public string DeathMessage
        {
            get
            {
                if (!playerDeathMessage.Value.Contains(";"))
                {
                    return playerDeathMessage.Value;
                }
                string[] choices = playerDeathMessage.Value.Split(';');
                int selection = (new Random()).Next(choices.Length);
                return choices[selection];
            }
        }
        public List<string> MutedPlayers => mutedPlayers;

        public string ConfigAsJson()
        {
            string jsonString = "{";

            // Discord Settings
            jsonString += "\"discord\":{";
            jsonString += $"\"webhook\":\"{(string.IsNullOrEmpty(WebHookURL) ? "unset" : "REDACTED")}\",";
            jsonString += $"\"fancierMessages\":\"{DiscordEmbedsEnabled}\",";
            jsonString += $"\"ignoredPlayers\":\"{mutedDiscordUserlist.Value}\"";
            jsonString += "},";

            // Notification Content Settings
            jsonString += "\"notificationContent\":{";
            jsonString += $"\"launchMessage\":\"{serverLaunchMessage.Value}\",";
            jsonString += $"\"startMessage\":\"{serverLoadedMessage.Value}\",";
            jsonString += $"\"stopMessage\":\"{serverStopMessage.Value}\",";
            jsonString += $"\"joinMessage\":\"{playerJoinMessage.Value}\",";
            jsonString += $"\"deathMessage\":\"{playerDeathMessage.Value}\",";
            jsonString += $"\"leaveMessage\":\"{playerLeaveMessage.Value}\"";
            jsonString += "},";

            // Notification Settings
            jsonString += "\"notificationToggles\":{";
            jsonString += $"\"launchMessageEnabled\":\"{LaunchMessageEnabled}\",";
            jsonString += $"\"loadedMessageEnabled\":\"{LoadedMessageEnabled}\",";
            jsonString += $"\"stopMessageEnabled\":\"{StopMessageEnabled}\",";
            jsonString += $"\"chatMessageEnabled\":\"{ChatMessageEnabled}\",";
            jsonString += $"\"chatShoutEnabled\":\"{ChatShoutEnabled}\",";
            jsonString += $"\"chatShoutPosEnabled\":\"{ChatShoutPosEnabled}\",";
            jsonString += $"\"chatPingEnabled\":\"{ChatPingEnabled}\",";
            jsonString += $"\"chatPingPosEnabled\":\"{ChatPingPosEnabled}\",";
            jsonString += $"\"playerJoinEnabled\":\"{PlayerJoinMessageEnabled}\",";
            jsonString += $"\"playerJoinPosEnabled\":\"{PlayerJoinPosEnabled}\",";
            jsonString += $"\"playerLeaveEnabled\":\"{PlayerLeaveMessageEnabled}\",";
            jsonString += $"\"playerLeavePosEnabled\":\"{PlayerLeavePosEnabled}\",";
            jsonString += $"\"playerDeathEnabled\":\"{PlayerDeathMessageEnabled}\",";
            jsonString += $"\"playerDeathPosEnabled\":\"{PlayerDeathPosEnabled}\",";
            jsonString += $"\"periodicLeaderboardEnabled\":\"{StatsAnnouncementEnabled}\",";
            jsonString += $"\"periodicLeaderabordPeriodSeconds\":\"{StatsAnnouncementPeriod}\"";
            jsonString += "},";

            // Stats Collection
            jsonString += "\"statsCollection\":{";
            jsonString += $"\"statsDeathEnabled\":\"{StatsDeathEnabled}\",";
            jsonString += $"\"statsJoinEnabled\":\"{StatsJoinEnabled}\",";
            jsonString += $"\"statsLeaveEnabled\":\"{StatsLeaveEnabled}\",";
            jsonString += $"\"statsPingEnabled\":\"{StatsPingEnabled}\",";
            jsonString += $"\"statsShoutEnabled\":\"{StatsShoutEnabled}\"";
            jsonString += "}}";
            return jsonString;
        }
    }
}