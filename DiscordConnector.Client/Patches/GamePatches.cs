using System;

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
                new Action<long, ZPackage>(RPC.Client.RPC_OnNewChatMessage));
            DiscordConnectorClientPlugin.StaticLogger.LogInfo($"Registered RPC: {RPC.Common.RPC_OnNewChatMessage}");
        }
    }
}
