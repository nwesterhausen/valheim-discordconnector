using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace DiscordConnector.Patches
{
    [HarmonyPatch(typeof(RandomEvent))]
    internal class RandomEventPatches
    {

        [HarmonyPatch(nameof(RandomEvent.OnActivate))]
        internal class OnActivate
        {

            private static void Prefix(ref RandomEvent __instance)
            {
                bool active = __instance.m_active;
                float duration = __instance.m_duration;
                string name = __instance.m_name;
                float time = __instance.m_time;
                float remaining = duration - time;
                Vector3 pos = __instance.m_pos;
                Plugin.StaticLogger.LogDebug(
                    $"Random event OnActivate {name}: {active} for {duration} at {pos}. (time: {time})"
                );



                List<String> involvedPlayers = new List<string>();
                foreach (ZNet.PlayerInfo playerInfo in ZNet.instance.GetPlayerList())
                {
                    if (RandEventSystem.instance.IsInsideRandomEventArea(__instance, playerInfo.m_position))
                    {
                        involvedPlayers.Add(playerInfo.m_name);
                    }
                }
                Plugin.StaticLogger.LogDebug(
                    $"Involved players in event: {(string.Join(",", involvedPlayers.ToArray()))}"
                );

                if (__instance.m_time > 0)
                {

                    if (Plugin.StaticConfig.EventResumedMessageEnabled)
                    {
                        string message = Plugin.StaticConfig.EventResumedMesssage
                            .Replace("%PLAYERS%", string.Join(",", involvedPlayers.ToArray()))
                            .Replace("%EVENT_START_MSG%", Localization.instance.Localize(__instance.m_startMessage))
                            .Replace("%EVENT_END_MSG%", Localization.instance.Localize(__instance.m_endMessage));
                        if (Plugin.StaticConfig.EventResumedPosEnabled)
                        {
                            DiscordApi.SendMessage(message, pos);
                        }
                        else
                        {
                            DiscordApi.SendMessage(message);
                        }
                    }
                }
                else
                {
                    if (Plugin.StaticConfig.EventStartMessageEnabled)
                    {
                        string message = Plugin.StaticConfig.EventStartMessage
                            .Replace("%EVENT_MSG%", Localization.instance.Localize(__instance.m_startMessage))
                            .Replace("%PLAYERS%", string.Join(",", involvedPlayers.ToArray()))
                            .Replace("%EVENT_START_MSG%", Localization.instance.Localize(__instance.m_startMessage))
                            .Replace("%EVENT_END_MSG%", Localization.instance.Localize(__instance.m_endMessage));
                        if (Plugin.StaticConfig.EventStartPosEnabled)
                        {
                            DiscordApi.SendMessage(message, pos);
                        }
                        else
                        {
                            DiscordApi.SendMessage(message);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(nameof(RandomEvent.OnDeactivate))]
        internal class OnDeactivate
        {

            private static void Prefix(ref RandomEvent __instance, ref bool end)
            {
                bool active = __instance.m_active;
                float duration = __instance.m_duration;
                string name = __instance.m_name;
                float time = __instance.m_time;
                Vector3 pos = __instance.m_pos;
                Plugin.StaticLogger.LogDebug(
                    $"Random event OnDeactivate {name}: End?{active} for {duration} at {pos}. (time: {time})"
                );

                List<String> involvedPlayers = new List<string>();
                foreach (ZNet.PlayerInfo playerInfo in ZNet.instance.GetPlayerList())
                {
                    if (RandEventSystem.instance.IsInsideRandomEventArea(__instance, playerInfo.m_position))
                    {
                        involvedPlayers.Add(playerInfo.m_name);
                    }
                }
                Plugin.StaticLogger.LogDebug(
                    $"Involved players in event: {(string.Join(",", involvedPlayers.ToArray()))}"
                );

                if (!end)
                {
                    if (Plugin.StaticConfig.EventPausedMessageEnabled)
                    {
                        string message = Plugin.StaticConfig.EventPausedMesssage
                            .Replace("%EVENT_START_MSG%", Localization.instance.Localize(__instance.m_startMessage))
                            .Replace("%EVENT_END_MSG%", Localization.instance.Localize(__instance.m_endMessage))
                            .Replace("%PLAYERS%", string.Join(",", involvedPlayers.ToArray()));
                        if (Plugin.StaticConfig.EventPausedPosEnabled)
                        {
                            DiscordApi.SendMessage(message, pos);
                        }
                        else
                        {
                            DiscordApi.SendMessage(message);
                        }
                    }
                }
                else
                {
                    if (Plugin.StaticConfig.EventStopMessageEnabled)
                    {
                        string message = Plugin.StaticConfig.EventStopMesssage
                            .Replace("%EVENT_MSG%", Localization.instance.Localize(__instance.m_endMessage))
                            .Replace("%EVENT_START_MSG%", Localization.instance.Localize(__instance.m_startMessage))
                            .Replace("%EVENT_END_MSG%", Localization.instance.Localize(__instance.m_endMessage))
                            .Replace("%PLAYERS%", string.Join(",", involvedPlayers.ToArray()));
                        if (Plugin.StaticConfig.EventStopPosEnabled)
                        {

                            DiscordApi.SendMessage(message, pos);
                        }
                        else
                        {
                            DiscordApi.SendMessage(message);
                        }
                    }
                }
            }
        }
    }
}
