using System;
using BepInEx.Configuration;

namespace DiscordConnector.Config
{
    internal class BotConfig
    {
        private ConfigFile config;
        private const string BOT_SETTINGS = "Discord Bot Integration";
        public static string ConfigExtension = "bot";
        private ConfigEntry<string> discordBotAuthorization;
        private ConfigEntry<int> discordBotPort;

        private static string _generated_authHeader;

        public BotConfig(ConfigFile configFile)
        {
            config = configFile;
            _generated_authHeader = Utility.RandomAlphanumericString(32);

            LoadConfig();
        }
        private void LoadConfig()
        {
            discordBotAuthorization = config.Bind<string>(BOT_SETTINGS,
                "Authorization Header",
                _generated_authHeader,
                "This is the required authorization header needed to allow the Discord bot to execute commands with this server." + Environment.NewLine +
                "The default value for this is randomly generated when the plugin is loaded.");
            
            discordBotPort = config.Bind<int>(BOT_SETTINGS,
                "Listening Port",
                20736,
                "The port that the integration will listen on for communication from the Discord bot. If you are behind a firewall, this port needs to be forwarded appropriately.");
        }

        public string ConfigAsJson()
        {
            string jsonString = "{";


            jsonString += $"\"discordBotAuthorization\":\"REDACTED\",";
            jsonString += $"\"discordBotPort\":\"{DiscordBotPort}\"";

            jsonString += "}";
            return jsonString;
        }

        public string DiscordBotAuthorization => discordBotAuthorization.Value;
        public int DiscordBotPort => discordBotPort.Value;
    }
}
