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
    private ConfigFile config;
    private static List<String> mutedPlayers;
    private static Regex mutedPlayersRegex;
    private const string MAIN_SETTINGS = "Main Settings";

    // Main Settings
    private ConfigEntry<string> webhookUrl;
    private ConfigEntry<string> webhookUrl2;
    private ConfigEntry<string> webhookEvents;
    private ConfigEntry<string> webhook2Events;
    private ConfigEntry<bool> discordEmbedMessagesToggle;
    private ConfigEntry<string> mutedDiscordUserList;
    private ConfigEntry<string> mutedDiscordUserListRegex;
    private ConfigEntry<bool> collectStatsToggle;
    private ConfigEntry<bool> sendPositionsToggle;
    private ConfigEntry<bool> announcePlayerFirsts;
    private ConfigEntry<RetrievalDiscernmentMethods> playerLookupPreference;
    private ConfigEntry<bool> allowNonPlayerShoutLogging;
    private ConfigEntry<bool> enableSQLite;

    private WebhookEntry primaryWebhook;
    private WebhookEntry secondaryWebhook;

    public MainConfig(ConfigFile configFile)
    {
        config = configFile;
        LoadConfig();
        mutedPlayers = new List<string>(mutedDiscordUserList.Value.Split(';'));
        if (String.IsNullOrEmpty(mutedDiscordUserListRegex.Value))
        {
            mutedPlayersRegex = new Regex(@"a^");
        }
        else
        {
            mutedPlayersRegex = new Regex(mutedDiscordUserListRegex.Value);
        }

        primaryWebhook = new WebhookEntry(webhookUrl.Value, Webhook.StringToEventList(webhookEvents.Value));
        secondaryWebhook = new WebhookEntry(webhookUrl2.Value, Webhook.StringToEventList(webhook2Events.Value));
    }

    public void ReloadConfig()
    {
        config.Reload();
        config.Save();

        mutedPlayers = new List<string>(mutedDiscordUserList.Value.Split(';'));
        if (String.IsNullOrEmpty(mutedDiscordUserListRegex.Value))
        {
            mutedPlayersRegex = new Regex(@"a^");
        }
        else
        {
            mutedPlayersRegex = new Regex(mutedDiscordUserListRegex.Value);
        }

        primaryWebhook = new WebhookEntry(webhookUrl.Value, Webhook.StringToEventList(webhookEvents.Value));
        secondaryWebhook = new WebhookEntry(webhookUrl2.Value, Webhook.StringToEventList(webhook2Events.Value));
    }

    private void LoadConfig()
    {

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
            "Full list of valid options here: https://discordconnector.valheim.nwest.games/config/main.html#webhook-events");

        webhookUrl2 = config.Bind<string>(MAIN_SETTINGS,
            "Secondary Webhook URL",
            "",
            "Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of " + Environment.NewLine +
            "Discord's documentation: https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook");

        webhook2Events = config.Bind<string>(MAIN_SETTINGS,
            "Secondary Webhook Events",
            "ALL",
            "Specify a subset of possible events to send to the primary webhook. Previously all events would go to the primary webhook." + Environment.NewLine +
            "Format should be the keyword 'ALL' or a semi-colon separated list, e.g. 'serverLaunch;serverStart;serverSave;'" + Environment.NewLine +
            "Full list of valid options here: https://discordconnector.valheim.nwest.games/config/main.html#webhook-events");

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

        enableSQLite = config.Bind<bool>(MAIN_SETTINGS,
            "Enable SQLite Database",
            false,
            "Enable the new SQLite database backend and migrate (if necessary) from the previous version LiteDB backend.");

        config.Save();
    }

    public string ConfigAsJson()
    {
        string jsonString = "{";
        jsonString += "\"discord\":{";
        jsonString += $"\"webhook\":\"{(string.IsNullOrEmpty(webhookUrl.Value) ? "unset" : "REDACTED")}\",";
        jsonString += $"\"webhookEvents\":\"{webhookEvents.Value}\",";
        jsonString += $"\"webhook2\":\"{(string.IsNullOrEmpty(webhookUrl2.Value) ? "unset" : "REDACTED")}\",";
        jsonString += $"\"webhook2Events\":\"{webhook2Events.Value}\",";
        jsonString += $"\"fancierMessages\":\"{DiscordEmbedsEnabled}\",";
        jsonString += $"\"ignoredPlayers\":\"{mutedDiscordUserList.Value}\",";
        jsonString += $"\"ignoredPlayersRegex\":\"{mutedDiscordUserListRegex.Value}\"";
        jsonString += "},";
        jsonString += $"\"collectStatsEnabled\":\"{CollectStatsEnabled}\",";
        jsonString += $"\"sendPositionsEnabled\":\"{SendPositionsEnabled}\",";
        jsonString += $"\"announcePlayerFirsts\":\"{AnnouncePlayerFirsts}\",";
        jsonString += $"\"enableSQLite\":\"{EnableSQLite}\",";
        jsonString += $"\"playerLookupPreference\":\"{RecordRetrievalDiscernmentMethod}\"";
        jsonString += "}";
        return jsonString;
    }

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
    public bool EnableSQLite => enableSQLite.Value;

}
