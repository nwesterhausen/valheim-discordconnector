using System;
using BepInEx.Configuration;

namespace DiscordConnector.Config
{
    internal class MessagesConfig
    {
        private static ConfigFile config;

        public static string ConfigExtension = "messages";

        // config header strings
        private const string SERVER_MESSAGES = "Messages.Server";
        private const string PLAYER_MESSAGES = "Messages.Player";
        private const string PLAYER_FIRSTS_MESSAGES = "Messages.PlayerFirsts";
        private const string EVENT_MESSAGES = "Messages.Events";

        // Server Messages
        private ConfigEntry<string> serverLaunchMessage;
        private ConfigEntry<string> serverLoadedMessage;
        private ConfigEntry<string> serverStopMessage;
        private ConfigEntry<string> serverShutdownMessage;
        private ConfigEntry<string> serverSavedMessage;

        // Player Messages
        private ConfigEntry<string> playerJoinMessage;
        private ConfigEntry<string> playerLeaveMessage;
        private ConfigEntry<string> playerDeathMessage;
        private ConfigEntry<string> playerPingMessage;
        private ConfigEntry<string> playerShoutMessage;

        // Player First Messages
        private ConfigEntry<string> playerFirstJoinMessage;
        private ConfigEntry<string> playerFirstLeaveMessage;
        private ConfigEntry<string> playerFirstDeathMessage;
        private ConfigEntry<string> playerFirstPingMessage;
        private ConfigEntry<string> playerFirstShoutMessage;

        // Event Messages
        private ConfigEntry<string> eventStartMessage;
        private ConfigEntry<string> eventPausedMessage;
        private ConfigEntry<string> eventStopMessage;

        public MessagesConfig(ConfigFile configFile)
        {
            config = configFile;
            LoadConfig();
        }

        private void LoadConfig()
        {
            // Messages.Server
            serverLaunchMessage = config.Bind<string>(SERVER_MESSAGES,
                "Server Launch Message",
                "Server is starting up.",
                "Set the message that will be sent when the server starts up." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: Server is starting;Server beginning to load" + Environment.NewLine +
                "If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.");
            serverLoadedMessage = config.Bind<string>(SERVER_MESSAGES,
                "Server Started Message",
                "Server has started!",
                "Set the message that will be sent when the server has loaded the map and is ready for connections." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.");
            serverStopMessage = config.Bind<string>(SERVER_MESSAGES,
                "Server Stop Message",
                "Server is stopping.",
                "Set the message that will be sent when the server shuts down." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.");
            serverShutdownMessage = config.Bind<string>(SERVER_MESSAGES,
                "Server Shutdown Message",
                "Server has stopped!",
                "Set the message that will be sent when the server finishes shutting down." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.");
            serverSavedMessage = config.Bind<string>(SERVER_MESSAGES,
                "Server Saved Message",
                "The world has been saved.",
                "Set the message that will be sent when the server saves the world data." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.");

            // Messages.Player
            playerJoinMessage = config.Bind<string>(PLAYER_MESSAGES,
                "Player Join Message",
                "%PLAYER_NAME% has joined.",
                "Set the message that will be sent when a player joins the server" + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: %PLAYER_NAME% has joined;%PLAYER_NAME% awakens;%PLAYER_NAME% arrives");
            playerDeathMessage = config.Bind<string>(PLAYER_MESSAGES,
                "Player Death Message",
                "%PLAYER_NAME% has died.",
                "Set the message that will be sent when a player dies." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
            playerLeaveMessage = config.Bind<string>(PLAYER_MESSAGES,
                "Player Leave Message",
                "%PLAYER_NAME% has left.",
                "Set the message that will be sent when a player leaves the server." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
            playerPingMessage = config.Bind<string>(PLAYER_MESSAGES,
                "Player Ping Message",
                "%PLAYER_NAME% pings the map.",
                "Set the message that will be sent when a player pings the map." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
            playerShoutMessage = config.Bind<string>(PLAYER_MESSAGES,
                "Player Shout Message",
                "%PLAYER_NAME% shouts **%SHOUT%**.",
                "Set the message that will be sent when a player shouts on the server. You can put %SHOUT% anywhere you want the content of the shout to be." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");

            // Messages.PlayerFirsts
            playerFirstJoinMessage = config.Bind<string>(PLAYER_FIRSTS_MESSAGES,
                "Player First Join Message",
                "Welcome %PLAYER_NAME%, it's their first time on the server!",
                "Set the message that will be sent when a player joins the server" + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
            playerFirstDeathMessage = config.Bind<string>(PLAYER_FIRSTS_MESSAGES,
                "Player First Death Message",
                "%PLAYER_NAME% has died for the first time.",
                "Set the message that will be sent when a player dies." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
            playerFirstLeaveMessage = config.Bind<string>(PLAYER_FIRSTS_MESSAGES,
                "Player First Leave Message",
                "%PLAYER_NAME% has left for the first time.",
                "Set the message that will be sent when a player leaves the server." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
            playerFirstPingMessage = config.Bind<string>(PLAYER_FIRSTS_MESSAGES,
                "Player First Ping Message",
                "%PLAYER_NAME% pings the map for the first time.",
                "Set the message that will be sent when a player pings the map." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");
            playerFirstShoutMessage = config.Bind<string>(PLAYER_FIRSTS_MESSAGES,
                "Player First Shout Message",
                "%PLAYER_NAME% shouts for the first time.",
                "Set the message that will be sent when a player shouts on the server. %SHOUT% works in this message to include what was shouted." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");

            // Messages.Events
            eventStartMessage = config.Bind<string>(EVENT_MESSAGES,
                "Event Start Message",
                "**Event**: %EVENT_MSG% around %PLAYERS%",
                "Set the message that will be sent when a random event starts on the server. Sending the coordinates is enabled by default in the toggles config." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "The special string %EVENT_MSG% will be replaced with the message that is displayed on the screen when the event starts." + Environment.NewLine +
                "The special string %PLAYERS% will be replaced with a list of players in the event area.");
            eventStopMessage = config.Bind<string>(EVENT_MESSAGES,
                "Event Stop Message",
                "**Event**: %EVENT_MSG%",
                "Set the message that will be sent when a random event stops on the server. Sending the coordinates is enabled by default in the toggles config." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "The special string %EVENT_MSG% will be replaced with the message that is displayed on the screen when the event stops.");
            eventPausedMessage = config.Bind<string>(EVENT_MESSAGES,
                "Event Paused Message",
                "**Event**: %EVENT_END_MSG% -- for now! (Currently paused due to no players in the event area.)",
                "Set the message that will be sent when a random event is paused due to players leaving the area. Sending the coordinates is enabled by default in the toggles config." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "The special string %EVENT_START_MSG% will be replaced with the message that is displayed on the screen when the event starts." + Environment.NewLine +
                "The special string %EVENT_END_MSG% will be replaced with the message that is displayed on the screen when the event ends.");

            config.Save();
        }

        public string ConfigAsJson()
        {
            string jsonString = "{";

            jsonString += $"\"{SERVER_MESSAGES}\":{{";
            jsonString += $"\"launchMessage\":\"{serverLaunchMessage.Value}\",";
            jsonString += $"\"startMessage\":\"{serverLoadedMessage.Value}\",";
            jsonString += $"\"stopMessage\":\"{serverStopMessage.Value}\",";
            jsonString += $"\"shutdownMessage\":\"{serverShutdownMessage.Value}\",";
            jsonString += $"\"savedMessage\":\"{serverSavedMessage.Value}\"";
            jsonString += "},";

            jsonString += $"\"{PLAYER_MESSAGES}\":{{";
            jsonString += $"\"joinMessage\":\"{playerJoinMessage.Value}\",";
            jsonString += $"\"deathMessage\":\"{playerDeathMessage.Value}\",";
            jsonString += $"\"leaveMessage\":\"{playerLeaveMessage.Value}\",";
            jsonString += $"\"pingMessage\":\"{playerPingMessage.Value}\",";
            jsonString += $"\"shoutMessage\":\"{playerShoutMessage.Value}\"";
            jsonString += "},";

            jsonString += $"\"{PLAYER_FIRSTS_MESSAGES}\":{{";
            jsonString += $"\"joinMessage\":\"{playerJoinMessage.Value}\",";
            jsonString += $"\"deathMessage\":\"{playerDeathMessage.Value}\",";
            jsonString += $"\"leaveMessage\":\"{playerLeaveMessage.Value}\",";
            jsonString += $"\"pingMessage\":\"{playerPingMessage.Value}\",";
            jsonString += $"\"shoutMessage\":\"{playerShoutMessage.Value}\"";
            jsonString += "},";

            jsonString += $"\"{EVENT_MESSAGES}\":{{";
            jsonString += $"\"eventStartMessage\":\"{eventStartMessage.Value}\",";
            jsonString += $"\"eventPausedMessage\":\"{eventPausedMessage.Value}\",";
            jsonString += $"\"eventStopMessage\":\"{eventStopMessage.Value}\"";
            jsonString += "}";

            jsonString += "}";

            return jsonString;
        }

        private static string GetRandomStringFromValue(ConfigEntry<string> configEntry)
        {
            if (!configEntry.Value.Contains(";"))
            {
                return configEntry.Value;
            }
            string[] choices = configEntry.Value.Split(';');
            int selection = (new Random()).Next(choices.Length);
            return choices[selection];
        }

        // Messages.Server
        public string LaunchMessage => GetRandomStringFromValue(serverLaunchMessage);
        public string LoadedMessage => GetRandomStringFromValue(serverLoadedMessage);
        public string StopMessage => GetRandomStringFromValue(serverStopMessage);
        public string ShutdownMessage => GetRandomStringFromValue(serverShutdownMessage);
        public string SaveMessage => GetRandomStringFromValue(serverSavedMessage);

        // Messages.Player
        public string JoinMessage => GetRandomStringFromValue(playerJoinMessage);
        public string LeaveMessage => GetRandomStringFromValue(playerLeaveMessage);
        public string DeathMessage => GetRandomStringFromValue(playerDeathMessage);
        public string PingMessage => GetRandomStringFromValue(playerPingMessage);
        public string ShoutMessage => GetRandomStringFromValue(playerShoutMessage);

        // Messages.PlayerFirsts
        public string PlayerFirstJoinMessage => GetRandomStringFromValue(playerFirstJoinMessage);
        public string PlayerFirstLeaveMessage => GetRandomStringFromValue(playerFirstLeaveMessage);
        public string PlayerFirstDeathMessage => GetRandomStringFromValue(playerFirstDeathMessage);
        public string PlayerFirstPingMessage => GetRandomStringFromValue(playerFirstPingMessage);
        public string PlayerFirstShoutMessage => GetRandomStringFromValue(playerFirstShoutMessage);

        // Messages.Events
        public string EventStartMesssage => GetRandomStringFromValue(eventStartMessage);
        public string EventPausedMesssage => GetRandomStringFromValue(eventPausedMessage);
        public string EventStopMesssage => GetRandomStringFromValue(eventStopMessage);

    }
}
