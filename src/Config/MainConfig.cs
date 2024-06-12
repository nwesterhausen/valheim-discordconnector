using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BepInEx.Configuration;

namespace DiscordConnector.Config;

internal class MainConfig
{
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
    public const string RetrieveBySteamID = "PlayerId: Treat each PlayerId as a separate player";
    public const string RetrieveByNameAndSteamID = "NameAndPlayerId: Treat each [PlayerId:CharacterName] combo as a separate player";
    public const string RetrieveByName = "Name: Treat each CharacterName as a separate player";
    private readonly ConfigFile config;
    private static List<string> mutedPlayers;
    private static Regex mutedPlayersRegex;
    private const string MAIN_SETTINGS = "Main Settings";

    // Main Settings
    private ConfigEntry<string> defaultWebhookUsernameOverride;
    private ConfigEntry<string> webhookUrl;
    private ConfigEntry<string> webhookUrl2;
    private ConfigEntry<string> webhookEvents;
    private ConfigEntry<string> webhook2Events;
    private ConfigEntry<string> webhookUsernameOverride;
    private ConfigEntry<string> webhook2UsernameOverride;
    private ConfigEntry<string> webhookAvatarOverride;
    private ConfigEntry<string> webhook2AvatarOverride;
    private ConfigEntry<bool> discordEmbedMessagesToggle;
    private ConfigEntry<string> mutedDiscordUserList;
    private ConfigEntry<string> mutedDiscordUserListRegex;
    private ConfigEntry<bool> collectStatsToggle;
    private ConfigEntry<bool> sendPositionsToggle;
    private ConfigEntry<bool> announcePlayerFirsts;
    private ConfigEntry<RetrievalDiscernmentMethods> playerLookupPreference;
    private ConfigEntry<bool> allowNonPlayerShoutLogging;
    private ConfigEntry<bool> logDebugMessages;
    private ConfigEntry<bool> allowMentionsHereEveryone;
    private ConfigEntry<bool> allowMentionsAnyRole;
    private ConfigEntry<bool> allowMentionsAnyUser;
    private ConfigEntry<string> allowedRoleMentions;
    private ConfigEntry<string> allowedUserMentions;
    private List<string> allowedRoleMentionsList;
    private List<string> allowedUserMentionsList;

    private WebhookEntry primaryWebhook;
    private WebhookEntry secondaryWebhook;

    /// <summary>
    /// Creates a new MainConfig object with the given config file.
    /// </summary>
    public MainConfig(ConfigFile configFile)
    {
        config = configFile;
        LoadConfig();

        Plugin.StaticLogger.SetLogLevel(logDebugMessages.Value);

        UpdateMutedPlayers();
        UpdateWebhooks();
        UpdateAllowedMentions();
    }

    /// <summary>
    /// Reloads the config file and updates the muted players and webhook entries.
    /// </summary>
    public void ReloadConfig()
    {
        config.Reload();
        config.Save();

        Plugin.StaticLogger.SetLogLevel(logDebugMessages.Value);

        UpdateMutedPlayers();
        UpdateWebhooks();
        UpdateAllowedMentions();
    }

    /// <summary>
    /// Updates the allowed mentions lists with the values from the config file.
    /// </summary>
    private void UpdateAllowedMentions()
    {
        if (string.IsNullOrEmpty(allowedRoleMentions.Value))
        {
            allowedRoleMentionsList = [];
        }
        else
        {
            allowedRoleMentionsList = new List<string>(allowedRoleMentions.Value.Split(';'));
        }

        if (string.IsNullOrEmpty(allowedUserMentions.Value))
        {
            allowedUserMentionsList = [];
        }
        else
        {
            allowedUserMentionsList = new List<string>(allowedUserMentions.Value.Split(';'));
        }
    }

    /// <summary>
    /// Updates the muted players list with the values from the config file.
    /// </summary>
    private void UpdateMutedPlayers()
    {
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
    }

    /// <summary>
    /// Updates the webhook entries with the values from the config file.
    /// </summary>
    private void UpdateWebhooks()
    {
        primaryWebhook = new WebhookEntry(webhookUrl.Value, Webhook.StringToEventList(webhookEvents.Value));
        if (!string.IsNullOrEmpty(webhookUsernameOverride.Value))
        {
            primaryWebhook.UsernameOverride = webhookUsernameOverride.Value;
        }
        if (!string.IsNullOrEmpty(webhookAvatarOverride.Value))
        {
            primaryWebhook.AvatarOverride = webhookAvatarOverride.Value;
        }

        secondaryWebhook = new WebhookEntry(webhookUrl2.Value, Webhook.StringToEventList(webhook2Events.Value));
        if (!string.IsNullOrEmpty(webhook2UsernameOverride.Value))
        {
            secondaryWebhook.UsernameOverride = webhook2UsernameOverride.Value;
        }
        if (!string.IsNullOrEmpty(webhook2AvatarOverride.Value))
        {
            secondaryWebhook.AvatarOverride = webhook2AvatarOverride.Value;
        }
    }

    private void LoadConfig()
    {
        defaultWebhookUsernameOverride = config.Bind<string>(MAIN_SETTINGS,
            "Default Webhook Username Override",
            "",
            "Override the username of all webhooks for this instance of Discord Connector. If left blank, the webhook will use the default name (assigned by Discord)." + Environment.NewLine +
            "This setting will be used for all webhooks unless overridden by a specific webhook username override setting.");

        webhookUrl = config.Bind<string>(MAIN_SETTINGS,
            "Webhook URL",
            "",
            "Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of " + Environment.NewLine +
            "Discord's documentation: https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook");

        webhookEvents = config.Bind<string>(MAIN_SETTINGS,
            "Webhook Events",
            "ALL",
            "Specify a subset of possible events to send to the primary webhook. Previously all events would go to the primary webhook." + Environment.NewLine +
            "Format should be the keyword 'ALL' or a semi-colon separated list, e.g. 'serverLifecycle;playerAll;playerFirstAll;leaderboardsAll;'" + Environment.NewLine +
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
            "Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of " + Environment.NewLine +
            "Discord's documentation: https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook");

        webhook2Events = config.Bind<string>(MAIN_SETTINGS,
            "Secondary Webhook Events",
            "ALL",
            "Specify a subset of possible events to send to the secondary webhook." + Environment.NewLine +
            "Format should be the keyword 'ALL' or a semi-colon separated list, e.g. 'serverLaunch;serverStart;serverSave;'" + Environment.NewLine +
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

        logDebugMessages = config.Bind<bool>(MAIN_SETTINGS,
            "Log Debug Messages",
            false,
            "Enable this setting to listen to debug messages from the mod. This will help with troubleshooting issues.");

        discordEmbedMessagesToggle = config.Bind<bool>(MAIN_SETTINGS,
            "Use fancier discord messages",
            false,
            "Enable this setting to use embeds in the messages sent to Discord. Currently this will affect the position details for the messages.");

        mutedDiscordUserList = config.Bind<string>(MAIN_SETTINGS,
            "Ignored Players",
            "",
            "It may be that you have some players that you never want to send Discord messages for. Adding a player name to this list will ignore them." + Environment.NewLine +
            "Format should be a semicolon-separated list: Stuart;John McJohnny;Weird-name1");

        mutedDiscordUserListRegex = config.Bind<string>(MAIN_SETTINGS,
            "Ignored Players (Regex)",
            "",
            "It may be that you have some players that you never want to send Discord messages for. This option lets you provide a regular expression to filter out players if their name matches." + Environment.NewLine +
            "Format should be a valid string for a .NET Regex (reference: https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference)");

        sendPositionsToggle = config.Bind<bool>(MAIN_SETTINGS,
            "Send Positions with Messages",
            true,
            "Disable this setting to disable any positions/coordinates being sent with messages (e.g. players deaths or players joining/leaving). (Overwrites any individual setting.)");

        collectStatsToggle = config.Bind<bool>(MAIN_SETTINGS,
            "Collect Player Stats",
            true,
            "Disable this setting to disable all stat collection. (Overwrites any individual setting.)");

        announcePlayerFirsts = config.Bind<bool>(MAIN_SETTINGS,
            "Announce Player Firsts",
            true,
            "Disable this setting to disable all extra announcements the first time each player does something. (Overwrites any individual setting.)");

        playerLookupPreference = config.Bind<RetrievalDiscernmentMethods>(MAIN_SETTINGS,
            "How to discern players in Record Retrieval",
            RetrievalDiscernmentMethods.PlayerId,
            "Choose a method for how players will be separated from the results of a record query (used for statistic leader boards)." + Environment.NewLine +
            RetrieveByName + Environment.NewLine +
            RetrieveBySteamID + Environment.NewLine +
            RetrieveByNameAndSteamID
        );

        allowNonPlayerShoutLogging = config.Bind<bool>(MAIN_SETTINGS,
            "Send Non-Player Shouts to Discord",
            false,
            "Enable this setting to have shouts which are performed by other mods/the server/non-players to be sent to Discord as well." + Environment.NewLine +
            "Note: These are still subject to censure by the muted player regex and list.");

        allowMentionsHereEveryone = config.Bind<bool>(MAIN_SETTINGS,
            "Allow @here and @everyone mentions",
            false,
            "Enable this setting to allow messages sent to Discord to mention @here and @everyone. Per the Discord API, these share the same setting." + Environment.NewLine +
            "Note: There is no filtering in place to prevent abuse of these mentions (e.g. in a shout or player's name).");

        allowMentionsAnyRole = config.Bind<bool>(MAIN_SETTINGS,
            "Allow @role mentions",
            true,
            "Enable this setting to allow messages sent to Discord to mention roles. Roles mentioned this way use the format `<@&role_id>`" + Environment.NewLine +
            "Note: There is no filtering in place to prevent abuse of these mentions (e.g. in a shout or player's name).");

        allowMentionsAnyUser = config.Bind<bool>(MAIN_SETTINGS,
            "Allow @user mentions",
            true,
            "Enable this setting to allow messages sent to Discord to mention users. Users mentioned this way use the format `<@user_id>`" + Environment.NewLine +
            "Note: There is no filtering in place to prevent abuse of these mentions (e.g. in a shout or player's name).");

        allowedRoleMentions = config.Bind<string>(MAIN_SETTINGS,
            "Allowed Role Mentions",
            "",
            "A semicolon-separated list of role IDs that are allowed to be mentioned in messages sent to Discord. These are just a number (no carets), e.g. `123;234`" + Environment.NewLine +
            "Note: This setting is overshadowed if 'Allow @role mentions` is enabled, and only when that is disabled will these roles still be allowed to be mentioned.");

        allowedUserMentions = config.Bind<string>(MAIN_SETTINGS,
            "Allowed User Mentions",
            "",
            "A semicolon-separated list of user IDs that are allowed to be mentioned in messages sent to Discord. These are just a number (no carets), e.g. `123;234`" + Environment.NewLine +
            "Note: This setting is overshadowed if 'Allow @user mentions` is enabled, and only when that is disabled will these users still be allowed to be mentioned.");

        config.Save();
    }

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
        jsonString += $"\"ignoredPlayers\":\"{mutedDiscordUserList.Value}\",";
        jsonString += $"\"ignoredPlayersList\":[";
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
        jsonString += $"\"allowedRoleMentionsList\":[";
        for (int i = 0; i < allowedRoleMentionsList.Count; i++)
        {
            jsonString += $"\"{allowedRoleMentionsList[i]}\"";
            if (i < allowedRoleMentionsList.Count - 1)
            {
                jsonString += ",";
            }
        }
        jsonString += "],";
        jsonString += $"\"allowedUserMentions\":\"{allowedUserMentions.Value}\",";
        jsonString += $"\"allowedUserMentionsList\":[";
        for (int i = 0; i < allowedUserMentionsList.Count; i++)
        {
            jsonString += $"\"{allowedUserMentionsList[i]}\"";
            if (i < allowedUserMentionsList.Count - 1)
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

    public string DefaultWebhookUsernameOverride => defaultWebhookUsernameOverride.Value;
    public WebhookEntry PrimaryWebhook => primaryWebhook;
    public WebhookEntry SecondaryWebhook => secondaryWebhook;
    public bool CollectStatsEnabled => collectStatsToggle.Value;
    public bool DiscordEmbedsEnabled => discordEmbedMessagesToggle.Value;
    public bool SendPositionsEnabled => sendPositionsToggle.Value;
    public List<string> MutedPlayers => mutedPlayers;
    public Regex MutedPlayersRegex => mutedPlayersRegex;
    public bool AnnouncePlayerFirsts => announcePlayerFirsts.Value;
    public RetrievalDiscernmentMethods RecordRetrievalDiscernmentMethod => playerLookupPreference.Value;
    public bool AllowNonPlayerShoutLogging => allowNonPlayerShoutLogging.Value;
    public bool AllowMentionsHereEveryone => allowMentionsHereEveryone.Value;
    public bool AllowMentionsAnyRole => allowMentionsAnyRole.Value;
    public bool AllowMentionsAnyUser => allowMentionsAnyUser.Value;
    public List<string> AllowedRoleMentions => allowedRoleMentionsList;
    public List<string> AllowedUserMentions => allowedUserMentionsList;

}
