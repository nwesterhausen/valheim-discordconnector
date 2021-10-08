using HarmonyLib;
using UnityEngine;

namespace DiscordConnector.Patches
{
    internal class RandEventSystemPatches
    {

        [HarmonyPatch(typeof(RandEventSystem), nameof(RandEventSystem.SetRandomEvent))]
        internal class SetRandomEvent
        {

            private static void Postfix(ref RandomEvent ev, ref Vector3 pos)
            {
                if (ev == null)
                {
                    Plugin.StaticLogger.LogDebug(
                        $"Random event cleared at {pos}"
                    );
                    return;
                }
                bool active = ev.m_active;
                float duration = ev.m_duration;
                string name = ev.m_name;
                string message = ev.m_startMessage;
                Plugin.StaticLogger.LogDebug(
                    $"Random event setRE? {name}: {active} for {duration} at {pos}. \"{message}\""
                );
            }
        }

        [HarmonyPatch(typeof(RandomEvent), nameof(RandomEvent.OnActivate))]
        internal class OnActivate
        {

            private static void Postfix(ref RandomEvent __instance)
            {
                bool active = __instance.m_active;
                float duration = __instance.m_duration;
                string name = __instance.m_name;
                string message = __instance.m_startMessage;
                Vector3 pos = __instance.m_pos;
                Plugin.StaticLogger.LogDebug(
                    $"Random event onStart? {name}: {active} for {duration} at {pos}. \"{message}\""
                );
            }
        }
    }
}
