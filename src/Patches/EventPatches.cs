using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace DiscordConnector.Patches
{
    internal class RandEventSystemPatches
    {

        [HarmonyPatch(typeof(RandEventSystem), nameof(RandEventSystem.SetRandomEvent))]
        internal class SetRandomEvent
        {

            private static void Prefix(ref RandomEvent ev, ref Vector3 pos)
            {
                if (ev == null)
                {
                    Plugin.StaticLogger.LogDebug(
                        $"Random event cleared"
                    );
                    return;
                }
                bool active = ev.m_active;
                float duration = ev.m_duration;
                string name = ev.m_name;
                Plugin.StaticLogger.LogDebug(
                    $"Random event system SetRandomEvent? {active}: {name} for {duration} at {pos}."
                );
            }
        }

    }
    internal class RandEventPatches
    {

        [HarmonyPatch(typeof(RandomEvent), nameof(RandomEvent.OnActivate))]
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

                if (Plugin.StaticConfig.EventStartMessageEnabled)
                {
                    string message = Plugin.StaticConfig.EventStartMessage.Replace("%EVENT_MSG%", Localization.instance.Localize(__instance.m_startMessage));
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

        [HarmonyPatch(typeof(RandomEvent), nameof(RandomEvent.OnDeactivate))]
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

                if (!end)
                {
                    if (Plugin.StaticConfig.EventPausedMessageEnabled)
                    {
                        string message = Plugin.StaticConfig.EventPausedMesssage
                                .Replace("%EVENT_START_MSG%", Localization.instance.Localize(__instance.m_startMessage))
                                .Replace("%EVENT_END_MSG%", Localization.instance.Localize(__instance.m_endMessage));
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
                        string message = Plugin.StaticConfig.EventStopMesssage.Replace("%EVENT_MSG%", Localization.instance.Localize(__instance.m_endMessage));
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
