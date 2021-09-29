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

        // Webhook Url
        private ConfigEntry<string> webhookurl;

        // Logged Information Toggles
        private ConfigEntry<bool> serverstatus;

        public PluginConfig(ConfigFile config)
        {
            PluginConfig.config = config;
            LoadConfig();
        }

        public void LoadConfig()
        {
            webhookurl = config.Bind<string>(DISCORD_SETTINGS,
                "Webhook URL",
                "https://discordapp/hook/HASH",
                "Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of " + Environment.NewLine +
                "Discord's documentation: https://support.discord.com/hc/en-us/articles/228383668-Intro-to-Webhook");

            serverstatus = config.Bind<bool>(NOTIFICATION_SETTINGS,
                "Server Status Notifications",
                true,
                "If enabled, this will send a message to discord when the server starts or stops.");


            config.Save();
        }
    }
}