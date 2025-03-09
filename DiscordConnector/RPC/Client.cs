using System;

using HarmonyLib;

namespace DiscordConnector.RPC;

public class Client
{
    private static void RPC_OnNewChatMessage(ZRpc zRpc, ZPackage pkg)
    {
        // In the client, we don't need to do anything with the chat message.
    }


    [HarmonyPatch(typeof(ZNet), nameof(ZNet.OnNewConnection))]
    static class ClientRPCRegistrations
    {
        [HarmonyPrefix]
        static void Prefix(ZNet __instance, ZNetPeer peer)
        {
            if (__instance.IsServer())
            {
                return;
            }
            
            peer.m_rpc.Register<ZPackage>(Common.RPC_OnNewChatMessage, RPC_OnNewChatMessage);

            // Register OnNewChatMessage RPC
            //ZRoutedRpc.instance.Register<ZPackage>(Common.RPC_OnNewChatMessage,
            //    new Action<long, ZPackage>(RPC_OnNewChatMessage));
        }
    }
}
