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
		this._serverLaunchToggle = configFile.Bind(MessagesToggles,
			"Server Launch Notifications",
			true,
			"If enabled, this will send a message to Discord when the server launches.");
		this._serverLoadedToggle = configFile.Bind(MessagesToggles,
			"Server Loaded Notifications",
			true,
			"If enabled, this will send a message to Discord when the server has loaded the map and is ready for connections.");
		this._serverStopToggle = configFile.Bind(MessagesToggles,
			"Server Stopping Notifications",
			true,
			"If enabled, this will send a message to Discord when the server begins shut down.");
		this._serverShutdownToggle = configFile.Bind(MessagesToggles,
			"Server Shutdown Notifications",
			true,
			"If enabled, this will send a message to Discord when the server has shut down.");
		this._serverSaveToggle = configFile.Bind(MessagesToggles,
			"Server World Save Notifications",
			true,
			"If enabled, this will send a message to Discord when the server saves the world.");
		this._chatShoutToggle = configFile.Bind(MessagesToggles,
			"Chat Shout Messages Notifications",
			true,
			"If enabled, this will send a message to Discord when a player shouts on the server.");
		this._chatPingToggle = configFile.Bind(MessagesToggles,
			"Ping Notifications",
			true,
			"If enabled, this will send a message to Discord when a player pings on the map.");
		this._playerJoinToggle = configFile.Bind(MessagesToggles,
			"Player Join Notifications",
			true,
			"If enabled, this will send a message to Discord when a player joins the server.");
		this._playerDeathToggle = configFile.Bind(MessagesToggles,
			"Player Death Notifications",
			true,
			"If enabled, this will send a message to Discord when a player dies on the server.");
		this._playerLeaveToggle = configFile.Bind(MessagesToggles,
			"Player Leave Notifications",
			true,
			"If enabled, this will send a message to Discord when a player leaves the server.");
		this._eventStartMessageToggle = configFile.Bind(MessagesToggles,
			"Event Start Notifications",
			true,
			"If enabled, this will send a message to Discord when a random event starts on the server.");
		this._eventStopMessageToggle = configFile.Bind(MessagesToggles,
			"Event Stop Notifications",
			true,
			"If enabled, this will send a message to Discord when a random event stops on the server.");
		this._eventPausedMessageToggle = configFile.Bind(MessagesToggles,
			"Event Paused Notifications",
			true,
			"If enabled, this will send a message to Discord when a random event is paused due to players leaving the area.");
		this._eventResumedMessageToggle = configFile.Bind(MessagesToggles,
			"Event Resumed Notifications",
			true,
			"If enabled, this will send a message to Discord when a random event is resumed.");
		this._chatShoutAllCaps = configFile.Bind(MessagesToggles,
			"Send All Caps Shout Messages",
			false,
			"If enabled, this will send all shout messages to Discord in all caps.");
		this._newDayNumberToggle = configFile.Bind(MessagesToggles,
			"Send Message For New Day Number",
			true,
			"If enabled, this will send a message with the current day number on new days.");

		// Position Toggles
		this._playerJoinPosToggle = configFile.Bind(PositionToggles,
			"Include POS With Player Join",
			false,
			"If enabled, this will include the coordinates of the player when they join.");
		this._playerLeavePosToggle = configFile.Bind(PositionToggles,
			"Include POS With Player Leave",
			false,
			"If enabled, this will include the coordinates of the player when they leave.");
		this._playerDeathPosToggle = configFile.Bind(PositionToggles,
			"Include POS With Player Death",
			true,
			"If enabled, this will include the coordinates of the player when they die.");
		this._chatPingPosToggle = configFile.Bind(PositionToggles,
			"Ping Notifications Include Position",
			true,
			"If enabled, includes the coordinates of the ping.");
		this._chatShoutPosToggle = configFile.Bind(PositionToggles,
			"Chat Shout Messages Position Notifications",
			false,
			"If enabled, this will include the coordinates of the player when they shout.");
		this._eventStartPosToggle = configFile.Bind(PositionToggles,
			"Event Start Messages Position Notifications",
			true,
			"If enabled, this will include the coordinates of the random event when the start message is sent.");
		this._eventStopPosToggle = configFile.Bind(PositionToggles,
			"Event Stop Messages Position Notifications",
			true,
			"If enabled, this will include the coordinates of the random event when the stop message is sent.");
		this._eventPausedPosToggle = configFile.Bind(PositionToggles,
			"Event Paused Messages Position Notifications",
			true,
			"If enabled, this will include the coordinates of the random event when the paused message is sent.");
		this._eventResumedPosToggle = configFile.Bind(PositionToggles,
			"Event Resumed Messages Position Notifications",
			true,
			"If enabled, this will include the coordinates of the random event when the resumed message is sent.");

		// Statistic Settings
		this._collectStatsDeaths = configFile.Bind(StatsToggles,
			"Collect and Send Player Death Stats",
			true,
			"If enabled, will allow collection of the number of times a player has died.");
		this._collectStatsJoins = configFile.Bind(StatsToggles,
			"Collect and Send Player Join Stats",
			true,
			"If enabled, will allow collection of how many times a player has joined the game.");
		this._collectStatsLeaves = configFile.Bind(StatsToggles,
			"Collect and Send Player Leave Stats",
			true,
			"If enabled, will allow collection of how many times a player has left the game.");
		this._collectStatsPings = configFile.Bind(StatsToggles,
			"Collect and Send Player Ping Stats",
			true,
			"If enabled, will allow collection of the number of pings made by a player.");
		this._collectStatsShouts = configFile.Bind(StatsToggles,
			"Collect and Send Player Shout Stats",
			true,
			"If enabled, will allow collection of the number of times a player has shouted.");

		// Player Firsts
		this._announcePlayerFirstDeath = configFile.Bind(PlayerFirstsToggles,
			"Send a Message for the First Death of a Player",
			true,
			"If enabled, this will send an extra message on a player's first death.");
		this._announcePlayerFirstJoin = configFile.Bind(PlayerFirstsToggles,
			"Send a Message for the First Join of a Player",
			true,
			"If enabled, this will send an extra message on a player's first join to the server.");
		this._announcePlayerFirstLeave = configFile.Bind(PlayerFirstsToggles,
			"Send a Message for the First Leave of a Player",
			false,
			"If enabled, this will send an extra message on a player's first leave from the server.");
		this._announcePlayerFirstPing = configFile.Bind(PlayerFirstsToggles,
			"Send a Message for the First Ping of a Player",
			false,
			"If enabled, this will send an extra message on a player's first ping.");
		this._announcePlayerFirstShout = configFile.Bind(PlayerFirstsToggles,
			"Send a Message for the First Shout of a Player",
			false,
			"If enabled, this will send an extra message on a player's first shout.");

		this._debugEveryEventCheck = configFile.Bind(DebugToggles,
			"Debug Message for Every Event Check",
			false,
			"If enabled, this will write a log message at the DEBUG level every time it checks for an event (every 1s).");
		this._debugEveryEventPlayerPosCheck = configFile.Bind(DebugToggles,
			"Debug Message for Every Event Player Location Check",
			false,
			"If enabled, this will write a log message at the DEBUG level every time the EventWatcher checks players' locations.");
		this._debugEventChanges = configFile.Bind(DebugToggles,
			"Debug Message for Every Event Change",
			false,
			"If enabled, this will write a log message at the DEBUG level when a change in event status is detected.");
		this._debugHttpRequestResponses = configFile.Bind(DebugToggles,
			"Debug Message for HTTP Request Responses",
			false,
			"If enabled, this will write a log message at the DEBUG level with the content of HTTP request responses." +
			Environment.NewLine +
			"Nearly all of these requests are when data is sent to the Discord Webhook.");
		this._debugDatabaseMethods = configFile.Bind(DebugToggles,
			"Debug Message for Database Methods",
			false,
			"If enabled, this will write a log message at the DEBUG level with logs generated while executing database methods.");
		this._debugLeaderboardOperations = configFile.Bind(DebugToggles,
			"Debug Leaderboard Operations",
			false,
			"If enabled, logs detailed information about leaderboard operations, including data retrieval and message sending.");

		configFile.Save();
	}

	public bool LaunchMessageEnabled => this._serverLaunchToggle.Value;
	public bool LoadedMessageEnabled => this._serverLoadedToggle.Value;
	public bool StopMessageEnabled => this._serverStopToggle.Value;
	public bool ShutdownMessageEnabled => this._serverShutdownToggle.Value;
	public bool WorldSaveMessageEnabled => this._serverSaveToggle.Value;
	public bool ChatShoutEnabled => this._chatShoutToggle.Value;
	public bool ChatShoutPosEnabled => this._chatShoutPosToggle.Value;
	public bool ChatPingEnabled => this._chatPingToggle.Value;
	public bool ChatPingPosEnabled => this._chatPingPosToggle.Value;
	public bool PlayerJoinMessageEnabled => this._playerJoinToggle.Value;
	public bool PlayerJoinPosEnabled => this._playerJoinPosToggle.Value;
	public bool PlayerDeathMessageEnabled => this._playerDeathToggle.Value;
	public bool PlayerDeathPosEnabled => this._playerDeathPosToggle.Value;
	public bool PlayerLeaveMessageEnabled => this._playerLeaveToggle.Value;
	public bool PlayerLeavePosEnabled => this._playerLeavePosToggle.Value;
	public bool StatsDeathEnabled => this._collectStatsDeaths.Value;
	public bool StatsJoinEnabled => this._collectStatsJoins.Value;
	public bool StatsLeaveEnabled => this._collectStatsLeaves.Value;
	public bool StatsPingEnabled => this._collectStatsPings.Value;
	public bool StatsShoutEnabled => this._collectStatsShouts.Value;
	public bool AnnouncePlayerFirstDeathEnabled => this._announcePlayerFirstDeath.Value;
	public bool AnnouncePlayerFirstJoinEnabled => this._announcePlayerFirstJoin.Value;
	public bool AnnouncePlayerFirstLeaveEnabled => this._announcePlayerFirstLeave.Value;
	public bool AnnouncePlayerFirstPingEnabled => this._announcePlayerFirstPing.Value;
	public bool AnnouncePlayerFirstShoutEnabled => this._announcePlayerFirstShout.Value;
	public bool EventStartMessageEnabled => this._eventStartMessageToggle.Value;
	public bool EventPausedMessageEnabled => this._eventPausedMessageToggle.Value;
	public bool EventResumedMessageEnabled => this._eventResumedMessageToggle.Value;
	public bool EventStopMessageEnabled => this._eventStopMessageToggle.Value;
	public bool EventStartPosEnabled => this._eventStartPosToggle.Value;
	public bool EventPausedPosEnabled => this._eventPausedPosToggle.Value;
	public bool EventStopPosEnabled => this._eventStopPosToggle.Value;
	public bool EventResumedPosEnabled => this._eventResumedPosToggle.Value;
	public bool DebugEveryPlayerPosCheck => this._debugEveryEventPlayerPosCheck.Value;
	public bool DebugEveryEventCheck => this._debugEveryEventCheck.Value;
	public bool DebugEveryEventChange => this._debugEventChanges.Value;
	public bool DebugHttpRequestResponse => this._debugHttpRequestResponses.Value;
	public bool DebugDatabaseMethods => this._debugDatabaseMethods.Value;
	public bool ChatShoutAllCaps => this._chatShoutAllCaps.Value;
	public bool NewDayNumberEnabled => this._newDayNumberToggle.Value;
	public bool DebugLeaderboardOperations => this._debugLeaderboardOperations.Value;

	public string ConfigAsJson()
	{
		var jsonString = "{";
		jsonString += $"\"{MessagesToggles}\":{{";
		jsonString += $"\"launchMessageEnabled\":\"{this.LaunchMessageEnabled}\",";
		jsonString += $"\"loadedMessageEnabled\":\"{this.LoadedMessageEnabled}\",";
		jsonString += $"\"stopMessageEnabled\":\"{this.StopMessageEnabled}\",";
		jsonString += $"\"shutdownMessageEnabled\":\"{this.ShutdownMessageEnabled}\",";
		jsonString += $"\"chatShoutEnabled\":\"{this.ChatShoutEnabled}\",";
		jsonString += $"\"chatPingEnabled\":\"{this.ChatPingEnabled}\",";
		jsonString += $"\"playerJoinEnabled\":\"{this.PlayerJoinMessageEnabled}\",";
		jsonString += $"\"playerLeaveEnabled\":\"{this.PlayerLeaveMessageEnabled}\",";
		jsonString += $"\"playerDeathEnabled\":\"{this.PlayerDeathMessageEnabled}\",";
		jsonString += $"\"eventStartEnabled\":\"{this.EventStartMessageEnabled}\",";
		jsonString += $"\"eventPausedEnabled\":\"{this.EventStopMessageEnabled}\",";
		jsonString += $"\"eventStoppedEnabled\":\"{this.EventPausedMessageEnabled}\",";
		jsonString += $"\"eventResumedEnabled\":\"{this.EventResumedMessageEnabled}\",";
		jsonString += $"\"chatShoutAllCaps\":\"{this.ChatShoutAllCaps}\",";
		jsonString += $"\"newDayNumberToggle\":\"{this.NewDayNumberEnabled}\"";
		jsonString += "},";

		jsonString += $"\"{PositionToggles}\":{{";
		jsonString += $"\"chatShoutPosEnabled\":\"{this.ChatShoutPosEnabled}\",";
		jsonString += $"\"chatPingPosEnabled\":\"{this.ChatPingPosEnabled}\",";
		jsonString += $"\"playerJoinPosEnabled\":\"{this.PlayerJoinPosEnabled}\",";
		jsonString += $"\"playerLeavePosEnabled\":\"{this.PlayerLeavePosEnabled}\",";
		jsonString += $"\"playerDeathPosEnabled\":\"{this.PlayerDeathPosEnabled}\",";
		jsonString += $"\"eventStartPosEnabled\":\"{this.EventStartPosEnabled}\",";
		jsonString += $"\"eventStopPosEnabled\":\"{this.EventStopPosEnabled}\",";
		jsonString += $"\"eventPausedPosEnabled\":\"{this.EventPausedPosEnabled}\",";
		jsonString += $"\"eventResumedPosEnabled\":\"{this.EventResumedPosEnabled}\"";
		jsonString += "},";

		jsonString += $"\"{StatsToggles}\":{{";
		jsonString += $"\"statsDeathEnabled\":\"{this.StatsDeathEnabled}\",";
		jsonString += $"\"statsJoinEnabled\":\"{this.StatsJoinEnabled}\",";
		jsonString += $"\"statsLeaveEnabled\":\"{this.StatsLeaveEnabled}\",";
		jsonString += $"\"statsPingEnabled\":\"{this.StatsPingEnabled}\",";
		jsonString += $"\"statsShoutEnabled\":\"{this.StatsShoutEnabled}\"";
		jsonString += "},";

		jsonString += $"\"{PlayerFirstsToggles}\":{{";
		jsonString += $"\"announceFirstDeathEnabled\":\"{this.AnnouncePlayerFirstDeathEnabled}\",";
		jsonString += $"\"announceFirstJoinEnabled\":\"{this.AnnouncePlayerFirstJoinEnabled}\",";
		jsonString += $"\"announceFirstLeaveEnabled\":\"{this.AnnouncePlayerFirstLeaveEnabled}\",";
		jsonString += $"\"announceFirstPingEnabled\":\"{this.AnnouncePlayerFirstPingEnabled}\",";
		jsonString += $"\"announceFirstShoutEnabled\":\"{this.AnnouncePlayerFirstShoutEnabled}\"";
		jsonString += "},";

		jsonString += $"\"{DebugToggles}\":{{";
		jsonString += $"\"debugEveryPlayerPosCheck\":\"{this.DebugEveryPlayerPosCheck}\",";
		jsonString += $"\"debugEveryEventCheck\":\"{this.DebugEveryEventCheck}\",";
		jsonString += $"\"debugEventChanges\":\"{this.DebugEveryEventChange}\",";
		jsonString += $"\"debugDatabaseMethods\":\"{this.DebugDatabaseMethods}\",";
		jsonString += $"\"debugHttpRequestResponses\":\"{this.DebugHttpRequestResponse}\",";
		jsonString += $"\"debugLeaderboardOperations\":\"{this.DebugLeaderboardOperations}\"";
		jsonString += "}";

		jsonString += "}";
		return jsonString;
	}
}
