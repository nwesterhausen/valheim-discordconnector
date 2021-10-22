using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace DiscordConnector
{
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
            public static Vector3 Pos => HaveActiveEvent ? Event.m_pos : new Vector3(0, 0, 0);
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
                        Plugin.StaticLogger.LogDebug($"Unable to check location for {playerInfo.m_name} because their location is not public.");
                    }
                    else if (RandEventSystem.instance.IsInsideRandomEventArea(Event, playerInfo.m_position))
                    {
                        playerList.Add(playerInfo.m_name);
                    }
                }
                return playerList.ToArray();
            }
        }

        private bool WasRunning, HadActiveEvent;
        private float PreviousElapsed;
        private System.Timers.Timer randEventTimer;

        public EventWatcher()
        {
            WasRunning = false;
            HadActiveEvent = false;
            PreviousElapsed = 0;


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
                $"Involved Players: {string.Join(",", Status.InvolvedPlayersList())}";
                Plugin.StaticLogger.LogDebug(message);

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
                    }
                }
            }
            else
            {
                Plugin.StaticLogger.LogDebug("Event check ran, no current events (or world isn't loaded yet).");

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
                }
            }
            HadActiveEvent = Status.HaveActiveEvent;
            WasRunning = Status.IsRunning;
            PreviousElapsed = Status.Elapsed;
        }

        private void TriggerEventStart()
        {
            if (Plugin.StaticConfig.EventStartMessageEnabled)
            {
                string message = Plugin.StaticConfig.EventStartMessage
                    .Replace("%EVENT_MSG%", Status.StartMessage)
                    // .Replace("%PLAYERS%", Status.InvolvedPlayersList().Length == 0 ? "players" : string.Join(",", Status.InvolvedPlayersList()))
                    .Replace("%PLAYERS%", "") //! Removed because of unreliability for now
                    .Replace("%EVENT_START_MSG%", Status.StartMessage)
                    .Replace("%EVENT_END_MSG%", Status.EndMessage);
                if (Plugin.StaticConfig.EventStartPosEnabled)
                {
                    DiscordApi.SendMessage(message, Status.Pos);
                }
                else
                {
                    DiscordApi.SendMessage(message);
                }
            }
        }
        private void TriggerEventPaused()
        {
            if (Plugin.StaticConfig.EventPausedMessageEnabled)
            {
                string message = Plugin.StaticConfig.EventPausedMesssage
                    // .Replace("%PLAYERS%", Status.InvolvedPlayersList().Length == 0 ? "players" : string.Join(",", Status.InvolvedPlayersList()))
                    .Replace("%PLAYERS%", "") //! Removed because of unreliability for now
                    .Replace("%EVENT_START_MSG%", Status.StartMessage)
                    .Replace("%EVENT_END_MSG%", Status.EndMessage);
                if (Plugin.StaticConfig.EventPausedPosEnabled)
                {
                    DiscordApi.SendMessage(message, Status.Pos);
                }
                else
                {
                    DiscordApi.SendMessage(message);
                }
            }
        }
        private void TriggerEventResumed()
        {
            if (Plugin.StaticConfig.EventResumedMessageEnabled)
            {
                string message = Plugin.StaticConfig.EventResumedMesssage
                    // .Replace("%PLAYERS%", Status.InvolvedPlayersList().Length == 0 ? "players" : string.Join(",", Status.InvolvedPlayersList()))
                    .Replace("%PLAYERS%", "") //! Removed because of unreliability for now
                    .Replace("%EVENT_START_MSG%", Status.StartMessage)
                    .Replace("%EVENT_END_MSG%", Status.EndMessage);
                if (Plugin.StaticConfig.EventResumedPosEnabled)
                {
                    DiscordApi.SendMessage(message, Status.Pos);
                }
                else
                {
                    DiscordApi.SendMessage(message);
                }
            }
        }
        private void TriggerEventStop()
        {
            if (Plugin.StaticConfig.EventStopMessageEnabled)
            {
                string message = Plugin.StaticConfig.EventStopMesssage
                    .Replace("%EVENT_MSG%", Status.EndMessage)
                    // .Replace("%PLAYERS%", Status.InvolvedPlayersList().Length == 0 ? "players" : string.Join(",", Status.InvolvedPlayersList()))
                    .Replace("%PLAYERS%", "") //! Removed because of unreliability for now
                    .Replace("%EVENT_START_MSG%", Status.StartMessage)
                    .Replace("%EVENT_END_MSG%", Status.EndMessage);
                if (Plugin.StaticConfig.EventStopPosEnabled)
                {

                    DiscordApi.SendMessage(message, Status.Pos);
                }
                else
                {
                    DiscordApi.SendMessage(message);
                }
            }
        }
    }
}
