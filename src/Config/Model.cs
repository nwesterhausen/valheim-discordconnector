
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
#nullable enable
    /// <summary>
    /// Default username to use for webhooks
    /// </summary>
    public string? Username { get; set; } = string.Empty;
    /// <summary>
    /// Default avatar URL to use for webhooks
    /// </summary>
    public string? Avatar { get; set; } = string.Empty;
    /// <summary>
    /// Default mentions configuration to use for messages sent to Discord
    /// </summary>
    public MentionsConfig? Mentions { get; set; } = new MentionsConfig();
#nullable restore
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
#nullable enable
    /// <summary>
    /// A username to use for this webhook. If not provided, the default username for the webhook endpoint will be used.
    /// </summary>
    public string? Username { get; set; } = string.Empty;
    /// <summary>
    /// An avatar URL to use for this webhook. If not provided, the default avatar for the webhook endpoint will be used.
    /// </summary>
    public string? Avatar { get; set; } = string.Empty;
    /// <summary>
    /// A mentions configuration to use for this webhook. If not provided, the default mentions configuration for the webhook endpoint will be used.
    /// </summary>
    public MentionsConfig? Mentions { get; set; }
#nullable restore
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

}
