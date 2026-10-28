using System;

using DiscordConnector.RPC;

using HarmonyLib;

namespace DiscordConnector.Patches;

internal class GamePatches
{
    [HarmonyPatch(typeof(Game), nameof(Game.Start))]
    internal static class GameStartPatch
    {
        private static void Prefix()
        {
            // Register RPC
            ZRoutedRpc.instance.Register(RPC.Common.RPC_OnNewChatMessage,
                new Action<long, ZPackage>(Server.RPC_OnNewChatMessage));
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Registered RPC: {RPC.Common.RPC_OnNewChatMessage}");
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.Awake))]
    internal class LoadWorld
    {
        private static void Prefix()
        {
            if (DiscordConnectorPlugin.StaticConfig.LaunchMessageEnabled)
            {
                DiscordApi.SendMessage(
                    Webhook.Event.ServerLaunch,
                    MessageTransformer.FormatServerMessage(DiscordConnectorPlugin.StaticConfig.LaunchMessage)
                );
            }
        }
    }


    [HarmonyPatch(typeof(Game), nameof(Game.OnApplicationQuit))]
    internal class Shutdown
    {
        [HarmonyBefore("HackShardGaming.WorldofValheimServerSideCharacters")]
        private static void Prefix()
        {
            if (HostPlayer.IsListenServer)
            {
                // The host quitting the game is also the host leaving.
                ZNetPeer? host = HostPlayer.Peer();
                if (host != null)
                {
                    Handlers.Leave(host);
                }
            }

            if (DiscordConnectorPlugin.StaticConfig.StopMessageEnabled)
            {
                DiscordApi.SendMessage(
                    Webhook.Event.ServerStop,
                    MessageTransformer.FormatServerMessage(DiscordConnectorPlugin.StaticConfig.StopMessage)
                );
            }

            if (DiscordConnectorPlugin.IsHeadless())
            {
                DiscordConnectorPlugin.StaticEventWatcher.Dispose();
            }
        }

        private static void Postfix()
        {
            if (DiscordConnectorPlugin.StaticConfig.ShutdownMessageEnabled)
            {
                DiscordApi.SendMessage(
                    Webhook.Event.ServerShutdown,
                    MessageTransformer.FormatServerMessage(DiscordConnectorPlugin.StaticConfig.ShutdownMessage)
                );
            }

            // The process exits right after this. Give queued messages (including the two above) a moment to go
            // out, otherwise the stop/shutdown messages are often lost.
            if (!DiscordApi.Flush(TimeSpan.FromSeconds(5)))
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning("Some Discord messages were not sent before shutdown.");
            }
        }
    }
}
