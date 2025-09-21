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
		this.configPath = Path.Combine(Paths.ConfigPath, DiscordConnectorPlugin.LegacyConfigPath);

		// Migrate configs if needed, since we now nest them in a subdirectory
		this.migrateConfigIfNeeded();

		// Set up the config file paths
		var mainConfigFilename = $"{DiscordConnectorPlugin.LegacyModName}.cfg";
		var messageConfigFilename = $"{DiscordConnectorPlugin.LegacyModName}-{MessagesConfig.ConfigExtension}.cfg";
		var togglesConfigFilename = $"{DiscordConnectorPlugin.LegacyModName}-{TogglesConfig.ConfigExtension}.cfg";
		var variableConfigFilename = $"{DiscordConnectorPlugin.LegacyModName}-{VariableConfig.ConfigExtension}.cfg";
		var leaderBoardConfigFilename =
			$"{DiscordConnectorPlugin.LegacyModName}-{LeaderBoardConfig.ConfigExtension}.cfg";
		var extraWebhooksConfigFilename =
			$"{DiscordConnectorPlugin.LegacyModName}-{ExtraWebhookConfig.ConfigExtension}.cfg";

		var mainConfigPath = Path.Combine(this.configPath, mainConfigFilename);
		var messagesConfigPath = Path.Combine(this.configPath, messageConfigFilename);
		var togglesConfigPath = Path.Combine(this.configPath, togglesConfigFilename);
		var variableConfigPath = Path.Combine(this.configPath, variableConfigFilename);
		var leaderBoardConfigPath = Path.Combine(this.configPath, leaderBoardConfigFilename);
		var extraWebhooksConfigPath = Path.Combine(this.configPath, extraWebhooksConfigFilename);

		DiscordConnectorPlugin.StaticLogger.LogDebug($"Main config: {mainConfigPath}");
		DiscordConnectorPlugin.StaticLogger.LogDebug($"Messages config: {messagesConfigPath}");
		DiscordConnectorPlugin.StaticLogger.LogDebug($"Toggles config: {togglesConfigPath}");
		DiscordConnectorPlugin.StaticLogger.LogDebug($"Variable config: {variableConfigPath}");
		DiscordConnectorPlugin.StaticLogger.LogDebug($"Leader board config: {leaderBoardConfigPath}");
		DiscordConnectorPlugin.StaticLogger.LogDebug($"Extra Webhook config: {extraWebhooksConfigPath}");

		this.mainConfig = new MainConfig(new ConfigFile(mainConfigPath, true));
		this.messagesConfig = new MessagesConfig(new ConfigFile(messagesConfigPath, true));
		this.togglesConfig = new TogglesConfig(new ConfigFile(togglesConfigPath, true));
		this.variableConfig = new VariableConfig(new ConfigFile(variableConfigPath, true));
		this.leaderBoardConfig = new LeaderBoardConfig(new ConfigFile(leaderBoardConfigPath, true));
		this.extraWebhookConfig = new ExtraWebhookConfig(new ConfigFile(extraWebhooksConfigPath, true));

		DiscordConnectorPlugin.StaticLogger.LogDebug("Configuration Loaded");
		DiscordConnectorPlugin.StaticLogger.LogDebug(
			$"Muted Players Regex pattern ('a^' is default for no matches): {this.mainConfig.MutedPlayersRegex}");
		this.DumpConfigAsJson();
	}

	// Exposed Config Values


	// Toggles.Messages
	public bool LaunchMessageEnabled => this.togglesConfig.LaunchMessageEnabled;
	public bool LoadedMessageEnabled => this.togglesConfig.LoadedMessageEnabled;
	public bool StopMessageEnabled => this.togglesConfig.StopMessageEnabled;
	public bool ShutdownMessageEnabled => this.togglesConfig.ShutdownMessageEnabled;
	public bool WorldSaveMessageEnabled => this.togglesConfig.WorldSaveMessageEnabled;
	public bool ChatShoutEnabled => this.togglesConfig.ChatShoutEnabled;
	public bool ChatPingEnabled => this.togglesConfig.ChatPingEnabled;
	public bool PlayerJoinMessageEnabled => this.togglesConfig.PlayerJoinMessageEnabled;
	public bool PlayerDeathMessageEnabled => this.togglesConfig.PlayerDeathMessageEnabled;
	public bool PlayerLeaveMessageEnabled => this.togglesConfig.PlayerLeaveMessageEnabled;
	public bool EventStartMessageEnabled => this.togglesConfig.EventStartMessageEnabled;
	public bool EventStopMessageEnabled => this.togglesConfig.EventStopMessageEnabled;
	public bool EventPausedMessageEnabled => this.togglesConfig.EventPausedMessageEnabled;
	public bool EventResumedMessageEnabled => this.togglesConfig.EventResumedMessageEnabled;
	public bool ChatShoutAllCaps => this.togglesConfig.ChatShoutAllCaps;
	public bool NewDayNumberEnabled => this.togglesConfig.NewDayNumberEnabled;

	// Toggles.Stats
	public bool StatsDeathEnabled => this.mainConfig.CollectStatsEnabled && this.togglesConfig.StatsDeathEnabled;
	public bool StatsJoinEnabled => this.mainConfig.CollectStatsEnabled && this.togglesConfig.StatsJoinEnabled;
	public bool StatsLeaveEnabled => this.mainConfig.CollectStatsEnabled && this.togglesConfig.StatsLeaveEnabled;
	public bool StatsPingEnabled => this.mainConfig.CollectStatsEnabled && this.togglesConfig.StatsPingEnabled;
	public bool StatsShoutEnabled => this.mainConfig.CollectStatsEnabled && this.togglesConfig.StatsShoutEnabled;

	// Toggles.Positions
	public bool ChatPingPosEnabled => this.mainConfig.SendPositionsEnabled && this.togglesConfig.ChatPingPosEnabled;
	public bool ChatShoutPosEnabled => this.mainConfig.SendPositionsEnabled && this.togglesConfig.ChatShoutPosEnabled;
	public bool PlayerJoinPosEnabled => this.mainConfig.SendPositionsEnabled && this.togglesConfig.PlayerJoinPosEnabled;
	public bool PlayerDeathPosEnabled => this.mainConfig.SendPositionsEnabled && this.togglesConfig.PlayerDeathPosEnabled;
	public bool PlayerLeavePosEnabled => this.mainConfig.SendPositionsEnabled && this.togglesConfig.PlayerLeavePosEnabled;
	public bool EventStartPosEnabled => this.mainConfig.SendPositionsEnabled && this.togglesConfig.EventStartPosEnabled;
	public bool EventStopPosEnabled => this.mainConfig.SendPositionsEnabled && this.togglesConfig.EventStopPosEnabled;
	public bool EventPausedPosEnabled => this.mainConfig.SendPositionsEnabled && this.togglesConfig.EventPausedPosEnabled;
	public bool EventResumedPosEnabled => this.mainConfig.SendPositionsEnabled && this.togglesConfig.EventResumedPosEnabled;

	// Main Config
	public string DefaultWebhookUsernameOverride => this.mainConfig.DefaultWebhookUsernameOverride;
	public WebhookEntry PrimaryWebhook => this.mainConfig.PrimaryWebhook;
	public WebhookEntry SecondaryWebhook => this.mainConfig.SecondaryWebhook;
	public bool CollectStatsEnabled => this.mainConfig.CollectStatsEnabled;
	public bool DiscordEmbedsEnabled => this.mainConfig.DiscordEmbedsEnabled;
	public bool SendPositionsEnabled => this.mainConfig.SendPositionsEnabled;
	public bool ShowPlayerIds => this.mainConfig.ShowPlayerIds;
	public bool AnnouncePlayerFirsts => this.mainConfig.AnnouncePlayerFirsts;

	// Embed Field Visibility Properties
	public bool EmbedTitleEnabled => this.mainConfig.EmbedTitleEnabled;
	public bool EmbedDescriptionEnabled => this.mainConfig.EmbedDescriptionEnabled;
	public bool EmbedAuthorEnabled => this.mainConfig.EmbedAuthorEnabled;
	public bool EmbedThumbnailEnabled => this.mainConfig.EmbedThumbnailEnabled;
	public bool EmbedFooterEnabled => this.mainConfig.EmbedFooterEnabled;
	public bool EmbedTimestampEnabled => this.mainConfig.EmbedTimestampEnabled;

	// Embed Color Properties
	public string EmbedDefaultColor => this.mainConfig.EmbedDefaultColor;
	public string EmbedServerStartColor => this.mainConfig.EmbedServerStartColor;
	public string EmbedServerStopColor => this.mainConfig.EmbedServerStopColor;
	public string EmbedPlayerJoinColor => this.mainConfig.EmbedPlayerJoinColor;
	public string EmbedPlayerLeaveColor => this.mainConfig.EmbedPlayerLeaveColor;
	public string EmbedDeathEventColor => this.mainConfig.EmbedDeathEventColor;
	public string EmbedShoutMessageColor => this.mainConfig.EmbedShoutMessageColor;
	public string EmbedOtherEventColor => this.mainConfig.EmbedOtherEventColor;
	public string EmbedWorldEventColor => this.mainConfig.EmbedWorldEventColor;
	public string EmbedNewDayColor => this.mainConfig.EmbedNewDayColor;
	public string EmbedServerSaveColor => this.mainConfig.EmbedServerSaveColor;
	public string EmbedPositionMessageColor => this.mainConfig.EmbedPositionMessageColor;
	public string EmbedActivePlayersColor => this.mainConfig.EmbedActivePlayersColor;
	public string EmbedLeaderboardEmbedColor => this.mainConfig.EmbedLeaderboardEmbedColor;

	// Other Embed Customization Properties
	public string EmbedFooterText => this.mainConfig.EmbedFooterText;
	public List<string> EmbedFieldDisplayOrder => this.mainConfig.EmbedFieldDisplayOrderList;
	public string EmbedUrlTemplate => this.mainConfig.EmbedUrlTemplate;
	public string EmbedAuthorIconUrl => this.mainConfig.EmbedAuthorIconUrl;
	public string EmbedThumbnailUrl => this.mainConfig.EmbedThumbnailUrl;
	public string ServerName => "Valheim"; // Default server name if not otherwise specified

	public MainConfig.RetrievalDiscernmentMethods RecordRetrievalDiscernmentMethod => this.mainConfig.RecordRetrievalDiscernmentMethod;

	public List<string> MutedPlayers => this.mainConfig.MutedPlayers;
	public Regex MutedPlayersRegex => this.mainConfig.MutedPlayersRegex;
	public bool AllowNonPlayerShoutLogging => this.mainConfig.AllowNonPlayerShoutLogging;
	public bool AllowMentionsHereEveryone => this.mainConfig.AllowMentionsHereEveryone;
	public bool AllowMentionsAnyRole => this.mainConfig.AllowMentionsAnyRole;
	public bool AllowMentionsAnyUser => this.mainConfig.AllowMentionsAnyUser;
	public List<string> AllowedRoleMentions => this.mainConfig.AllowedRoleMentions;
	public List<string> AllowedUserMentions => this.mainConfig.AllowedUserMentions;


	// Messages.Server
	public string LaunchMessage => this.messagesConfig.LaunchMessage;
	public string LoadedMessage => this.messagesConfig.LoadedMessage;
	public string StopMessage => this.messagesConfig.StopMessage;
	public string ShutdownMessage => this.messagesConfig.ShutdownMessage;
	public string SaveMessage => this.messagesConfig.SaveMessage;
	public string NewDayMessage => this.messagesConfig.NewDayMessage;

	// Messages.Players
	public string JoinMessage => this.messagesConfig.JoinMessage;
	public string LeaveMessage => this.messagesConfig.LeaveMessage;
	public string DeathMessage => this.messagesConfig.DeathMessage;
	public string PingMessage => this.messagesConfig.PingMessage;
	public string ShoutMessage => this.messagesConfig.ShoutMessage;

	// Messages.PlayerFirsts
	public string PlayerFirstDeathMessage => this.messagesConfig.PlayerFirstDeathMessage;
	public string PlayerFirstJoinMessage => this.messagesConfig.PlayerFirstJoinMessage;
	public string PlayerFirstLeaveMessage => this.messagesConfig.PlayerFirstLeaveMessage;
	public string PlayerFirstPingMessage => this.messagesConfig.PlayerFirstPingMessage;
	public string PlayerFirstShoutMessage => this.messagesConfig.PlayerFirstShoutMessage;

	public bool AnnouncePlayerFirstDeathEnabled => this.mainConfig.AnnouncePlayerFirsts && this.togglesConfig.AnnouncePlayerFirstDeathEnabled;

	public bool AnnouncePlayerFirstJoinEnabled => this.mainConfig.AnnouncePlayerFirsts && this.togglesConfig.AnnouncePlayerFirstJoinEnabled;

	public bool AnnouncePlayerFirstLeaveEnabled => this.mainConfig.AnnouncePlayerFirsts && this.togglesConfig.AnnouncePlayerFirstLeaveEnabled;

	public bool AnnouncePlayerFirstPingEnabled => this.mainConfig.AnnouncePlayerFirsts && this.togglesConfig.AnnouncePlayerFirstPingEnabled;

	public bool AnnouncePlayerFirstShoutEnabled => this.mainConfig.AnnouncePlayerFirsts && this.togglesConfig.AnnouncePlayerFirstShoutEnabled;

	// Messages.Events
	public string EventStartMessage => this.messagesConfig.EventStartMessage;
	public string EventStopMessage => this.messagesConfig.EventStopMessage;
	public string EventPausedMessage => this.messagesConfig.EventPausedMessage;
	public string EventResumedMessage => this.messagesConfig.EventResumedMessage;

	// Variable Definition
	public string UserVariable => this.variableConfig.UserVariable;
	public string UserVariable1 => this.variableConfig.UserVariable1;
	public string UserVariable2 => this.variableConfig.UserVariable2;
	public string UserVariable3 => this.variableConfig.UserVariable3;
	public string UserVariable4 => this.variableConfig.UserVariable4;
	public string UserVariable5 => this.variableConfig.UserVariable5;
	public string UserVariable6 => this.variableConfig.UserVariable6;
	public string UserVariable7 => this.variableConfig.UserVariable7;
	public string UserVariable8 => this.variableConfig.UserVariable8;
	public string UserVariable9 => this.variableConfig.UserVariable9;

	// Configured Dynamic Variables
	public string PosVarFormat => this.variableConfig.PosVarFormat;
	public string AppendedPosFormat => this.variableConfig.AppendedPosFormat;

	// Debug Toggles
	public bool DebugEveryPlayerPosCheck => this.togglesConfig.DebugEveryPlayerPosCheck;
	public bool DebugEveryEventCheck => this.togglesConfig.DebugEveryEventCheck;
	public bool DebugEveryEventChange => this.togglesConfig.DebugEveryEventChange;
	public bool DebugHttpRequestResponse => this.togglesConfig.DebugHttpRequestResponse;
	public bool DebugDatabaseMethods => this.togglesConfig.DebugDatabaseMethods;
	public bool DebugLeaderboardOperations => this.togglesConfig.DebugLeaderboardOperations;

	// Leader board Messages
	public string LeaderBoardTopPlayerHeading => this.messagesConfig.LeaderBoardTopPlayerHeading;
	public string LeaderBoardBottomPlayersHeading => this.messagesConfig.LeaderBoardBottomPlayersHeading;
	public string LeaderBoardHighestHeading => this.messagesConfig.LeaderBoardHighestHeading;
	public string LeaderBoardLowestHeading => this.messagesConfig.LeaderBoardLowestHeading;

	// Leader board configs
	public LeaderBoardConfigReference[] LeaderBoards => this.leaderBoardConfig.LeaderBoards;

	public ActivePlayersAnnouncementConfigValues ActivePlayersAnnouncement => this.leaderBoardConfig.ActivePlayersAnnouncement;

	// Extra webhook config
	public List<WebhookEntry> ExtraWebhooks => this.extraWebhookConfig.GetWebhookEntries();

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
		if (!Directory.Exists(this.configPath))
		{
			Directory.CreateDirectory(this.configPath);
		}

		foreach (var extension in ConfigExtensions)
		{
			var oldConfig =
				Path.Combine(Paths.ConfigPath, $"{DiscordConnectorPlugin.LegacyModName}-{extension}.cfg");
			var newConfig = Path.Combine(this.configPath, $"{DiscordConnectorPlugin.LegacyModName}-{extension}.cfg");
			// Main config has special handling (no -main extension on it)
			if (extension.Equals("main"))
			{
				// Main config uses no extensions
				oldConfig = Path.Combine(Paths.ConfigPath, $"{DiscordConnectorPlugin.LegacyModName}.cfg");
				newConfig = Path.Combine(this.configPath, $"{DiscordConnectorPlugin.LegacyModName}.cfg");
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
		var jsonString = "{";

		jsonString += $"\"Config.Main\":{this.mainConfig.ConfigAsJson()},";
		jsonString += $"\"Config.Messages\":{this.messagesConfig.ConfigAsJson()},";
		jsonString += $"\"Config.Toggles\":{this.togglesConfig.ConfigAsJson()},";
		jsonString += $"\"Config.Variables\":{this.variableConfig.ConfigAsJson()},";
		jsonString += $"\"Config.LeaderBoard\":{this.leaderBoardConfig.ConfigAsJson()},";
		jsonString += $"\"Config.ExtraWebhooks\":{this.extraWebhookConfig.ConfigAsJson()}";

		jsonString += "}";

		Task.Run(() =>
		{
			var configDump = Path.Combine(this.configPath, ConfigJsonFilename);
			File.WriteAllText(configDump, jsonString);
			DiscordConnectorPlugin.StaticLogger.LogDebug($"Dumped configuration files to {ConfigJsonFilename}");
		});
	}
}
