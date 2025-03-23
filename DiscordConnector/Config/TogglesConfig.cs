using System;

using BepInEx.Configuration;

namespace DiscordConnector.Config;

internal class TogglesConfig
{
    // Config Header Strings
    private const string MessagesToggles = "Toggles.Messages";
    private const string PositionToggles = "Toggles.Position";
    private const string StatsToggles = "Toggles.Stats";
    private const string PlayerFirstsToggles = "Toggles.PlayerFirsts";
    private const string DebugToggles = "Toggles.DebugMessages";
    public const string ConfigExtension = "toggles";

    // Player-firsts Settings
    private readonly ConfigEntry<bool> _announcePlayerFirstDeath;
    private readonly ConfigEntry<bool> _announcePlayerFirstJoin;
    private readonly ConfigEntry<bool> _announcePlayerFirstLeave;
    private readonly ConfigEntry<bool> _announcePlayerFirstPing;
    private readonly ConfigEntry<bool> _announcePlayerFirstShout;
    private readonly ConfigEntry<bool> _chatPingPosToggle;
    private readonly ConfigEntry<bool> _chatPingToggle;
    private readonly ConfigEntry<bool> _chatShoutAllCaps;
    private readonly ConfigEntry<bool> _chatShoutPosToggle;
    private readonly ConfigEntry<bool> _chatShoutToggle;
    private readonly ConfigEntry<bool> _collectStatsDeaths;

    // Statistic collection settings
    private readonly ConfigEntry<bool> _collectStatsJoins;
    private readonly ConfigEntry<bool> _collectStatsLeaves;
    private readonly ConfigEntry<bool> _collectStatsPings;
    private readonly ConfigEntry<bool> _collectStatsShouts;
    private readonly ConfigEntry<bool> _debugDatabaseMethods;
    private readonly ConfigEntry<bool> _debugEventChanges;

    // Debug Message Toggles
    private readonly ConfigEntry<bool> _debugEveryEventCheck;
    private readonly ConfigEntry<bool> _debugEveryEventPlayerPosCheck;
    private readonly ConfigEntry<bool> _debugHttpRequestResponses;
    private readonly ConfigEntry<bool> _debugLeaderboardOperations;
    private readonly ConfigEntry<bool> _eventPausedMessageToggle;
    private readonly ConfigEntry<bool> _eventPausedPosToggle;
    private readonly ConfigEntry<bool> _eventResumedMessageToggle;
    private readonly ConfigEntry<bool> _eventResumedPosToggle;
    private readonly ConfigEntry<bool> _eventStartMessageToggle;
    private readonly ConfigEntry<bool> _eventStartPosToggle;
    private readonly ConfigEntry<bool> _eventStopMessageToggle;
    private readonly ConfigEntry<bool> _eventStopPosToggle;
    private readonly ConfigEntry<bool> _newDayNumberToggle;
    private readonly ConfigEntry<bool> _playerDeathPosToggle;
    private readonly ConfigEntry<bool> _playerDeathToggle;
    private readonly ConfigEntry<bool> _playerJoinPosToggle;
    private readonly ConfigEntry<bool> _playerJoinToggle;

    // Position Coordinates Toggles
    private readonly ConfigEntry<bool> _playerLeavePosToggle;
    private readonly ConfigEntry<bool> _playerLeaveToggle;

    // Logged Information Toggles
    private readonly ConfigEntry<bool> _serverLaunchToggle;
    private readonly ConfigEntry<bool> _serverLoadedToggle;
    private readonly ConfigEntry<bool> _serverSaveToggle;
    private readonly ConfigEntry<bool> _serverShutdownToggle;
    private readonly ConfigEntry<bool> _serverStopToggle;

    public TogglesConfig(ConfigFile configFile)
    {
        // Message Toggles
        _serverLaunchToggle = configFile.Bind(MessagesToggles,
            "Server Launch Notifications",
            true,
            "If enabled, this will send a message to Discord when the server launches.");
        _serverLoadedToggle = configFile.Bind(MessagesToggles,
            "Server Loaded Notifications",
            true,
            "If enabled, this will send a message to Discord when the server has loaded the map and is ready for connections.");
        _serverStopToggle = configFile.Bind(MessagesToggles,
            "Server Stopping Notifications",
            true,
            "If enabled, this will send a message to Discord when the server begins shut down.");
        _serverShutdownToggle = configFile.Bind(MessagesToggles,
            "Server Shutdown Notifications",
            true,
            "If enabled, this will send a message to Discord when the server has shut down.");
        _serverSaveToggle = configFile.Bind(MessagesToggles,
            "Server World Save Notifications",
            true,
            "If enabled, this will send a message to Discord when the server saves the world.");
        _chatShoutToggle = configFile.Bind(MessagesToggles,
            "Chat Shout Messages Notifications",
            true,
            "If enabled, this will send a message to Discord when a player shouts on the server.");
        _chatPingToggle = configFile.Bind(MessagesToggles,
            "Ping Notifications",
            true,
            "If enabled, this will send a message to Discord when a player pings on the map.");
        _playerJoinToggle = configFile.Bind(MessagesToggles,
            "Player Join Notifications",
            true,
            "If enabled, this will send a message to Discord when a player joins the server.");
        _playerDeathToggle = configFile.Bind(MessagesToggles,
            "Player Death Notifications",
            true,
            "If enabled, this will send a message to Discord when a player dies on the server.");
        _playerLeaveToggle = configFile.Bind(MessagesToggles,
            "Player Leave Notifications",
            true,
            "If enabled, this will send a message to Discord when a player leaves the server.");
        _eventStartMessageToggle = configFile.Bind(MessagesToggles,
            "Event Start Notifications",
            true,
            "If enabled, this will send a message to Discord when a random event starts on the server.");
        _eventStopMessageToggle = configFile.Bind(MessagesToggles,
            "Event Stop Notifications",
            true,
            "If enabled, this will send a message to Discord when a random event stops on the server.");
        _eventPausedMessageToggle = configFile.Bind(MessagesToggles,
            "Event Paused Notifications",
            true,
            "If enabled, this will send a message to Discord when a random event is paused due to players leaving the area.");
        _eventResumedMessageToggle = configFile.Bind(MessagesToggles,
            "Event Resumed Notifications",
            true,
            "If enabled, this will send a message to Discord when a random event is resumed.");
        _chatShoutAllCaps = configFile.Bind(MessagesToggles,
            "Send All Caps Shout Messages",
            false,
            "If enabled, this will send all shout messages to Discord in all caps.");
        _newDayNumberToggle = configFile.Bind(MessagesToggles,
            "Send Message For New Day Number",
            true,
            "If enabled, this will send a message with the current day number on new days.");

        // Position Toggles
        _playerJoinPosToggle = configFile.Bind(PositionToggles,
            "Include POS With Player Join",
            false,
            "If enabled, this will include the coordinates of the player when they join.");
        _playerLeavePosToggle = configFile.Bind(PositionToggles,
            "Include POS With Player Leave",
            false,
            "If enabled, this will include the coordinates of the player when they leave.");
        _playerDeathPosToggle = configFile.Bind(PositionToggles,
            "Include POS With Player Death",
            true,
            "If enabled, this will include the coordinates of the player when they die.");
        _chatPingPosToggle = configFile.Bind(PositionToggles,
            "Ping Notifications Include Position",
            true,
            "If enabled, includes the coordinates of the ping.");
        _chatShoutPosToggle = configFile.Bind(PositionToggles,
            "Chat Shout Messages Position Notifications",
            false,
            "If enabled, this will include the coordinates of the player when they shout.");
        _eventStartPosToggle = configFile.Bind(PositionToggles,
            "Event Start Messages Position Notifications",
            true,
            "If enabled, this will include the coordinates of the random event when the start message is sent.");
        _eventStopPosToggle = configFile.Bind(PositionToggles,
            "Event Stop Messages Position Notifications",
            true,
            "If enabled, this will include the coordinates of the random event when the stop message is sent.");
        _eventPausedPosToggle = configFile.Bind(PositionToggles,
            "Event Paused Messages Position Notifications",
            true,
            "If enabled, this will include the coordinates of the random event when the paused message is sent.");
        _eventResumedPosToggle = configFile.Bind(PositionToggles,
            "Event Resumed Messages Position Notifications",
            true,
            "If enabled, this will include the coordinates of the random event when the resumed message is sent.");

        // Statistic Settings
        _collectStatsDeaths = configFile.Bind(StatsToggles,
            "Collect and Send Player Death Stats",
            true,
            "If enabled, will allow collection of the number of times a player has died.");
        _collectStatsJoins = configFile.Bind(StatsToggles,
            "Collect and Send Player Join Stats",
            true,
            "If enabled, will allow collection of how many times a player has joined the game.");
        _collectStatsLeaves = configFile.Bind(StatsToggles,
            "Collect and Send Player Leave Stats",
            true,
            "If enabled, will allow collection of how many times a player has left the game.");
        _collectStatsPings = configFile.Bind(StatsToggles,
            "Collect and Send Player Ping Stats",
            true,
            "If enabled, will allow collection of the number of pings made by a player.");
        _collectStatsShouts = configFile.Bind(StatsToggles,
            "Collect and Send Player Shout Stats",
            true,
            "If enabled, will allow collection of the number of times a player has shouted.");

        // Player Firsts
        _announcePlayerFirstDeath = configFile.Bind(PlayerFirstsToggles,
            "Send a Message for the First Death of a Player",
            true,
            "If enabled, this will send an extra message on a player's first death.");
        _announcePlayerFirstJoin = configFile.Bind(PlayerFirstsToggles,
            "Send a Message for the First Join of a Player",
            true,
            "If enabled, this will send an extra message on a player's first join to the server.");
        _announcePlayerFirstLeave = configFile.Bind(PlayerFirstsToggles,
            "Send a Message for the First Leave of a Player",
            false,
            "If enabled, this will send an extra message on a player's first leave from the server.");
        _announcePlayerFirstPing = configFile.Bind(PlayerFirstsToggles,
            "Send a Message for the First Ping of a Player",
            false,
            "If enabled, this will send an extra message on a player's first ping.");
        _announcePlayerFirstShout = configFile.Bind(PlayerFirstsToggles,
            "Send a Message for the First Shout of a Player",
            false,
            "If enabled, this will send an extra message on a player's first shout.");

        _debugEveryEventCheck = configFile.Bind(DebugToggles,
            "Debug Message for Every Event Check",
            false,
            "If enabled, this will write a log message at the DEBUG level every time it checks for an event (every 1s).");
        _debugEveryEventPlayerPosCheck = configFile.Bind(DebugToggles,
            "Debug Message for Every Event Player Location Check",
            false,
            "If enabled, this will write a log message at the DEBUG level every time the EventWatcher checks players' locations.");
        _debugEventChanges = configFile.Bind(DebugToggles,
            "Debug Message for Every Event Change",
            false,
            "If enabled, this will write a log message at the DEBUG level when a change in event status is detected.");
        _debugHttpRequestResponses = configFile.Bind(DebugToggles,
            "Debug Message for HTTP Request Responses",
            false,
            "If enabled, this will write a log message at the DEBUG level with the content of HTTP request responses." +
            Environment.NewLine +
            "Nearly all of these requests are when data is sent to the Discord Webhook.");
        _debugDatabaseMethods = configFile.Bind(DebugToggles,
            "Debug Message for Database Methods",
            false,
            "If enabled, this will write a log message at the DEBUG level with logs generated while executing database methods.");
        _debugLeaderboardOperations = configFile.Bind(DebugToggles,
            "Debug Leaderboard Operations",
            false,
            "If enabled, logs detailed information about leaderboard operations, including data retrieval and message sending.");

        configFile.Save();
    }

    public bool LaunchMessageEnabled => _serverLaunchToggle.Value;
    public bool LoadedMessageEnabled => _serverLoadedToggle.Value;
    public bool StopMessageEnabled => _serverStopToggle.Value;
    public bool ShutdownMessageEnabled => _serverShutdownToggle.Value;
    public bool WorldSaveMessageEnabled => _serverSaveToggle.Value;
    public bool ChatShoutEnabled => _chatShoutToggle.Value;
    public bool ChatShoutPosEnabled => _chatShoutPosToggle.Value;
    public bool ChatPingEnabled => _chatPingToggle.Value;
    public bool ChatPingPosEnabled => _chatPingPosToggle.Value;
    public bool PlayerJoinMessageEnabled => _playerJoinToggle.Value;
    public bool PlayerJoinPosEnabled => _playerJoinPosToggle.Value;
    public bool PlayerDeathMessageEnabled => _playerDeathToggle.Value;
    public bool PlayerDeathPosEnabled => _playerDeathPosToggle.Value;
    public bool PlayerLeaveMessageEnabled => _playerLeaveToggle.Value;
    public bool PlayerLeavePosEnabled => _playerLeavePosToggle.Value;
    public bool StatsDeathEnabled => _collectStatsDeaths.Value;
    public bool StatsJoinEnabled => _collectStatsJoins.Value;
    public bool StatsLeaveEnabled => _collectStatsLeaves.Value;
    public bool StatsPingEnabled => _collectStatsPings.Value;
    public bool StatsShoutEnabled => _collectStatsShouts.Value;
    public bool AnnouncePlayerFirstDeathEnabled => _announcePlayerFirstDeath.Value;
    public bool AnnouncePlayerFirstJoinEnabled => _announcePlayerFirstJoin.Value;
    public bool AnnouncePlayerFirstLeaveEnabled => _announcePlayerFirstLeave.Value;
    public bool AnnouncePlayerFirstPingEnabled => _announcePlayerFirstPing.Value;
    public bool AnnouncePlayerFirstShoutEnabled => _announcePlayerFirstShout.Value;
    public bool EventStartMessageEnabled => _eventStartMessageToggle.Value;
    public bool EventPausedMessageEnabled => _eventPausedMessageToggle.Value;
    public bool EventResumedMessageEnabled => _eventResumedMessageToggle.Value;
    public bool EventStopMessageEnabled => _eventStopMessageToggle.Value;
    public bool EventStartPosEnabled => _eventStartPosToggle.Value;
    public bool EventPausedPosEnabled => _eventPausedPosToggle.Value;
    public bool EventStopPosEnabled => _eventStopPosToggle.Value;
    public bool EventResumedPosEnabled => _eventResumedPosToggle.Value;
    public bool DebugEveryPlayerPosCheck => _debugEveryEventPlayerPosCheck.Value;
    public bool DebugEveryEventCheck => _debugEveryEventCheck.Value;
    public bool DebugEveryEventChange => _debugEventChanges.Value;
    public bool DebugHttpRequestResponse => _debugHttpRequestResponses.Value;
    public bool DebugDatabaseMethods => _debugDatabaseMethods.Value;
    public bool ChatShoutAllCaps => _chatShoutAllCaps.Value;
    public bool NewDayNumberEnabled => _newDayNumberToggle.Value;
    public bool DebugLeaderboardOperations => _debugLeaderboardOperations.Value;

    public string ConfigAsJson()
    {
        string jsonString = "{";
        jsonString += $"\"{MessagesToggles}\":{{";
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

        jsonString += $"\"{PositionToggles}\":{{";
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

        jsonString += $"\"{StatsToggles}\":{{";
        jsonString += $"\"statsDeathEnabled\":\"{StatsDeathEnabled}\",";
        jsonString += $"\"statsJoinEnabled\":\"{StatsJoinEnabled}\",";
        jsonString += $"\"statsLeaveEnabled\":\"{StatsLeaveEnabled}\",";
        jsonString += $"\"statsPingEnabled\":\"{StatsPingEnabled}\",";
        jsonString += $"\"statsShoutEnabled\":\"{StatsShoutEnabled}\"";
        jsonString += "},";

        jsonString += $"\"{PlayerFirstsToggles}\":{{";
        jsonString += $"\"announceFirstDeathEnabled\":\"{AnnouncePlayerFirstDeathEnabled}\",";
        jsonString += $"\"announceFirstJoinEnabled\":\"{AnnouncePlayerFirstJoinEnabled}\",";
        jsonString += $"\"announceFirstLeaveEnabled\":\"{AnnouncePlayerFirstLeaveEnabled}\",";
        jsonString += $"\"announceFirstPingEnabled\":\"{AnnouncePlayerFirstPingEnabled}\",";
        jsonString += $"\"announceFirstShoutEnabled\":\"{AnnouncePlayerFirstShoutEnabled}\"";
        jsonString += "},";

        jsonString += $"\"{DebugToggles}\":{{";
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
