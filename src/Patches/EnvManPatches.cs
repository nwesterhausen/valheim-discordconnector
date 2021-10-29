using HarmonyLib;
using UnityEngine;

namespace DiscordConnector.Patches
{
    internal class EnvManPatches
    {

        [HarmonyPatch(typeof(EnvMan), nameof(EnvMan.OnMorning))]
        internal class OnMorning
        {
            private static void Postfix(EnvMan __instance)
            {
                Plugin.StaticLogger.LogDebug("I think it's morning.");
                if (Plugin.StaticConfig.AnnounceNewDay)
                {
                    string message = MessageTransformer.FormatNewDayMessage(__instance.GetCurrentDay());
                    DiscordApi.SendMessage(message);
                }
            }
        }
    }
}
