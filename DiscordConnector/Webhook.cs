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
    public static readonly HashSet<Event> ServerEvents = new HashSet<Event>
    {
        Event.ServerLaunch, Event.ServerStart, Event.ServerStop,
        Event.ServerShutdown, Event.ServerSave, Event.ServerLifecycle
    };
    
    // Server lifecycle specific event collections
    public static readonly HashSet<Event> ServerLaunchEvents = new HashSet<Event>
    {
        Event.ServerLaunch
    };
    
    public static readonly HashSet<Event> ServerStartEvents = new HashSet<Event>
    {
        Event.ServerStart
    };
    
    public static readonly HashSet<Event> ServerStopEvents = new HashSet<Event>
    {
        Event.ServerStop
    };
    
    public static readonly HashSet<Event> ServerShutdownEvents = new HashSet<Event>
    {
        Event.ServerShutdown
    };
    
    public static readonly HashSet<Event> ServerSaveEvents = new HashSet<Event>
    {
        Event.ServerSave
    };
    
    // World event collections
    public static readonly HashSet<Event> WorldEvents = new HashSet<Event>
    {
        Event.EventStart, Event.EventPaused, Event.EventResumed, Event.EventStop,
        Event.EventLifecycle, Event.NewDayNumber
    };
    
    // Player event collections
    public static readonly HashSet<Event> PlayerJoinEvents = new HashSet<Event>
    {
        Event.PlayerJoin, Event.PlayerFirstJoin
    };
    
    public static readonly HashSet<Event> PlayerLeaveEvents = new HashSet<Event>
    {
        Event.PlayerLeave, Event.PlayerFirstLeave
    };
    
    public static readonly HashSet<Event> PlayerDeathEvents = new HashSet<Event>
    {
        Event.PlayerDeath, Event.PlayerFirstDeath
    };
    
    public static readonly HashSet<Event> PlayerShoutEvents = new HashSet<Event>
    {
        Event.PlayerShout, Event.PlayerFirstShout
    };
    
    public static readonly HashSet<Event> PlayerPingEvents = new HashSet<Event>
    {
        Event.PlayerPing, Event.PlayerFirstPing
    };
    
    // Combined player events
    public static readonly HashSet<Event> AllPlayerEvents = new HashSet<Event>
    {
        Event.PlayerJoin, Event.PlayerFirstJoin,
        Event.PlayerLeave, Event.PlayerFirstLeave,
        Event.PlayerDeath, Event.PlayerFirstDeath,
        Event.PlayerShout, Event.PlayerFirstShout,
        Event.PlayerPing, Event.PlayerFirstPing,
        Event.PlayerAll, Event.PlayerFirstAll
    };

    public static Event StringToEvent(string eventToken)
    {
        switch (eventToken)
        {
            case "ALL":
                return Event.ALL;
            case "serverLifecycle":
                return Event.ServerLifecycle;
            case "eventLifecycle":
                return Event.EventLifecycle;
            case "playerAll":
                return Event.PlayerAll;
            case "playerFirstAll":
                return Event.PlayerFirstAll;
            case "leaderboardsAll":
                return Event.LeaderboardsAll;

            case "serverLaunch":
                return Event.ServerLaunch;
            case "serverStart":
                return Event.ServerStart;
            case "serverStop":
                return Event.ServerStop;
            case "serverShutdown":
                return Event.ServerShutdown;
            case "serverSave":
                return Event.ServerSave;

            case "eventStart":
                return Event.EventStart;
            case "eventPaused":
                return Event.EventPaused;
            case "eventResumed":
                return Event.EventResumed;
            case "eventStop":
                return Event.EventStop;

            case "playerJoin":
                return Event.PlayerJoin;
            case "playerLeave":
                return Event.PlayerLeave;
            case "playerShout":
                return Event.PlayerShout;
            case "playerPing":
                return Event.PlayerPing;
            case "playerDeath":
                return Event.PlayerDeath;

            case "playerFirstJoin":
                return Event.PlayerFirstJoin;
            case "playerFirstLeave":
                return Event.PlayerFirstLeave;
            case "playerFirstShout":
                return Event.PlayerFirstShout;
            case "playerFirstPing":
                return Event.PlayerFirstPing;
            case "playerFirstDeath":
                return Event.PlayerFirstDeath;

            case "activePlayers":
                return Event.ActivePlayers;
            case "leaderboard1":
                return Event.Leaderboard1;
            case "leaderboard2":
                return Event.Leaderboard2;
            case "leaderboard3":
                return Event.Leaderboard3;
            case "leaderboard4":
                return Event.Leaderboard4;
            case "leaderboard5":
                return Event.Leaderboard5;

            case "cronjob":
                return Event.CronJob;

            case "newDayNumber":
                return Event.NewDayNumber;

            default:
                DiscordConnectorPlugin.StaticLogger.LogDebug($"Unmatched event token '{eventToken}'");
                return Event.None;
        }
    }

    public static List<Event> StringToEventList(string configEntry)
    {
        //Guard against empty string
        if (string.IsNullOrEmpty(configEntry))
        {
            return [];
        }

        //Clean string (remove all non-word non-semi-colon characters)
        string cleaned = Regex.Replace(configEntry, @"[^;\w]", "");
        DiscordConnectorPlugin.StaticLogger.LogDebug($"Webhooks: cleaned config entry '{configEntry}' => '{cleaned}'");

        // Check for ALL case
        if (cleaned.Equals("ALL"))
        {
            return [Event.ALL];
        }

        List<Event> events = [];

        foreach (string ev in cleaned.Split(';'))
        {
            events.Add(StringToEvent(ev));
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
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Coerced null or empty {whichWebhook} webhook event list to empty list.");
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
