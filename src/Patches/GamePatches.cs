using System.Collections.Generic;
using HarmonyLib;

namespace DiscordConnector.Patches
{
    internal class GamePatches
    {

        [HarmonyPatch(typeof(Game), nameof(Game.Awake))]
        internal class LoadWorld
        {
            private static void Prefix()
            {
                if (Plugin.StaticConfig.LaunchMessageEnabled)
                {
                    DiscordApi.SendMessage(
                        Plugin.StaticConfig.LaunchMessage.Replace("%PUBLICIP%", Plugin.PublicIpAddress)
                    );
                }
            }
        }


        [HarmonyPatch(typeof(Game), nameof(Game.OnApplicationQuit))]
        internal class Shutdown
        {
            [HarmonyBefore(new string[] { "HackShardGaming.WorldofValheimServerSideCharacters" })]
            private static void Prefix()
            {
                if (Plugin.StaticConfig.StopMessageEnabled)
                {
                    DiscordApi.SendMessage(
                        Plugin.StaticConfig.StopMessage.Replace("%PUBLICIP%", Plugin.PublicIpAddress)
                        );
                }
            }
            private static void Postfix()
            {
                if (Plugin.StaticConfig.ShutdownMessageEnabled)
                {
                    DiscordApi.SendMessage(
                        Plugin.StaticConfig.ShutdownMessage.Replace("%PUBLICIP%", Plugin.PublicIpAddress)
                        );
                }
            }
        }
    }
}
