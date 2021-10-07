using System;
using System.Collections.Generic;
using System.IO;
using BepInEx.Configuration;

namespace DiscordConnector
{
    internal class PluginConfig
    {
        private MainConfig mainConfig;
        private MessagesConfig messagesConfig;
        private TogglesConfig togglesConfig;

        public PluginConfig(ConfigFile config)
        {
            // Set up the config file paths
            string messageConfigFilename = $"{PluginInfo.PLUGIN_ID}-{MessagesConfig.ConfigExention}.cfg";
            string togglesConfigFilename = $"{PluginInfo.PLUGIN_ID}-{TogglesConfig.ConfigExention}.cfg";
            string messagesConfigPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, messageConfigFilename);
            string togglesConfigPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, togglesConfigFilename);

            Plugin.StaticLogger.LogDebug($"Messages config: {messagesConfigPath}");
            Plugin.StaticLogger.LogDebug($"Toggles config: {togglesConfigPath}");

            mainConfig = new MainConfig(config);
            messagesConfig = new MessagesConfig(new BepInEx.Configuration.ConfigFile(messagesConfigPath, true));
            togglesConfig = new TogglesConfig(new BepInEx.Configuration.ConfigFile(togglesConfigPath, true));

            Plugin.StaticLogger.LogDebug("Configuration Loaded");
            Plugin.StaticLogger.LogDebug(ConfigAsJson());
        }

        // Exposed Config Values


        // Toggles
        public bool LaunchMessageEnabled => togglesConfig.LaunchMessageEnabled;
        public bool LoadedMessageEnabled => togglesConfig.LoadedMessageEnabled;
        public bool StopMessageEnabled => togglesConfig.StopMessageEnabled;
        public bool ChatMessageEnabled => togglesConfig.ChatMessageEnabled;
        public bool ChatShoutEnabled => togglesConfig.ChatShoutEnabled;
        public bool ChatShoutPosEnabled => togglesConfig.ChatShoutPosEnabled;
        public bool ChatPingEnabled => togglesConfig.ChatPingEnabled;
        public bool ChatPingPosEnabled => togglesConfig.ChatPingPosEnabled;
        public bool PlayerJoinMessageEnabled => togglesConfig.PlayerJoinMessageEnabled;
        public bool PlayerJoinPosEnabled => togglesConfig.PlayerJoinPosEnabled;
        public bool PlayerDeathMessageEnabled => togglesConfig.PlayerDeathMessageEnabled;
        public bool PlayerDeathPosEnabled => togglesConfig.PlayerDeathPosEnabled;
        public bool PlayerLeaveMessageEnabled => togglesConfig.PlayerLeaveMessageEnabled;
        public bool PlayerLeavePosEnabled => togglesConfig.PlayerLeavePosEnabled;
        public bool StatsDeathEnabled => togglesConfig.StatsDeathEnabled;
        public bool StatsJoinEnabled => togglesConfig.StatsJoinEnabled;
        public bool StatsLeaveEnabled => togglesConfig.StatsLeaveEnabled;
        public bool StatsPingEnabled => togglesConfig.StatsPingEnabled;
        public bool StatsShoutEnabled => togglesConfig.StatsShoutEnabled;

        // Main Config
        public string WebHookURL => mainConfig.WebHookURL;
        public bool StatsAnnouncementEnabled => mainConfig.StatsAnnouncementEnabled;
        public int StatsAnnouncementPeriod => mainConfig.StatsAnnouncementPeriod;
        public bool CollectStatsEnabled => mainConfig.CollectStatsEnabled;
        public bool DiscordEmbedsEnabled => mainConfig.DiscordEmbedsEnabled;
        public bool SendPositionsEnabled => mainConfig.SendPositionsEnabled;
        public List<string> MutedPlayers => mainConfig.MutedPlayers;


        // Messages
        public string LaunchMessage => messagesConfig.LaunchMessage;
        public string LoadedMessage => messagesConfig.LoadedMessage;
        public string StopMessage => messagesConfig.StopMessage;
        public string JoinMessage => messagesConfig.JoinMessage;
        public string LeaveMessage => messagesConfig.LeaveMessage;
        public string DeathMessage => messagesConfig.DeathMessage;
        public string PingMessage => messagesConfig.PingMessage;
        public string ShoutMessage => messagesConfig.ShoutMessage;
        public string ConfigAsJson()
        {
            string jsonString = "{";

            // Discord Settings
            jsonString += $"\"Config.Main\":{mainConfig.ConfigAsJson()},";
            jsonString += $"\"Config.Messages\":{messagesConfig.ConfigAsJson()},";
            jsonString += $"\"Config.Toggles\":{togglesConfig.ConfigAsJson()}";

            jsonString += "}";
            return jsonString;
        }

        private void CreateFileIfNotExist(string filePath)
        {
            if (!File.Exists(filePath))
            {
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.Close();
                }
            }
        }
    }
}