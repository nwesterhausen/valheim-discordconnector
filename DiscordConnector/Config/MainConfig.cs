using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

using BepInEx.Configuration;

namespace DiscordConnector.Config;

internal class MainConfig
{
    /// <summary>
    ///     Allowed methods for differentiating between players on the server
    /// </summary>
    public enum RetrievalDiscernmentMethods
    {
        [Description(RetrieveBySteamID)] PlayerId,
        [Description(RetrieveByName)] Name,

        [Description(RetrieveByNameAndSteamID)]
        NameAndPlayerId
    }

    private const string RetrieveBySteamID = "PlayerId: Treat each PlayerId as a separate player";

    private const string RetrieveByNameAndSteamID =
        "NameAndPlayerId: Treat each [PlayerId:CharacterName] combo as a separate player";

    private const string RetrieveByName = "Name: Treat each CharacterName as a separate player";

    private const string MainSettings = "Main Settings";
    private const string EmbedConfigSettings = "Settings - Embed Configuration";
    private const string EmbedStylingSettings = "Settings - Embed Styling";
    private static List<string> s_mutedPlayers = [];
    private readonly List<string> _embedFieldDisplayOrderList;
    private static Regex s_mutedPlayersRegex = new(@"a^");
    private readonly ConfigEntry<string> _allowedRoleMentions;
    private readonly ConfigEntry<string> _allowedUserMentions;
    private readonly ConfigEntry<bool> _allowMentionsAnyRole;
    private readonly ConfigEntry<bool> _allowMentionsAnyUser;
    private readonly ConfigEntry<bool> _allowMentionsHereEveryone;
    private readonly ConfigEntry<bool> _allowNonPlayerShoutLogging;
    private readonly ConfigEntry<bool> _announcePlayerFirsts;
    private readonly ConfigEntry<bool> _collectStatsToggle;

    // Main Settings
    private readonly ConfigEntry<string> _defaultWebhookUsernameOverride;
    private readonly ConfigEntry<bool> _discordEmbedMessagesToggle;
    private readonly ConfigEntry<string> _embedActivePlayersColor;
    private readonly ConfigEntry<string> _embedAuthorIconUrl;
    private readonly ConfigEntry<bool> _embedAuthorToggle;
    private readonly ConfigEntry<string> _embedDeathEventColor;
    private readonly ConfigEntry<string> _embedDefaultColor;
    private readonly ConfigEntry<bool> _embedDescriptionToggle;
    private readonly ConfigEntry<string> _embedFieldDisplayOrder;
    private readonly ConfigEntry<string> _embedFooterText;
    private readonly ConfigEntry<bool> _embedFooterToggle;
    private readonly ConfigEntry<string> _embedLeaderboardEmbedColor;
    private readonly ConfigEntry<string> _embedNewDayColor;
    private readonly ConfigEntry<string> _embedOtherEventColor;
    private readonly ConfigEntry<string> _embedPlayerJoinColor;
    private readonly ConfigEntry<string> _embedPlayerLeaveColor;
    private readonly ConfigEntry<string> _embedPositionMessageColor;
    private readonly ConfigEntry<string> _embedServerSaveColor;
    private readonly ConfigEntry<string> _embedServerStartColor;
    private readonly ConfigEntry<string> _embedServerStopColor;
    private readonly ConfigEntry<string> _embedShoutMessageColor;
    private readonly ConfigEntry<bool> _embedThumbnailToggle;
    private readonly ConfigEntry<string> _embedThumbnailUrl;
    private readonly ConfigEntry<bool> _embedTimestampToggle;

    // Embed Configuration Settings
    private readonly ConfigEntry<bool> _embedTitleToggle;
    private readonly ConfigEntry<string> _embedUrlTemplate;
    private readonly ConfigEntry<string> _embedWorldEventColor;
    private readonly ConfigEntry<bool> _logDebugMessages;
    private readonly ConfigEntry<string> _mutedDiscordUserList;
    private readonly ConfigEntry<string> _mutedDiscordUserListRegex;
    private readonly ConfigEntry<RetrievalDiscernmentMethods> _playerLookupPreference;

    private readonly ConfigEntry<bool> _sendPositionsToggle;
    private readonly ConfigEntry<bool> _showPlayerIdsToggle;
    private readonly ConfigEntry<string> _webhook2AvatarOverride;
    private readonly ConfigEntry<string> _webhook2Events;
    private readonly ConfigEntry<string> _webhook2UsernameOverride;
    private readonly ConfigEntry<string> _webhookAvatarOverride;
    private readonly ConfigEntry<string> _webhookEvents;
    private readonly ConfigEntry<string> _webhookUrl;
    private readonly ConfigEntry<string> _webhookUrl2;
    private readonly ConfigEntry<string> _webhookUsernameOverride;

    /// <summary>
    ///     Creates a new MainConfig object with the given config file.
    /// </summary>
    public MainConfig(ConfigFile configFile)
    {
        _defaultWebhookUsernameOverride = configFile.Bind<string>(MainSettings,
            "Default Webhook Username Override",
            "",
            "Override the username of all webhooks for this instance of Discord Connector. If left blank, the webhook will use the default name (assigned by Discord)." +
            Environment.NewLine +
            "This setting will be used for all webhooks unless overridden by a specific webhook username override setting.");

        _webhookUrl = configFile.Bind<string>(MainSettings,
            "Webhook URL",
            "",
            "Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of " +
            Environment.NewLine +
            "Discord's documentation: https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook");

        _webhookEvents = configFile.Bind<string>(MainSettings,
            "Webhook Events",
            "ALL",
            "Specify a subset of possible events to send to the primary webhook. Previously all events would go to the primary webhook." +
            Environment.NewLine +
            "Format should be the keyword 'ALL' or a semi-colon separated list, e.g. 'serverLifecycle;playerAll;playerFirstAll;leaderboardsAll;'" +
            Environment.NewLine +
            "Full list of valid options here: https://discord-connector.valheim.games.nwest.one/config/main.html#webhook-events");

        _webhookUsernameOverride = configFile.Bind<string>(MainSettings,
            "Webhook Username Override",
            "",
            "Override the username of the webhook. If left blank, the webhook will use the default name.");

        _webhookAvatarOverride = configFile.Bind<string>(MainSettings,
            "Webhook Avatar Override",
            "",
            "Override the avatar of the primary webhook with the image at this URL." + Environment.NewLine +
            "If left blank, the webhook will use the avatar set in your Discord server's settings.");

        _webhookUrl2 = configFile.Bind<string>(MainSettings,
            "Secondary Webhook URL",
            "",
            "Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of " +
            Environment.NewLine +
            "Discord's documentation: https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook");

        _webhook2Events = configFile.Bind<string>(MainSettings,
            "Secondary Webhook Events",
            "ALL",
            "Specify a subset of possible events to send to the secondary webhook." + Environment.NewLine +
            "Format should be the keyword 'ALL' or a semi-colon separated list, e.g. 'serverLaunch;serverStart;serverSave;'" +
            Environment.NewLine +
            "Full list of valid options here: https://discord-connector.valheim.games.nwest.one/config/main.html#webhook-events");

        _webhook2UsernameOverride = configFile.Bind<string>(MainSettings,
            "Secondary Webhook Username Override",
            "",
            "Override the username of the secondary webhook." + Environment.NewLine +
            "If left blank, the webhook will use the default username set in the main config.");

        _webhook2AvatarOverride = configFile.Bind<string>(MainSettings,
            "Secondary Webhook Avatar Override",
            "",
            "Override the avatar of the secondary webhook with the image at this URL." + Environment.NewLine +
            "If left blank, the webhook will use the avatar set in your Discord server's settings.");

        _logDebugMessages = configFile.Bind(MainSettings,
            "Log Debug Messages",
            false,
            "Enable this setting to listen to debug messages from the mod. This will help with troubleshooting issues.");

        _discordEmbedMessagesToggle = configFile.Bind(MainSettings,
            "Use fancier discord messages",
            false,
            "Enable this setting to use embeds in the messages sent to Discord.");

        // Embed Field Visibility Configuration
        _embedTitleToggle = configFile.Bind(EmbedConfigSettings,
            "Show Embed Title",
            true,
            "Enable this setting to show the title field in Discord embeds.");

        _embedDescriptionToggle = configFile.Bind(EmbedConfigSettings,
            "Show Embed Description",
            true,
            "Enable this setting to show the description field in Discord embeds.");

        _embedAuthorToggle = configFile.Bind(EmbedConfigSettings,
            "Show Embed Author",
            true,
            "Enable this setting to show the author field in Discord embeds. This typically displays the server or player name.");

        _embedThumbnailToggle = configFile.Bind(EmbedConfigSettings,
            "Show Embed Thumbnail",
            true,
            "Enable this setting to show a thumbnail image in Discord embeds. This appears in the top-right of the embed.");

        _embedFooterToggle = configFile.Bind(EmbedConfigSettings,
            "Show Embed Footer",
            true,
            "Enable this setting to show the footer text in Discord embeds.");

        _embedTimestampToggle = configFile.Bind(EmbedConfigSettings,
            "Show Embed Timestamp",
            true,
            "Enable this setting to show a timestamp in Discord embeds.");

        _embedAuthorIconUrl = configFile.Bind(EmbedConfigSettings,
            "Author Icon URL",
            "https://discord-connector.valheim.games.nwest.one/embed/author_icon.png",
            "The URL for the small icon (32x32px) that appears next to the author name in Discord embeds.");

        _embedThumbnailUrl = configFile.Bind(EmbedConfigSettings,
            "Thumbnail URL",
            "https://discord-connector.valheim.games.nwest.one/embed/thumbnail.png",
            "The URL for the larger thumbnail image (ideally 256x256px) that appears in the top-right of Discord embeds.");

        // Embed Color Configuration
        _embedDefaultColor = configFile.Bind(EmbedStylingSettings,
            "Default Embed Color",
            "#7289DA",
            "The default color for embeds when no specific color is defined. Use hex color format (e.g., #7289DA for Discord Blurple).");

        _embedServerStartColor = configFile.Bind(EmbedStylingSettings,
            "Server Start Color",
            "#43B581",
            "The color for server start/launch event embeds. Use hex color format (e.g., #43B581 for a green shade).");

        _embedServerStopColor = configFile.Bind(EmbedStylingSettings,
            "Server Stop Color",
            "#F04747",
            "The color for server stop/shutdown event embeds. Use hex color format (e.g., #F04747 for a red shade).");

        _embedPlayerJoinColor = configFile.Bind(EmbedStylingSettings,
            "Player Join Color",
            "#43B581",
            "The color for player join event embeds. Use hex color format (e.g., #43B581 for a green shade).");

        _embedPlayerLeaveColor = configFile.Bind(EmbedStylingSettings,
            "Player Leave Color",
            "#FAA61A",
            "The color for player leave event embeds. Use hex color format (e.g., #FAA61A for an orange shade).");

        _embedDeathEventColor = configFile.Bind(EmbedStylingSettings,
            "Death Event Color",
            "#F04747",
            "The color for player death event embeds. Use hex color format (e.g., #F04747 for a red shade).");

        _embedShoutMessageColor = configFile.Bind(EmbedStylingSettings,
            "Shout Message Color",
            "#7289DA",
            "The color for player shout message embeds. Use hex color format (e.g., #7289DA for Discord Blurple).");

        _embedOtherEventColor = configFile.Bind(EmbedStylingSettings,
            "Other Event Color",
            "#747F8D",
            "The color for other miscellaneous event embeds. Use hex color format (e.g., #747F8D for a neutral gray).");

        _embedWorldEventColor = configFile.Bind(EmbedStylingSettings,
            "World Event Color",
            "#8B5CF6",
            "The color for world event embeds (e.g., forest events, raids). Use hex color format (e.g., #8B5CF6 for a purple shade).");

        _embedNewDayColor = configFile.Bind(EmbedStylingSettings,
            "New Day Color",
            "#FFD700",
            "The color for new day event embeds. Use hex color format (e.g., #FFD700 for a gold shade).");

        _embedServerSaveColor = configFile.Bind(EmbedStylingSettings,
            "Server Save Color",
            "#1D8BF1",
            "The color for server save event embeds. Use hex color format (e.g., #1D8BF1 for a vibrant blue shade).");

        _embedPositionMessageColor = configFile.Bind(EmbedStylingSettings,
            "Position Message Color",
            "#3498DB",
            "The color for position message embeds. Use hex color format (e.g., #3498DB for a bright blue shade).");

        _embedActivePlayersColor = configFile.Bind(EmbedStylingSettings,
            "Active Players Color",
            "#4B84FF",
            "The color for active player announcement embeds. Use hex color format (e.g., #4B84FF for a vibrant blue shade).");

        _embedLeaderboardEmbedColor = configFile.Bind(EmbedStylingSettings,
            "Leaderboard Embed Color",
            "#9B59B6",
            "The color for leaderboard announcement embeds. Use hex color format (e.g., #9B59B6 for a purple shade).");

        // Other Embed Customization
        _embedFooterText = configFile.Bind(EmbedConfigSettings,
            "Footer Text",
            "Valheim Server | {worldName}",
            "The text to display in the embed footer. You can use variables like {worldName}, {serverName}, and {timestamp}.");

        _embedFieldDisplayOrder = configFile.Bind(EmbedConfigSettings,
            "Field Display Order",
            "position;event;player;details",
            "The order in which to display embed fields. Format should be a semicolon-separated list of field identifiers.");

        _embedUrlTemplate = configFile.Bind(EmbedConfigSettings,
            "Embed URL Template",
            "",
            "Optional URL template for the embed title. When set, the title becomes a clickable link. You can use variables like {worldName}, {serverName}, {playerName}.");

        _mutedDiscordUserList = configFile.Bind<string>(MainSettings,
            "Ignored Players",
            "",
            "It may be that you have some players that you never want to send Discord messages for. Adding a player name to this list will ignore them." +
            Environment.NewLine +
            "Format should be a semicolon-separated list: Stuart;John McJohnny;Weird-name1");

        _mutedDiscordUserListRegex = configFile.Bind<string>(MainSettings,
            "Ignored Players (Regex)",
            "",
            "It may be that you have some players that you never want to send Discord messages for. This option lets you provide a regular expression to filter out players if their name matches." +
            Environment.NewLine +
            "Format should be a valid string for a .NET Regex (reference: https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference)");

        _sendPositionsToggle = configFile.Bind(MainSettings,
            "Send Positions with Messages",
            true,
            "Disable this setting to disable any positions/coordinates being sent with messages (e.g. players deaths or players joining/leaving). (Overwrites any individual setting.)");

        _showPlayerIdsToggle = configFile.Bind(EmbedConfigSettings,
            "Show Player IDs in Messages",
            false,
            "Enable this setting to show player IDs (Steam IDs) in Discord embed messages. This can be useful for server administration but may not be desired for regular public servers.");

        _collectStatsToggle = configFile.Bind(MainSettings,
            "Collect Player Stats",
            true,
            "Disable this setting to disable all stat collection. (Overwrites any individual setting.)");

        _announcePlayerFirsts = configFile.Bind(MainSettings,
            "Announce Player Firsts",
            true,
            "Disable this setting to disable all extra announcements the first time each player does something. (Overwrites any individual setting.)");

        _playerLookupPreference = configFile.Bind(MainSettings,
            "How to discern players in Record Retrieval",
            RetrievalDiscernmentMethods.PlayerId,
            "Choose a method for how players will be separated from the results of a record query (used for statistic leader boards)." +
            Environment.NewLine +
            RetrieveByName + Environment.NewLine +
            RetrieveBySteamID + Environment.NewLine +
            RetrieveByNameAndSteamID
        );

        _allowNonPlayerShoutLogging = configFile.Bind(MainSettings,
            "Send Non-Player Shouts to Discord",
            false,
            "Enable this setting to have shouts which are performed by other mods/the server/non-players to be sent to Discord as well." +
            Environment.NewLine +
            "Note: These are still subject to censure by the muted player regex and list.");

        _allowMentionsHereEveryone = configFile.Bind(MainSettings,
            "Allow @here and @everyone mentions",
            false,
            "Enable this setting to allow messages sent to Discord to mention @here and @everyone. Per the Discord API, these share the same setting." +
            Environment.NewLine +
            "Note: There is no filtering in place to prevent abuse of these mentions (e.g. in a shout or player's name).");

        _allowMentionsAnyRole = configFile.Bind(MainSettings,
            "Allow @role mentions",
            true,
            "Enable this setting to allow messages sent to Discord to mention roles. Roles mentioned this way use the format `<@&role_id>`" +
            Environment.NewLine +
            "Note: There is no filtering in place to prevent abuse of these mentions (e.g. in a shout or player's name).");

        _allowMentionsAnyUser = configFile.Bind(MainSettings,
            "Allow @user mentions",
            true,
            "Enable this setting to allow messages sent to Discord to mention users. Users mentioned this way use the format `<@user_id>`" +
            Environment.NewLine +
            "Note: There is no filtering in place to prevent abuse of these mentions (e.g. in a shout or player's name).");

        _allowedRoleMentions = configFile.Bind<string>(MainSettings,
            "Allowed Role Mentions",
            "",
            "A semicolon-separated list of role IDs that are allowed to be mentioned in messages sent to Discord. These are just a number (no carets), e.g. `123;234`" +
            Environment.NewLine +
            "Note: This setting is overshadowed if 'Allow @role mentions` is enabled, and only when that is disabled will these roles still be allowed to be mentioned.");

        _allowedUserMentions = configFile.Bind<string>(MainSettings,
            "Allowed User Mentions",
            "",
            "A semicolon-separated list of user IDs that are allowed to be mentioned in messages sent to Discord. These are just a number (no carets), e.g. `123;234`" +
            Environment.NewLine +
            "Note: This setting is overshadowed if 'Allow @user mentions` is enabled, and only when that is disabled will these users still be allowed to be mentioned.");

        configFile.Save();

        DiscordConnectorPlugin.StaticLogger.SetLogLevel(_logDebugMessages.Value);
        // Update Muted Players
        if (string.IsNullOrEmpty(_mutedDiscordUserList.Value))
        {
            s_mutedPlayers = [];
        }
        else
        {
            s_mutedPlayers = new List<string>(_mutedDiscordUserList.Value.Split(';'));
        }

        if (string.IsNullOrEmpty(_mutedDiscordUserListRegex.Value))
        {
            s_mutedPlayersRegex = new Regex(@"a^");
        }
        else
        {
            s_mutedPlayersRegex = new Regex(_mutedDiscordUserListRegex.Value);
        }

        // Update Webhooks
        PrimaryWebhook = new WebhookEntry(_webhookUrl.Value, Webhook.StringToEventList(_webhookEvents.Value),
            whichWebhook: "Primary");
        if (!string.IsNullOrEmpty(_webhookUsernameOverride.Value))
        {
            PrimaryWebhook.UsernameOverride = _webhookUsernameOverride.Value;
        }

        if (!string.IsNullOrEmpty(_webhookAvatarOverride.Value))
        {
            PrimaryWebhook.AvatarOverride = _webhookAvatarOverride.Value;
        }

        SecondaryWebhook = new WebhookEntry(_webhookUrl2.Value, Webhook.StringToEventList(_webhook2Events.Value),
            whichWebhook: "Secondary");
        if (!string.IsNullOrEmpty(_webhook2UsernameOverride.Value))
        {
            SecondaryWebhook.UsernameOverride = _webhook2UsernameOverride.Value;
        }

        if (!string.IsNullOrEmpty(_webhook2AvatarOverride.Value))
        {
            SecondaryWebhook.AvatarOverride = _webhook2AvatarOverride.Value;
        }

        // Update Allowed Mentions
        if (string.IsNullOrEmpty(_allowedRoleMentions.Value))
        {
            AllowedRoleMentions = [];
        }
        else
        {
            AllowedRoleMentions = new List<string>(_allowedRoleMentions.Value.Split(';'));
        }

        if (string.IsNullOrEmpty(_allowedUserMentions.Value))
        {
            AllowedUserMentions = [];
        }
        else
        {
            AllowedUserMentions = new List<string>(_allowedUserMentions.Value.Split(';'));
        }

        // Update Embed Field Display Order
        if (string.IsNullOrEmpty(_embedFieldDisplayOrder.Value))
        {
            _embedFieldDisplayOrderList = ["position", "event", "player", "details"];
        }
        else
        {
            _embedFieldDisplayOrderList = new List<string>(_embedFieldDisplayOrder.Value.Split(';'));
        }
    }

    public string DefaultWebhookUsernameOverride => _defaultWebhookUsernameOverride.Value;
    public WebhookEntry PrimaryWebhook { get; }

    public WebhookEntry SecondaryWebhook { get; }

    public bool CollectStatsEnabled => _collectStatsToggle.Value;
    public bool DiscordEmbedsEnabled => _discordEmbedMessagesToggle.Value;

    // Embed Field Visibility Properties
    public bool EmbedTitleEnabled => _embedTitleToggle.Value;
    public bool EmbedDescriptionEnabled => _embedDescriptionToggle.Value;
    public bool EmbedAuthorEnabled => _embedAuthorToggle.Value;
    public bool EmbedThumbnailEnabled => _embedThumbnailToggle.Value;
    public bool EmbedFooterEnabled => _embedFooterToggle.Value;
    public bool EmbedTimestampEnabled => _embedTimestampToggle.Value;

    // Embed Color Properties
    public string EmbedDefaultColor => _embedDefaultColor.Value;
    public string EmbedServerStartColor => _embedServerStartColor.Value;
    public string EmbedServerStopColor => _embedServerStopColor.Value;
    public string EmbedPlayerJoinColor => _embedPlayerJoinColor.Value;
    public string EmbedPlayerLeaveColor => _embedPlayerLeaveColor.Value;
    public string EmbedDeathEventColor => _embedDeathEventColor.Value;
    public string EmbedShoutMessageColor => _embedShoutMessageColor.Value;
    public string EmbedOtherEventColor => _embedOtherEventColor.Value;
    public string EmbedWorldEventColor => _embedWorldEventColor.Value;
    public string EmbedNewDayColor => _embedNewDayColor.Value;
    public string EmbedServerSaveColor => _embedServerSaveColor.Value;
    public string EmbedPositionMessageColor => _embedPositionMessageColor.Value;
    public string EmbedActivePlayersColor => _embedActivePlayersColor.Value;
    public string EmbedLeaderboardEmbedColor => _embedLeaderboardEmbedColor.Value;

    // Other Embed Customization Properties
    public string EmbedFooterText => _embedFooterText.Value;
    public List<string> EmbedFieldDisplayOrderList => _embedFieldDisplayOrderList;
    public string EmbedUrlTemplate => _embedUrlTemplate.Value;
    public string EmbedAuthorIconUrl => _embedAuthorIconUrl.Value;
    public string EmbedThumbnailUrl => _embedThumbnailUrl.Value;
    public bool SendPositionsEnabled => _sendPositionsToggle.Value;
    public bool ShowPlayerIds => _showPlayerIdsToggle.Value;
    public List<string> MutedPlayers => s_mutedPlayers;
    public Regex MutedPlayersRegex => s_mutedPlayersRegex;
    public bool AnnouncePlayerFirsts => _announcePlayerFirsts.Value;
    public RetrievalDiscernmentMethods RecordRetrievalDiscernmentMethod => _playerLookupPreference.Value;
    public bool AllowNonPlayerShoutLogging => _allowNonPlayerShoutLogging.Value;
    public bool AllowMentionsHereEveryone => _allowMentionsHereEveryone.Value;
    public bool AllowMentionsAnyRole => _allowMentionsAnyRole.Value;
    public bool AllowMentionsAnyUser => _allowMentionsAnyUser.Value;
    public List<string> AllowedRoleMentions { get; }

    public List<string> AllowedUserMentions { get; }

    // Color conversion utility methods
    public int GetColorDecimal(string hexColor)
    {
        if (string.IsNullOrEmpty(hexColor) || (!hexColor.StartsWith("#") && hexColor.Length != 7))
        {
            // Return a default Discord blurple color if invalid
            return 7506394; // #7289DA
        }

        try
        {
            // Remove the # character and parse as hex
            string colorHex = hexColor.TrimStart('#');
            if (int.TryParse(colorHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int colorValue))
            {
                return colorValue;
            }

            return 7506394; // Default if parsing fails
        }
        catch
        {
            // Return default on any exception
            return 7506394;
        }
    }

    // Get color for specific event types
    public string GetEventColor(Webhook.Event eventType)
    {
        return eventType switch
        {
            Webhook.Event.ServerLaunch or Webhook.Event.ServerStart => EmbedServerStartColor,
            Webhook.Event.ServerStop or Webhook.Event.ServerShutdown => EmbedServerStopColor,
            Webhook.Event.PlayerJoin or Webhook.Event.PlayerFirstJoin => EmbedPlayerJoinColor,
            Webhook.Event.PlayerLeave or Webhook.Event.PlayerFirstLeave => EmbedPlayerLeaveColor,
            Webhook.Event.PlayerDeath or Webhook.Event.PlayerFirstDeath => EmbedDeathEventColor,
            Webhook.Event.PlayerShout or Webhook.Event.PlayerFirstShout => EmbedShoutMessageColor,
            Webhook.Event.NewDayNumber => EmbedNewDayColor,
            Webhook.Event.ServerSave => EmbedServerSaveColor,
            Webhook.Event.EventStart or Webhook.Event.EventStop => EmbedWorldEventColor,
            _ => EmbedOtherEventColor
        };
    }

    // Validate a hex color code
    public bool IsValidHexColor(string color)
    {
        if (string.IsNullOrEmpty(color) || !color.StartsWith("#") || color.Length != 7)
        {
            return false;
        }

        return int.TryParse(color.TrimStart('#'), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _);
    }

    public string ConfigAsJson()
    {
        string jsonString = "{";
        jsonString += "\"discord\":{";
        jsonString += $"\"defaultWebhookUsernameOverride\":\"{_defaultWebhookUsernameOverride.Value}\",";
        jsonString += $"\"webhook\":\"{(string.IsNullOrEmpty(_webhookUrl.Value) ? "unset" : "REDACTED")}\",";
        jsonString += $"\"webhookEvents\":\"{_webhookEvents.Value}\",";
        jsonString += $"\"webhookUsernameOverride\":\"{_webhookUsernameOverride.Value}\",";
        jsonString += $"\"webhookAvatarOverride\":\"{_webhookAvatarOverride.Value}\",";
        jsonString += $"\"webhook2\":\"{(string.IsNullOrEmpty(_webhookUrl2.Value) ? "unset" : "REDACTED")}\",";
        jsonString += $"\"webhook2Events\":\"{_webhook2Events.Value}\",";
        jsonString += $"\"webhook2UsernameOverride\":\"{_webhook2UsernameOverride.Value}\",";
        jsonString += $"\"webhook2AvatarOverride\":\"{_webhook2AvatarOverride.Value}\",";
        jsonString += $"\"logDebugMessages\":\"{_logDebugMessages.Value}\",";
        jsonString += $"\"fancierMessages\":\"{DiscordEmbedsEnabled}\",";

        // Add embed configuration to JSON
        jsonString += "\"embedConfig\":{";
        jsonString += $"\"titleEnabled\":\"{EmbedTitleEnabled}\",";
        jsonString += $"\"descriptionEnabled\":\"{EmbedDescriptionEnabled}\",";
        jsonString += $"\"embedFieldDisplayOrder\":\"{EmbedFieldDisplayOrderList}\",";
        jsonString += $"\"authorEnabled\":\"{EmbedAuthorEnabled}\",";
        jsonString += $"\"thumbnailEnabled\":\"{EmbedThumbnailEnabled}\",";
        jsonString += $"\"footerEnabled\":\"{EmbedFooterEnabled}\",";
        jsonString += $"\"timestampEnabled\":\"{EmbedTimestampEnabled}\",";
        jsonString += $"\"defaultColor\":\"{EmbedDefaultColor}\",";
        jsonString += $"\"serverStartColor\":\"{EmbedServerStartColor}\",";
        jsonString += $"\"serverStopColor\":\"{EmbedServerStopColor}\",";
        jsonString += $"\"playerJoinColor\":\"{EmbedPlayerJoinColor}\",";
        jsonString += $"\"playerLeaveColor\":\"{EmbedPlayerLeaveColor}\",";
        jsonString += $"\"deathEventColor\":\"{EmbedDeathEventColor}\",";
        jsonString += $"\"shoutMessageColor\":\"{EmbedShoutMessageColor}\",";
        jsonString += $"\"otherEventColor\":\"{EmbedOtherEventColor}\",";
        jsonString += $"\"worldEventColor\":\"{EmbedWorldEventColor}\",";
        jsonString += $"\"newDayColor\":\"{EmbedNewDayColor}\",";
        jsonString += $"\"serverSaveColor\":\"{EmbedServerSaveColor}\",";
        jsonString += $"\"positionMessageColor\":\"{EmbedPositionMessageColor}\",";
        jsonString += $"\"activePlayersColor\":\"{EmbedActivePlayersColor}\",";
        jsonString += $"\"leaderboardEmbedColor\":\"{EmbedLeaderboardEmbedColor}\",";
        jsonString += $"\"footerText\":\"{EmbedFooterText}\",";
        jsonString += $"\"urlTemplate\":\"{EmbedUrlTemplate}\",";
        jsonString += $"\"authorIconUrl\":\"{EmbedAuthorIconUrl}\",";
        jsonString += $"\"thumbnailUrl\":\"{EmbedThumbnailUrl}\"";
        jsonString += "},";
        jsonString += $"\"ignoredPlayers\":\"{_mutedDiscordUserList.Value}\",";
        jsonString += "\"ignoredPlayersList\":[";
        for (int i = 0; i < s_mutedPlayers.Count; i++)
        {
            jsonString += $"\"{s_mutedPlayers[i]}\"";
            if (i < s_mutedPlayers.Count - 1)
            {
                jsonString += ",";
            }
        }

        jsonString += "],";
        jsonString += $"\"ignoredPlayersRegex\":\"{_mutedDiscordUserListRegex.Value}\",";
        jsonString += $"\"allowMentionsHereEveryone\":\"{_allowMentionsHereEveryone.Value}\",";
        jsonString += $"\"allowMentionsAnyRole\":\"{_allowMentionsAnyRole.Value}\",";
        jsonString += $"\"allowMentionsAnyUser\":\"{_allowMentionsAnyUser.Value}\",";
        jsonString += $"\"allowedRoleMentions\":\"{_allowedRoleMentions.Value}\",";
        jsonString += "\"allowedRoleMentionsList\":[";
        for (int i = 0; i < AllowedRoleMentions.Count; i++)
        {
            jsonString += $"\"{AllowedRoleMentions[i]}\"";
            if (i < AllowedRoleMentions.Count - 1)
            {
                jsonString += ",";
            }
        }

        jsonString += "],";
        jsonString += $"\"allowedUserMentions\":\"{_allowedUserMentions.Value}\",";
        jsonString += "\"allowedUserMentionsList\":[";
        for (int i = 0; i < AllowedUserMentions.Count; i++)
        {
            jsonString += $"\"{AllowedUserMentions[i]}\"";
            if (i < AllowedUserMentions.Count - 1)
            {
                jsonString += ",";
            }
        }

        jsonString += "]";
        jsonString += "},";
        jsonString += $"\"collectStatsEnabled\":\"{CollectStatsEnabled}\",";
        jsonString += $"\"sendPositionsEnabled\":\"{SendPositionsEnabled}\",";
        jsonString += $"\"announcePlayerFirsts\":\"{AnnouncePlayerFirsts}\",";
        jsonString += $"\"playerLookupPreference\":\"{RecordRetrievalDiscernmentMethod}\"";
        jsonString += "}";
        return jsonString;
    }
}
