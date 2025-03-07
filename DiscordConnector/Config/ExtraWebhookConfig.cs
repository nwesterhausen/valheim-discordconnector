using System;
using System.Collections.Generic;

using BepInEx.Configuration;

namespace DiscordConnector.Config;

internal class ExtraWebhookConfig
{
    /// <summary>
    ///     The maximum number of webhooks that can be defined in the config file.
    /// </summary>
    private const int MAX_WEBHOOKS = 16;

    /// <summary>
    ///     Title of the section in the config file
    /// </summary>
    private const string EXTRA_WEBHOOKS = "Extra Webhooks";

    /// <summary>
    ///     The config file extension for this config file.
    /// </summary>
    public static string ConfigExtension = "extraWebhooks";

    /// <summary>
    ///     A reference to the config file that this config is using.
    /// </summary>
    private readonly ConfigFile config;

    /// <summary>
    ///     Creates a new ExtraWebhookConfig object with the given config file.
    /// </summary>
    /// <param name="configFile">The config file to use for this config</param>
    public ExtraWebhookConfig(ConfigFile configFile)
    {
        config = configFile;

        webhookUrlList = [];
        webhookEventsList = [];
        webhookUsernameOverrideList = [];
        webhookAvatarOverrideList = [];


        for (int i = 0; i < MAX_WEBHOOKS; i++)
        {
            webhookUrlList.Add(config.Bind<string>(
                EXTRA_WEBHOOKS,
                $"Webhook URL {i + 1}",
                "",
                "Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of " +
                Environment.NewLine +
                "Discord's documentation: https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook")
            );

            webhookEventsList.Add(config.Bind<string>(
                EXTRA_WEBHOOKS,
                $"Webhook Events {i + 1}",
                "ALL",
                $"Specify a subset of possible events to send to this webhook ({i + 1}). Previously all events would go the webhook." +
                Environment.NewLine +
                "Format should be the keyword 'ALL' or a semi-colon separated list, e.g. 'serverLaunch;serverStart;serverSave;'" +
                Environment.NewLine +
                "Full list of valid options here: https://discord-connector.valheim.games.nwest.one/config/main.html#webhook-events")
            );

            webhookUsernameOverrideList.Add(config.Bind<string>(
                EXTRA_WEBHOOKS,
                $"Webhook Username Override {i + 1}",
                "",
                "Override the username of this webhook." + Environment.NewLine +
                "If left blank, the webhook will use the default username set in the main config.")
            );

            webhookAvatarOverrideList.Add(config.Bind<string>(
                EXTRA_WEBHOOKS,
                $"Webhook Avatar Override {i + 1}",
                "",
                "Override the avatar of this webhook with the image at the given URL." + Environment.NewLine +
                "If left blank, the webhook will use the avatar defined on the Discord webhook in your server's settings.")
            );
        }

        config.Save();

        webhookEntries = LoadWebhookEntries();
    }

    /// <summary>
    ///     The config entries for the webhook urls.
    /// </summary>
    private List<ConfigEntry<string>> webhookUrlList { get; }

    /// <summary>
    ///     The config entries for the webhook events.
    /// </summary>
    private List<ConfigEntry<string>> webhookEventsList { get; }

    /// <summary>
    ///     The config entries for the webhook username overrides.
    /// </summary>
    private List<ConfigEntry<string>> webhookUsernameOverrideList { get; }

    /// <summary>
    ///     The config entries for the webhook avatar overrides.
    /// </summary>
    private List<ConfigEntry<string>> webhookAvatarOverrideList { get; }

    /// <summary>
    ///     The webhook entries defined in the config file.
    /// </summary>
    private List<WebhookEntry> webhookEntries { get; }

    /// <summary>
    ///     Converts the config entries into a list of WebhookEntry objects
    /// </summary>
    /// <returns>A list of webhooks ready for use. Only returns webhooks that are set up correctly.</returns>
    private List<WebhookEntry> LoadWebhookEntries()
    {
        // Check that all the lists are the same length
        if (webhookUrlList.Count != webhookEventsList.Count ||
            webhookUrlList.Count != webhookUsernameOverrideList.Count ||
            webhookUrlList.Count != webhookAvatarOverrideList.Count)
        {
            DiscordConnectorPlugin.StaticLogger.LogError(
                "Webhook lists are not the same length. This should not happen.");
            DiscordConnectorPlugin.StaticLogger.LogError(
                $"URLs: {webhookUrlList.Count}, Events: {webhookEventsList.Count}, Usernames: {webhookUsernameOverrideList.Count}, Avatars: {webhookAvatarOverrideList.Count}");
            return [];
        }

        List<WebhookEntry> entries = [];

        for (int i = 0; i < webhookUrlList.Count; i++)
        {
            // If either the URL or the events are empty, skip this entry
            if (string.IsNullOrEmpty(webhookUrlList[i].Value) || string.IsNullOrEmpty(webhookEventsList[i].Value))
            {
                DiscordConnectorPlugin.StaticLogger.LogDebug(
                    $"ExtraWebhooks: Skipping Webhook {i + 1} because URL or Events are empty.");
                continue;
            }

            // Convert the events string into a list of events
            List<Webhook.Event> events = Webhook.StringToEventList(webhookEventsList[i].Value);

            // If the events list is empty, skip this entry
            if (events.Count == 0)
            {
                DiscordConnectorPlugin.StaticLogger.LogDebug(
                    $"ExtraWebhooks: Skipping Webhook {i + 1} because events are empty.");
                continue;
            }

            // Create a new WebhookEntry object
            WebhookEntry entry = new(
                webhookUrlList[i].Value,
                events
            );

            if (!string.IsNullOrEmpty(webhookUsernameOverrideList[i].Value))
            {
                entry.UsernameOverride = webhookUsernameOverrideList[i].Value;
            }

            if (!string.IsNullOrEmpty(webhookAvatarOverrideList[i].Value))
            {
                entry.AvatarOverride = webhookAvatarOverrideList[i].Value;
            }

            entries.Add(entry);
        }

        if (entries.Count > 0)
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug(
                $"ExtraWebhooks: Loaded {entries.Count} webhooks from the config file.");
        }

        return entries;
    }

    /// <summary>
    ///     Returns this config as a JSON string (used mostly for debugging)
    /// </summary>
    /// <returns>JSON String of this config file</returns>
    public string ConfigAsJson()
    {
        string jsonString = "{";

        for (int i = 0; i < MAX_WEBHOOKS; i++)
        {
            jsonString +=
                $"\"webhookURL{i + 1}\": \"{(string.IsNullOrEmpty(webhookUrlList[i].Value) ? "unset" : "REDACTED")}\",";
            jsonString += $"\"webhookEvents{i + 1}\": \"{webhookEventsList[i].Value}\",";
            jsonString += $"\"webhookUsernameOverride{i + 1}\": \"{webhookUsernameOverrideList[i].Value}\",";
            jsonString += $"\"webhookAvatarOverride{i + 1}\": \"{webhookAvatarOverrideList[i].Value}\"";

            if (i < MAX_WEBHOOKS - 1)
            {
                jsonString += ",";
            }
        }

        jsonString += "}";

        return jsonString;
    }

    /// <summary>
    ///     Returns the webhook entries defined in the config file.
    /// </summary>
    /// <returns>A list of webhook entries</returns>
    public List<WebhookEntry> GetWebhookEntries()
    {
        return webhookEntries;
    }
}
