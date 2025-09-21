using System;
using BepInEx.Configuration;

namespace DiscordConnector.Config;

internal class MessagesConfig
{
	// config header strings
	private const string ServerMessages = "Messages.Server";
	private const string PlayerMessages = "Messages.Player";
	private const string PlayerFirstsMessages = "Messages.PlayerFirsts";
	private const string EventMessages = "Messages.Events";
	private const string BoardMessages = "Messages.LeaderBoards";

	public const string ConfigExtension = "messages";
	private readonly ConfigEntry<string> _eventPausedMessage;
	private readonly ConfigEntry<string> _eventResumedMessage;

	// Event Messages
	private readonly ConfigEntry<string> _eventStartMessage;
	private readonly ConfigEntry<string> _eventStopMessage;
	private readonly ConfigEntry<string> _leaderBoardBottomPlayersMessage;
	private readonly ConfigEntry<string> _leaderBoardHighestPlayerMessage;
	private readonly ConfigEntry<string> _leaderBoardLowestPlayerMessage;

	// Board Messages
	private readonly ConfigEntry<string> _leaderBoardTopPlayersMessage;
	private readonly ConfigEntry<string> _playerDeathMessage;
	private readonly ConfigEntry<string> _playerFirstDeathMessage;

	// Player First Messages
	private readonly ConfigEntry<string> _playerFirstJoinMessage;
	private readonly ConfigEntry<string> _playerFirstLeaveMessage;
	private readonly ConfigEntry<string> _playerFirstPingMessage;
	private readonly ConfigEntry<string> _playerFirstShoutMessage;

	// Player Messages
	private readonly ConfigEntry<string> _playerJoinMessage;
	private readonly ConfigEntry<string> _playerLeaveMessage;
	private readonly ConfigEntry<string> _playerPingMessage;
	private readonly ConfigEntry<string> _playerShoutMessage;

	// Server Messages
	private readonly ConfigEntry<string> _serverLaunchMessage;
	private readonly ConfigEntry<string> _serverLoadedMessage;
	private readonly ConfigEntry<string> _serverNewDayMessage;
	private readonly ConfigEntry<string> _serverSavedMessage;
	private readonly ConfigEntry<string> _serverShutdownMessage;
	private readonly ConfigEntry<string> _serverStopMessage;

	public MessagesConfig(ConfigFile configFile)
	{
		// Messages.Server
		this._serverLaunchMessage = configFile.Bind<string>(ServerMessages,
			"Server Launch Message",
			"Server is starting up.",
			"Set the message that will be sent when the server starts up." + Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
			Environment.NewLine +
			"Random choice example: Server is starting;Server beginning to load" + Environment.NewLine +
			"If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.");
		this._serverLoadedMessage = configFile.Bind<string>(ServerMessages,
			"Server Started Message",
			"Server has started!",
			"Set the message that will be sent when the server has loaded the map and is ready for connections." +
			Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
			Environment.NewLine +
			"If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.");
		this._serverStopMessage = configFile.Bind<string>(ServerMessages,
			"Server Stop Message",
			"Server is stopping.",
			"Set the message that will be sent when the server shuts down." + Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
			Environment.NewLine +
			"If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.");
		this._serverShutdownMessage = configFile.Bind<string>(ServerMessages,
			"Server Shutdown Message",
			"Server has stopped!",
			"Set the message that will be sent when the server finishes shutting down." + Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
			Environment.NewLine +
			"If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.");
		this._serverSavedMessage = configFile.Bind<string>(ServerMessages,
			"Server Saved Message",
			"The world has been saved.",
			"Set the message that will be sent when the server saves the world data." + Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
			Environment.NewLine +
			"If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.");
		this._serverNewDayMessage = configFile.Bind<string>(ServerMessages,
			"Server New Day Message",
			"Day Number %DAY_NUMBER%",
			"Set the message that will be sent when a new day starts." + Environment.NewLine +
			"The %DAY_NUMBER% variable gets replaced with the day number.");

		// Messages.Player
		this._playerJoinMessage = configFile.Bind<string>(PlayerMessages,
			"Player Join Message",
			"%PLAYER_NAME% has joined.",
			"Set the message that will be sent when a player joins the server" + Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
			Environment.NewLine +
			"Random choice example: %PLAYER_NAME% has joined;%PLAYER_NAME% awakens;%PLAYER_NAME% arrives");
		this._playerDeathMessage = configFile.Bind<string>(PlayerMessages,
			"Player Death Message",
			"%PLAYER_NAME% has died.",
			"Set the message that will be sent when a player dies." + Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
		this._playerLeaveMessage = configFile.Bind<string>(PlayerMessages,
			"Player Leave Message",
			"%PLAYER_NAME% has left.",
			"Set the message that will be sent when a player leaves the server." + Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
		this._playerPingMessage = configFile.Bind<string>(PlayerMessages,
			"Player Ping Message",
			"%PLAYER_NAME% pings the map.",
			"Set the message that will be sent when a player pings the map." + Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
		this._playerShoutMessage = configFile.Bind<string>(PlayerMessages,
			"Player Shout Message",
			"%PLAYER_NAME% shouts **%SHOUT%**.",
			"Set the message that will be sent when a player shouts on the server. You can put %SHOUT% anywhere you want the content of the shout to be." +
			Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");

		// Messages.PlayerFirsts
		this._playerFirstJoinMessage = configFile.Bind<string>(PlayerFirstsMessages,
			"Player First Join Message",
			"Welcome %PLAYER_NAME%, it's their first time on the server!",
			"Set the message that will be sent when a player joins the server" + Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
		this._playerFirstDeathMessage = configFile.Bind<string>(PlayerFirstsMessages,
			"Player First Death Message",
			"%PLAYER_NAME% has died for the first time.",
			"Set the message that will be sent when a player dies." + Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
		this._playerFirstLeaveMessage = configFile.Bind<string>(PlayerFirstsMessages,
			"Player First Leave Message",
			"%PLAYER_NAME% has left for the first time.",
			"Set the message that will be sent when a player leaves the server." + Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
		this._playerFirstPingMessage = configFile.Bind<string>(PlayerFirstsMessages,
			"Player First Ping Message",
			"%PLAYER_NAME% pings the map for the first time.",
			"Set the message that will be sent when a player pings the map." + Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
		this._playerFirstShoutMessage = configFile.Bind<string>(PlayerFirstsMessages,
			"Player First Shout Message",
			"%PLAYER_NAME% shouts for the first time.",
			"Set the message that will be sent when a player shouts on the server. %SHOUT% works in this message to include what was shouted." +
			Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");

		// Messages.Events
		this._eventStartMessage = configFile.Bind<string>(EventMessages,
			"Event Start Message",
			"**Event**: %EVENT_MSG%",
			"Set the message that will be sent when a random event starts on the server. Sending the coordinates is enabled by default in the toggles config." +
			Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
			Environment.NewLine +
			"The special string %EVENT_MSG% will be replaced with the message that is displayed on the screen when the event starts."); // + Environment.NewLine +
		// "The special string %PLAYERS% will be replaced with a list of players in the event area."); //! Removed due to unreliability
		this._eventStopMessage = configFile.Bind<string>(EventMessages,
			"Event Stop Message",
			"**Event**: %EVENT_MSG%",
			"Set the message that will be sent when a random event stops on the server. Sending the coordinates is enabled by default in the toggles config." +
			Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
			Environment.NewLine +
			"The special string %EVENT_MSG% will be replaced with the message that is displayed on the screen when the event stops.");
		this._eventPausedMessage = configFile.Bind<string>(EventMessages,
			"Event Paused Message",
			"**Event**: %EVENT_END_MSG% — for now! (Currently paused due to no players in the event area.)",
			"Set the message that will be sent when a random event is paused due to players leaving the area. Sending the coordinates is enabled by default in the toggles config." +
			Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
			Environment.NewLine +
			"The special string %EVENT_START_MSG% will be replaced with the message that is displayed on the screen when the event starts." +
			Environment.NewLine +
			"The special string %EVENT_END_MSG% will be replaced with the message that is displayed on the screen when the event ends.");
		this._eventResumedMessage = configFile.Bind<string>(EventMessages,
			"Event Resumed Message",
			"**Event**: %EVENT_START_MSG%",
			"Set the message that will be sent when a random event is resumed due to players re-entering the area. Sending the coordinates is enabled by default in the toggles config." +
			Environment.NewLine +
			"If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
			Environment.NewLine +
			"The special string %EVENT_START_MSG% will be replaced with the message that is displayed on the screen when the event starts." +
			Environment.NewLine +
			"The special string %EVENT_END_MSG% will be replaced with the message that is displayed on the screen when the event ends."); // + Environment.NewLine +
		// "The special string %PLAYERS% will be replaced with a list of players in the event area."); //! Removed due to unreliability

		// Board Messages
		this._leaderBoardTopPlayersMessage = configFile.Bind<string>(BoardMessages,
			"Leader Board Heading for Top N Players",
			"Top %N% Player Leader Boards:",
			"Set the message that is included as a heading when this leader board is sent." + Environment.NewLine +
			"Include %N% to include the number of rankings returned (the configured number)");
		this._leaderBoardBottomPlayersMessage = configFile.Bind<string>(BoardMessages,
			"Leader Board Heading for Bottom N Players",
			"Bottom %N% Player Leader Boards:",
			"Set the message that is included as a heading when this leader board is sent." + Environment.NewLine +
			"Include %N% to include the number of rankings returned (the configured number)");
		this._leaderBoardHighestPlayerMessage = configFile.Bind<string>(BoardMessages,
			"Leader Board Heading for Highest Player",
			"Top Performer",
			"Set the message that is included as a heading when this leader board is sent." + Environment.NewLine +
			"Include %N% to include the number of rankings returned (the configured number)");
		this._leaderBoardLowestPlayerMessage = configFile.Bind<string>(BoardMessages,
			"Leader Board Heading for Lowest Player",
			"Bottom Performer",
			"Set the message that is included as a heading when this leader board is sent." + Environment.NewLine +
			"Include %N% to include the number of rankings returned (the configured number)");

		configFile.Save();
	}

	// Messages.Server
	public string LaunchMessage => GetRandomStringFromValue(this._serverLaunchMessage);
	public string LoadedMessage => GetRandomStringFromValue(this._serverLoadedMessage);
	public string StopMessage => GetRandomStringFromValue(this._serverStopMessage);
	public string ShutdownMessage => GetRandomStringFromValue(this._serverShutdownMessage);
	public string SaveMessage => GetRandomStringFromValue(this._serverSavedMessage);
	public string NewDayMessage => GetRandomStringFromValue(this._serverNewDayMessage);

	// Messages.Player
	public string JoinMessage => GetRandomStringFromValue(this._playerJoinMessage);
	public string LeaveMessage => GetRandomStringFromValue(this._playerLeaveMessage);
	public string DeathMessage => GetRandomStringFromValue(this._playerDeathMessage);
	public string PingMessage => GetRandomStringFromValue(this._playerPingMessage);
	public string ShoutMessage => GetRandomStringFromValue(this._playerShoutMessage);

	// Messages.PlayerFirsts
	public string PlayerFirstJoinMessage => GetRandomStringFromValue(this._playerFirstJoinMessage);
	public string PlayerFirstLeaveMessage => GetRandomStringFromValue(this._playerFirstLeaveMessage);
	public string PlayerFirstDeathMessage => GetRandomStringFromValue(this._playerFirstDeathMessage);
	public string PlayerFirstPingMessage => GetRandomStringFromValue(this._playerFirstPingMessage);
	public string PlayerFirstShoutMessage => GetRandomStringFromValue(this._playerFirstShoutMessage);

	// Messages.Events
	public string EventStartMessage => GetRandomStringFromValue(this._eventStartMessage);
	public string EventPausedMessage => GetRandomStringFromValue(this._eventPausedMessage);
	public string EventStopMessage => GetRandomStringFromValue(this._eventStopMessage);
	public string EventResumedMessage => GetRandomStringFromValue(this._eventResumedMessage);

	// Messages.LeaderBoards
	public string LeaderBoardTopPlayerHeading => GetRandomStringFromValue(this._leaderBoardTopPlayersMessage);
	public string LeaderBoardBottomPlayersHeading => GetRandomStringFromValue(this._leaderBoardBottomPlayersMessage);
	public string LeaderBoardHighestHeading => GetRandomStringFromValue(this._leaderBoardHighestPlayerMessage);
	public string LeaderBoardLowestHeading => GetRandomStringFromValue(this._leaderBoardLowestPlayerMessage);

	public string ConfigAsJson()
	{
		var jsonString = "{";

		jsonString += $"\"{ServerMessages}\":{{";
		jsonString += $"\"launchMessage\":\"{this._serverLaunchMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"startMessage\":\"{this._serverLoadedMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"stopMessage\":\"{this._serverStopMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"shutdownMessage\":\"{this._serverShutdownMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"savedMessage\":\"{this._serverSavedMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"serverNewDayMessage\":\"{this._serverNewDayMessage.Value.Replace("\"", "\\\"")}\"";
		jsonString += "},";

		jsonString += $"\"{PlayerMessages}\":{{";
		jsonString += $"\"joinMessage\":\"{this._playerJoinMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"deathMessage\":\"{this._playerDeathMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"leaveMessage\":\"{this._playerLeaveMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"pingMessage\":\"{this._playerPingMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"shoutMessage\":\"{this._playerShoutMessage.Value.Replace("\"", "\\\"")}\"";
		jsonString += "},";

		jsonString += $"\"{PlayerFirstsMessages}\":{{";
		jsonString += $"\"joinMessage\":\"{this._playerJoinMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"deathMessage\":\"{this._playerDeathMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"leaveMessage\":\"{this._playerLeaveMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"pingMessage\":\"{this._playerPingMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"shoutMessage\":\"{this._playerShoutMessage.Value.Replace("\"", "\\\"")}\"";
		jsonString += "},";

		jsonString += $"\"{EventMessages}\":{{";
		jsonString += $"\"eventStartMessage\":\"{this._eventStartMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"eventPausedMessage\":\"{this._eventPausedMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"eventResumedMessage\":\"{this._eventResumedMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString += $"\"eventStopMessage\":\"{this._eventStopMessage.Value.Replace("\"", "\\\"")}\"";
		jsonString += "},";

		jsonString += $"\"{BoardMessages}\":{{";
		jsonString +=
			$"\"leaderBoardTopPlayersMessage\":\"{this._leaderBoardTopPlayersMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString +=
			$"\"leaderBoardBottomPlayersMessage\":\"{this._leaderBoardBottomPlayersMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString +=
			$"\"leaderBoardHighestPlayerMessage\":\"{this._leaderBoardHighestPlayerMessage.Value.Replace("\"", "\\\"")}\",";
		jsonString +=
			$"\"leaderBoardLowestPlayerMessage\":\"{this._leaderBoardLowestPlayerMessage.Value.Replace("\"", "\\\"")}\"";
		jsonString += "}";

		jsonString += "}";

		return jsonString;
	}

	private static string GetRandomStringFromValue(ConfigEntry<string> configEntry)
	{
		if (string.IsNullOrEmpty(configEntry.Value))
		{
			return "";
		}

		if (!configEntry.Value.Contains(";"))
		{
			return configEntry.Value;
		}

		var choices = configEntry.Value.Split(';');
		var selection = new Random().Next(choices.Length);
		return choices[selection];
	}
}
