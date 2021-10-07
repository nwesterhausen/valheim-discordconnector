using System;
using BepInEx.Configuration;

namespace DiscordConnector
{
    internal class MessagesConfig
    {
        private static ConfigFile config;

        public static string ConfigExention = "messages";

        // config header strings
        private const string SERVER_MESSAGES = "Messages.Server";
        private const string PLAYER_MESSAGES = "Messages.Player";
        private const string PLAYER_FIRSTS_MESSAGES = "Messages.PlayerFirsts";

        // Server Messages
        private ConfigEntry<string> serverLaunchMessage;
        private ConfigEntry<string> serverLoadedMessage;
        private ConfigEntry<string> serverStopMessage;
        private ConfigEntry<string> serverShutdownMessage;

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

        public MessagesConfig(ConfigFile configFile)
        {
            config = configFile;
            LoadConfig();
        }

        private void LoadConfig()
        {
            // Message Settings

            serverLaunchMessage = config.Bind<string>(SERVER_MESSAGES,
                "Server Launch Message",
                "Server is starting up.",
                "Set the message that will be sent when the server starts up." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: 'Server is starting;Server beginning to load'");

            serverLoadedMessage = config.Bind<string>(SERVER_MESSAGES,
                "Server Started Message",
                "Server has started!",
                "Set the message that will be sent when the server has loaded the map and is ready for connections." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: 'Server has started;Server ready!'");

            serverStopMessage = config.Bind<string>(SERVER_MESSAGES,
                "Server Stop Message",
                "Server is stopping.",
                "Set the message that will be sent when the server shuts down." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: 'Server stopping;Valheim signing off!'");

            serverShutdownMessage = config.Bind<string>(SERVER_MESSAGES,
                "Server Shutdown Message",
                "Server has stoped!",
                "Set the message that will be sent when the server finishes shutting down." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");

            playerJoinMessage = config.Bind<string>(PLAYER_MESSAGES,
                "Player Join Message",
                "%PLAYER_NAME% has joined.",
                "Set the message that will be sent when a player joins the server" + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: '%PLAYER_NAME% has joined;%PLAYER_NAME% awakens;%PLAYER_NAME% arrives'");

            playerDeathMessage = config.Bind<string>(PLAYER_MESSAGES,
                "Player Death Message",
                "%PLAYER_NAME% has died.",
                "Set the message that will be sent when a player dies." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: '%PLAYER_NAME% has died;%PLAYER_NAME% passed on'");

            playerLeaveMessage = config.Bind<string>(PLAYER_MESSAGES,
                "Player Leave Message",
                "%PLAYER_NAME% has left.",
                "Set the message that will be sent when a player leaves the server." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: '%PLAYER_NAME% has left;%PLAYER_NAME% has moved on;%PLAYER_NAME% returns to dreams'");

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
                "Set the message that will be sent when a player shouts on the server." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");

            config.Save();
        }

        public string ConfigAsJson()
        {
            string jsonString = "{";

            jsonString += $"\"{SERVER_MESSAGES}\":{{";
            jsonString += $"\"launchMessage\":\"{serverLaunchMessage.Value}\",";
            jsonString += $"\"startMessage\":\"{serverLoadedMessage.Value}\",";
            jsonString += $"\"stopMessage\":\"{serverStopMessage.Value}\",";
            jsonString += $"\"shutdownMessage\":\"{serverShutdownMessage.Value}\"";
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
            jsonString += "}";

            jsonString += "}";

            return jsonString;
        }

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
        public string ShutdownMessage
        {
            get
            {
                if (!serverShutdownMessage.Value.Contains(";"))
                {
                    return serverShutdownMessage.Value;
                }
                string[] choices = serverShutdownMessage.Value.Split(';');
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
        public string DeathMessage
        {
            get
            {
                if (!playerDeathMessage.Value.Contains(";"))
                {
                    return playerDeathMessage.Value;
                }
                string[] choices = playerDeathMessage.Value.Split(';');
                int selection = (new Random()).Next(choices.Length);
                return choices[selection];
            }
        }
        public string PingMessage
        {
            get
            {
                if (!playerPingMessage.Value.Contains(";"))
                {
                    return playerPingMessage.Value;
                }
                string[] choices = playerPingMessage.Value.Split(';');
                int selection = (new Random()).Next(choices.Length);
                return choices[selection];
            }
        }
        public string ShoutMessage
        {
            get
            {
                if (!playerShoutMessage.Value.Contains(";"))
                {
                    return playerShoutMessage.Value;
                }
                string[] choices = playerShoutMessage.Value.Split(';');
                int selection = (new Random()).Next(choices.Length);
                return choices[selection];
            }
        }
        public string PlayerFirstJoinMessage
        {
            get
            {
                if (!playerFirstJoinMessage.Value.Contains(";"))
                {
                    return playerFirstJoinMessage.Value;
                }
                string[] choices = playerFirstJoinMessage.Value.Split(';');
                int selection = (new Random()).Next(choices.Length);
                return choices[selection];
            }
        }
        public string PlayerFirstLeaveMessage
        {
            get
            {
                if (!playerFirstLeaveMessage.Value.Contains(";"))
                {
                    return playerFirstLeaveMessage.Value;
                }
                string[] choices = playerFirstLeaveMessage.Value.Split(';');
                int selection = (new Random()).Next(choices.Length);
                return choices[selection];
            }
        }
        public string PlayerFirstDeathMessage
        {
            get
            {
                if (!playerFirstDeathMessage.Value.Contains(";"))
                {
                    return playerFirstDeathMessage.Value;
                }
                string[] choices = playerFirstDeathMessage.Value.Split(';');
                int selection = (new Random()).Next(choices.Length);
                return choices[selection];
            }
        }
        public string PlayerFirstPingMessage
        {
            get
            {
                if (!playerFirstPingMessage.Value.Contains(";"))
                {
                    return playerFirstPingMessage.Value;
                }
                string[] choices = playerFirstPingMessage.Value.Split(';');
                int selection = (new Random()).Next(choices.Length);
                return choices[selection];
            }
        }
        public string PlayerFirstShoutMessage
        {
            get
            {
                if (!playerFirstShoutMessage.Value.Contains(";"))
                {
                    return playerFirstShoutMessage.Value;
                }
                string[] choices = playerFirstShoutMessage.Value.Split(';');
                int selection = (new Random()).Next(choices.Length);
                return choices[selection];
            }
        }

    }
}