using HarmonyLib;

namespace DiscordConnector.Patches
{
    internal class ZNetPatches
    {

        [HarmonyPatch(typeof(ZNet), "LoadWorld")]
        internal class LoadWorld
        {
            private static void Postfix(ref ZNet __instance)
            {
                DiscordApi.SendMessage("Server has started!");
            }
        }

        [HarmonyPatch(typeof(ZNet), "Shutdown")]
        internal class Shutdown
        {
            private static void Prefix(ref ZNet __instance)
            {
                DiscordApi.SendMessage("Server is shutting down!");
            }
        }
    }
}