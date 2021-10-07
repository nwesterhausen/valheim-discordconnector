using System;
using BepInEx.Configuration;

namespace DiscordConnector
{
    internal class MessagesConfig
    {
        private static ConfigFile config;

        public static string ConfigExention = "messages";

        // config header strings
        private const string MESSAGES_SETTINGS = "Message Settings";

        // Logged Information Messages
        private ConfigEntry<string> serverLaunchMessage;
        private ConfigEntry<string> serverLoadedMessage;
        private ConfigEntry<string> serverStopMessage;
        private ConfigEntry<string> playerJoinMessage;
        private ConfigEntry<string> playerLeaveMessage;
        private ConfigEntry<string> playerDeathMessage;
        private ConfigEntry<string> playerPingMessage;
        private ConfigEntry<string> playerShoutMessage;

        public MessagesConfig(ConfigFile configFile)
        {
            config = configFile;
            LoadConfig();
        }

        private void LoadConfig()
        {
            // Message Settings

            serverLaunchMessage = config.Bind<string>(MESSAGES_SETTINGS,
                "Server Launch Message",
                "Server is starting up.",
                "Set the message that will be sent when the server starts up." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: 'Server is starting;Server beginning to load'");

            serverLoadedMessage = config.Bind<string>(MESSAGES_SETTINGS,
                "Server Started Message",
                "Server has started!",
                "Set the message that will be sent when the server has loaded the map and is ready for connections." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: 'Server has started;Server ready!'");

            serverStopMessage = config.Bind<string>(MESSAGES_SETTINGS,
                "Server Stop Message",
                "Server is stopping.",
                "Set the message that will be sent when the server shuts down." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: 'Server stopping;Valheim signing off!'");

            playerJoinMessage = config.Bind<string>(MESSAGES_SETTINGS,
                "Player Join Message",
                "%PLAYER_NAME% has joined.",
                "Set the message that will be sent when a player joins the server" + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: '%PLAYER_NAME% has joined;%PLAYER_NAME% awakens;%PLAYER_NAME% arrives'");

            playerDeathMessage = config.Bind<string>(MESSAGES_SETTINGS,
                "Player Death Message",
                "%PLAYER_NAME% has died.",
                "Set the message that will be sent when a player dies." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: '%PLAYER_NAME% has died;%PLAYER_NAME% passed on'");

            playerLeaveMessage = config.Bind<string>(MESSAGES_SETTINGS,
                "Player Leave Message",
                "%PLAYER_NAME% has left.",
                "Set the message that will be sent when a player leaves the server." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'" + Environment.NewLine +
                "Random choice example: '%PLAYER_NAME% has left;%PLAYER_NAME% has moved on;%PLAYER_NAME% returns to dreams'");

            playerPingMessage = config.Bind<string>(MESSAGES_SETTINGS,
                "Player Ping Message",
                "%PLAYER_NAME% pings the map.",
                "Set the message that will be sent when a player pings the map." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");

            playerShoutMessage = config.Bind<string>(MESSAGES_SETTINGS,
                "Player Shout Message",
                "%PLAYER_NAME% shouts **%SHOUT%**.",
                "Set the message that will be sent when a player shouts on the server." + Environment.NewLine +
                "If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'");

            config.Save();
        }

        public string ConfigAsJson()
        {
            string jsonString = "{";
            jsonString += $"\"launchMessage\":\"{serverLaunchMessage.Value}\",";
            jsonString += $"\"startMessage\":\"{serverLoadedMessage.Value}\",";
            jsonString += $"\"stopMessage\":\"{serverStopMessage.Value}\",";
            jsonString += $"\"joinMessage\":\"{playerJoinMessage.Value}\",";
            jsonString += $"\"deathMessage\":\"{playerDeathMessage.Value}\",";
            jsonString += $"\"leaveMessage\":\"{playerLeaveMessage.Value}\",";
            jsonString += $"\"pingMessage\":\"{playerPingMessage.Value}\",";
            jsonString += $"\"shoutMessage\":\"{playerShoutMessage.Value}\"";
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

    }
}