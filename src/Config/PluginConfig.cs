using System.Collections.Generic;
using BepInEx.Configuration;
using DiscordConnector.Config;

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
            string messageConfigFilename = $"{PluginInfo.PLUGIN_ID}-{MessagesConfig.ConfigExtension}.cfg";
            string togglesConfigFilename = $"{PluginInfo.PLUGIN_ID}-{TogglesConfig.ConfigExtension}.cfg";
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


        // Toggles.Messages
        public bool LaunchMessageEnabled => togglesConfig.LaunchMessageEnabled;
        public bool LoadedMessageEnabled => togglesConfig.LoadedMessageEnabled;
        public bool StopMessageEnabled => togglesConfig.StopMessageEnabled;
        public bool ShutdownMessageEnabled => togglesConfig.ShutdownMessageEnabled;
        public bool WorldSaveMessageEnabled => togglesConfig.WorldSaveMessageEnabled;
        public bool ChatShoutEnabled => togglesConfig.ChatShoutEnabled;
        public bool ChatPingEnabled => togglesConfig.ChatPingEnabled;
        public bool PlayerJoinMessageEnabled => togglesConfig.PlayerJoinMessageEnabled;
        public bool PlayerDeathMessageEnabled => togglesConfig.PlayerDeathMessageEnabled;
        public bool PlayerLeaveMessageEnabled => togglesConfig.PlayerLeaveMessageEnabled;
        public bool EventStartMessageEnabled => togglesConfig.EventStartMessageEnabled;
        public bool EventStopMessageEnabled => togglesConfig.EventStopMessageEnabled;
        public bool EventPausedMessageEnabled => togglesConfig.EventPausedMessageEnabled;

        public bool EventResumedMessageEnabled => togglesConfig.EventResumedMessageEnabled;

        // Toggles.Stats
        public bool StatsDeathEnabled => mainConfig.CollectStatsEnabled && togglesConfig.StatsDeathEnabled;
        public bool StatsJoinEnabled => mainConfig.CollectStatsEnabled && togglesConfig.StatsJoinEnabled;
        public bool StatsLeaveEnabled => mainConfig.CollectStatsEnabled && togglesConfig.StatsLeaveEnabled;
        public bool StatsPingEnabled => mainConfig.CollectStatsEnabled && togglesConfig.StatsPingEnabled;
        public bool StatsShoutEnabled => mainConfig.CollectStatsEnabled && togglesConfig.StatsShoutEnabled;

        // Toggles.Positions
        public bool ChatPingPosEnabled => mainConfig.SendPositionsEnabled && togglesConfig.ChatPingPosEnabled;
        public bool ChatShoutPosEnabled => mainConfig.SendPositionsEnabled && togglesConfig.ChatShoutPosEnabled;
        public bool PlayerJoinPosEnabled => mainConfig.SendPositionsEnabled && togglesConfig.PlayerJoinPosEnabled;
        public bool PlayerDeathPosEnabled => mainConfig.SendPositionsEnabled && togglesConfig.PlayerDeathPosEnabled;
        public bool PlayerLeavePosEnabled => mainConfig.SendPositionsEnabled && togglesConfig.PlayerLeavePosEnabled;
        public bool EventStartPosEnabled => mainConfig.SendPositionsEnabled && togglesConfig.EventStartPosEnabled;
        public bool EventStopPosEnabled => mainConfig.SendPositionsEnabled && togglesConfig.EventStopPosEnabled;
        public bool EventPausedPosEnabled => mainConfig.SendPositionsEnabled && togglesConfig.EventPausedPosEnabled;
        public bool EventResumedPosEnabled => mainConfig.SendPositionsEnabled && togglesConfig.EventResumedPosEnabled;

        // Main Config
        public string WebHookURL => mainConfig.WebHookURL;
        public bool StatsAnnouncementEnabled => mainConfig.StatsAnnouncementEnabled;
        public int StatsAnnouncementPeriod => mainConfig.StatsAnnouncementPeriod;
        public bool CollectStatsEnabled => mainConfig.CollectStatsEnabled;
        public bool DiscordEmbedsEnabled => mainConfig.DiscordEmbedsEnabled;
        public bool SendPositionsEnabled => mainConfig.SendPositionsEnabled;
        public bool AnnouncePlayerFirsts => mainConfig.AnnouncePlayerFirsts;
        public List<string> MutedPlayers => mainConfig.MutedPlayers;


        // Messages.Server
        public string LaunchMessage => messagesConfig.LaunchMessage;
        public string LoadedMessage => messagesConfig.LoadedMessage;
        public string StopMessage => messagesConfig.StopMessage;
        public string ShutdownMessage => messagesConfig.ShutdownMessage;
        public string SaveMessage => messagesConfig.SaveMessage;

        // Messages.Players
        public string JoinMessage => messagesConfig.JoinMessage;
        public string LeaveMessage => messagesConfig.LeaveMessage;
        public string DeathMessage => messagesConfig.DeathMessage;
        public string PingMessage => messagesConfig.PingMessage;
        public string ShoutMessage => messagesConfig.ShoutMessage;

        // Messages.PlayerFirsts
        public string PlayerFirstDeathMessage => messagesConfig.PlayerFirstDeathMessage;
        public string PlayerFirstJoinMessage => messagesConfig.PlayerFirstJoinMessage;
        public string PlayerFirstLeaveMessage => messagesConfig.PlayerFirstLeaveMessage;
        public string PlayerFirstPingMessage => messagesConfig.PlayerFirstPingMessage;
        public string PlayerFirstShoutMessage => messagesConfig.PlayerFirstShoutMessage;

        // Toggles.Leaderboard
        public bool LeaderboardDeathEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.LeaderboardDeathEnabled;
        public bool LeaderboardPingEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.LeaderboardDeathEnabled;
        public bool LeaderboardSessionEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.LeaderboardDeathEnabled;
        public bool LeaderboardShoutEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.LeaderboardDeathEnabled;

        public bool AnnouncePlayerFirstDeathEnabled => mainConfig.AnnouncePlayerFirsts && togglesConfig.AnnouncePlayerFirstDeathEnabled;
        public bool AnnouncePlayerFirstJoinEnabled => mainConfig.AnnouncePlayerFirsts && togglesConfig.AnnouncePlayerFirstJoinEnabled;
        public bool AnnouncePlayerFirstLeaveEnabled => mainConfig.AnnouncePlayerFirsts && togglesConfig.AnnouncePlayerFirstLeaveEnabled;
        public bool AnnouncePlayerFirstPingEnabled => mainConfig.AnnouncePlayerFirsts && togglesConfig.AnnouncePlayerFirstPingEnabled;
        public bool AnnouncePlayerFirstShoutEnabled => mainConfig.AnnouncePlayerFirsts && togglesConfig.AnnouncePlayerFirstShoutEnabled;

        // Messages.Events
        public string EventStartMessage => messagesConfig.EventStartMesssage;
        public string EventStopMesssage => messagesConfig.EventStopMesssage;
        public string EventPausedMesssage => messagesConfig.EventPausedMesssage;
        public string EventResumedMesssage => messagesConfig.EventResumedMesssage;

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
    }
}
