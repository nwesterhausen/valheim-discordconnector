using HarmonyLib;

namespace DiscordConnector.Patches;
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
                    Webhook.Event.ServerLaunch,
                    MessageTransformer.FormatServerMessage(Plugin.StaticConfig.LaunchMessage)
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
                    Webhook.Event.ServerStop,
                    MessageTransformer.FormatServerMessage(Plugin.StaticConfig.StopMessage)
                );
            }
            if (Plugin.IsHeadless())
            {
                Plugin.StaticEventWatcher.Dispose();
            }
        }
        private static void Postfix()
        {
            if (Plugin.StaticConfig.ShutdownMessageEnabled)
            {
                DiscordApi.SendMessage(
                    Webhook.Event.ServerShutdown,
                    MessageTransformer.FormatServerMessage(Plugin.StaticConfig.ShutdownMessage)
                );
            }
        }
    }
}
