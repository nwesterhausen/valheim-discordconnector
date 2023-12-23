using System;
using BepInEx.Configuration;

namespace DiscordConnectorLite.Config
{
    internal class TogglesConfig
    {
        private static ConfigFile config;
        public static string ConfigExtension = "toggles";

        // Config Header Strings
        private const string MESSAGES_TOGGLES = "Toggles.Messages";
        private const string DEBUG_TOGGLES = "Toggles.DebugMessages";

        // Logged Information Toggles
        private ConfigEntry<bool> serverLaunchToggle;
        private ConfigEntry<bool> serverLoadedToggle;
        private ConfigEntry<bool> serverStopToggle;
        private ConfigEntry<bool> serverShutdownToggle;
        private ConfigEntry<bool> serverSaveToggle;

        private ConfigEntry<bool> debugHttpRequestResponses;

        public TogglesConfig(ConfigFile configFile)
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

            debugHttpRequestResponses = config.Bind(DEBUG_TOGGLES,
                                                          "Debug Message for HTTP Request Responses",
                                                          false,
                                                          "If enabled, this will write a log message at the DEBUG level with the content of HTTP request responses." + Environment.NewLine +
                                                          "Nearly all of these requests are when data is sent to the Discord Webhook.");

            config.Save();
        }

        public string ConfigAsJson()
        {
            string jsonString = "{";
            jsonString += $"\"{MESSAGES_TOGGLES}\":{{";
            jsonString += $"\"launchMessageEnabled\":\"{LaunchMessageEnabled}\",";
            jsonString += $"\"loadedMessageEnabled\":\"{LoadedMessageEnabled}\",";
            jsonString += $"\"stopMessageEnabled\":\"{StopMessageEnabled}\",";
            jsonString += $"\"shutdownMessageEnabled\":\"{ShutdownMessageEnabled}\"";
            jsonString += "},";

            jsonString += $"\"{DEBUG_TOGGLES}\":{{";
            jsonString += $"\"debugHttpRequestResponses\":\"{DebugHttpRequestResponse}\"";
            jsonString += "}";

            jsonString += "}";
            return jsonString;
        }

        public bool LaunchMessageEnabled => serverLaunchToggle.Value;
        public bool LoadedMessageEnabled => serverLoadedToggle.Value;
        public bool StopMessageEnabled => serverStopToggle.Value;
        public bool ShutdownMessageEnabled => serverShutdownToggle.Value;
        public bool WorldSaveMessageEnabled => serverSaveToggle.Value;
        private bool DebugHttpRequestResponse => debugHttpRequestResponses.Value;
    }
}
