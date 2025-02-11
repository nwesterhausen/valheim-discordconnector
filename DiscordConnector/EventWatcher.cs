using System;
using System.Collections.Generic;
using System.Timers;

namespace DiscordConnector;
internal class EventWatcher
{
    private static class Status
    {
        /// <summary>
        /// True if there is currently an active event on the map.
        /// </summary>
        public static bool HaveActiveEvent => RandEventSystem.HaveActiveEvent();
        public static RandomEvent Event
        {
            get
            {
                if (!HaveActiveEvent)
                {
                    return null;
                }
                return RandEventSystem.instance.GetCurrentRandomEvent();
            }
        }
        public static string Name => HaveActiveEvent ? Event.m_name : "";
        public static float Duration => HaveActiveEvent ? Event.m_duration : 0;
        public static float Elapsed => HaveActiveEvent ? Event.m_time : 0;
        public static bool IsRunning => HaveActiveEvent ? RandEventSystem.instance.IsAnyPlayerInEventArea(Event) : false;
        public static UnityEngine.Vector3 Pos => HaveActiveEvent ? Event.m_pos : new UnityEngine.Vector3(0, 0, 0);
        public static string EndMessage => HaveActiveEvent ? Localization.instance.Localize(Event.m_endMessage) : "";
        public static string StartMessage => HaveActiveEvent ? Localization.instance.Localize(Event.m_startMessage) : "";

        public static string[] InvolvedPlayersList()
        {
            List<String> playerList = new List<string>();
            if (!HaveActiveEvent)
            {
                return playerList.ToArray();
            }
            foreach (ZNet.PlayerInfo playerInfo in ZNet.instance.GetPlayerList())
            {
                if (!playerInfo.m_publicPosition)
                {
                    if (DiscordConnectorPlugin.StaticConfig.DebugEveryPlayerPosCheck)
                    {
                        DiscordConnectorPlugin.StaticLogger.LogDebug($"Unable to check location for {playerInfo.m_name} because their location is not public.");
                    }
                }
                else if (RandEventSystem.instance.IsInsideRandomEventArea(Event, playerInfo.m_position))
                {
                    playerList.Add(playerInfo.m_name);
                    if (DiscordConnectorPlugin.StaticConfig.DebugEveryPlayerPosCheck)
                    {
                        DiscordConnectorPlugin.StaticLogger.LogDebug($"{playerInfo.m_name} is at {playerInfo.m_position}");
                    }
                }
            }
            return playerList.ToArray();
        }
    }

    private bool WasRunning, HadActiveEvent;
    private float PreviousElapsed;
    private string PreviousEventStartMessage, PreviousEventEndMessage;
    private UnityEngine.Vector3 PreviousEventPos;
    private System.Timers.Timer randEventTimer;

    public EventWatcher()
    {
        WasRunning = false;
        HadActiveEvent = false;
        PreviousElapsed = 0;
        PreviousEventStartMessage = "";
        PreviousEventEndMessage = "";
        PreviousEventPos = new UnityEngine.Vector3();


        randEventTimer = new System.Timers.Timer();
        randEventTimer.Elapsed += CheckRandomEvent;
        randEventTimer.Interval = 1 * 1000; // 1 seconds
    }

    /// <summary>
    /// Activate the EventWatcher after the Event System has loaded! Otherwise it will provide false-positives.
    /// </summary>
    public void Activate()
    {
        randEventTimer.Start();
    }
    public void Dispose()
    {
        randEventTimer.Stop();
    }
    public void CheckRandomEvent(object sender, ElapsedEventArgs elapsedEventArgs)
    {
        if (Status.HaveActiveEvent)
        {
            /// <summary>
            /// Printing a detailed debug message with all the pieces we gather from Status.
            /// </summary>
            string message = $"Currently an event: {Status.HaveActiveEvent}. {Status.StartMessage} | {Status.EndMessage}" + Environment.NewLine +
            $"Event: {Status.Name} at {Status.Pos}. Status.IsRunning: {Status.IsRunning}. {Status.Elapsed} of {Status.Duration} seconds completed." + Environment.NewLine +
            $"PreviousEventStartMsg: {PreviousEventStartMessage}, PreviousEventEndMsg: {PreviousEventEndMessage}, PreviousEventPos: {PreviousEventPos}" + Environment.NewLine +
            $"Involved Players: {string.Join(",", Status.InvolvedPlayersList())}";
            if (DiscordConnectorPlugin.StaticConfig.DebugEveryEventCheck)
            {
                DiscordConnectorPlugin.StaticLogger.LogDebug(message);
            }

            if (Status.IsRunning)
            {

                /// <summary>
                /// This checks for what has changed from the last time we checked the Random Event status.
                /// If 
                ///     there was no event active before
                /// and current event is running
                ///         Then
                ///     The change is to START
                /// </summary>
                if (!HadActiveEvent)
                {
                    TriggerEventStart();
                    if (DiscordConnectorPlugin.StaticConfig.DebugEveryEventChange)
                    {
                        DiscordConnectorPlugin.StaticLogger.LogDebug(message);
                    }
                }

                /// <summary>
                /// This checks for what has changed from the last time we checked the Random Event status.
                /// If 
                ///     an event was listed as active
                /// and an event was not running
                /// and current event is running
                ///         Then
                ///     The change is from PAUSED to RESUMED
                /// </summary>
                if (HadActiveEvent && !WasRunning)
                {
                    TriggerEventResumed();
                    if (DiscordConnectorPlugin.StaticConfig.DebugEveryEventChange)
                    {
                        DiscordConnectorPlugin.StaticLogger.LogDebug(message);
                    }
                }
            }
            else
            {
                /// <summary>
                /// This checks for what has changed from the last time we checked the Random Event status.
                /// If 
                ///     an event was not listed as active
                /// and current event is not running
                ///         OR
                /// If 
                ///     an event was listed as active
                /// and an event was running
                /// and current event is not running
                ///         Then
                ///     The change is from RESUMED to PAUSED
                /// </summary>
                if ((!HadActiveEvent)
                    || (HadActiveEvent && WasRunning))
                {
                    TriggerEventPaused();
                    if (DiscordConnectorPlugin.StaticConfig.DebugEveryEventChange)
                    {
                        DiscordConnectorPlugin.StaticLogger.LogDebug(message);
                    }
                }
            }

            if (Status.Pos != UnityEngine.Vector3.zero)
            {
                PreviousEventStartMessage = Status.StartMessage;
                PreviousEventEndMessage = Status.EndMessage;
                PreviousEventPos = Status.Pos;
            }
        }
        else
        {
            if (DiscordConnectorPlugin.StaticConfig.DebugEveryEventCheck)
            {
                DiscordConnectorPlugin.StaticLogger.LogDebug(
                    $"PreviousEventStartMsg: {PreviousEventStartMessage}, PreviousEventEndMsg: {PreviousEventEndMessage}, PreviousEventPos: {PreviousEventPos}" + Environment.NewLine +
                    "Event check ran, no current events (or world isn't loaded yet)."
                );
            }

            /// <summary>
            /// This checks for what has changed from the last time we checked the Random Event status.
            /// If 
            ///     an event was listed as active
            ///         Then
            ///     The change is to STOP
            /// </summary>
            if (HadActiveEvent)
            {
                TriggerEventStop();
                if (DiscordConnectorPlugin.StaticConfig.DebugEveryEventChange)
                {
                    DiscordConnectorPlugin.StaticLogger.LogDebug("Event stopped!");
                }
            }
        }
        HadActiveEvent = Status.HaveActiveEvent;
        WasRunning = Status.IsRunning;
        PreviousElapsed = Status.Elapsed;
    }

    internal void TriggerEventStart()
    {
        if (DiscordConnectorPlugin.StaticConfig.EventStartMessageEnabled)
        {
            string message = MessageTransformer.FormatEventStartMessage(
                DiscordConnectorPlugin.StaticConfig.EventResumedMessage,
                Status.StartMessage,
                Status.EndMessage
            // string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
            );
            if (!DiscordConnectorPlugin.StaticConfig.EventStartPosEnabled)
            {
                DiscordApi.SendMessage(Webhook.Event.EventStart, message);
                return;
            }
            if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
            {
                DiscordApi.SendMessage(Webhook.Event.EventStart, message, Status.Pos);
            }
            else
            {
                message = MessageTransformer.FormatEventStartMessage(
                    DiscordConnectorPlugin.StaticConfig.EventResumedMessage,
                    Status.EndMessage,
                    Status.StartMessage,
                    // string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
                    Status.Pos
                );
                DiscordApi.SendMessage(Webhook.Event.EventStart, message);
            }
        }
    }
    internal void TriggerEventPaused()
    {
        if (DiscordConnectorPlugin.StaticConfig.EventPausedMessageEnabled)
        {
            string message = MessageTransformer.FormatEventMessage(
                DiscordConnectorPlugin.StaticConfig.EventPausedMessage,
                Status.StartMessage,
                Status.EndMessage
            // string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
            );
            if (!DiscordConnectorPlugin.StaticConfig.EventPausedPosEnabled)
            {
                DiscordApi.SendMessage(Webhook.Event.EventPaused, message);
                return;
            }
            if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
            {
                DiscordApi.SendMessage(Webhook.Event.EventPaused, message, Status.Pos);
            }
            else
            {
                message = MessageTransformer.FormatEventMessage(
                    DiscordConnectorPlugin.StaticConfig.EventPausedMessage,
                    Status.StartMessage,
                    Status.EndMessage,
                    // string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
                    Status.Pos
                );
                DiscordApi.SendMessage(Webhook.Event.EventPaused, message);
            }
        }
    }
    internal void TriggerEventResumed()
    {
        if (DiscordConnectorPlugin.StaticConfig.EventResumedMessageEnabled)
        {
            string message = MessageTransformer.FormatEventMessage(
                DiscordConnectorPlugin.StaticConfig.EventResumedMessage,
                Status.StartMessage,
                Status.EndMessage
            // string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
            );
            if (!DiscordConnectorPlugin.StaticConfig.EventResumedPosEnabled)
            {
                DiscordApi.SendMessage(Webhook.Event.EventResumed, message);
                return;
            }
            if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
            {
                DiscordApi.SendMessage(Webhook.Event.EventResumed, message, Status.Pos);
            }
            else
            {
                message = MessageTransformer.FormatEventMessage(
                    DiscordConnectorPlugin.StaticConfig.EventResumedMessage,
                    Status.StartMessage,
                    Status.EndMessage,
                    // string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
                    Status.Pos
                );
                DiscordApi.SendMessage(Webhook.Event.EventResumed, message);
            }
        }
    }
    internal void TriggerEventStop()
    {
        if (DiscordConnectorPlugin.StaticConfig.EventStopMessageEnabled)
        {
            string message = MessageTransformer.FormatEventEndMessage(
                DiscordConnectorPlugin.StaticConfig.EventStopMessage,
                PreviousEventStartMessage,
                PreviousEventEndMessage
            // string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
            );
            if (!DiscordConnectorPlugin.StaticConfig.EventStopPosEnabled)
            {
                DiscordApi.SendMessage(Webhook.Event.EventStop, message);
                return;
            }
            if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
            {
                DiscordApi.SendMessage(Webhook.Event.EventStop, message, PreviousEventPos);
            }
            else
            {
                message = MessageTransformer.FormatEventEndMessage(
                    DiscordConnectorPlugin.StaticConfig.EventStopMessage,
                    PreviousEventStartMessage,
                    PreviousEventEndMessage,
                    // string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
                    PreviousEventPos
                );
                DiscordApi.SendMessage(Webhook.Event.EventStop, message);
            }
        }
    }
}
