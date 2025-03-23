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
        _serverLaunchMessage = configFile.Bind<string>(ServerMessages,
            "Server Launch Message",
            "Server is starting up.",
            "Set the message that will be sent when the server starts up." + Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
            Environment.NewLine +
            "Random choice example: Server is starting;Server beginning to load" + Environment.NewLine +
            "If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.");
        _serverLoadedMessage = configFile.Bind<string>(ServerMessages,
            "Server Started Message",
            "Server has started!",
            "Set the message that will be sent when the server has loaded the map and is ready for connections." +
            Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
            Environment.NewLine +
            "If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.");
        _serverStopMessage = configFile.Bind<string>(ServerMessages,
            "Server Stop Message",
            "Server is stopping.",
            "Set the message that will be sent when the server shuts down." + Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
            Environment.NewLine +
            "If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.");
        _serverShutdownMessage = configFile.Bind<string>(ServerMessages,
            "Server Shutdown Message",
            "Server has stopped!",
            "Set the message that will be sent when the server finishes shutting down." + Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
            Environment.NewLine +
            "If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.");
        _serverSavedMessage = configFile.Bind<string>(ServerMessages,
            "Server Saved Message",
            "The world has been saved.",
            "Set the message that will be sent when the server saves the world data." + Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
            Environment.NewLine +
            "If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.");
        _serverNewDayMessage = configFile.Bind<string>(ServerMessages,
            "Server New Day Message",
            "Day Number %DAY_NUMBER%",
            "Set the message that will be sent when a new day starts." + Environment.NewLine +
            "The %DAY_NUMBER% variable gets replaced with the day number.");

        // Messages.Player
        _playerJoinMessage = configFile.Bind<string>(PlayerMessages,
            "Player Join Message",
            "%PLAYER_NAME% has joined.",
            "Set the message that will be sent when a player joins the server" + Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
            Environment.NewLine +
            "Random choice example: %PLAYER_NAME% has joined;%PLAYER_NAME% awakens;%PLAYER_NAME% arrives");
        _playerDeathMessage = configFile.Bind<string>(PlayerMessages,
            "Player Death Message",
            "%PLAYER_NAME% has died.",
            "Set the message that will be sent when a player dies." + Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
        _playerLeaveMessage = configFile.Bind<string>(PlayerMessages,
            "Player Leave Message",
            "%PLAYER_NAME% has left.",
            "Set the message that will be sent when a player leaves the server." + Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
        _playerPingMessage = configFile.Bind<string>(PlayerMessages,
            "Player Ping Message",
            "%PLAYER_NAME% pings the map.",
            "Set the message that will be sent when a player pings the map." + Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
        _playerShoutMessage = configFile.Bind<string>(PlayerMessages,
            "Player Shout Message",
            "%PLAYER_NAME% shouts **%SHOUT%**.",
            "Set the message that will be sent when a player shouts on the server. You can put %SHOUT% anywhere you want the content of the shout to be." +
            Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");

        // Messages.PlayerFirsts
        _playerFirstJoinMessage = configFile.Bind<string>(PlayerFirstsMessages,
            "Player First Join Message",
            "Welcome %PLAYER_NAME%, it's their first time on the server!",
            "Set the message that will be sent when a player joins the server" + Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
        _playerFirstDeathMessage = configFile.Bind<string>(PlayerFirstsMessages,
            "Player First Death Message",
            "%PLAYER_NAME% has died for the first time.",
            "Set the message that will be sent when a player dies." + Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
        _playerFirstLeaveMessage = configFile.Bind<string>(PlayerFirstsMessages,
            "Player First Leave Message",
            "%PLAYER_NAME% has left for the first time.",
            "Set the message that will be sent when a player leaves the server." + Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
        _playerFirstPingMessage = configFile.Bind<string>(PlayerFirstsMessages,
            "Player First Ping Message",
            "%PLAYER_NAME% pings the map for the first time.",
            "Set the message that will be sent when a player pings the map." + Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
        _playerFirstShoutMessage = configFile.Bind<string>(PlayerFirstsMessages,
            "Player First Shout Message",
            "%PLAYER_NAME% shouts for the first time.",
            "Set the message that will be sent when a player shouts on the server. %SHOUT% works in this message to include what was shouted." +
            Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");

        // Messages.Events
        _eventStartMessage = configFile.Bind<string>(EventMessages,
            "Event Start Message",
            "**Event**: %EVENT_MSG%",
            "Set the message that will be sent when a random event starts on the server. Sending the coordinates is enabled by default in the toggles config." +
            Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
            Environment.NewLine +
            "The special string %EVENT_MSG% will be replaced with the message that is displayed on the screen when the event starts."); // + Environment.NewLine +
        // "The special string %PLAYERS% will be replaced with a list of players in the event area."); //! Removed due to unreliability
        _eventStopMessage = configFile.Bind<string>(EventMessages,
            "Event Stop Message",
            "**Event**: %EVENT_MSG%",
            "Set the message that will be sent when a random event stops on the server. Sending the coordinates is enabled by default in the toggles config." +
            Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
            Environment.NewLine +
            "The special string %EVENT_MSG% will be replaced with the message that is displayed on the screen when the event stops.");
        _eventPausedMessage = configFile.Bind<string>(EventMessages,
            "Event Paused Message",
            "**Event**: %EVENT_END_MSG% — for now! (Currently paused due to no players in the event area.)",
            "Set the message that will be sent when a random event is paused due to players leaving the area. Sending the coordinates is enabled by default in the toggles config." +
            Environment.NewLine +
            "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" +
            Environment.NewLine +
            "The special string %EVENT_START_MSG% will be replaced with the message that is displayed on the screen when the event starts." +
            Environment.NewLine +
            "The special string %EVENT_END_MSG% will be replaced with the message that is displayed on the screen when the event ends.");
        _eventResumedMessage = configFile.Bind<string>(EventMessages,
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
        _leaderBoardTopPlayersMessage = configFile.Bind<string>(BoardMessages,
            "Leader Board Heading for Top N Players",
            "Top %N% Player Leader Boards:",
            "Set the message that is included as a heading when this leader board is sent." + Environment.NewLine +
            "Include %N% to include the number of rankings returned (the configured number)");
        _leaderBoardBottomPlayersMessage = configFile.Bind<string>(BoardMessages,
            "Leader Board Heading for Bottom N Players",
            "Bottom %N% Player Leader Boards:",
            "Set the message that is included as a heading when this leader board is sent." + Environment.NewLine +
            "Include %N% to include the number of rankings returned (the configured number)");
        _leaderBoardHighestPlayerMessage = configFile.Bind<string>(BoardMessages,
            "Leader Board Heading for Highest Player",
            "Top Performer",
            "Set the message that is included as a heading when this leader board is sent." + Environment.NewLine +
            "Include %N% to include the number of rankings returned (the configured number)");
        _leaderBoardLowestPlayerMessage = configFile.Bind<string>(BoardMessages,
            "Leader Board Heading for Lowest Player",
            "Bottom Performer",
            "Set the message that is included as a heading when this leader board is sent." + Environment.NewLine +
            "Include %N% to include the number of rankings returned (the configured number)");

        configFile.Save();
    }

    // Messages.Server
    public string LaunchMessage => GetRandomStringFromValue(_serverLaunchMessage);
    public string LoadedMessage => GetRandomStringFromValue(_serverLoadedMessage);
    public string StopMessage => GetRandomStringFromValue(_serverStopMessage);
    public string ShutdownMessage => GetRandomStringFromValue(_serverShutdownMessage);
    public string SaveMessage => GetRandomStringFromValue(_serverSavedMessage);
    public string NewDayMessage => GetRandomStringFromValue(_serverNewDayMessage);

    // Messages.Player
    public string JoinMessage => GetRandomStringFromValue(_playerJoinMessage);
    public string LeaveMessage => GetRandomStringFromValue(_playerLeaveMessage);
    public string DeathMessage => GetRandomStringFromValue(_playerDeathMessage);
    public string PingMessage => GetRandomStringFromValue(_playerPingMessage);
    public string ShoutMessage => GetRandomStringFromValue(_playerShoutMessage);

    // Messages.PlayerFirsts
    public string PlayerFirstJoinMessage => GetRandomStringFromValue(_playerFirstJoinMessage);
    public string PlayerFirstLeaveMessage => GetRandomStringFromValue(_playerFirstLeaveMessage);
    public string PlayerFirstDeathMessage => GetRandomStringFromValue(_playerFirstDeathMessage);
    public string PlayerFirstPingMessage => GetRandomStringFromValue(_playerFirstPingMessage);
    public string PlayerFirstShoutMessage => GetRandomStringFromValue(_playerFirstShoutMessage);

    // Messages.Events
    public string EventStartMessage => GetRandomStringFromValue(_eventStartMessage);
    public string EventPausedMessage => GetRandomStringFromValue(_eventPausedMessage);
    public string EventStopMessage => GetRandomStringFromValue(_eventStopMessage);
    public string EventResumedMessage => GetRandomStringFromValue(_eventResumedMessage);

    // Messages.LeaderBoards
    public string LeaderBoardTopPlayerHeading => GetRandomStringFromValue(_leaderBoardTopPlayersMessage);
    public string LeaderBoardBottomPlayersHeading => GetRandomStringFromValue(_leaderBoardBottomPlayersMessage);
    public string LeaderBoardHighestHeading => GetRandomStringFromValue(_leaderBoardHighestPlayerMessage);
    public string LeaderBoardLowestHeading => GetRandomStringFromValue(_leaderBoardLowestPlayerMessage);

    public string ConfigAsJson()
    {
        string jsonString = "{";

        jsonString += $"\"{ServerMessages}\":{{";
        jsonString += $"\"launchMessage\":\"{_serverLaunchMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"startMessage\":\"{_serverLoadedMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"stopMessage\":\"{_serverStopMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"shutdownMessage\":\"{_serverShutdownMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"savedMessage\":\"{_serverSavedMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"serverNewDayMessage\":\"{_serverNewDayMessage.Value.Replace("\"", "\\\"")}\"";
        jsonString += "},";

        jsonString += $"\"{PlayerMessages}\":{{";
        jsonString += $"\"joinMessage\":\"{_playerJoinMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"deathMessage\":\"{_playerDeathMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"leaveMessage\":\"{_playerLeaveMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"pingMessage\":\"{_playerPingMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"shoutMessage\":\"{_playerShoutMessage.Value.Replace("\"", "\\\"")}\"";
        jsonString += "},";

        jsonString += $"\"{PlayerFirstsMessages}\":{{";
        jsonString += $"\"joinMessage\":\"{_playerJoinMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"deathMessage\":\"{_playerDeathMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"leaveMessage\":\"{_playerLeaveMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"pingMessage\":\"{_playerPingMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"shoutMessage\":\"{_playerShoutMessage.Value.Replace("\"", "\\\"")}\"";
        jsonString += "},";

        jsonString += $"\"{EventMessages}\":{{";
        jsonString += $"\"eventStartMessage\":\"{_eventStartMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"eventPausedMessage\":\"{_eventPausedMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"eventResumedMessage\":\"{_eventResumedMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString += $"\"eventStopMessage\":\"{_eventStopMessage.Value.Replace("\"", "\\\"")}\"";
        jsonString += "},";

        jsonString += $"\"{BoardMessages}\":{{";
        jsonString +=
            $"\"leaderBoardTopPlayersMessage\":\"{_leaderBoardTopPlayersMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString +=
            $"\"leaderBoardBottomPlayersMessage\":\"{_leaderBoardBottomPlayersMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString +=
            $"\"leaderBoardHighestPlayerMessage\":\"{_leaderBoardHighestPlayerMessage.Value.Replace("\"", "\\\"")}\",";
        jsonString +=
            $"\"leaderBoardLowestPlayerMessage\":\"{_leaderBoardLowestPlayerMessage.Value.Replace("\"", "\\\"")}\"";
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

        string[] choices = configEntry.Value.Split(';');
        int selection = new Random().Next(choices.Length);
        return choices[selection];
    }
}
