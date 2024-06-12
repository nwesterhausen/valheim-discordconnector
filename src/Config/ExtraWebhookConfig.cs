using System;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace DiscordConnector.Config;

internal class ExtraWebhookConfig
{
  /// <summary>
  /// The config file extension for this config file.
  /// </summary>
  public static string ConfigExtension = "extraWebhooks";
  /// <summary>
  /// The maximum number of webhooks that can be defined in the config file.
  /// </summary>
  private const int MAX_WEBHOOKS = 16;

  /// <summary>
  /// The config entries for the webhook urls.
  /// </summary>
  private List<ConfigEntry<string>> webhookUrlList { get; set; }
  /// <summary>
  /// The config entries for the webhook events.
  /// </summary>
  private List<ConfigEntry<string>> webhookEventsList { get; set; }
  /// <summary>
  /// The config entries for the webhook username overrides.
  /// </summary>
  private List<ConfigEntry<string>> webhookUsernameOverrideList { get; set; }
  /// <summary>
  /// The webhook entries defined in the config file.
  /// </summary>
  private List<WebhookEntry> webhookEntries { get; set; }
  /// <summary>
  /// A reference to the config file that this config is using.
  /// </summary>
  private readonly ConfigFile config;
  /// <summary>
  /// Title of the section in the config file
  /// </summary>
  private const string EXTRA_WEBHOOKS = "Extra Webhooks";

  /// <summary>
  /// Creates a new ExtraWebhookConfig object with the given config file.
  /// </summary>
  /// <param name="configFile">The config file to use for this config</param>
  public ExtraWebhookConfig(ConfigFile configFile)
  {
    config = configFile;

    webhookUrlList = [];
    webhookEventsList = [];
    webhookUsernameOverrideList = [];

    LoadConfig();

    webhookEntries = LoadWebhookEntries();
  }

  /// <summary>
  /// Reloads the config file and updates the webhook entries.
  /// </summary>
  public void ReloadConfig()
  {
    config.Reload();
    config.Save();

    webhookEntries = LoadWebhookEntries();
  }


  /// <summary>
  /// Initializes the config file with the default values and the config entries available.
  /// </summary>
  private void LoadConfig()
  {
    for (int i = 0; i < MAX_WEBHOOKS; i++)
    {
      webhookUrlList.Add(config.Bind<string>(
          EXTRA_WEBHOOKS,
          $"Webhook URL {i + 1}",
          "",
          $"Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of " + Environment.NewLine +
          "Discord's documentation: https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook")
        );

      webhookEventsList.Add(config.Bind<string>(
          EXTRA_WEBHOOKS,
          $"Webhook Events {i + 1}",
          "ALL",
          $"Specify a subset of possible events to send to this webhook ({i + 1}). Previously all events would go the webhook." + Environment.NewLine +
          "Format should be the keyword 'ALL' or a semi-colon separated list, e.g. 'serverLaunch;serverStart;serverSave;'" + Environment.NewLine +
          "Full list of valid options here: https://discord-connector.valheim.games.nwest.one/config/main.html#webhook-events")
      );

      webhookUsernameOverrideList.Add(config.Bind<string>(
          EXTRA_WEBHOOKS,
          $"Webhook Username Override {i + 1}",
          "",
          $"Optional: Override the username of the webhook for this entry ({i + 1})." + Environment.NewLine +
          "If left blank, the webhook will use the default username set in the main config.")
        );
    }

    config.Save();
  }

  /// <summary>
  /// Converts the config entries into a list of WebhookEntry objects
  /// </summary>
  /// <returns>A list of webhooks ready for use. Only returns webhooks that are set up correctly.</returns>
  private List<WebhookEntry> LoadWebhookEntries()
  {
    List<WebhookEntry> entries = [];

    for (int i = 0; i < webhookUrlList.Count; i++)
    {
      // If either the URL or the events are empty, skip this entry
      if (string.IsNullOrEmpty(webhookUrlList[i].Value) || string.IsNullOrEmpty(webhookEventsList[i].Value))
      {
        continue;
      }
      entries.Add(new WebhookEntry(webhookUrlList[i].Value, Webhook.StringToEventList(webhookEventsList[i].Value), webhookUsernameOverrideList[i].Value));
    }

    return entries;
  }

  /// <summary>
  /// Returns this config as a JSON string (used mostly for debugging)
  /// </summary>
  /// <returns>JSON String of this config file</returns>
  public string ConfigAsJson()
  {
    string jsonString = "{";

    for (int i = 0; i < MAX_WEBHOOKS; i++)
    {
      jsonString += $"\"webhookURL{i + 1}\": \"{webhookUrlList[i].Value}\",";
      jsonString += $"\"webhookEvents{i + 1}\": \"{webhookEventsList[i].Value}\",";
      jsonString += $"\"webhookUsernameOverride{i + 1}\": \"{webhookUsernameOverrideList[i].Value}\",";
    }

    jsonString += "}";

    return jsonString;
  }

  /// <summary>
  /// Returns the webhook entries defined in the config file.
  /// </summary>
  /// <returns>A list of webhook entries</returns>
  public List<WebhookEntry> GetWebhookEntries()
  {
    return webhookEntries;
  }
}
