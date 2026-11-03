using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DiscordConnector;

public class Webhook
{
    public enum Event
    {
        ServerLaunch,
        ServerStart,
        ServerStop,
        ServerShutdown,
        ServerSave,
        EventStart,
        EventPaused,
        EventResumed,
        EventStop,
        PlayerJoin,
        PlayerLeave,
        PlayerShout,
        PlayerPing,
        PlayerDeath,
        PlayerFirstJoin,
        PlayerFirstLeave,
        PlayerFirstShout,
        PlayerFirstPing,
        PlayerFirstDeath,
        ActivePlayers,
        Leaderboard1,
        Leaderboard2,
        Leaderboard3,
        Leaderboard4,
        Leaderboard5,
        ALL,
        ServerLifecycle,
        EventLifecycle,
        PlayerAll,
        PlayerFirstAll,
        LeaderboardsAll,
        None,
        Other,
        CronJob,
        NewDayNumber
    }

    // Event category collections to facilitate easier event type checking
    public static readonly HashSet<Event> ServerEvents = new()
    {
        Event.ServerLaunch,
        Event.ServerStart,
        Event.ServerStop,
        Event.ServerShutdown,
        Event.ServerSave,
        Event.ServerLifecycle
    };

    // Server lifecycle specific event collections
    public static readonly HashSet<Event> ServerLaunchEvents = new() { Event.ServerLaunch };

    public static readonly HashSet<Event> ServerStartEvents = new() { Event.ServerStart };

    public static readonly HashSet<Event> ServerStopEvents = new() { Event.ServerStop };

    public static readonly HashSet<Event> ServerShutdownEvents = new() { Event.ServerShutdown };

    public static readonly HashSet<Event> ServerSaveEvents = new() { Event.ServerSave };

    // World event collections
    public static readonly HashSet<Event> WorldEvents = new()
    {
        Event.EventStart,
        Event.EventPaused,
        Event.EventResumed,
        Event.EventStop,
        Event.EventLifecycle,
        Event.NewDayNumber
    };

    // Player event collections
    public static readonly HashSet<Event> PlayerJoinEvents = new() { Event.PlayerJoin, Event.PlayerFirstJoin };

    public static readonly HashSet<Event> PlayerLeaveEvents = new() { Event.PlayerLeave, Event.PlayerFirstLeave };

    public static readonly HashSet<Event> PlayerDeathEvents = new() { Event.PlayerDeath, Event.PlayerFirstDeath };

    public static readonly HashSet<Event> PlayerShoutEvents = new() { Event.PlayerShout, Event.PlayerFirstShout };

    public static readonly HashSet<Event> PlayerPingEvents = new() { Event.PlayerPing, Event.PlayerFirstPing };

    // Combined player events
    public static readonly HashSet<Event> AllPlayerEvents = new()
    {
        Event.PlayerJoin,
        Event.PlayerFirstJoin,
        Event.PlayerLeave,
        Event.PlayerFirstLeave,
        Event.PlayerDeath,
        Event.PlayerFirstDeath,
        Event.PlayerShout,
        Event.PlayerFirstShout,
        Event.PlayerPing,
        Event.PlayerFirstPing,
        Event.PlayerAll,
        Event.PlayerFirstAll
    };

    // Config tokens, matched without regard to case ("playerdeath" and "PlayerDeath" both work).
    private static readonly Dictionary<string, Event> s_eventTokens = new(StringComparer.OrdinalIgnoreCase)
    {
        { "ALL", Event.ALL },
        { "serverLifecycle", Event.ServerLifecycle },
        { "eventLifecycle", Event.EventLifecycle },
        { "playerAll", Event.PlayerAll },
        { "playerFirstAll", Event.PlayerFirstAll },
        { "leaderboardsAll", Event.LeaderboardsAll },
        { "serverLaunch", Event.ServerLaunch },
        { "serverStart", Event.ServerStart },
        { "serverStop", Event.ServerStop },
        { "serverShutdown", Event.ServerShutdown },
        { "serverSave", Event.ServerSave },
        { "eventStart", Event.EventStart },
        { "eventPaused", Event.EventPaused },
        { "eventResumed", Event.EventResumed },
        { "eventStop", Event.EventStop },
        { "playerJoin", Event.PlayerJoin },
        { "playerLeave", Event.PlayerLeave },
        { "playerShout", Event.PlayerShout },
        { "playerPing", Event.PlayerPing },
        { "playerDeath", Event.PlayerDeath },
        { "playerFirstJoin", Event.PlayerFirstJoin },
        { "playerFirstLeave", Event.PlayerFirstLeave },
        { "playerFirstShout", Event.PlayerFirstShout },
        { "playerFirstPing", Event.PlayerFirstPing },
        { "playerFirstDeath", Event.PlayerFirstDeath },
        { "activePlayers", Event.ActivePlayers },
        { "leaderboard1", Event.Leaderboard1 },
        { "leaderboard2", Event.Leaderboard2 },
        { "leaderboard3", Event.Leaderboard3 },
        { "leaderboard4", Event.Leaderboard4 },
        { "leaderboard5", Event.Leaderboard5 },
        { "cronjob", Event.CronJob },
        { "newDayNumber", Event.NewDayNumber },
        // Listed in docs/config/webhook.events.md but never parsed (issue #382).
        { "other", Event.Other }
    };

    public static Event StringToEvent(string eventToken)
    {
        if (eventToken != null && s_eventTokens.TryGetValue(eventToken.Trim(), out Event ev))
        {
            return ev;
        }

        DiscordConnectorPlugin.StaticLogger.LogWarning(
            $"Unknown webhook event '{eventToken}' in config (see docs/config/webhook.events.md for valid names).");
        return Event.None;
    }

    public static List<Event> StringToEventList(string configEntry)
    {
        //Guard against empty string
        if (string.IsNullOrEmpty(configEntry))
        {
            return [];
        }

        // ';' is the documented separator, but people also use ',', ':' or spaces (issue #357), which used to glue
        // two names together into one unknown name. Treat all of them as separators, then drop anything else.
        string cleaned = Regex.Replace(configEntry.Trim(), @"[,:\s]+", ";");
        cleaned = Regex.Replace(cleaned, @"[^;\w]", "");
        DiscordConnectorPlugin.StaticLogger.LogDebug($"Webhooks: cleaned config entry '{configEntry}' => '{cleaned}'");

        // Check for ALL case
        if (cleaned.Equals("ALL", StringComparison.OrdinalIgnoreCase))
        {
            return [Event.ALL];
        }

        List<Event> events = [];

        foreach (string ev in cleaned.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
        {
            Event parsed = StringToEvent(ev);
            if (parsed != Event.None && !events.Contains(parsed))
            {
                events.Add(parsed);
            }
        }

        DiscordConnectorPlugin.StaticLogger.LogDebug(
            $"Webhooks: parsed config entry '{configEntry}' => '{string.Join(", ", events)}'");

        return events;
    }
}

internal class WebhookEntry
{
    /// <summary>
    ///     Create a new WebhookEntry, defaulting to all events
    /// </summary>
    /// <param name="url">webhook endpoint</param>
    public WebhookEntry(string url)
    {
        Url = url;
        FireOnEvents = [Webhook.Event.ALL];
    }

    /// <summary>
    ///     Create a new WebhookEntry
    /// </summary>
    /// <param name="url">webhook endpoint</param>
    /// <param name="fireOnEvents">events to trigger this webhook</param>
    /// <param name="usernameOverride">(Optional) username override</param>
    /// <param name="avatarOverride">(Optional) avatar override</param>
    public WebhookEntry(string url, List<Webhook.Event> fireOnEvents, string usernameOverride = "",
        string avatarOverride = "", string whichWebhook = "")
    {
        if (string.IsNullOrEmpty(url))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug(
                $"Coerced null or empty {whichWebhook} webhook url to empty string. Ignoring event list.");
            Url = "";
            FireOnEvents = [];
            return;
        }

        Url = url;

        if (fireOnEvents == null || fireOnEvents.Count == 0)
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug(
                $"Coerced null or empty {whichWebhook} webhook event list to empty list.");
            FireOnEvents = [];
        }
        else
        {
            FireOnEvents = fireOnEvents;
        }

        if (!string.IsNullOrEmpty(usernameOverride))
        {
            UsernameOverride = usernameOverride;
        }

        if (!string.IsNullOrEmpty(avatarOverride))
        {
            AvatarOverride = avatarOverride;
        }
    }

    /// <summary>
    ///     The webhook endpoint URL
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    ///     Which events should trigger this webhook
    /// </summary>
    public List<Webhook.Event> FireOnEvents { get; set; }

    /// <summary>
    ///     The username to use for this webhook
    /// </summary>
    public string UsernameOverride { get; set; } = string.Empty;

    /// <summary>
    ///     The URL of the avatar to use for this webhook
    /// </summary>
    public string AvatarOverride { get; set; } = string.Empty;

    /// <summary>
    ///     Check if the webhook has a username override
    /// </summary>
    /// <returns>True if a username override exists for this webhook</returns>
    public bool HasUsernameOverride()
    {
        return !string.IsNullOrEmpty(UsernameOverride);
    }

    /// <summary>
    ///     Check if the webhook has an avatar override
    /// </summary>
    /// <returns>True if an avatar override exists for this webhook</returns>
    public bool HasAvatarOverride()
    {
        return !string.IsNullOrEmpty(AvatarOverride);
    }

    internal bool HasEvent(Webhook.Event ev)
    {
        if (FireOnEvents.Count == 0)
        {
            return false;
        }

        if (FireOnEvents.Contains(Webhook.Event.ALL))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug("Webhook has 'ALL' enabled");
            return true;
        }

        if (FireOnEvents.Contains(Webhook.Event.PlayerAll))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Checking if {ev} is part of PlayerAll");
            if (
                ev == Webhook.Event.PlayerDeath ||
                ev == Webhook.Event.PlayerJoin ||
                ev == Webhook.Event.PlayerLeave ||
                ev == Webhook.Event.PlayerPing ||
                ev == Webhook.Event.PlayerShout)
            {
                return true;
            }
        }

        if (FireOnEvents.Contains(Webhook.Event.PlayerFirstAll))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Checking if {ev} is part of PlayerFirstAll");
            if (
                ev == Webhook.Event.PlayerFirstDeath ||
                ev == Webhook.Event.PlayerFirstJoin ||
                ev == Webhook.Event.PlayerFirstLeave ||
                ev == Webhook.Event.PlayerFirstPing ||
                ev == Webhook.Event.PlayerFirstShout)
            {
                return true;
            }
        }

        if (FireOnEvents.Contains(Webhook.Event.EventLifecycle))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Checking if {ev} is part of EventLifecycle");
            if (
                ev == Webhook.Event.EventStart ||
                ev == Webhook.Event.EventStop ||
                ev == Webhook.Event.EventResumed ||
                ev == Webhook.Event.EventPaused)
            {
                return true;
            }
        }

        if (FireOnEvents.Contains(Webhook.Event.ServerLifecycle))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Checking if {ev} is part of ServerLifecycle");
            if (
                ev == Webhook.Event.ServerLaunch ||
                ev == Webhook.Event.ServerShutdown ||
                ev == Webhook.Event.ServerStart ||
                ev == Webhook.Event.ServerStop ||
                ev == Webhook.Event.NewDayNumber)
            {
                return true;
            }
        }

        if (FireOnEvents.Contains(Webhook.Event.LeaderboardsAll))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Checking if {ev} is part of LeaderboardsAll");
            if (
                ev == Webhook.Event.ActivePlayers ||
                ev == Webhook.Event.Leaderboard1 ||
                ev == Webhook.Event.Leaderboard2 ||
                ev == Webhook.Event.Leaderboard3 ||
                ev == Webhook.Event.Leaderboard4 ||
                ev == Webhook.Event.Leaderboard5)
            {
                return true;
            }
        }

        DiscordConnectorPlugin.StaticLogger.LogDebug($"Checking for exact match of {ev}");
        return FireOnEvents.Contains(ev);
    }
}
