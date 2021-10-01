using System;
using BepInEx.Configuration;

namespace DiscordConnector
{
    class PluginConfig
    {
        public static ConfigFile config;

        // config header strings
        private const string DISCORD_SETTINGS = "Discord Settings";
        private const string NOTIFICATION_SETTINGS = "Notification Settings";
        private const string NOTIFICATION_CONTENT_SETTINGS = "Notification Content Settings";

        // Webhook Url
        private ConfigEntry<string> webhookUrl;

        // Logged Information Toggles
        private ConfigEntry<bool> serverLaunchToggle;
        private ConfigEntry<bool> serverLoadedToggle;
        private ConfigEntry<bool> serverStopToggle;

        // Logged Information Messages
        private ConfigEntry<string> serverLaunchMessage;
        private ConfigEntry<string> ServerLoadedMessage;
        private ConfigEntry<string> ServerStopMessage;

        public PluginConfig(ConfigFile config)
        {
            PluginConfig.config = config;
            LoadConfig();
        }

        public void LoadConfig()
        {
            webhookUrl = config.Bind<string>(DISCORD_SETTINGS,
                "Webhook URL",
                "",
                "Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of " + Environment.NewLine +
                "Discord's documentation: https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook");

            // Message Toggles

            serverLaunchToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Server Launch Notifications",
                true,
                "If enabled, this will send a message to Discord when the server launches (and the plugin is loaded)." + Environment.NewLine +
                "EX: Server has started. | Server has stopped.");

            serverLoadedToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Server Loaded Notifications",
                true,
                "If enabled, this will send a message to Discord when the server has loaded the map and is ready for connections." + Environment.NewLine +
                "EX: Server has started. | Server has stopped.");

            serverStopToggle = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Server Status Notifications",
                true,
                "If enabled, this will send a message to Discord when the server shuts down." + Environment.NewLine +
                "EX: Server has started. | Server has stopped.");

            // Message Settings

            serverLaunchMessage = config.Bind<string>(NOTIFICATION_CONTENT_SETTINGS,
                "Server Launch Message",
                "Server is starting up.",
                "Set the message that will be sent when the server starts up.");

            ServerLoadedMessage = config.Bind<string>(NOTIFICATION_CONTENT_SETTINGS,
                "Server Started Message",
                "Server has started!",
                "Set the message that will be sent when the server has loaded the map and is ready for connections.");

            ServerStopMessage = config.Bind<string>(NOTIFICATION_CONTENT_SETTINGS,
                "Server Stop Message",
                "Server is stopping.",
                "Set the message that will be sent when the server shuts down.");


            config.Save();
        }

        // Exposed Config Values

        public string WebHookURL { get => webhookUrl.Value; }
        public string LaunchMessage { get => serverLaunchMessage.Value; }
        public string LoadedMessage { get => ServerLoadedMessage.Value; }
        public string StopMessage { get => ServerStopMessage.Value; }
        public bool LaunchMessageEnabled { get => serverLaunchToggle.Value; }
        public bool LoadedMessageEnabled { get => serverLaunchToggle.Value; }
        public bool StopMessageEnabled { get => serverLaunchToggle.Value; }
    }
}