using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using DiscordConnector.Config;

namespace DiscordConnector;

internal class PluginConfig
{
    private const string ConfigJsonFilename = "config-dump.json";

    /// <summary>
    ///     Valid extensions for the config files, plus a reference for main.
    /// </summary>
    internal static string[] ConfigExtensions =
        new[] { "messages", "variables", "leaderBoard", "toggles", "extraWebhooks", "main" };

    public readonly string configPath;
    private readonly ExtraWebhookConfig extraWebhookConfig;
    private readonly LeaderBoardConfig leaderBoardConfig;
    private readonly MainConfig mainConfig;
    private readonly MessagesConfig messagesConfig;
    private readonly TogglesConfig togglesConfig;
    private readonly VariableConfig variableConfig;

    public PluginConfig(ConfigFile config)
    {
        // Set up base path for config and other files
        configPath = Path.Combine(Paths.ConfigPath, DiscordConnectorPlugin.LegacyConfigPath);

        // Migrate configs if needed, since we now nest them in a subdirectory
        migrateConfigIfNeeded();

        // Set up the config file paths
        string mainConfigFilename = $"{DiscordConnectorPlugin.LegacyModName}.cfg";
        string messageConfigFilename = $"{DiscordConnectorPlugin.LegacyModName}-{MessagesConfig.ConfigExtension}.cfg";
        string togglesConfigFilename = $"{DiscordConnectorPlugin.LegacyModName}-{TogglesConfig.ConfigExtension}.cfg";
        string variableConfigFilename = $"{DiscordConnectorPlugin.LegacyModName}-{VariableConfig.ConfigExtension}.cfg";
        string leaderBoardConfigFilename = $"{DiscordConnectorPlugin.LegacyModName}-{LeaderBoardConfig.ConfigExtension}.cfg";
        string extraWebhooksConfigFilename =
            $"{DiscordConnectorPlugin.LegacyModName}-{ExtraWebhookConfig.ConfigExtension}.cfg";

        string mainConfigPath = Path.Combine(configPath, mainConfigFilename);
        string messagesConfigPath = Path.Combine(configPath, messageConfigFilename);
        string togglesConfigPath = Path.Combine(configPath, togglesConfigFilename);
        string variableConfigPath = Path.Combine(configPath, variableConfigFilename);
        string leaderBoardConfigPath = Path.Combine(configPath, leaderBoardConfigFilename);
        string extraWebhooksConfigPath = Path.Combine(configPath, extraWebhooksConfigFilename);

        DiscordConnectorPlugin.StaticLogger.LogDebug($"Main config: {mainConfigPath}");
        DiscordConnectorPlugin.StaticLogger.LogDebug($"Messages config: {messagesConfigPath}");
        DiscordConnectorPlugin.StaticLogger.LogDebug($"Toggles config: {togglesConfigPath}");
        DiscordConnectorPlugin.StaticLogger.LogDebug($"Variable config: {variableConfigPath}");
        DiscordConnectorPlugin.StaticLogger.LogDebug($"Leader board config: {leaderBoardConfigPath}");
        DiscordConnectorPlugin.StaticLogger.LogDebug($"Extra Webhook config: {extraWebhooksConfigPath}");

        mainConfig = new MainConfig(new ConfigFile(mainConfigPath, true));
        messagesConfig = new MessagesConfig(new ConfigFile(messagesConfigPath, true));
        togglesConfig = new TogglesConfig(new ConfigFile(togglesConfigPath, true));
        variableConfig = new VariableConfig(new ConfigFile(variableConfigPath, true));
        leaderBoardConfig = new LeaderBoardConfig(new ConfigFile(leaderBoardConfigPath, true));
        extraWebhookConfig = new ExtraWebhookConfig(new ConfigFile(extraWebhooksConfigPath, true));

        DiscordConnectorPlugin.StaticLogger.LogDebug("Configuration Loaded");
        DiscordConnectorPlugin.StaticLogger.LogDebug(
            $"Muted Players Regex pattern ('a^' is default for no matches): {mainConfig.MutedPlayersRegex}");
        DumpConfigAsJson();
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
    public bool ChatShoutAllCaps => togglesConfig.ChatShoutAllCaps;
    public bool NewDayNumberEnabled => togglesConfig.NewDayNumberEnabled;

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
    public string DefaultWebhookUsernameOverride => mainConfig.DefaultWebhookUsernameOverride;
    public WebhookEntry PrimaryWebhook => mainConfig.PrimaryWebhook;
    public WebhookEntry SecondaryWebhook => mainConfig.SecondaryWebhook;
    public bool CollectStatsEnabled => mainConfig.CollectStatsEnabled;
    public bool DiscordEmbedsEnabled => mainConfig.DiscordEmbedsEnabled;
    public bool SendPositionsEnabled => mainConfig.SendPositionsEnabled;
    public bool ShowPlayerIds => mainConfig.ShowPlayerIds;
    public bool AnnouncePlayerFirsts => mainConfig.AnnouncePlayerFirsts;
    
    // Embed Field Visibility Properties
    public bool EmbedTitleEnabled => mainConfig.EmbedTitleEnabled;
    public bool EmbedDescriptionEnabled => mainConfig.EmbedDescriptionEnabled;
    public bool EmbedAuthorEnabled => mainConfig.EmbedAuthorEnabled;
    public bool EmbedThumbnailEnabled => mainConfig.EmbedThumbnailEnabled;
    public bool EmbedFooterEnabled => mainConfig.EmbedFooterEnabled;
    public bool EmbedTimestampEnabled => mainConfig.EmbedTimestampEnabled;
    
    // Embed Color Properties
    public string EmbedDefaultColor => mainConfig.EmbedDefaultColor;
    public string EmbedServerStartColor => mainConfig.EmbedServerStartColor;
    public string EmbedServerStopColor => mainConfig.EmbedServerStopColor;
    public string EmbedPlayerJoinColor => mainConfig.EmbedPlayerJoinColor;
    public string EmbedPlayerLeaveColor => mainConfig.EmbedPlayerLeaveColor;
    public string EmbedDeathEventColor => mainConfig.EmbedDeathEventColor;
    public string EmbedShoutMessageColor => mainConfig.EmbedShoutMessageColor;
    public string EmbedOtherEventColor => mainConfig.EmbedOtherEventColor;
    public string EmbedWorldEventColor => mainConfig.EmbedWorldEventColor;
    public string EmbedNewDayColor => mainConfig.EmbedNewDayColor;
    public string EmbedServerSaveColor => mainConfig.EmbedServerSaveColor;
    public string EmbedPositionMessageColor => mainConfig.EmbedPositionMessageColor;
    public string EmbedActivePlayersColor => mainConfig.EmbedActivePlayersColor;
    public string EmbedLeaderboardEmbedColor => mainConfig.EmbedLeaderboardEmbedColor;
    
    // Other Embed Customization Properties
    public string EmbedFooterText => mainConfig.EmbedFooterText;
    public List<string> EmbedFieldDisplayOrder => mainConfig.EmbedFieldDisplayOrder;
    public string EmbedUrlTemplate => mainConfig.EmbedUrlTemplate;
    public string EmbedAuthorIconUrl => mainConfig.EmbedAuthorIconUrl;
    public string EmbedThumbnailUrl => mainConfig.EmbedThumbnailUrl;
    public string ServerName => "Valheim"; // Default server name if not otherwise specified

    public MainConfig.RetrievalDiscernmentMethods RecordRetrievalDiscernmentMethod =>
        mainConfig.RecordRetrievalDiscernmentMethod;

    public List<string> MutedPlayers => mainConfig.MutedPlayers;
    public Regex MutedPlayersRegex => mainConfig.MutedPlayersRegex;
    public bool AllowNonPlayerShoutLogging => mainConfig.AllowNonPlayerShoutLogging;
    public bool AllowMentionsHereEveryone => mainConfig.AllowMentionsHereEveryone;
    public bool AllowMentionsAnyRole => mainConfig.AllowMentionsAnyRole;
    public bool AllowMentionsAnyUser => mainConfig.AllowMentionsAnyUser;
    public List<string> AllowedRoleMentions => mainConfig.AllowedRoleMentions;
    public List<string> AllowedUserMentions => mainConfig.AllowedUserMentions;


    // Messages.Server
    public string LaunchMessage => messagesConfig.LaunchMessage;
    public string LoadedMessage => messagesConfig.LoadedMessage;
    public string StopMessage => messagesConfig.StopMessage;
    public string ShutdownMessage => messagesConfig.ShutdownMessage;
    public string SaveMessage => messagesConfig.SaveMessage;
    public string NewDayMessage => messagesConfig.NewDayMessage;

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

    public bool AnnouncePlayerFirstDeathEnabled =>
        mainConfig.AnnouncePlayerFirsts && togglesConfig.AnnouncePlayerFirstDeathEnabled;

    public bool AnnouncePlayerFirstJoinEnabled =>
        mainConfig.AnnouncePlayerFirsts && togglesConfig.AnnouncePlayerFirstJoinEnabled;

    public bool AnnouncePlayerFirstLeaveEnabled =>
        mainConfig.AnnouncePlayerFirsts && togglesConfig.AnnouncePlayerFirstLeaveEnabled;

    public bool AnnouncePlayerFirstPingEnabled =>
        mainConfig.AnnouncePlayerFirsts && togglesConfig.AnnouncePlayerFirstPingEnabled;

    public bool AnnouncePlayerFirstShoutEnabled =>
        mainConfig.AnnouncePlayerFirsts && togglesConfig.AnnouncePlayerFirstShoutEnabled;

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
    public bool DebugLeaderboardOperations => togglesConfig.DebugLeaderboardOperations;

    // Leader board Messages
    public string LeaderBoardTopPlayerHeading => messagesConfig.LeaderBoardTopPlayerHeading;
    public string LeaderBoardBottomPlayersHeading => messagesConfig.LeaderBoardBottomPlayersHeading;
    public string LeaderBoardHighestHeading => messagesConfig.LeaderBoardHighestHeading;
    public string LeaderBoardLowestHeading => messagesConfig.LeaderBoardLowestHeading;

    // Leader board configs
    public LeaderBoardConfigReference[] LeaderBoards => leaderBoardConfig.LeaderBoards;

    public ActivePlayersAnnouncementConfigValues ActivePlayersAnnouncement =>
        leaderBoardConfig.ActivePlayersAnnouncement;

    // Extra webhook config
    public List<WebhookEntry> ExtraWebhooks => extraWebhookConfig.GetWebhookEntries();

    /// <summary>
    ///     In 2.1.0, moving to using a subdirectory for config files, since there are a handful of different files to manage
    ///     and the feature was requested.
    ///     This method will make the new config sub-directory (if it doesn't exist) and then move the DiscordConnector config
    ///     files into the new config
    ///     sub-directory. If the files already exist in the new sub-directory, then this will log a warning for each config
    ///     that exists there, since they
    ///     should not exist there yet!
    /// </summary>
    internal void migrateConfigIfNeeded()
    {
        if (!Directory.Exists(configPath))
        {
            Directory.CreateDirectory(configPath);
        }

        foreach (string extension in ConfigExtensions)
        {
            string oldConfig = Path.Combine(Paths.ConfigPath, $"{DiscordConnectorPlugin.LegacyModName}-{extension}.cfg");
            string newConfig = Path.Combine(configPath, $"{DiscordConnectorPlugin.LegacyModName}-{extension}.cfg");
            // Main config has special handling (no -main extension on it)
            if (extension.Equals("main"))
            {
                // Main config uses no extensions
                oldConfig = Path.Combine(Paths.ConfigPath, $"{DiscordConnectorPlugin.LegacyModName}.cfg");
                newConfig = Path.Combine(configPath, $"{DiscordConnectorPlugin.LegacyModName}.cfg");
            }

            if (File.Exists(oldConfig))
            {
                if (File.Exists(newConfig))
                {
                    // There already exists a config in the destination, which is weird because configs also exist in the old location
                    DiscordConnectorPlugin.StaticLogger.LogWarning(
                        $"Expected to be moving {extension} config from pre-2.1.0 location to new config location, but already exists!");
                }
                else
                {
                    // Migrate the file if it doesn't already exist there.
                    File.Move(oldConfig, newConfig);
                }
            }
        }
    }

    /// <summary>
    ///     Writes the loaded configuration to a JSON file in the config directory.
    /// </summary>
    public void DumpConfigAsJson()
    {
        string jsonString = "{";

        jsonString += $"\"Config.Main\":{mainConfig.ConfigAsJson()},";
        jsonString += $"\"Config.Messages\":{messagesConfig.ConfigAsJson()},";
        jsonString += $"\"Config.Toggles\":{togglesConfig.ConfigAsJson()},";
        jsonString += $"\"Config.Variables\":{variableConfig.ConfigAsJson()},";
        jsonString += $"\"Config.LeaderBoard\":{leaderBoardConfig.ConfigAsJson()},";
        jsonString += $"\"Config.ExtraWebhooks\":{extraWebhookConfig.ConfigAsJson()}";

        jsonString += "}";

        Task.Run(() =>
        {
            string configDump = Path.Combine(configPath, ConfigJsonFilename);
            File.WriteAllText(configDump, jsonString);
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Dumped configuration files to {ConfigJsonFilename}");
        });
    }
}
