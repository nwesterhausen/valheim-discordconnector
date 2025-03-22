using System;
using BepInEx.Configuration;

namespace DiscordConnector.Config;

internal class TogglesConfig
{
    // Config Header Strings
    private const string MESSAGES_TOGGLES = "Toggles.Messages";
    private const string POSITION_TOGGLES = "Toggles.Position";
    private const string STATS_TOGGLES = "Toggles.Stats";
    private const string PLAYER_FIRSTS_TOGGLES = "Toggles.PlayerFirsts";
    private const string DEBUG_TOGGLES = "Toggles.DebugMessages";
    private static ConfigFile config;
    public static string ConfigExtension = "toggles";

    // Player-firsts Settings
    private ConfigEntry<bool> announcePlayerFirstDeath;
    private ConfigEntry<bool> announcePlayerFirstJoin;
    private ConfigEntry<bool> announcePlayerFirstLeave;
    private ConfigEntry<bool> announcePlayerFirstPing;
    private ConfigEntry<bool> announcePlayerFirstShout;
    private ConfigEntry<bool> chatPingPosToggle;
    private ConfigEntry<bool> chatPingToggle;
    private ConfigEntry<bool> chatShoutAllCaps;
    private ConfigEntry<bool> chatShoutPosToggle;
    private ConfigEntry<bool> chatShoutToggle;
    private ConfigEntry<bool> collectStatsDeaths;

    // Statistic collection settings
    private ConfigEntry<bool> collectStatsJoins;
    private ConfigEntry<bool> collectStatsLeaves;
    private ConfigEntry<bool> collectStatsPings;
    private ConfigEntry<bool> collectStatsShouts;
    private ConfigEntry<bool> debugDatabaseMethods;
    private ConfigEntry<bool> debugEventChanges;

    // Debug Message Toggles
    private ConfigEntry<bool> debugEveryEventCheck;
    private ConfigEntry<bool> debugEveryEventPlayerPosCheck;
    private ConfigEntry<bool> debugHttpRequestResponses;
    private ConfigEntry<bool> debugLeaderboardOperations;
    private ConfigEntry<bool> eventPausedMessageToggle;
    private ConfigEntry<bool> eventPausedPosToggle;
    private ConfigEntry<bool> eventResumedMessageToggle;
    private ConfigEntry<bool> eventResumedPosToggle;
    private ConfigEntry<bool> eventStartMessageToggle;
    private ConfigEntry<bool> eventStartPosToggle;
    private ConfigEntry<bool> eventStopMessageToggle;
    private ConfigEntry<bool> eventStopPosToggle;
    private ConfigEntry<bool> newDayNumberToggle;
    private ConfigEntry<bool> playerDeathPosToggle;
    private ConfigEntry<bool> playerDeathToggle;
    private ConfigEntry<bool> playerJoinPosToggle;
    private ConfigEntry<bool> playerJoinToggle;

    // Position Coordinates Toggles
    private ConfigEntry<bool> playerLeavePosToggle;
    private ConfigEntry<bool> playerLeaveToggle;

    // Logged Information Toggles
    private ConfigEntry<bool> serverLaunchToggle;
    private ConfigEntry<bool> serverLoadedToggle;
    private ConfigEntry<bool> serverSaveToggle;
    private ConfigEntry<bool> serverShutdownToggle;
    private ConfigEntry<bool> serverStopToggle;

    public TogglesConfig(ConfigFile configFile)
    {
        config = configFile;

        // Message Toggles
        serverLaunchToggle = config.Bind(MESSAGES_TOGGLES,
            "Server Launch Notifications",
            true,
            "If enabled, this will send a message to Discord when the server launches.");
        serverLoadedToggle = config.Bind(MESSAGES_TOGGLES,
            "Server Loaded Notifications",
            true,
            "If enabled, this will send a message to Discord when the server has loaded the map and is ready for connections.");
        serverStopToggle = config.Bind(MESSAGES_TOGGLES,
            "Server Stopping Notifications",
            true,
            "If enabled, this will send a message to Discord when the server begins shut down.");
        serverShutdownToggle = config.Bind(MESSAGES_TOGGLES,
            "Server Shutdown Notifications",
            true,
            "If enabled, this will send a message to Discord when the server has shut down.");
        serverSaveToggle = config.Bind(MESSAGES_TOGGLES,
            "Server World Save Notifications",
            true,
            "If enabled, this will send a message to Discord when the server saves the world.");
        chatShoutToggle = config.Bind(MESSAGES_TOGGLES,
            "Chat Shout Messages Notifications",
            true,
            "If enabled, this will send a message to Discord when a player shouts on the server.");
        chatPingToggle = config.Bind(MESSAGES_TOGGLES,
            "Ping Notifications",
            true,
            "If enabled, this will send a message to Discord when a player pings on the map.");
        playerJoinToggle = config.Bind(MESSAGES_TOGGLES,
            "Player Join Notifications",
            true,
            "If enabled, this will send a message to Discord when a player joins the server.");
        playerDeathToggle = config.Bind(MESSAGES_TOGGLES,
            "Player Death Notifications",
            true,
            "If enabled, this will send a message to Discord when a player dies on the server.");
        playerLeaveToggle = config.Bind(MESSAGES_TOGGLES,
            "Player Leave Notifications",
            true,
            "If enabled, this will send a message to Discord when a player leaves the server.");
        eventStartMessageToggle = config.Bind(MESSAGES_TOGGLES,
            "Event Start Notifications",
            true,
            "If enabled, this will send a message to Discord when a random event starts on the server.");
        eventStopMessageToggle = config.Bind(MESSAGES_TOGGLES,
            "Event Stop Notifications",
            true,
            "If enabled, this will send a message to Discord when a random event stops on the server.");
        eventPausedMessageToggle = config.Bind(MESSAGES_TOGGLES,
            "Event Paused Notifications",
            true,
            "If enabled, this will send a message to Discord when a random event is paused due to players leaving the area.");
        eventResumedMessageToggle = config.Bind(MESSAGES_TOGGLES,
            "Event Resumed Notifications",
            true,
            "If enabled, this will send a message to Discord when a random event is resumed.");
        chatShoutAllCaps = config.Bind(MESSAGES_TOGGLES,
            "Send All Caps Shout Messages",
            false,
            "If enabled, this will send all shout messages to Discord in all caps.");
        newDayNumberToggle = config.Bind(MESSAGES_TOGGLES,
            "Send Message For New Day Number",
            true,
            "If enabled, this will send a message with the current day number on new days.");

        // Position Toggles
        playerJoinPosToggle = config.Bind(POSITION_TOGGLES,
            "Include POS With Player Join",
            false,
            "If enabled, this will include the coordinates of the player when they join.");
        playerLeavePosToggle = config.Bind(POSITION_TOGGLES,
            "Include POS With Player Leave",
            false,
            "If enabled, this will include the coordinates of the player when they leave.");
        playerDeathPosToggle = config.Bind(POSITION_TOGGLES,
            "Include POS With Player Death",
            true,
            "If enabled, this will include the coordinates of the player when they die.");
        chatPingPosToggle = config.Bind(POSITION_TOGGLES,
            "Ping Notifications Include Position",
            true,
            "If enabled, includes the coordinates of the ping.");
        chatShoutPosToggle = config.Bind(POSITION_TOGGLES,
            "Chat Shout Messages Position Notifications",
            false,
            "If enabled, this will include the coordinates of the player when they shout.");
        eventStartPosToggle = config.Bind(POSITION_TOGGLES,
            "Event Start Messages Position Notifications",
            true,
            "If enabled, this will include the coordinates of the random event when the start message is sent.");
        eventStopPosToggle = config.Bind(POSITION_TOGGLES,
            "Event Stop Messages Position Notifications",
            true,
            "If enabled, this will include the coordinates of the random event when the stop message is sent.");
        eventPausedPosToggle = config.Bind(POSITION_TOGGLES,
            "Event Paused Messages Position Notifications",
            true,
            "If enabled, this will include the coordinates of the random event when the paused message is sent.");
        eventResumedPosToggle = config.Bind(POSITION_TOGGLES,
            "Event Resumed Messages Position Notifications",
            true,
            "If enabled, this will include the coordinates of the random event when the resumed message is sent.");

        // Statistic Settings
        collectStatsDeaths = config.Bind(STATS_TOGGLES,
            "Collect and Send Player Death Stats",
            true,
            "If enabled, will allow collection of the number of times a player has died.");
        collectStatsJoins = config.Bind(STATS_TOGGLES,
            "Collect and Send Player Join Stats",
            true,
            "If enabled, will allow collection of how many times a player has joined the game.");
        collectStatsLeaves = config.Bind(STATS_TOGGLES,
            "Collect and Send Player Leave Stats",
            true,
            "If enabled, will allow collection of how many times a player has left the game.");
        collectStatsPings = config.Bind(STATS_TOGGLES,
            "Collect and Send Player Ping Stats",
            true,
            "If enabled, will allow collection of the number of pings made by a player.");
        collectStatsShouts = config.Bind(STATS_TOGGLES,
            "Collect and Send Player Shout Stats",
            true,
            "If enabled, will allow collection of the number of times a player has shouted.");

        // Player Firsts
        announcePlayerFirstDeath = config.Bind(PLAYER_FIRSTS_TOGGLES,
            "Send a Message for the First Death of a Player",
            true,
            "If enabled, this will send an extra message on a player's first death.");
        announcePlayerFirstJoin = config.Bind(PLAYER_FIRSTS_TOGGLES,
            "Send a Message for the First Join of a Player",
            true,
            "If enabled, this will send an extra message on a player's first join to the server.");
        announcePlayerFirstLeave = config.Bind(PLAYER_FIRSTS_TOGGLES,
            "Send a Message for the First Leave of a Player",
            false,
            "If enabled, this will send an extra message on a player's first leave from the server.");
        announcePlayerFirstPing = config.Bind(PLAYER_FIRSTS_TOGGLES,
            "Send a Message for the First Ping of a Player",
            false,
            "If enabled, this will send an extra message on a player's first ping.");
        announcePlayerFirstShout = config.Bind(PLAYER_FIRSTS_TOGGLES,
            "Send a Message for the First Shout of a Player",
            false,
            "If enabled, this will send an extra message on a player's first shout.");

        debugEveryEventCheck = config.Bind(DEBUG_TOGGLES,
            "Debug Message for Every Event Check",
            false,
            "If enabled, this will write a log message at the DEBUG level every time it checks for an event (every 1s).");
        debugEveryEventPlayerPosCheck = config.Bind(DEBUG_TOGGLES,
            "Debug Message for Every Event Player Location Check",
            false,
            "If enabled, this will write a log message at the DEBUG level every time the EventWatcher checks players' locations.");
        debugEventChanges = config.Bind(DEBUG_TOGGLES,
            "Debug Message for Every Event Change",
            false,
            "If enabled, this will write a log message at the DEBUG level when a change in event status is detected.");
        debugHttpRequestResponses = config.Bind(DEBUG_TOGGLES,
            "Debug Message for HTTP Request Responses",
            false,
            "If enabled, this will write a log message at the DEBUG level with the content of HTTP request responses." +
            Environment.NewLine +
            "Nearly all of these requests are when data is sent to the Discord Webhook.");
        debugDatabaseMethods = config.Bind(DEBUG_TOGGLES,
            "Debug Message for Database Methods",
            false,
            "If enabled, this will write a log message at the DEBUG level with logs generated while executing database methods.");
        debugLeaderboardOperations = config.Bind(DEBUG_TOGGLES, 
            "Debug Leaderboard Operations", 
            false,
            "If enabled, logs detailed information about leaderboard operations, including data retrieval and message sending.");

        config.Save();
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
    public bool PlayerDeathMessageEnabled => playerDeathToggle.Value;
    public bool PlayerDeathPosEnabled => playerDeathPosToggle.Value;
    public bool PlayerLeaveMessageEnabled => playerLeaveToggle.Value;
    public bool PlayerLeavePosEnabled => playerLeavePosToggle.Value;
    public bool StatsDeathEnabled => collectStatsDeaths.Value;
    public bool StatsJoinEnabled => collectStatsJoins.Value;
    public bool StatsLeaveEnabled => collectStatsLeaves.Value;
    public bool StatsPingEnabled => collectStatsPings.Value;
    public bool StatsShoutEnabled => collectStatsShouts.Value;
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
    public bool DebugEveryPlayerPosCheck => debugEveryEventPlayerPosCheck.Value;
    public bool DebugEveryEventCheck => debugEveryEventCheck.Value;
    public bool DebugEveryEventChange => debugEventChanges.Value;
    public bool DebugHttpRequestResponse => debugHttpRequestResponses.Value;
    public bool DebugDatabaseMethods => debugDatabaseMethods.Value;
    public bool ChatShoutAllCaps => chatShoutAllCaps.Value;
    public bool NewDayNumberEnabled => newDayNumberToggle.Value;
    public bool DebugLeaderboardOperations => debugLeaderboardOperations.Value;

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
        jsonString += $"\"eventResumedEnabled\":\"{EventResumedMessageEnabled}\",";
        jsonString += $"\"chatShoutAllCaps\":\"{ChatShoutAllCaps}\",";
        jsonString += $"\"newDayNumberToggle\":\"{NewDayNumberEnabled}\"";
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

        jsonString += $"\"{PLAYER_FIRSTS_TOGGLES}\":{{";
        jsonString += $"\"announceFirstDeathEnabled\":\"{AnnouncePlayerFirstDeathEnabled}\",";
        jsonString += $"\"announceFirstJoinEnabled\":\"{AnnouncePlayerFirstJoinEnabled}\",";
        jsonString += $"\"announceFirstLeaveEnabled\":\"{AnnouncePlayerFirstLeaveEnabled}\",";
        jsonString += $"\"announceFirstPingEnabled\":\"{AnnouncePlayerFirstPingEnabled}\",";
        jsonString += $"\"announceFirstShoutEnabled\":\"{AnnouncePlayerFirstShoutEnabled}\"";
        jsonString += "},";

        jsonString += $"\"{DEBUG_TOGGLES}\":{{";
        jsonString += $"\"debugEveryPlayerPosCheck\":\"{DebugEveryPlayerPosCheck}\",";
        jsonString += $"\"debugEveryEventCheck\":\"{DebugEveryEventCheck}\",";
        jsonString += $"\"debugEventChanges\":\"{DebugEveryEventChange}\",";
        jsonString += $"\"debugDatabaseMethods\":\"{DebugDatabaseMethods}\",";
        jsonString += $"\"debugHttpRequestResponses\":\"{DebugHttpRequestResponse}\",";
        jsonString += $"\"debugLeaderboardOperations\":\"{DebugLeaderboardOperations}\"";
        jsonString += "}";

        jsonString += "}";
        return jsonString;
    }
}
