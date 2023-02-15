
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        ALL,
        ServerLifecycle,
        EventLifecycle,
        PlayerAll,
        PlayerFirstAll,
        None,
        Other,
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

            default:
                Plugin.StaticLogger.LogDebug($"Unmatched event token '{eventToken}'");
                return Event.None;
        }
    }

    public static List<Event> StringToEventList(string configEntry)
    {
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

        return events;
    }
}

class WebhookEntry
{
    public string Url { get; set; }
    public List<Webhook.Event> FireOnEvents { get; set; }

    internal bool HasEvent(Webhook.Event ev)
    {
        return FireOnEvents.Contains(ev) || FireOnEvents.Contains(Webhook.Event.ALL);
    }
}