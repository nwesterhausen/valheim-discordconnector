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

    public const string RetrieveBySteamID = "PlayerId: Treat each PlayerId as a separate player";

    public const string RetrieveByNameAndSteamID =
        "NameAndPlayerId: Treat each [PlayerId:CharacterName] combo as a separate player";

    public const string RetrieveByName = "Name: Treat each CharacterName as a separate player";
    private const string MAIN_SETTINGS = "Main Settings";
    private const string EMBED_CONFIG_SETTINGS = "Settings - Embed Configuration";
    private const string EMBED_STYLING_SETTINGS = "Settings - Embed Styling";
    private static List<string> mutedPlayers;
    private static Regex mutedPlayersRegex;
    private readonly ConfigFile config;
    private ConfigEntry<string> allowedRoleMentions;
    private ConfigEntry<string> allowedUserMentions;
    private ConfigEntry<bool> allowMentionsAnyRole;
    private ConfigEntry<bool> allowMentionsAnyUser;
    private ConfigEntry<bool> allowMentionsHereEveryone;
    private ConfigEntry<bool> allowNonPlayerShoutLogging;
    private ConfigEntry<bool> announcePlayerFirsts;
    private ConfigEntry<bool> collectStatsToggle;

    // Main Settings
    private ConfigEntry<string> defaultWebhookUsernameOverride;
    private ConfigEntry<bool> discordEmbedMessagesToggle;
    private ConfigEntry<bool> logDebugMessages;
    private ConfigEntry<string> mutedDiscordUserList;
    private ConfigEntry<string> mutedDiscordUserListRegex;
    private ConfigEntry<RetrievalDiscernmentMethods> playerLookupPreference;
    
    // Embed Configuration Settings
    private ConfigEntry<bool> embedTitleToggle;
    private ConfigEntry<bool> embedDescriptionToggle;
    private ConfigEntry<bool> embedAuthorToggle;
    private ConfigEntry<bool> embedThumbnailToggle;
    private ConfigEntry<bool> embedFooterToggle;
    private ConfigEntry<bool> embedTimestampToggle;
    private ConfigEntry<string> embedDefaultColor;
    private ConfigEntry<string> embedServerStartColor;
    private ConfigEntry<string> embedServerStopColor;
    private ConfigEntry<string> embedPlayerJoinColor;
    private ConfigEntry<string> embedPlayerLeaveColor;
    private ConfigEntry<string> embedDeathEventColor;
    private ConfigEntry<string> embedShoutMessageColor;
    private ConfigEntry<string> embedOtherEventColor;
    private ConfigEntry<string> embedWorldEventColor;
    private ConfigEntry<string> embedNewDayColor;
    private ConfigEntry<string> embedServerSaveColor;
    private ConfigEntry<string> embedPositionMessageColor;
    private ConfigEntry<string> embedActivePlayersColor;
    private ConfigEntry<string> embedLeaderboardEmbedColor;
    private ConfigEntry<string> embedFooterText;
    private ConfigEntry<string> embedFieldDisplayOrder;
    private ConfigEntry<string> embedUrlTemplate;
    private ConfigEntry<string> embedAuthorIconUrl;
    private ConfigEntry<string> embedThumbnailUrl;

    private ConfigEntry<bool> sendPositionsToggle;
    private ConfigEntry<bool> showPlayerIdsToggle;
    private ConfigEntry<string> webhook2AvatarOverride;
    private ConfigEntry<string> webhook2Events;
    private ConfigEntry<string> webhook2UsernameOverride;
    private ConfigEntry<string> webhookAvatarOverride;
    private ConfigEntry<string> webhookEvents;
    private ConfigEntry<string> webhookUrl;
    private ConfigEntry<string> webhookUrl2;
    private ConfigEntry<string> webhookUsernameOverride;

    /// <summary>
    ///     Creates a new MainConfig object with the given config file.
    /// </summary>
    public MainConfig(ConfigFile configFile)
    {
        config = configFile;
        
        defaultWebhookUsernameOverride = config.Bind<string>(MAIN_SETTINGS,
            "Default Webhook Username Override",
            "",
            "Override the username of all webhooks for this instance of Discord Connector. If left blank, the webhook will use the default name (assigned by Discord)." +
            Environment.NewLine +
            "This setting will be used for all webhooks unless overridden by a specific webhook username override setting.");

        webhookUrl = config.Bind<string>(MAIN_SETTINGS,
            "Webhook URL",
            "",
            "Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of " +
            Environment.NewLine +
            "Discord's documentation: https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook");

        webhookEvents = config.Bind<string>(MAIN_SETTINGS,
            "Webhook Events",
            "ALL",
            "Specify a subset of possible events to send to the primary webhook. Previously all events would go to the primary webhook." +
            Environment.NewLine +
            "Format should be the keyword 'ALL' or a semi-colon separated list, e.g. 'serverLifecycle;playerAll;playerFirstAll;leaderboardsAll;'" +
            Environment.NewLine +
            "Full list of valid options here: https://discord-connector.valheim.games.nwest.one/config/main.html#webhook-events");

        webhookUsernameOverride = config.Bind<string>(MAIN_SETTINGS,
            "Webhook Username Override",
            "",
            "Override the username of the webhook. If left blank, the webhook will use the default name.");

        webhookAvatarOverride = config.Bind<string>(MAIN_SETTINGS,
            "Webhook Avatar Override",
            "",
            "Override the avatar of the primary webhook with the image at this URL." + Environment.NewLine +
            "If left blank, the webhook will use the avatar set in your Discord server's settings.");

        webhookUrl2 = config.Bind<string>(MAIN_SETTINGS,
            "Secondary Webhook URL",
            "",
            "Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of " +
            Environment.NewLine +
            "Discord's documentation: https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook");

        webhook2Events = config.Bind<string>(MAIN_SETTINGS,
            "Secondary Webhook Events",
            "ALL",
            "Specify a subset of possible events to send to the secondary webhook." + Environment.NewLine +
            "Format should be the keyword 'ALL' or a semi-colon separated list, e.g. 'serverLaunch;serverStart;serverSave;'" +
            Environment.NewLine +
            "Full list of valid options here: https://discord-connector.valheim.games.nwest.one/config/main.html#webhook-events");

        webhook2UsernameOverride = config.Bind<string>(MAIN_SETTINGS,
            "Secondary Webhook Username Override",
            "",
            "Override the username of the secondary webhook." + Environment.NewLine +
            "If left blank, the webhook will use the default username set in the main config.");

        webhook2AvatarOverride = config.Bind<string>(MAIN_SETTINGS,
            "Secondary Webhook Avatar Override",
            "",
            "Override the avatar of the secondary webhook with the image at this URL." + Environment.NewLine +
            "If left blank, the webhook will use the avatar set in your Discord server's settings.");

        logDebugMessages = config.Bind(MAIN_SETTINGS,
            "Log Debug Messages",
            false,
            "Enable this setting to listen to debug messages from the mod. This will help with troubleshooting issues.");

        discordEmbedMessagesToggle = config.Bind(MAIN_SETTINGS,
            "Use fancier discord messages",
            false,
            "Enable this setting to use embeds in the messages sent to Discord.");
            
        // Embed Field Visibility Configuration
        embedTitleToggle = config.Bind(EMBED_CONFIG_SETTINGS,
            "Show Embed Title",
            true,
            "Enable this setting to show the title field in Discord embeds.");
            
        embedDescriptionToggle = config.Bind(EMBED_CONFIG_SETTINGS,
            "Show Embed Description",
            true,
            "Enable this setting to show the description field in Discord embeds.");
            
        embedAuthorToggle = config.Bind(EMBED_CONFIG_SETTINGS,
            "Show Embed Author",
            true,
            "Enable this setting to show the author field in Discord embeds. This typically displays the server or player name.");
            
        embedThumbnailToggle = config.Bind(EMBED_CONFIG_SETTINGS,
            "Show Embed Thumbnail",
            true,
            "Enable this setting to show a thumbnail image in Discord embeds. This appears in the top-right of the embed.");
            
        embedFooterToggle = config.Bind(EMBED_CONFIG_SETTINGS,
            "Show Embed Footer",
            true,
            "Enable this setting to show the footer text in Discord embeds.");
            
        embedTimestampToggle = config.Bind(EMBED_CONFIG_SETTINGS,
            "Show Embed Timestamp",
            true,
            "Enable this setting to show a timestamp in Discord embeds.");

        embedAuthorIconUrl = config.Bind(EMBED_CONFIG_SETTINGS,
            "Author Icon URL",
            "https://cdn2.steamgriddb.com/icon/7d2b92b6726c241134dae6cd3fb8c182/32/32x32.png",
            "The URL for the small icon (32x32px) that appears next to the author name in Discord embeds.");
            
        embedThumbnailUrl = config.Bind(EMBED_CONFIG_SETTINGS,
            "Thumbnail URL",
            "https://cdn2.steamgriddb.com/icon/d17892563a6984845a0e23df7841f903/32/256x256.png",
            "The URL for the larger thumbnail image (ideally 256x256px) that appears in the top-right of Discord embeds.");
            
        // Embed Color Configuration
        embedDefaultColor = config.Bind(EMBED_STYLING_SETTINGS,
            "Default Embed Color",
            "#7289DA",
            "The default color for embeds when no specific color is defined. Use hex color format (e.g., #7289DA for Discord Blurple).");
            
        embedServerStartColor = config.Bind(EMBED_STYLING_SETTINGS,
            "Server Start Color",
            "#43B581",
            "The color for server start/launch event embeds. Use hex color format (e.g., #43B581 for a green shade).");
            
        embedServerStopColor = config.Bind(EMBED_STYLING_SETTINGS,
            "Server Stop Color",
            "#F04747",
            "The color for server stop/shutdown event embeds. Use hex color format (e.g., #F04747 for a red shade).");
            
        embedPlayerJoinColor = config.Bind(EMBED_STYLING_SETTINGS,
            "Player Join Color",
            "#43B581",
            "The color for player join event embeds. Use hex color format (e.g., #43B581 for a green shade).");
            
        embedPlayerLeaveColor = config.Bind(EMBED_STYLING_SETTINGS,
            "Player Leave Color",
            "#FAA61A",
            "The color for player leave event embeds. Use hex color format (e.g., #FAA61A for an orange shade).");
            
        embedDeathEventColor = config.Bind(EMBED_STYLING_SETTINGS,
            "Death Event Color",
            "#F04747",
            "The color for player death event embeds. Use hex color format (e.g., #F04747 for a red shade).");
            
        embedShoutMessageColor = config.Bind(EMBED_STYLING_SETTINGS,
            "Shout Message Color",
            "#7289DA",
            "The color for player shout message embeds. Use hex color format (e.g., #7289DA for Discord Blurple).");
            
        embedOtherEventColor = config.Bind(EMBED_STYLING_SETTINGS,
            "Other Event Color",
            "#747F8D",
            "The color for other miscellaneous event embeds. Use hex color format (e.g., #747F8D for a neutral gray).");
            
        embedWorldEventColor = config.Bind(EMBED_STYLING_SETTINGS,
            "World Event Color",
            "#8B5CF6",
            "The color for world event embeds (e.g., forest events, raids). Use hex color format (e.g., #8B5CF6 for a purple shade).");
            
        embedNewDayColor = config.Bind(EMBED_STYLING_SETTINGS,
            "New Day Color",
            "#FFD700",
            "The color for new day event embeds. Use hex color format (e.g., #FFD700 for a gold shade).");
            
        embedServerSaveColor = config.Bind(EMBED_STYLING_SETTINGS,
            "Server Save Color",
            "#1D8BF1",
            "The color for server save event embeds. Use hex color format (e.g., #1D8BF1 for a vibrant blue shade).");
            
        embedPositionMessageColor = config.Bind(EMBED_STYLING_SETTINGS,
            "Position Message Color",
            "#3498DB",
            "The color for position message embeds. Use hex color format (e.g., #3498DB for a bright blue shade).");
            
        embedActivePlayersColor = config.Bind(EMBED_STYLING_SETTINGS,
            "Active Players Color",
            "#4B84FF",
            "The color for active player announcement embeds. Use hex color format (e.g., #4B84FF for a vibrant blue shade).");
            
        embedLeaderboardEmbedColor = config.Bind(EMBED_STYLING_SETTINGS,
            "Leaderboard Embed Color",
            "#FFAA00",
            "The color for leaderboard announcement embeds. Use hex color format (e.g., #FFAA00 for a gold/yellow shade).");
            
        // Other Embed Customization
        embedFooterText = config.Bind(EMBED_CONFIG_SETTINGS,
            "Footer Text",
            "Valheim Server | {worldName}",
            "The text to display in the embed footer. You can use variables like {worldName}, {serverName}, and {timestamp}.");
            
        embedFieldDisplayOrder = config.Bind(EMBED_CONFIG_SETTINGS,
            "Field Display Order",
            "position;event;player;details",
            "The order in which to display embed fields. Format should be a semicolon-separated list of field identifiers.");
            
        embedUrlTemplate = config.Bind(EMBED_CONFIG_SETTINGS,
            "Embed URL Template",
            "",
            "Optional URL template for the embed title. When set, the title becomes a clickable link. You can use variables like {worldName}, {serverName}, {playerName}.");

        mutedDiscordUserList = config.Bind<string>(MAIN_SETTINGS,
            "Ignored Players",
            "",
            "It may be that you have some players that you never want to send Discord messages for. Adding a player name to this list will ignore them." +
            Environment.NewLine +
            "Format should be a semicolon-separated list: Stuart;John McJohnny;Weird-name1");

        mutedDiscordUserListRegex = config.Bind<string>(MAIN_SETTINGS,
            "Ignored Players (Regex)",
            "",
            "It may be that you have some players that you never want to send Discord messages for. This option lets you provide a regular expression to filter out players if their name matches." +
            Environment.NewLine +
            "Format should be a valid string for a .NET Regex (reference: https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference)");

        sendPositionsToggle = config.Bind(MAIN_SETTINGS,
            "Send Positions with Messages",
            true,
            "Disable this setting to disable any positions/coordinates being sent with messages (e.g. players deaths or players joining/leaving). (Overwrites any individual setting.)");

        showPlayerIdsToggle = config.Bind(EMBED_CONFIG_SETTINGS,
            "Show Player IDs in Messages",
            false,
            "Enable this setting to show player IDs (Steam IDs) in Discord embed messages. This can be useful for server administration but may not be desired for regular public servers.");

        collectStatsToggle = config.Bind(MAIN_SETTINGS,
            "Collect Player Stats",
            true,
            "Disable this setting to disable all stat collection. (Overwrites any individual setting.)");

        announcePlayerFirsts = config.Bind(MAIN_SETTINGS,
            "Announce Player Firsts",
            true,
            "Disable this setting to disable all extra announcements the first time each player does something. (Overwrites any individual setting.)");

        playerLookupPreference = config.Bind(MAIN_SETTINGS,
            "How to discern players in Record Retrieval",
            RetrievalDiscernmentMethods.PlayerId,
            "Choose a method for how players will be separated from the results of a record query (used for statistic leader boards)." +
            Environment.NewLine +
            RetrieveByName + Environment.NewLine +
            RetrieveBySteamID + Environment.NewLine +
            RetrieveByNameAndSteamID
        );

        allowNonPlayerShoutLogging = config.Bind(MAIN_SETTINGS,
            "Send Non-Player Shouts to Discord",
            false,
            "Enable this setting to have shouts which are performed by other mods/the server/non-players to be sent to Discord as well." +
            Environment.NewLine +
            "Note: These are still subject to censure by the muted player regex and list.");

        allowMentionsHereEveryone = config.Bind(MAIN_SETTINGS,
            "Allow @here and @everyone mentions",
            false,
            "Enable this setting to allow messages sent to Discord to mention @here and @everyone. Per the Discord API, these share the same setting." +
            Environment.NewLine +
            "Note: There is no filtering in place to prevent abuse of these mentions (e.g. in a shout or player's name).");

        allowMentionsAnyRole = config.Bind(MAIN_SETTINGS,
            "Allow @role mentions",
            true,
            "Enable this setting to allow messages sent to Discord to mention roles. Roles mentioned this way use the format `<@&role_id>`" +
            Environment.NewLine +
            "Note: There is no filtering in place to prevent abuse of these mentions (e.g. in a shout or player's name).");

        allowMentionsAnyUser = config.Bind(MAIN_SETTINGS,
            "Allow @user mentions",
            true,
            "Enable this setting to allow messages sent to Discord to mention users. Users mentioned this way use the format `<@user_id>`" +
            Environment.NewLine +
            "Note: There is no filtering in place to prevent abuse of these mentions (e.g. in a shout or player's name).");

        allowedRoleMentions = config.Bind<string>(MAIN_SETTINGS,
            "Allowed Role Mentions",
            "",
            "A semicolon-separated list of role IDs that are allowed to be mentioned in messages sent to Discord. These are just a number (no carets), e.g. `123;234`" +
            Environment.NewLine +
            "Note: This setting is overshadowed if 'Allow @role mentions` is enabled, and only when that is disabled will these roles still be allowed to be mentioned.");

        allowedUserMentions = config.Bind<string>(MAIN_SETTINGS,
            "Allowed User Mentions",
            "",
            "A semicolon-separated list of user IDs that are allowed to be mentioned in messages sent to Discord. These are just a number (no carets), e.g. `123;234`" +
            Environment.NewLine +
            "Note: This setting is overshadowed if 'Allow @user mentions` is enabled, and only when that is disabled will these users still be allowed to be mentioned.");

        config.Save();

        DiscordConnectorPlugin.StaticLogger.SetLogLevel(logDebugMessages.Value);
        // Update Muted Players
        if (string.IsNullOrEmpty(mutedDiscordUserList.Value))
        {
            mutedPlayers = [];
        }
        else
        {
            mutedPlayers = new List<string>(mutedDiscordUserList.Value.Split(';'));
        }

        if (string.IsNullOrEmpty(mutedDiscordUserListRegex.Value))
        {
            mutedPlayersRegex = new Regex(@"a^");
        }
        else
        {
            mutedPlayersRegex = new Regex(mutedDiscordUserListRegex.Value);
        }
        
        // Update Webhooks
        PrimaryWebhook = new WebhookEntry(webhookUrl.Value, Webhook.StringToEventList(webhookEvents.Value), whichWebhook: "Primary");
        if (!string.IsNullOrEmpty(webhookUsernameOverride.Value))
        {
            PrimaryWebhook.UsernameOverride = webhookUsernameOverride.Value;
        }

        if (!string.IsNullOrEmpty(webhookAvatarOverride.Value))
        {
            PrimaryWebhook.AvatarOverride = webhookAvatarOverride.Value;
        }

        SecondaryWebhook = new WebhookEntry(webhookUrl2.Value, Webhook.StringToEventList(webhook2Events.Value), whichWebhook: "Secondary");
        if (!string.IsNullOrEmpty(webhook2UsernameOverride.Value))
        {
            SecondaryWebhook.UsernameOverride = webhook2UsernameOverride.Value;
        }

        if (!string.IsNullOrEmpty(webhook2AvatarOverride.Value))
        {
            SecondaryWebhook.AvatarOverride = webhook2AvatarOverride.Value;
        }
        
        // Update Allowed Mentions
        if (string.IsNullOrEmpty(allowedRoleMentions.Value))
        {
            AllowedRoleMentions = [];
        }
        else
        {
            AllowedRoleMentions = new List<string>(allowedRoleMentions.Value.Split(';'));
        }

        if (string.IsNullOrEmpty(allowedUserMentions.Value))
        {
            AllowedUserMentions = [];
        }
        else
        {
            AllowedUserMentions = new List<string>(allowedUserMentions.Value.Split(';'));
        }
        
        // Update Embed Field Display Order
        if (string.IsNullOrEmpty(embedFieldDisplayOrder.Value))
        {
            EmbedFieldDisplayOrder = new List<string> { "position", "event", "player", "details" };
        }
        else
        {
            EmbedFieldDisplayOrder = new List<string>(embedFieldDisplayOrder.Value.Split(';'));
        }
    }

    public string DefaultWebhookUsernameOverride => defaultWebhookUsernameOverride.Value;
    public WebhookEntry PrimaryWebhook { get; private set; }

    public WebhookEntry SecondaryWebhook { get; private set; }

    public bool CollectStatsEnabled => collectStatsToggle.Value;
    public bool DiscordEmbedsEnabled => discordEmbedMessagesToggle.Value;
    
    // Embed Field Visibility Properties
    public bool EmbedTitleEnabled => embedTitleToggle.Value;
    public bool EmbedDescriptionEnabled => embedDescriptionToggle.Value;
    public bool EmbedAuthorEnabled => embedAuthorToggle.Value;
    public bool EmbedThumbnailEnabled => embedThumbnailToggle.Value;
    public bool EmbedFooterEnabled => embedFooterToggle.Value;
    public bool EmbedTimestampEnabled => embedTimestampToggle.Value;
    
    // Embed Color Properties
    public string EmbedDefaultColor => embedDefaultColor.Value;
    public string EmbedServerStartColor => embedServerStartColor.Value;
    public string EmbedServerStopColor => embedServerStopColor.Value;
    public string EmbedPlayerJoinColor => embedPlayerJoinColor.Value;
    public string EmbedPlayerLeaveColor => embedPlayerLeaveColor.Value;
    public string EmbedDeathEventColor => embedDeathEventColor.Value;
    public string EmbedShoutMessageColor => embedShoutMessageColor.Value;
    public string EmbedOtherEventColor => embedOtherEventColor.Value;
    public string EmbedWorldEventColor => embedWorldEventColor.Value;
    public string EmbedNewDayColor => embedNewDayColor.Value;
    public string EmbedServerSaveColor => embedServerSaveColor.Value;
    public string EmbedPositionMessageColor => embedPositionMessageColor.Value;
    public string EmbedActivePlayersColor => embedActivePlayersColor.Value;
    public string EmbedLeaderboardEmbedColor => embedLeaderboardEmbedColor.Value;
    
    // Other Embed Customization Properties
    public string EmbedFooterText => embedFooterText.Value;
    public List<string> EmbedFieldDisplayOrder { get; private set; }
    public string EmbedUrlTemplate => embedUrlTemplate.Value;
    public string EmbedAuthorIconUrl => embedAuthorIconUrl.Value;
    public string EmbedThumbnailUrl => embedThumbnailUrl.Value;
    
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
    public bool SendPositionsEnabled => sendPositionsToggle.Value;
    public bool ShowPlayerIds => showPlayerIdsToggle.Value;
    public List<string> MutedPlayers => mutedPlayers;
    public Regex MutedPlayersRegex => mutedPlayersRegex;
    public bool AnnouncePlayerFirsts => announcePlayerFirsts.Value;
    public RetrievalDiscernmentMethods RecordRetrievalDiscernmentMethod => playerLookupPreference.Value;
    public bool AllowNonPlayerShoutLogging => allowNonPlayerShoutLogging.Value;
    public bool AllowMentionsHereEveryone => allowMentionsHereEveryone.Value;
    public bool AllowMentionsAnyRole => allowMentionsAnyRole.Value;
    public bool AllowMentionsAnyUser => allowMentionsAnyUser.Value;
    public List<string> AllowedRoleMentions { get; private set; }

    public List<string> AllowedUserMentions { get; private set; }

    public string ConfigAsJson()
    {
        string jsonString = "{";
        jsonString += "\"discord\":{";
        jsonString += $"\"defaultWebhookUsernameOverride\":\"{defaultWebhookUsernameOverride.Value}\",";
        jsonString += $"\"webhook\":\"{(string.IsNullOrEmpty(webhookUrl.Value) ? "unset" : "REDACTED")}\",";
        jsonString += $"\"webhookEvents\":\"{webhookEvents.Value}\",";
        jsonString += $"\"webhookUsernameOverride\":\"{webhookUsernameOverride.Value}\",";
        jsonString += $"\"webhookAvatarOverride\":\"{webhookAvatarOverride.Value}\",";
        jsonString += $"\"webhook2\":\"{(string.IsNullOrEmpty(webhookUrl2.Value) ? "unset" : "REDACTED")}\",";
        jsonString += $"\"webhook2Events\":\"{webhook2Events.Value}\",";
        jsonString += $"\"webhook2UsernameOverride\":\"{webhook2UsernameOverride.Value}\",";
        jsonString += $"\"webhook2AvatarOverride\":\"{webhook2AvatarOverride.Value}\",";
        jsonString += $"\"logDebugMessages\":\"{logDebugMessages.Value}\",";
        jsonString += $"\"fancierMessages\":\"{DiscordEmbedsEnabled}\",";
        
        // Add embed configuration to JSON
        jsonString += "\"embedConfig\":{";
        jsonString += $"\"titleEnabled\":\"{EmbedTitleEnabled}\",";
        jsonString += $"\"descriptionEnabled\":\"{EmbedDescriptionEnabled}\",";
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
        jsonString += $"\"ignoredPlayers\":\"{mutedDiscordUserList.Value}\",";
        jsonString += "\"ignoredPlayersList\":[";
        for (int i = 0; i < mutedPlayers.Count; i++)
        {
            jsonString += $"\"{mutedPlayers[i]}\"";
            if (i < mutedPlayers.Count - 1)
            {
                jsonString += ",";
            }
        }

        jsonString += "],";
        jsonString += $"\"ignoredPlayersRegex\":\"{mutedDiscordUserListRegex.Value}\",";
        jsonString += $"\"allowMentionsHereEveryone\":\"{allowMentionsHereEveryone.Value}\",";
        jsonString += $"\"allowMentionsAnyRole\":\"{allowMentionsAnyRole.Value}\",";
        jsonString += $"\"allowMentionsAnyUser\":\"{allowMentionsAnyUser.Value}\",";
        jsonString += $"\"allowedRoleMentions\":\"{allowedRoleMentions.Value}\",";
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
        jsonString += $"\"allowedUserMentions\":\"{allowedUserMentions.Value}\",";
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
