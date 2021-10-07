// toggles

// toggles.messages
// - launch 
// - loaded
// - stop
// - join
// - leave
// - death
// - shout
// - ping

// toggles.position
// - join
// - leave
// - pings
// - shouts
// - deaths

// toggles.stats
// - allow record join
// - allow record ..
// - allow record ..
// - allow record ..
// - allow record ..

// toggles.leaderboard
// - send pings leaderboard
// - send join leaderboard
// - send .. leaderboard
// - send .. leaderboard
// - send .. leaderboard

using System;
using BepInEx.Configuration;

namespace DiscordConnector
{
    internal class TogglesConfig
    {
        private static ConfigFile config;
        public static string ConfigExention = "toggles";



        // config header strings
        private const string NOTIFICATION_SETTINGS = "Notification Settings";
        private const string STATISTIC_COLLECTION_SETTINGS = "Statistics Collection Settings and Opt-Outs";

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

        // Statistic collection settings
        private ConfigEntry<bool> collectStatsJoins;
        private ConfigEntry<bool> collectStatsLeaves;
        private ConfigEntry<bool> collectStatsDeaths;
        private ConfigEntry<bool> collectStatsShouts;
        private ConfigEntry<bool> collectStatsPings;

        public TogglesConfig(ConfigFile configFile)
        {
            config = configFile;

            LoadConfig();
        }

        private void LoadConfig()
        {
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

        public string ConfigAsJson()
        {
            string jsonString = "{";
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
            jsonString += $"\"playerDeathPosEnabled\":\"{PlayerDeathPosEnabled}\"";
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



        // Toggles
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
        public bool StatsDeathEnabled => collectStatsDeaths.Value;
        public bool StatsJoinEnabled => collectStatsJoins.Value;
        public bool StatsLeaveEnabled => collectStatsLeaves.Value;
        public bool StatsPingEnabled => collectStatsPings.Value;
        public bool StatsShoutEnabled => collectStatsShouts.Value;
    }
}