using System;
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
                string message = Localization.m_instance.Localize(__instance.m_startMessage);
                float time = __instance.m_time;
                float remaining = duration - time;
                Vector3 pos = __instance.m_pos;
                Plugin.StaticLogger.LogDebug(
                    $"Random event OnActivate {name}: {active} for {duration} at {pos}. (time: {time})"
                );

                DiscordApi.SendMessage(
                    $"**Event**: {message} at {pos}!"
                );
                
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
                    $"Random event OnDeactivate {name}: {active} for {duration} at {pos}. (time: {time})"
                );

                if (!end)
                {
                    DiscordApi.SendMessage(
                        $"**Event**: paused, no players in area!"
                    );
                }
                else
                {
                    DiscordApi.SendMessage(
                        $"**Event**: {Localization.m_instance.Localize(__instance.m_endMessage)}"
                    );
                }
            }
        }
    }
}
