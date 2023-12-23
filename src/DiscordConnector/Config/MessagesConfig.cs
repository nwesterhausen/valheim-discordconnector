using System;
using BepInEx.Configuration;

namespace DiscordConnectorLite.Config
{
    internal class MessagesConfig
    {
        private static ConfigFile config;

        public static string ConfigExtension = "messages";

        // config header strings
        private const string SERVER_MESSAGES = "Messages.Server";
        
        // Server Messages
        private ConfigEntry<string> serverLaunchMessage;
        private ConfigEntry<string> serverLoadedMessage;
        private ConfigEntry<string> serverStopMessage;
        private ConfigEntry<string> serverShutdownMessage;
        private ConfigEntry<string> serverSavedMessage;

        public MessagesConfig(ConfigFile configFile)
        {
            config = configFile;
            LoadConfig();
        }

        public void ReloadConfig()
        {
            config.Reload();
            config.Save();
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
        
    }
}
