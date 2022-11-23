using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
using DiscordConnector.Config;

namespace DiscordConnector
{
    internal class PluginConfig
    {
        private MainConfig mainConfig;
        private MessagesConfig messagesConfig;
        private TogglesConfig togglesConfig;
        private VariableConfig variableConfig;
        private LeaderBoardConfig leaderBoardConfig;
        private Dictionary<string, Regex> filenameDictionaryRegex;
        private string configPath;

        private static string[] configExtensions = new string[]{
            "messages",
            "variables",
            "leaderBoard",
            "toggles"
        };

        internal void migrateConfigIfNeeded()
        {
            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }

            foreach (string extension in configExtensions)
            {
                string oldConfig = Path.Combine(BepInEx.Paths.ConfigPath, $"{PluginInfo.PLUGIN_ID}-{extension}.cfg");
                if (File.Exists(oldConfig))
                {
                    string newConfig = Path.Combine(configPath, $"{PluginInfo.SHORT_PLUGIN_ID}-{extension}.cfg");
                    File.Move(oldConfig, newConfig);
                }
            }

            // Check the base config file separately
            string oldMainConfig = Path.Combine(BepInEx.Paths.ConfigPath, $"{PluginInfo.PLUGIN_ID}.cfg");
            if (File.Exists(oldMainConfig))
            {
                string newConfig = Path.Combine(configPath, $"{PluginInfo.SHORT_PLUGIN_ID}.cfg");
                File.Move(oldMainConfig, newConfig);
            }
        }

        public PluginConfig(ConfigFile config)
        {
            // Set up base path for config and other files
            configPath = Path.Combine(BepInEx.Paths.ConfigPath, PluginInfo.PLUGIN_ID);

            // Migrate configs if needed, since we now nest them in a subdirectory
            migrateConfigIfNeeded();

            // Set up the config file paths
            string mainConfigFilename = $"{PluginInfo.SHORT_PLUGIN_ID}.cfg";
            string messageConfigFilename = $"{PluginInfo.SHORT_PLUGIN_ID}-{MessagesConfig.ConfigExtension}.cfg";
            string togglesConfigFilename = $"{PluginInfo.SHORT_PLUGIN_ID}-{TogglesConfig.ConfigExtension}.cfg";
            string variableConfigFilename = $"{PluginInfo.SHORT_PLUGIN_ID}-{VariableConfig.ConfigExtension}.cfg";
            string leaderBoardConfigFilename = $"{PluginInfo.SHORT_PLUGIN_ID}-{LeaderBoardConfig.ConfigExtension}.cfg";

            string mainConfigPath = Path.Combine(configPath, mainConfigFilename);
            string messagesConfigPath = Path.Combine(configPath, messageConfigFilename);
            string togglesConfigPath = Path.Combine(configPath, togglesConfigFilename);
            string variableConfigPath = Path.Combine(configPath, variableConfigFilename);
            string leaderBoardConfigPath = Path.Combine(configPath, leaderBoardConfigFilename);

            Plugin.StaticLogger.LogDebug($"Main config: {mainConfigPath}");
            Plugin.StaticLogger.LogDebug($"Messages config: {messagesConfigPath}");
            Plugin.StaticLogger.LogDebug($"Toggles config: {togglesConfigPath}");
            Plugin.StaticLogger.LogDebug($"Variable config: {variableConfigPath}");
            Plugin.StaticLogger.LogDebug($"Leader board config: {leaderBoardConfigFilename}");

            mainConfig = new MainConfig(new BepInEx.Configuration.ConfigFile(mainConfigPath, true));
            messagesConfig = new MessagesConfig(new BepInEx.Configuration.ConfigFile(messagesConfigPath, true));
            togglesConfig = new TogglesConfig(new BepInEx.Configuration.ConfigFile(togglesConfigPath, true));
            variableConfig = new VariableConfig(new BepInEx.Configuration.ConfigFile(variableConfigPath, true));
            leaderBoardConfig = new LeaderBoardConfig(new BepInEx.Configuration.ConfigFile(leaderBoardConfigPath, true));

            Plugin.StaticLogger.LogDebug("Configuration Loaded");
            Plugin.StaticLogger.LogDebug($"Muted Players Regex pattern ('a^' is default for no matches): {mainConfig.MutedPlayersRegex.ToString()}");
            DumpConfigAsJson();

            filenameDictionaryRegex = new Dictionary<string, Regex>();
            filenameDictionaryRegex.Add("main", new Regex(@"games.nwest.valheim.discordconnector\.cfg$"));
            filenameDictionaryRegex.Add("messages", new Regex(@"games.nwest.valheim.discordconnector-message\.cfg$"));
            filenameDictionaryRegex.Add("toggles", new Regex(@"games.nwest.valheim.discordconnector-toggles\.cfg$"));
            filenameDictionaryRegex.Add("variables", new Regex(@"games.nwest.valheim.discordconnector-variables\.cfg$"));
            filenameDictionaryRegex.Add("leaderBoard", new Regex(@"games.nwest.valheim.discordconnector-leaderBoard\.cfg$"));
        }

        public void ReloadConfig()
        {
            mainConfig.ReloadConfig();
            messagesConfig.ReloadConfig();
            togglesConfig.ReloadConfig();
            variableConfig.ReloadConfig();
            leaderBoardConfig.ReloadConfig();
        }

        public void ReloadConfig(string configPath)
        {
            if (filenameDictionaryRegex["main"].IsMatch(configPath))
            {
                mainConfig.ReloadConfig();
            }

            if (filenameDictionaryRegex["messages"].IsMatch(configPath))
            {
                messagesConfig.ReloadConfig();
            }

            if (filenameDictionaryRegex["toggles"].IsMatch(configPath))
            {
                togglesConfig.ReloadConfig();
            }

            if (filenameDictionaryRegex["variables"].IsMatch(configPath))
            {
                variableConfig.ReloadConfig();
            }

            if (filenameDictionaryRegex["leaderBoard"].IsMatch(configPath))
            {
                leaderBoardConfig.ReloadConfig();
            }
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
        public bool CollectStatsEnabled => mainConfig.CollectStatsEnabled;
        public bool DiscordEmbedsEnabled => mainConfig.DiscordEmbedsEnabled;
        public bool SendPositionsEnabled => mainConfig.SendPositionsEnabled;
        public bool AnnouncePlayerFirsts => mainConfig.AnnouncePlayerFirsts;
        public MainConfig.RetrievalDiscernmentMethods RecordRetrievalDiscernmentMethod => mainConfig.RecordRetrievalDiscernmentMethod;
        public List<string> MutedPlayers => mainConfig.MutedPlayers;
        public Regex MutedPlayersRegex => mainConfig.MutedPlayersRegex;
        public bool AllowNonPlayerShoutLogging => mainConfig.AllowNonPlayerShoutLogging;


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

        public bool AnnouncePlayerFirstDeathEnabled => mainConfig.AnnouncePlayerFirsts && togglesConfig.AnnouncePlayerFirstDeathEnabled;
        public bool AnnouncePlayerFirstJoinEnabled => mainConfig.AnnouncePlayerFirsts && togglesConfig.AnnouncePlayerFirstJoinEnabled;
        public bool AnnouncePlayerFirstLeaveEnabled => mainConfig.AnnouncePlayerFirsts && togglesConfig.AnnouncePlayerFirstLeaveEnabled;
        public bool AnnouncePlayerFirstPingEnabled => mainConfig.AnnouncePlayerFirsts && togglesConfig.AnnouncePlayerFirstPingEnabled;
        public bool AnnouncePlayerFirstShoutEnabled => mainConfig.AnnouncePlayerFirsts && togglesConfig.AnnouncePlayerFirstShoutEnabled;

        // Messages.Events
        public string EventStartMessage => messagesConfig.EventStartMessage;
        public string EventStopMessage => messagesConfig.EventStopMessage;
        public string EventPausedMessage => messagesConfig.EventPausedMessage;
        public string EventResumedMessage => messagesConfig.EventResumedMessage;

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

        // Configured Dynamic Variables
        public string PosVarFormat => variableConfig.PosVarFormat;
        public string AppendedPosFormat => variableConfig.AppendedPosFormat;

        // Debug Toggles
        public bool DebugEveryPlayerPosCheck => togglesConfig.DebugEveryPlayerPosCheck;
        public bool DebugEveryEventCheck => togglesConfig.DebugEveryEventCheck;
        public bool DebugEveryEventChange => togglesConfig.DebugEveryEventChange;
        public bool DebugHttpRequestResponse => togglesConfig.DebugHttpRequestResponse;
        public bool DebugDatabaseMethods => togglesConfig.DebugDatabaseMethods;

        // Leader board Messages
        public string LeaderBoardTopPlayerHeading => messagesConfig.LeaderBoardTopPlayerHeading;
        public string LeaderBoardBottomPlayersHeading => messagesConfig.LeaderBoardBottomPlayersHeading;
        public string LeaderBoardHighestHeading => messagesConfig.LeaderBoardHighestHeading;
        public string LeaderBoardLowestHeading => messagesConfig.LeaderBoardLowestHeading;

        // Leader board configs
        public LeaderBoardConfigReference[] LeaderBoards => leaderBoardConfig.LeaderBoards;
        public ActivePlayersAnnouncementConfigValues ActivePlayersAnnouncement => leaderBoardConfig.ActivePlayersAnnouncement;

        public void DumpConfigAsJson()
        {
            string jsonString = "{";

            jsonString += $"\"Config.Main\":{mainConfig.ConfigAsJson()},";
            jsonString += $"\"Config.Messages\":{messagesConfig.ConfigAsJson()},";
            jsonString += $"\"Config.Toggles\":{togglesConfig.ConfigAsJson()},";
            jsonString += $"\"Config.Variables\":{variableConfig.ConfigAsJson()},";
            jsonString += $"\"Config.LeaderBoard\":{leaderBoardConfig.ConfigAsJson()}";

            jsonString += "}";

            System.Threading.Tasks.Task.Run(() =>
            {
                string configDump = Path.Combine(configPath, "config-debug.json");
                File.WriteAllText(configDump, jsonString);
                Plugin.StaticLogger.LogDebug("Dumped configuration files to JSON for debugging (if needed).");
            });
        }
    }
}
