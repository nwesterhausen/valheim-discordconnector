using System;
using BepInEx.Configuration;

namespace DiscordConnectorLite.Config
{
    internal class MainConfig
    {
        private static ConfigFile config;
        private const string MAIN_SETTINGS = "Main Settings";

        // Main Settings
        private ConfigEntry<string> webhookUrl;
        private ConfigEntry<bool> discordEmbedMessagesToggle;
        private ConfigEntry<bool> discordBotToggle;

        public MainConfig(ConfigFile configFile)
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
            webhookUrl = config.Bind<string>(MAIN_SETTINGS,
                "Webhook URL",
                "",
                "Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of " + Environment.NewLine +
                "Discord's documentation: https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook");

            discordEmbedMessagesToggle = config.Bind<bool>(MAIN_SETTINGS,
                "Use fancier discord messages",
                false,
                "Enable this setting to use embeds in the messages sent to Discord. Currently this will affect the position details for the messages.");

            discordBotToggle = config.Bind<bool>(MAIN_SETTINGS,
                "Enable Discord Bot Integration",
                false,
                "Enable this setting to allow Discord Bot integration with this plugin. See the -bot.cfg file for all the config options related to this integration." + Environment.NewLine +
                "When this is turned on, a listening HTTP webhook opens on a port specified in the config. This enables the Discord bot to communicate with this plugin (and the server).");

            config.Save();
        }

        public string ConfigAsJson()
        {
            string jsonString = "{";
            jsonString += "\"discord\":{";
            jsonString += $"\"webhook\":\"{(string.IsNullOrEmpty(WebHookURL) ? "unset" : "REDACTED")}\",";
            jsonString += $"\"fancierMessages\":\"{DiscordEmbedsEnabled}\",";
            jsonString += $"\"botWebhookEnabled\":\"{DiscordBotEnabled}\"";
            jsonString += "}";
            jsonString += "}";
            return jsonString;
        }

        public string WebHookURL => webhookUrl.Value;
        public bool DiscordEmbedsEnabled => discordEmbedMessagesToggle.Value;
        public bool DiscordBotEnabled => discordBotToggle.Value;

    }
}
