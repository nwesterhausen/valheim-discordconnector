using BepInEx.Configuration;

namespace DiscordConnector.Config
{
    internal class TogglesConfig
    {
        private static ConfigFile config;
        public static string ConfigExtension = "toggles";

        // Config Header Strings
        private const string MESSAGES_TOGGLES = "Toggles.Messages";
        private const string POSITION_TOGGLES = "Toggles.Position";
        private const string STATS_TOGGLES = "Toggles.Stats";
        private const string LEADERBOARD_TOGGLES = "Toggles.Leaderboard";
        private const string LEADERBOARD_TOGGLES_HIGHEST = "Toggles.Leaderboard.Highest";
        private const string LEADERBOARD_TOGGLES_LOWEST = "Toggles.Leaderboard.Lowest";
        private const string PLAYER_FIRSTS_TOGGLES = "Toggles.PlayerFirsts";

        // Logged Information Toggles
        private ConfigEntry<bool> serverLaunchToggle;
        private ConfigEntry<bool> serverLoadedToggle;
        private ConfigEntry<bool> serverStopToggle;
        private ConfigEntry<bool> serverShutdownToggle;
        private ConfigEntry<bool> serverSaveToggle;
        private ConfigEntry<bool> chatShoutToggle;
        private ConfigEntry<bool> chatPingToggle;
        private ConfigEntry<bool> playerDeathToggle;
        private ConfigEntry<bool> playerJoinToggle;
        private ConfigEntry<bool> playerLeaveToggle;
        private ConfigEntry<bool> eventStartMessageToggle;
        private ConfigEntry<bool> eventPausedMessageToggle;
        private ConfigEntry<bool> eventStopMessageToggle;
        private ConfigEntry<bool> eventResumedMessageToggle;

        // Position Coordinates Toggles
        private ConfigEntry<bool> playerLeavePosToggle;
        private ConfigEntry<bool> playerJoinPosToggle;
        private ConfigEntry<bool> playerDeathPosToggle;
        private ConfigEntry<bool> chatPingPosToggle;
        private ConfigEntry<bool> chatShoutPosToggle;
        private ConfigEntry<bool> eventStartPosToggle;
        private ConfigEntry<bool> eventPausedPosToggle;
        private ConfigEntry<bool> eventStopPosToggle;
        private ConfigEntry<bool> eventResumedPosToggle;

        // Statistic collection settings
        private ConfigEntry<bool> collectStatsJoins;
        private ConfigEntry<bool> collectStatsLeaves;
        private ConfigEntry<bool> collectStatsDeaths;
        private ConfigEntry<bool> collectStatsShouts;
        private ConfigEntry<bool> collectStatsPings;

        // Send Leaderboard Settings (highest)
        private ConfigEntry<bool> sendMostSessionLeaderboard;
        private ConfigEntry<bool> sendMostPingLeaderboard;
        private ConfigEntry<bool> sendMostDeathLeaderboard;
        private ConfigEntry<bool> sendMostShoutLeaderboard;

        // Send Leaderboard Settings (lowest)
        private ConfigEntry<bool> sendLeastSessionLeaderboard;
        private ConfigEntry<bool> sendLeastPingLeaderboard;
        private ConfigEntry<bool> sendLeastDeathLeaderboard;
        private ConfigEntry<bool> sendLeastShoutLeaderboard;

        // Send Leaderboard Settings (rankings)
        private ConfigEntry<bool> sendSessionRankingLeaderboard;
        private ConfigEntry<bool> sendPingRankingLeaderboard;
        private ConfigEntry<bool> sendDeathRankingLeaderboard;
        private ConfigEntry<bool> sendShoutRankingLeaderboard;

        // Player-firsts Settings
        private ConfigEntry<bool> announcePlayerFirstDeath;
        private ConfigEntry<bool> announcePlayerFirstJoin;
        private ConfigEntry<bool> announcePlayerFirstLeave;
        private ConfigEntry<bool> announcePlayerFirstShout;
        private ConfigEntry<bool> announcePlayerFirstPing;

        public TogglesConfig(ConfigFile configFile)
        {
            config = configFile;

            ReloadConfig();
        }

        public void ReloadConfig()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            // Message Toggles
            serverLaunchToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Server Launch Notifications",
                true,
                "If enabled, this will send a message to Discord when the server launches.");
            serverLoadedToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Server Loaded Notifications",
                true,
                "If enabled, this will send a message to Discord when the server has loaded the map and is ready for connections.");
            serverStopToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Server Stopping Notifications",
                true,
                "If enabled, this will send a message to Discord when the server begins shut down.");
            serverShutdownToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Server Shutdown Notifications",
                true,
                "If enabled, this will send a message to Discord when the server has shut down.");
            serverSaveToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Server World Save Notifications",
                true,
                "If enabled, this will send a message to Discord when the server saves the world.");
            chatShoutToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Chat Shout Messages Notifications",
                true,
                "If enabled, this will send a message to Discord when a player shouts on the server.");
            chatPingToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Ping Notifications",
                true,
                "If enabled, this will send a message to Discord when a player pings on the map.");
            playerJoinToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Player Join Notifications",
                true,
                "If enabled, this will send a message to Discord when a player joins the server.");
            playerDeathToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Player Death Notifications",
                true,
                "If enabled, this will send a message to Discord when a player dies on the server.");
            playerLeaveToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Player Leave Notifications",
                true,
                "If enabled, this will send a message to Discord when a player leaves the server.");
            eventStartMessageToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Event Start Notifications",
                true,
                "If enabled, this will send a message to Discord when a random event starts on the server.");
            eventStopMessageToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Event Stop Notifications",
                true,
                "If enabled, this will send a message to Discord when a random event stops on the server.");
            eventPausedMessageToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Event Paused Notifications",
                true,
                "If enabled, this will send a message to Discord when a random event is paused due to players leaving the area.");
            eventResumedMessageToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Event Resumed Notifications",
                true,
                "If enabled, this will send a message to Discord when a random event is resumed.");

            // Position Toggles
            playerJoinPosToggle = config.Bind<bool>(POSITION_TOGGLES,
                "Include POS With Player Join",
                false,
                "If enabled, this will include the coordinates of the player when they join.");
            playerLeavePosToggle = config.Bind<bool>(POSITION_TOGGLES,
                "Include POS With Player Leave",
                false,
                "If enabled, this will include the coordinates of the player when they leave.");
            playerDeathPosToggle = config.Bind<bool>(POSITION_TOGGLES,
                "Include POS With Player Death",
                true,
                "If enabled, this will include the coordinates of the player when they die.");
            chatPingPosToggle = config.Bind<bool>(POSITION_TOGGLES,
                "Ping Notifications Include Position",
                true,
                "If enabled, includes the coordinates of the ping.");
            chatShoutPosToggle = config.Bind<bool>(POSITION_TOGGLES,
                "Chat Shout Messages Position Notifications",
                false,
                "If enabled, this will include the coordinates of the player when they shout.");
            eventStartPosToggle = config.Bind<bool>(POSITION_TOGGLES,
                "Event Start Messages Position Notifications",
                true,
                "If enabled, this will include the coordinates of the random event when the start message is sent.");
            eventStopPosToggle = config.Bind<bool>(POSITION_TOGGLES,
                "Event Stop Messages Position Notifications",
                true,
                "If enabled, this will include the coordinates of the random event when the stop message is sent.");
            eventPausedPosToggle = config.Bind<bool>(POSITION_TOGGLES,
                "Event Paused Messages Position Notifications",
                true,
                "If enabled, this will include the coordinates of the random event when the paused message is sent.");
            eventResumedPosToggle = config.Bind<bool>(POSITION_TOGGLES,
                "Event Resumed Messages Position Notifications",
                true,
                "If enabled, this will include the coordinates of the random event when the resumed message is sent.");

            // Statistic Settings
            collectStatsDeaths = config.Bind<bool>(STATS_TOGGLES,
                "Collect and Send Player Death Stats",
                true,
                "If enabled, will allow collection of the number of times a player has died.");
            collectStatsJoins = config.Bind<bool>(STATS_TOGGLES,
                "Collect and Send Player Join Stats",
                true,
                "If enabled, will allow collection of how many times a player has joined the game.");
            collectStatsLeaves = config.Bind<bool>(STATS_TOGGLES,
                "Collect and Send Player Leave Stats",
                true,
                "If enabled, will allow collection of how many times a player has left the game.");
            collectStatsPings = config.Bind<bool>(STATS_TOGGLES,
                "Collect and Send Player Ping Stats",
                true,
                "If enabled, will allow collection of the number of pings made by a player.");
            collectStatsShouts = config.Bind<bool>(STATS_TOGGLES,
                "Collect and Send Player Shout Stats",
                true,
                "If enabled, will allow collection of the number of times a player has shouted.");

            // Leaderboard
            sendDeathRankingLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES,
                "Send Periodic Leaderboard for Player Deaths",
                true,
                "If enabled, will send a ranked leaderboard for player deaths at the interval.");
            sendPingRankingLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES,
                "Send Periodic Leaderboard for Player Pings",
                false,
                "If enabled, will send a ranked leaderboard for player pings at the interval.");
            sendSessionRankingLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES,
                "Send Periodic Leaderboard for Player Sessions",
                false,
                "If enabled, will send a ranked leaderboard for player sessions at the interval.");
            sendShoutRankingLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES,
                "Send Periodic Leaderboard for Player Shouts",
                false,
                "If enabled, will send a ranked leaderboard for player shouts at the interval.");

            // Leaderboard for Highest
            sendMostDeathLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES_HIGHEST,
                "Include Most Deaths in Periodic Leaderboard",
                true,
                "If enabled, will include player with the most deaths at the interval.");
            sendMostPingLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES_HIGHEST,
                "Include Most Pings in Periodic Leaderboard",
                false,
                "If enabled, will include player with the most pings at the interval.");
            sendMostSessionLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES_HIGHEST,
                "Include Most Sessions in Periodic Leaderboard",
                false,
                "If enabled, will include player with the most sessions at the interval.");
            sendMostShoutLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES_HIGHEST,
                "Include Most Shouts in Periodic Leaderboard",
                false,
                "If enabled, will include player with the most shouts at the interval.");

            // Leaderboard for Lowest
            sendLeastDeathLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES_LOWEST,
                "Include Least Deaths in Periodic Leaderboard",
                true,
                "If enabled, will include player with the least deaths at the interval.");
            sendLeastPingLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES_LOWEST,
                "Include Least Pings in Periodic Leaderboard",
                false,
                "If enabled, will include player with the least pings at the interval.");
            sendLeastSessionLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES_LOWEST,
                "Include Least Sessions in Periodic Leaderboard",
                false,
                "If enabled, will include player with the least sessions at the interval.");
            sendLeastShoutLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES_LOWEST,
                "Include Least Shouts in Periodic Leaderboard",
                false,
                "If enabled, will include player with the least shouts at the interval.");

            // Player Firsts
            announcePlayerFirstDeath = config.Bind<bool>(PLAYER_FIRSTS_TOGGLES,
                "Send a Message for the First Death of a Player",
                true,
                "If enabled, this will send an extra message on a player's first death.");
            announcePlayerFirstJoin = config.Bind<bool>(PLAYER_FIRSTS_TOGGLES,
                "Send a Message for the First Join of a Player",
                true,
                "If enabled, this will send an extra message on a player's first join to the server.");
            announcePlayerFirstLeave = config.Bind<bool>(PLAYER_FIRSTS_TOGGLES,
                "Send a Message for the First Leave of a Player",
                false,
                "If enabled, this will send an extra message on a player's first leave from the server.");
            announcePlayerFirstPing = config.Bind<bool>(PLAYER_FIRSTS_TOGGLES,
                "Send a Message for the First Ping of a Player",
                false,
                "If enabled, this will send an extra message on a player's first ping.");
            announcePlayerFirstShout = config.Bind<bool>(PLAYER_FIRSTS_TOGGLES,
                "Send a Message for the First Shout of a Player",
                false,
                "If enabled, this will send an extra message on a player's first shout.");

            config.Save();
        }

        public string ConfigAsJson()
        {
            string jsonString = "{";
            jsonString += $"\"{MESSAGES_TOGGLES}\":{{";
            jsonString += $"\"launchMessageEnabled\":\"{LaunchMessageEnabled}\",";
            jsonString += $"\"loadedMessageEnabled\":\"{LoadedMessageEnabled}\",";
            jsonString += $"\"stopMessageEnabled\":\"{StopMessageEnabled}\",";
            jsonString += $"\"shutdownMessageEnabled\":\"{ShutdownMessageEnabled}\",";
            jsonString += $"\"chatShoutEnabled\":\"{ChatShoutEnabled}\",";
            jsonString += $"\"chatPingEnabled\":\"{ChatPingEnabled}\",";
            jsonString += $"\"playerJoinEnabled\":\"{PlayerJoinMessageEnabled}\",";
            jsonString += $"\"playerLeaveEnabled\":\"{PlayerLeaveMessageEnabled}\",";
            jsonString += $"\"playerDeathEnabled\":\"{PlayerDeathMessageEnabled}\",";
            jsonString += $"\"eventStartEnabled\":\"{EventStartMessageEnabled}\",";
            jsonString += $"\"eventPausedEnabled\":\"{EventStopMessageEnabled}\",";
            jsonString += $"\"eventStoppedEnabled\":\"{EventPausedMessageEnabled}\",";
            jsonString += $"\"eventResumedEnabled\":\"{EventResumedMessageEnabled}\"";
            jsonString += "},";

            jsonString += $"\"{POSITION_TOGGLES}\":{{";
            jsonString += $"\"chatShoutPosEnabled\":\"{ChatShoutPosEnabled}\",";
            jsonString += $"\"chatPingPosEnabled\":\"{ChatPingPosEnabled}\",";
            jsonString += $"\"playerJoinPosEnabled\":\"{PlayerJoinPosEnabled}\",";
            jsonString += $"\"playerLeavePosEnabled\":\"{PlayerLeavePosEnabled}\",";
            jsonString += $"\"playerDeathPosEnabled\":\"{PlayerDeathPosEnabled}\",";
            jsonString += $"\"eventStartPosEnabled\":\"{EventStartPosEnabled}\",";
            jsonString += $"\"eventStopPosEnabled\":\"{EventStopPosEnabled}\",";
            jsonString += $"\"eventPausedPosEnabled\":\"{EventPausedPosEnabled}\",";
            jsonString += $"\"eventResumedPosEnabled\":\"{EventResumedPosEnabled}\"";
            jsonString += "},";

            jsonString += $"\"{STATS_TOGGLES}\":{{";
            jsonString += $"\"statsDeathEnabled\":\"{StatsDeathEnabled}\",";
            jsonString += $"\"statsJoinEnabled\":\"{StatsJoinEnabled}\",";
            jsonString += $"\"statsLeaveEnabled\":\"{StatsLeaveEnabled}\",";
            jsonString += $"\"statsPingEnabled\":\"{StatsPingEnabled}\",";
            jsonString += $"\"statsShoutEnabled\":\"{StatsShoutEnabled}\"";
            jsonString += "},";

            jsonString += $"\"{LEADERBOARD_TOGGLES}\":{{";
            jsonString += $"\"leaderboardDeathEnabled\":\"{RankedDeathLeaderboardEnabled}\",";
            jsonString += $"\"leaderboardPingEnabled\":\"{RankedPingLeaderboardEnabled}\",";
            jsonString += $"\"leaderboardShoutEnabled\":\"{RankedShoutLeaderboardEnabled}\",";
            jsonString += $"\"leaderboardSessionEnabled\":\"{RankedSessionLeaderboardEnabled}\"";
            jsonString += "},";

            jsonString += $"\"{LEADERBOARD_TOGGLES_HIGHEST}\":{{";
            jsonString += $"\"sendMostSessionLeaderboard\":\"{MostSessionLeaderboardEnabled}\",";
            jsonString += $"\"sendMostPingLeaderboard\":\"{MostPingLeaderboardEnabled}\",";
            jsonString += $"\"sendMostDeathLeaderboard\":\"{MostDeathLeaderboardEnabled}\",";
            jsonString += $"\"sendMostShoutLeaderboard\":\"{MostShoutLeaderboardEnabled}\"";
            jsonString += "},";

            jsonString += $"\"{LEADERBOARD_TOGGLES_LOWEST}\":{{";
            jsonString += $"\"sendLeastSessionLeaderboard\":\"{LeastSessionLeaderboardEnabled}\",";
            jsonString += $"\"sendLeastPingLeaderboard\":\"{LeastPingLeaderboardEnabled}\",";
            jsonString += $"\"sendLeastDeathLeaderboard\":\"{LeastDeathLeaderboardEnabled}\",";
            jsonString += $"\"sendLeastShoutLeaderboard\":\"{LeastShoutLeaderboardEnabled}\"";
            jsonString += "},";

            jsonString += $"\"{PLAYER_FIRSTS_TOGGLES}\":{{";
            jsonString += $"\"announceFirstDeathEnabled\":\"{AnnouncePlayerFirstDeathEnabled}\",";
            jsonString += $"\"announceFirstJoinEnabled\":\"{AnnouncePlayerFirstJoinEnabled}\",";
            jsonString += $"\"announceFirstLeaveEnabled\":\"{AnnouncePlayerFirstLeaveEnabled}\",";
            jsonString += $"\"announceFirstPingEnabled\":\"{AnnouncePlayerFirstPingEnabled}\",";
            jsonString += $"\"announceFirstShoutEnabled\":\"{AnnouncePlayerFirstShoutEnabled}\"";
            jsonString += "}";

            jsonString += "}";
            return jsonString;
        }

        public bool LaunchMessageEnabled => serverLaunchToggle.Value;
        public bool LoadedMessageEnabled => serverLoadedToggle.Value;
        public bool StopMessageEnabled => serverStopToggle.Value;
        public bool ShutdownMessageEnabled => serverShutdownToggle.Value;
        public bool WorldSaveMessageEnabled => serverSaveToggle.Value;
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
        public bool RankedDeathLeaderboardEnabled => sendDeathRankingLeaderboard.Value;
        public bool RankedPingLeaderboardEnabled => sendPingRankingLeaderboard.Value;
        public bool RankedSessionLeaderboardEnabled => sendSessionRankingLeaderboard.Value;
        public bool RankedShoutLeaderboardEnabled => sendShoutRankingLeaderboard.Value;
        public bool MostSessionLeaderboardEnabled => sendMostSessionLeaderboard.Value;
        public bool MostPingLeaderboardEnabled => sendMostPingLeaderboard.Value;
        public bool MostDeathLeaderboardEnabled => sendMostDeathLeaderboard.Value;
        public bool MostShoutLeaderboardEnabled => sendMostShoutLeaderboard.Value;
        public bool LeastSessionLeaderboardEnabled => sendLeastSessionLeaderboard.Value;
        public bool LeastPingLeaderboardEnabled => sendLeastPingLeaderboard.Value;
        public bool LeastDeathLeaderboardEnabled => sendLeastDeathLeaderboard.Value;
        public bool LeastShoutLeaderboardEnabled => sendLeastShoutLeaderboard.Value;
        public bool AnnouncePlayerFirstDeathEnabled => announcePlayerFirstDeath.Value;
        public bool AnnouncePlayerFirstJoinEnabled => announcePlayerFirstJoin.Value;
        public bool AnnouncePlayerFirstLeaveEnabled => announcePlayerFirstLeave.Value;
        public bool AnnouncePlayerFirstPingEnabled => announcePlayerFirstPing.Value;
        public bool AnnouncePlayerFirstShoutEnabled => announcePlayerFirstShout.Value;
        public bool EventStartMessageEnabled => eventStartMessageToggle.Value;
        public bool EventPausedMessageEnabled => eventPausedMessageToggle.Value;
        public bool EventResumedMessageEnabled => eventResumedMessageToggle.Value;
        public bool EventStopMessageEnabled => eventStopMessageToggle.Value;
        public bool EventStartPosEnabled => eventStartPosToggle.Value;
        public bool EventPausedPosEnabled => eventPausedPosToggle.Value;
        public bool EventStopPosEnabled => eventStopPosToggle.Value;
        public bool EventResumedPosEnabled => eventResumedPosToggle.Value;
    }
}
