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
    }

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

            default:
                Plugin.StaticLogger.LogDebug($"Unmatched event token '{eventToken}'");
                return Event.None;
        }
    }

    public static List<Event> StringToEventList(string configEntry)
    {
        //Guard against empty string
        if (string.IsNullOrEmpty(configEntry))
        {
            return new List<Event>();
        }

        //Clean string (remove all non-word non-semi-colon characters)
        string cleaned = Regex.Replace(configEntry, @"[^;\w]", "");
        Plugin.StaticLogger.LogDebug($"Webhooks: cleaned config entry '{configEntry}' => '{cleaned}'");

        // Check for ALL case
        if (cleaned.Equals("ALL"))
        {
            return new List<Event> { Event.ALL };
        }

        List<Event> events = new List<Event>();

        foreach (string ev in cleaned.Split(';'))
        {
            events.Add(StringToEvent(ev));
        }

        foreach (Webhook.Event we in events)
        {
            Plugin.StaticLogger.LogDebug($"Added {we} to list");
        }

        return events;
    }
}

class WebhookEntry
{
    public string Url { get; set; }
    public List<Webhook.Event> FireOnEvents { get; set; }

    public WebhookEntry(string url, List<Webhook.Event> fireOnEvents)
    {
        if (string.IsNullOrEmpty(url))
        {
            Plugin.StaticLogger.LogDebug($"Coerced null or empty webhook url to empty string. Ignoring event list.");
            Url = "";
            FireOnEvents = new List<Webhook.Event>();
            return;
        }

        Url = url;


        if (fireOnEvents == null || fireOnEvents.Count == 0)
        {
            Plugin.StaticLogger.LogDebug($"Coerced null or empty webhook event list to empty list.");
            FireOnEvents = new List<Webhook.Event>();
        }
        else
        {
            FireOnEvents = fireOnEvents;
        }
    }

    internal bool HasEvent(Webhook.Event ev)
    {
        if (FireOnEvents.Contains(Webhook.Event.ALL))
        {
            Plugin.StaticLogger.LogDebug("Webhook has 'ALL' enabled");
            return true;
        }

        if (FireOnEvents.Contains(Webhook.Event.PlayerAll))
        {
            Plugin.StaticLogger.LogDebug($"Checking if {ev} is part of PlayerAll");
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
            Plugin.StaticLogger.LogDebug($"Checking if {ev} is part of PlayerFirstAll");
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
            Plugin.StaticLogger.LogDebug($"Checking if {ev} is part of EventLifecycle");
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
            Plugin.StaticLogger.LogDebug($"Checking if {ev} is part of ServerLifecycle");
            if (
                ev == Webhook.Event.ServerLaunch ||
                ev == Webhook.Event.ServerShutdown ||
                ev == Webhook.Event.ServerStart ||
                ev == Webhook.Event.ServerStop)
            {
                return true;
            }
        }
        if (FireOnEvents.Contains(Webhook.Event.LeaderboardsAll))
        {
            Plugin.StaticLogger.LogDebug($"Checking if {ev} is part of LeaderboardsAll");
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

        Plugin.StaticLogger.LogDebug($"Checking for exact match of {ev}");
        return FireOnEvents.Contains(ev);
    }
}
