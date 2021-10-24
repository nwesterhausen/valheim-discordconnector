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
        private BotConfig botConfig;
        private VariableConfig variableConfig;

        public PluginConfig(ConfigFile config)
        {
            // Set up the config file paths
            string messageConfigFilename = $"{PluginInfo.PLUGIN_ID}-{MessagesConfig.ConfigExtension}.cfg";
            string togglesConfigFilename = $"{PluginInfo.PLUGIN_ID}-{TogglesConfig.ConfigExtension}.cfg";
            string botConfigFilename = $"{PluginInfo.PLUGIN_ID}-{BotConfig.ConfigExtension}.cfg";
            string variableConfigFilename = $"{PluginInfo.PLUGIN_ID}-{VariableConfig.ConfigExtension}.cfg";

            string messagesConfigPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, messageConfigFilename);
            string togglesConfigPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, togglesConfigFilename);
            string botConfigPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, botConfigFilename);
            string variableConfigPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, variableConfigFilename);

            Plugin.StaticLogger.LogDebug($"Messages config: {messagesConfigPath}");
            Plugin.StaticLogger.LogDebug($"Toggles config: {togglesConfigPath}");
            Plugin.StaticLogger.LogDebug($"Bot config: {botConfigPath}");
            Plugin.StaticLogger.LogDebug($"Variable config: {variableConfigPath}");

            mainConfig = new MainConfig(config);
            messagesConfig = new MessagesConfig(new BepInEx.Configuration.ConfigFile(messagesConfigPath, true));
            togglesConfig = new TogglesConfig(new BepInEx.Configuration.ConfigFile(togglesConfigPath, true));
            botConfig = new BotConfig(new BepInEx.Configuration.ConfigFile(botConfigPath, true));
            variableConfig = new VariableConfig(new BepInEx.Configuration.ConfigFile(variableConfigPath, true));

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
        public bool DiscordBotEnabled => mainConfig.DiscordBotEnabled;


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
        public bool RankedDeathLeaderboardEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.RankedDeathLeaderboardEnabled;
        public bool RankedPingLeaderboardEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.RankedPingLeaderboardEnabled;
        public bool RankedSessionLeaderboardEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.RankedSessionLeaderboardEnabled;
        public bool RankedShoutLeaderboardEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.RankedShoutLeaderboardEnabled;


        public int IncludedNumberOfRankings => mainConfig.IncludedNumberOfRankings;
        public bool MostSessionLeaderboardEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.MostSessionLeaderboardEnabled;
        public bool MostPingLeaderboardEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.MostPingLeaderboardEnabled;
        public bool MostDeathLeaderboardEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.MostDeathLeaderboardEnabled;
        public bool MostShoutLeaderboardEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.MostShoutLeaderboardEnabled;
        public bool LeastSessionLeaderboardEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.LeastSessionLeaderboardEnabled;
        public bool LeastPingLeaderboardEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.LeastPingLeaderboardEnabled;
        public bool LeastDeathLeaderboardEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.LeastDeathLeaderboardEnabled;
        public bool LeastShoutLeaderboardEnabled => mainConfig.StatsAnnouncementEnabled && togglesConfig.LeastShoutLeaderboardEnabled;

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

        // Debug Toggles
        public bool DebugEveryPlayerPosCheck => togglesConfig.DebugEveryPlayerPosCheck;
        public bool DebugEveryEventCheck => togglesConfig.DebugEveryEventCheck;
        public bool DebugEveryEventChange => togglesConfig.DebugEveryEventChange;
        public bool DebugHttpRequestResponse => togglesConfig.DebugHttpRequestResponse;

        public string ConfigAsJson()
        {
            string jsonString = "{";

            jsonString += $"\"Config.Main\":{mainConfig.ConfigAsJson()},";
            jsonString += $"\"Config.Messages\":{messagesConfig.ConfigAsJson()},";
            jsonString += $"\"Config.Toggles\":{togglesConfig.ConfigAsJson()},";
            jsonString += $"\"Config.Bot\":{botConfig.ConfigAsJson()}";
            jsonString += $"\"Config.Variables\":{variableConfig.ConfigAsJson()}";

            jsonString += "}";
            return jsonString;
        }
    }
}
