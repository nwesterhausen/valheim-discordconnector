using System;
using BepInEx.Configuration;

namespace DiscordConnector
{
    class PluginConfig
    {
        public static ConfigFile config;

        // config header strings
        private const string DISCORD_SETTINGS = "Discord Settings";
        private const string NOTIFICATION_SETTINGS = "Notification Settings";
        private const string NOTIFICATION_CONTENT_SETTINGS = "Notification Content Settings";
        private const string STATISTIC_COLLECTION_SETTINGS = "Statistics Collection Settings and Opt-Outs";

        // Webhook Url
        private ConfigEntry<string> webhookUrl;

        // Logged Information Toggles
        private ConfigEntry<bool> serverLaunchToggle;
        private ConfigEntry<bool> serverLoadedToggle;
        private ConfigEntry<bool> serverStopToggle;
        private ConfigEntry<bool> chatToggle;
        private ConfigEntry<bool> chatShoutToggle;
        private ConfigEntry<bool> chatShoutPosToggle;
        private ConfigEntry<bool> chatPingToggle;

        private ConfigEntry<bool> playerJoinToggle;
        private ConfigEntry<bool> playerLeaveToggle;
        private ConfigEntry<bool> statsAnnouncementToggle;
        private ConfigEntry<int> statsAnnouncementPeriod;

        // Logged Information Messages
        private ConfigEntry<string> serverLaunchMessage;
        private ConfigEntry<string> serverLoadedMessage;
        private ConfigEntry<string> serverStopMessage;
        private ConfigEntry<string> playerJoinMessage;
        private ConfigEntry<string> playerLeaveMessage;

        // Statistic collection settings
        private ConfigEntry<bool> collectStatsJoins;
        private ConfigEntry<bool> collectStatsLeaves;
        private ConfigEntry<bool> collectStatsDeaths;
        private ConfigEntry<bool> collectStatsShouts;
        private ConfigEntry<bool> collectStatsPings;

        public PluginConfig(ConfigFile config)
        {
            PluginConfig.config = config;
            LoadConfig();
        }

        public void LoadConfig()
        {
            webhookUrl = config.Bind<string>(DISCORD_SETTINGS,
                "Webhook URL",
                "",
                "Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of " + Environment.NewLine +
                "Discord's documentation: https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook");

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
                "EX: Nick pinged the map at -124, 81.4, -198.9!");

            playerJoinToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                          "Player Join Notifications",
                          true,
                          "If enabled, this will send a message to Discord when a player joins the server." + Environment.NewLine +
                          "EX: Player has joined");

            playerLeaveToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
              "Player Leave Notifications",
              true,
              "If enabled, this will send a message to Discord when a player leaves the server." + Environment.NewLine +
              "EX: Player has left.");

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
                "Set the message that will be sent when the server starts up.");

            serverLoadedMessage = config.Bind<string>(NOTIFICATION_CONTENT_SETTINGS,
                "Server Started Message",
                "Server has started!",
                "Set the message that will be sent when the server has loaded the map and is ready for connections.");

            serverStopMessage = config.Bind<string>(NOTIFICATION_CONTENT_SETTINGS,
                "Server Stop Message",
                "Server is stopping.",
                "Set the message that will be sent when the server shuts down.");

            playerJoinMessage = config.Bind<string>(NOTIFICATION_CONTENT_SETTINGS,
              "Player Join Message",
              "has joined.",
              "Set the message that will be sent when a player joins the server");

            playerLeaveMessage = config.Bind<string>(NOTIFICATION_CONTENT_SETTINGS,
              "Player Leave Message",
              "has left.",
              "Set the message that will be sent when a player leaves the server.");

            // Statistic Settings
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
        public string LaunchMessage => serverLaunchMessage.Value;
        public string LoadedMessage => serverLoadedMessage.Value;
        public string StopMessage => serverStopMessage.Value;
        public bool LaunchMessageEnabled => serverLaunchToggle.Value;
        public bool LoadedMessageEnabled => serverLaunchToggle.Value;
        public bool StopMessageEnabled => serverLaunchToggle.Value;
        public bool ChatMessageEnabled => chatToggle.Value;
        public bool ChatShoutEnabled => chatShoutToggle.Value;
        public bool ChatShoutPosEnabled => chatShoutPosToggle.Value;
        public bool ChatPingEnabled => chatPingToggle.Value;
        public bool PlayerJoinMessageEnabled => playerJoinToggle.Value;
        public bool PlayerLeaveMessageEnabled => playerLeaveToggle.Value;
        public string JoinMessage => playerJoinMessage.Value;
        public string LeaveMessage => playerLeaveMessage.Value;
    }
}