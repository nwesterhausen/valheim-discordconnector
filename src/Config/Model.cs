
using System.Collections.Generic;

namespace DiscordConnector.Config;

internal class Model
{
  public const string RetrieveBySteamID = "PlayerId: Treat each PlayerId as a separate player";
  public const string RetrieveByNameAndSteamID = "NameAndPlayerId: Treat each [PlayerId:CharacterName] combo as a separate player";
  public const string RetrieveByName = "Name: Treat each CharacterName as a separate player";
  /// <summary>
  /// Allowed methods for differentiating between players on the server
  /// </summary>
  public enum RetrievalDiscernmentMethods
  {
    [System.ComponentModel.Description(RetrieveBySteamID)]
    PlayerId,
    [System.ComponentModel.Description(RetrieveByName)]
    Name,
    [System.ComponentModel.Description(RetrieveByNameAndSteamID)]
    NameAndPlayerId,
  }
  /// <summary>
  /// Plugin configuration
  /// </summary>
  public class Config
  {
    /// <summary>
    /// Whether to enable collecting stats (player's join/leave/shout/death/ping)
    /// </summary>
    public bool CollectStats { get; set; } = true;
    /// <summary>
    /// Whether to enable sending player positions to Discord
    /// </summary>
    public bool SendPositions { get; set; } = true;
    /// <summary>
    /// Whether to enable sending a message to Discord for a player's first event
    /// </summary>
    public bool AnnouncePlayerFirsts { get; set; } = true;
    /// <summary>
    /// Whether to log debug messages to the INFO channel
    /// </summary>
    public bool LogDebugMessages { get; set; } = false;
    /// <summary>
    /// How to discern between different players on the server.
    /// </summary>
    public RetrievalDiscernmentMethods RetrievalDiscernmentMethod { get; set; } = RetrievalDiscernmentMethods.PlayerId;
    /// <summary>
    /// Configuration for Discord integration
    /// </summary>
    public DiscordConfig Discord { get; set; } = new DiscordConfig();
    /// <summary>
    /// Configuration for messages that can be sent to Discord
    /// </summary>
    public Messages Messages { get; set; } = new Messages();
    /// <summary>
    /// Debug options
    /// </summary>
    public DebugOptions Debug { get; set; } = new DebugOptions();
  }
  /// <summary>
  /// Configuration for Discord integration
  /// </summary>
  public class DiscordConfig
  {
    /// <summary>
    /// Webhook endpoints that can be used by the utility to send messages to Discord. Referenced by name when defining webhooks.
    /// </summary>
    public List<WebhookEndpoint> Endpoints { get; set; } = [];
    /// <summary>
    /// Webhooks that can be used to send messages to Discord.
    /// </summary>
    public List<WebhookConfig> Webhooks { get; set; } = [];
    /// <summary>
    /// Whether to send "fancier" messages to Discord. This doesn't work quite right.
    /// </summary>
    public bool FancierMessages { get; set; } = false;
    /// <summary>
    /// Default values to use for Discord webhook configuration.
    /// </summary>
    public DiscordDefaults defaults { get; set; } = new DiscordDefaults();
    /// <summary>
    /// A list of player names to ignore. If a player's name is in this list, they will be ignored.
    /// </summary>
    public List<string> IgnoredPlayers { get; set; } = [];
    /// <summary>
    /// A regex pattern to match against player names. If a player's name matches this pattern, they will be ignored.
    /// </summary>
    public string IgnoredPlayersRegex { get; set; } = string.Empty;
  }
  /// <summary>
  /// Default values to use for Discord webhook configuration.
  /// </summary>
  public class DiscordDefaults
  {
    /// <summary>
    /// Default username to use for webhooks
    /// </summary>
    public string Username { get; set; } = string.Empty;
    /// <summary>
    /// Default avatar URL to use for webhooks
    /// </summary>
    public string Avatar { get; set; } = string.Empty;
    /// <summary>
    /// Default mentions configuration to use for messages sent to Discord
    /// </summary>
    public MentionsConfig Mentions { get; set; } = new MentionsConfig();
  }
  /// <summary>
  /// Webhook endpoint configuration. Used to provide a reference to a webhook URL.
  /// </summary>
  public class WebhookEndpoint
  {
    /// <summary>
    /// URL endpoint for the webhook
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// Name of the webhook (used to reference the webhook in the configuration)
    /// </summary>
    public string Name { get; set; }
  }
  /// <summary>
  /// Webhook configuration. Used to define a webhook that can be used to send messages to Discord.
  /// </summary>
  public class WebhookConfig
  {
    /// <summary>
    /// Which webhook endpoint to use for this webhook
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;
    /// <summary>
    /// Which events to send to the webhook
    /// </summary>
    public List<Webhook.Event> Events { get; set; } = [Webhook.Event.ALL];
    /// <summary>
    /// A username to use for this webhook. If not provided, the default username for the webhook endpoint will be used.
    /// </summary>
    public string Username { get; set; } = string.Empty;
    /// <summary>
    /// An avatar URL to use for this webhook. If not provided, the default avatar for the webhook endpoint will be used.
    /// </summary>
    public string Avatar { get; set; } = string.Empty;
    /// <summary>
    /// A mentions configuration to use for this webhook. If not provided, the default mentions configuration for the webhook endpoint will be used.
    /// </summary>
    public MentionsConfig Mentions { get; set; } = new MentionsConfig();
  }
  /// <summary>
  /// What kinds of mentions are allowed in messages sent to Discord.
  /// </summary>
  public class MentionsConfig
  {
    /// <summary>
    /// Allow mentions of `@everyone` and `@here`
    /// </summary>
    public bool HereAndEveryone { get; set; } = false;
    /// <summary>
    /// Allow mentions of any role
    /// </summary>
    public bool AnyRole { get; set; } = false;
    /// <summary>
    /// Allow mentions of any user
    /// </summary>
    public bool AnyUser { get; set; } = false;
    /// <summary>
    /// Roles that are allowed to be mentioned. This supersedes `AnyRole`.
    /// </summary>
    public List<string> AllowedRoles { get; set; } = [];
    /// <summary>
    /// Users that are allowed to be mentioned. This supersedes `AnyUser`.
    /// </summary>
    public List<string> AllowedUsers { get; set; } = [];
  }
  /// <summary>
  /// Message configuration details. Includes a list of possible messages to send and whether to send the message.
  /// </summary>
  public class MessageConfig
  {
    /// <summary>
    /// Whether to send this message to Discord
    /// </summary>
    public bool Enabled { get; set; } = true;
    /// <summary>
    /// Whether to include the player's position in the message. In a ping, this will be the pinged position.
    /// </summary>
    public bool IncludePosition { get; set; } = false;
    /// <summary>
    /// List of possible messages to send. One will be chosen at random.
    /// </summary>
    public List<string> Messages { get; set; } = [];
    /// <summary>
    /// Whether to send a special additional message for the first time this event occurs.
    /// </summary>
    public bool FirstTimeMessageEnabled { get; set; } = false;
    /// <summary>
    /// List of possible messages to send for the first time this event occurs. One will be chosen at random.
    /// </summary>
    public List<string> FirstTimeMessages { get; set; } = [];
  }
  /// <summary>
  /// Configuration for messages that can be sent to Discord
  /// </summary>
  public class Messages
  {
    /// <summary>
    /// Message configuration for a server launch event, when the server is starting up.
    /// </summary>
    public MessageConfig ServerLaunch { get; set; } = new()
    {
      Messages = ["Server is starting up!"],
      Enabled = true
    };
    /// <summary>
    /// Message configuration for a server start event, when the server has started (and is ready to accept connections).
    /// </summary>
    public MessageConfig ServerStart { get; set; } = new()
    {
      Messages = ["Server has started!"],
      Enabled = true
    };
    /// <summary>
    /// Message configuration for a server stop event, when the server is stopping.
    /// </summary>
    public MessageConfig ServerStop { get; set; } = new()
    {
      Messages = ["Server is stopping!"],
      Enabled = true
    };
    /// <summary>
    /// Message configuration for a server shutdown event, when the server has stopped.
    /// </summary>
    public MessageConfig ServerShutdown { get; set; } = new()
    {
      Messages = ["Server has stopped!"],
      Enabled = true
    };
    /// <summary>
    /// Message configuration for a server save event, when the server is saving the world.
    /// </summary>
    public MessageConfig ServerSave { get; set; } = new()
    {
      Messages = ["The world has been saved!"],
      Enabled = true
    };
    /// <summary>
    /// Message configuration for a new day event, when a new day begins.
    /// </summary>
    public MessageConfig NewDayMessage { get; set; } = new()
    {
      Messages = ["Day %DAY_NUMBER% Begins!"],
      Enabled = true
    };
    /// <summary>
    /// Message configuration for when a player joins the server.
    /// </summary>
    public MessageConfig PlayerJoin { get; set; } = new()
    {
      Messages = ["%PLAYER_NAME% has joined the server! Online now: %NUM_PLAYERS%"],
      Enabled = true,
      FirstTimeMessages = ["%PLAYER_NAME% has joined the server for the first time!"],
      FirstTimeMessageEnabled = true
    };
    /// <summary>
    /// Message configuration for when a player leaves the server.
    /// </summary>
    public MessageConfig PlayerLeave { get; set; } = new()
    {
      Messages = ["%PLAYER_NAME% has left the server! Online now: %NUM_PLAYERS%"],
      Enabled = true,
      FirstTimeMessages = ["%PLAYER_NAME% has left the server for the first time!"],
      FirstTimeMessageEnabled = false
    };
    /// <summary>
    /// Message configuration for when a player dies.
    /// </summary>
    public MessageConfig PlayerDeath { get; set; } = new()
    {
      Messages = ["%PLAYER_NAME% has died!"],
      Enabled = true,
      FirstTimeMessages = ["%PLAYER_NAME% has died for the first time!"],
      FirstTimeMessageEnabled = true,
      IncludePosition = true
    };
    /// <summary>
    /// Message configuration for when a player shouts.
    /// </summary>
    public MessageConfig PlayerShout { get; set; } = new()
    {
      Messages = ["%PLAYER_NAME% shouts %SHOUT%"],
      Enabled = true,
      FirstTimeMessages = ["%PLAYER_NAME% shouts for the first time!"],
      FirstTimeMessageEnabled = false
    };
    /// <summary>
    /// Message configuration for when a player pings the map.
    /// </summary>
    public MessageConfig PlayerPing { get; set; } = new()
    {
      Messages = ["%PLAYER_NAME% has pinged the map."],
      Enabled = true,
      FirstTimeMessages = ["%PLAYER_NAME% has pinged the map for the first time."],
      FirstTimeMessageEnabled = true,
      IncludePosition = true
    };
    /// <summary>
    /// Message configuration for when an event starts.
    /// </summary>
    public MessageConfig EventStart { get; set; } = new()
    {
      Messages = ["**Event**: %EVENT_MSG%"],
      Enabled = true
    };
    /// <summary>
    /// Message configuration for when an event stops.
    /// </summary>
    public MessageConfig EventStop { get; set; } = new()
    {
      Messages = ["**Event**: %EVENT_MSG%"],
      Enabled = true
    };
    /// <summary>
    /// Message configuration for when an event is paused.
    /// </summary>
    public MessageConfig EventPaused { get; set; } = new()
    {
      Messages = ["**Event**: %EVENT_END_MSG% — for now! (Currently paused due to no players in the event area.)"],
      Enabled = true
    };
    /// <summary>
    /// Message configuration for when an event is resumed.
    /// </summary>
    public MessageConfig EventResumed { get; set; } = new()
    {
      Messages = ["**Event**: %EVENT_START_MSG%"],
      Enabled = true
    };
    /// <summary>
    /// Leaderboard title configuration for the top players.
    /// </summary>
    public MessageConfig LeaderboardTopPlayers { get; set; } = new()
    {
      Messages = ["Top %N% Player Leaderboards:"],
      Enabled = true
    };
    /// <summary>
    /// Leaderboard title configuration for the bottom players.
    /// </summary>
    public MessageConfig LeaderboardBottomPlayers { get; set; } = new()
    {
      Messages = ["Bottom %N% Faction Leaderboards:"],
      Enabled = true
    };
    /// <summary>
    /// Leaderboard title configuration for the top factions.
    /// </summary>
    public MessageConfig LeaderboardHighestPlayer { get; set; } = new()
    {
      Messages = ["Top Performer"],
      Enabled = true
    };
    /// <summary>
    /// Leaderboard title configuration for the bottom factions.
    /// </summary>
    public MessageConfig LeaderboardLowestPlayer { get; set; } = new()
    {
      Messages = ["Bottom Performer"],
      Enabled = true
    };
  }
  /// <summary>
  /// Configuration for debug options. These enable extra tracing/logging for debugging purposes. Do not enable these unless you are debugging the plugin.
  /// </summary>
  public class DebugOptions
  {
    /// <summary>
    /// Whether to log player position checks to the INFO channel
    /// </summary>
    public bool PlayerPositionChecks { get; set; } = false;
    /// <summary>
    /// Whether to log every event check to the INFO channel
    /// </summary>
    public bool EventChecks { get; set; } = false;
    /// <summary>
    /// Whether to log every event change to the INFO channel
    /// </summary>
    public bool EventChanges { get; set; } = false;
    /// <summary>
    /// Whether to enable logging for every database method call
    /// </summary>
    public bool DatabaseMethods { get; set; } = false;
    /// <summary>
    /// Whether to log every HTTP response to the INFO channel
    /// </summary>
    public bool HttpRequestResponses { get; set; } = false;
  }
}
