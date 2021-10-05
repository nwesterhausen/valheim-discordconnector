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
                "has joined.",
                "Set the message that will be sent when a player joins the server" + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: 'has joined;awakens;arrives'");

            playerLeaveMessage = config.Bind<string>(NOTIFICATION_CONTENT_SETTINGS,
                "Player Leave Message",
                "has left.",
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
        public bool LaunchMessageEnabled => serverLaunchToggle.Value;
        public bool LoadedMessageEnabled => serverLaunchToggle.Value;
        public bool StopMessageEnabled => serverLaunchToggle.Value;
        public bool ChatMessageEnabled => chatToggle.Value;
        public bool ChatShoutEnabled => chatShoutToggle.Value;
        public bool ChatShoutPosEnabled => chatShoutPosToggle.Value;
        public bool ChatPingEnabled => chatPingToggle.Value;
        public bool PlayerJoinMessageEnabled => playerJoinToggle.Value;
        public bool PlayerLeaveMessageEnabled => playerLeaveToggle.Value;
        public bool CollectStatsEnabled => collectStatsEnable.Value;
        public bool StatsDeathEnabled => collectStatsDeaths.Value;
        public bool StatsJoinEnabled => collectStatsJoins.Value;
        public bool StatsLeaveEnabled => collectStatsLeaves.Value;
        public bool StatsPingEnabled => collectStatsPings.Value;
        public bool StatsShoutEnabled => collectStatsShouts.Value;

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
    }
}