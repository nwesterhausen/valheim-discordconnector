using System.IO;
using BepInEx;
using BepInEx.Configuration;

namespace DiscordConnectorLite.Config
{
    internal class PluginConfig
    {
        private MainConfig mainConfig;
        private MessagesConfig messagesConfig;
        private TogglesConfig togglesConfig;
        private BotConfig botConfig;
        private VariableConfig variableConfig;

        public PluginConfig(ConfigFile config)
        {
            // Set up the config file paths
            string messageConfigFilename = $"{PluginInfo.PLUGIN_ID}-{MessagesConfig.ConfigExtension}.cfg";
            string togglesConfigFilename = $"{PluginInfo.PLUGIN_ID}-{TogglesConfig.ConfigExtension}.cfg";
            string botConfigFilename = $"{PluginInfo.PLUGIN_ID}-{BotConfig.ConfigExtension}.cfg";
            string variableConfigFilename = $"{PluginInfo.PLUGIN_ID}-{VariableConfig.ConfigExtension}.cfg";

            string messagesConfigPath = Path.Combine(Paths.ConfigPath, messageConfigFilename);
            string togglesConfigPath = Path.Combine(Paths.ConfigPath, togglesConfigFilename);
            string botConfigPath = Path.Combine(Paths.ConfigPath, botConfigFilename);
            string variableConfigPath = Path.Combine(Paths.ConfigPath, variableConfigFilename);

            Plugin.StaticLogger.LogDebug($"Messages config: {messagesConfigPath}");
            Plugin.StaticLogger.LogDebug($"Toggles config: {togglesConfigPath}");
            Plugin.StaticLogger.LogDebug($"Bot config: {botConfigPath}");
            Plugin.StaticLogger.LogDebug($"Variable config: {variableConfigPath}");

            mainConfig = new MainConfig(config);
            messagesConfig = new MessagesConfig(new ConfigFile(messagesConfigPath, true));
            togglesConfig = new TogglesConfig(new ConfigFile(togglesConfigPath, true));
            botConfig = new BotConfig(new ConfigFile(botConfigPath, true));
            variableConfig = new VariableConfig(new ConfigFile(variableConfigPath, true));

            Plugin.StaticLogger.LogDebug("Configuration Loaded");
            Plugin.StaticLogger.LogDebug(ConfigAsJson());
        }

        public void ReloadConfig()
        {
            mainConfig.ReloadConfig();
            messagesConfig.ReloadConfig();
            togglesConfig.ReloadConfig();
            variableConfig.ReloadConfig();
            botConfig.ReloadConfig();
        }

        // Exposed Config Values
        
        // Toggles.Messages
        public bool LaunchMessageEnabled => togglesConfig.LaunchMessageEnabled;
        public bool LoadedMessageEnabled => togglesConfig.LoadedMessageEnabled;
        public bool StopMessageEnabled => togglesConfig.StopMessageEnabled;
        public bool ShutdownMessageEnabled => togglesConfig.ShutdownMessageEnabled;
        public bool WorldSaveMessageEnabled => togglesConfig.WorldSaveMessageEnabled;
        
        // Main Config
        public string WebHookURL => mainConfig.WebHookURL;
        public bool DiscordEmbedsEnabled => mainConfig.DiscordEmbedsEnabled;
        public bool DiscordBotEnabled => mainConfig.DiscordBotEnabled;

        // Messages.Server
        public string LaunchMessage => messagesConfig.LaunchMessage;
        public string LoadedMessage => messagesConfig.LoadedMessage;
        public string StopMessage => messagesConfig.StopMessage;
        public string ShutdownMessage => messagesConfig.ShutdownMessage;
        public string SaveMessage => messagesConfig.SaveMessage;

        // Discord Bot Integration
        public string DiscordBotAuthorization => botConfig.DiscordBotAuthorization;
        public int DiscordBotPort => botConfig.DiscordBotPort;
        // Variable Definition
        public string UserVariable => variableConfig.UserVariable;
        public string UserVariable1 => variableConfig.UserVariable1;
        public string UserVariable2 => variableConfig.UserVariable2;
        public string UserVariable3 => variableConfig.UserVariable3;
        public string UserVariable4 => variableConfig.UserVariable4;
        public string UserVariable5 => variableConfig.UserVariable5;
        public string UserVariable6 => variableConfig.UserVariable6;
        public string UserVariable7 => variableConfig.UserVariable7;
        public string UserVariable8 => variableConfig.UserVariable8;
        public string UserVariable9 => variableConfig.UserVariable9;

        public string ConfigAsJson()
        {
            string jsonString = "{";

            jsonString += $"\"Config.Main\":{mainConfig.ConfigAsJson()},";
            jsonString += $"\"Config.Messages\":{messagesConfig.ConfigAsJson()},";
            jsonString += $"\"Config.Toggles\":{togglesConfig.ConfigAsJson()},";
            jsonString += $"\"Config.Bot\":{botConfig.ConfigAsJson()},";
            jsonString += $"\"Config.Variables\":{variableConfig.ConfigAsJson()}";

            jsonString += "}";
            return jsonString;
        }
    }
}
