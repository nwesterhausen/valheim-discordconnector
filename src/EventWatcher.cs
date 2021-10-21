using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace DiscordConnector
{
    internal static class EventWatcher
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
            public static bool IsActive => HaveActiveEvent ? Event.m_active : false;
            public static Vector3 Pos => HaveActiveEvent ? Event.m_pos : new Vector3(0, 0, 0);
            public static string EndMessage => HaveActiveEvent ? Event.m_endMessage : "";
            public static string StartMessage => HaveActiveEvent ? Event.m_startMessage : "";

            public static List<String> InvolvedPlayersList()
            {
                List<String> playerList = new List<string>();
                if (!HaveActiveEvent)
                {
                    return playerList;
                }
                foreach (ZNet.PlayerInfo playerInfo in ZNet.instance.GetPlayerList())
                {
                    if (RandEventSystem.instance.IsInsideRandomEventArea(Event, playerInfo.m_position))
                    {
                        playerList.Add(playerInfo.m_name);
                    }
                }
                return playerList;
            }
        }
        public static void CheckRandomEvent(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (Status.HaveActiveEvent)
            {
                string message = $"Currently an event: {Status.HaveActiveEvent}. {Status.StartMessage} | {Status.EndMessage}" + Environment.NewLine +
                $"Event: {Status.Name} at {Status.Pos}. Running: {Status.IsActive}. {Status.Elapsed} of {Status.Duration} ticks completed." + Environment.NewLine +
                $"Involved Players: {string.Join(",", Status.InvolvedPlayersList())}";

                Plugin.StaticLogger.LogDebug(message);
                return;
            }
            Plugin.StaticLogger.LogDebug("Event check ran, no current events (or world isn't loaded yet).");
        }
    }
}
