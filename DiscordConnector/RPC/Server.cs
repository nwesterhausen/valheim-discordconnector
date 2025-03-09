using System;

using HarmonyLib;

namespace DiscordConnector.RPC;

public static class Server
{
    private static void RPC_OnNewChatMessage(long sender, ZPackage pkg)
    {
        DiscordConnectorPlugin.StaticLogger.LogDebug($"Received {Common.RPC_OnNewChatMessage} from {sender}");
        return;
        try
        {
            if (ZNet.instance.IsServer() && ZNet.instance.IsDedicated())
            {
                DiscordConnectorPlugin.StaticLogger.LogDebug("RPC_OnNewChatMessage: Server");
            }
            else
            {
                DiscordConnectorPlugin.StaticLogger.LogDebug("RPC_OnNewChatMessage: Client");
            }
        }
        catch (Exception e)
        {
            DiscordConnectorPlugin.StaticLogger.LogError(e.ToString());
        }
         
        ZNetPeer peer = ZNet.instance.GetPeer(sender);
        if (peer == null)
        {
            DiscordConnectorPlugin.StaticLogger.LogError("RPC_OnNewChatMessage: Peer not found");
            return;
        }
                     
        DiscordConnectorPlugin.StaticLogger.LogDebug(
            $"RPC_OnNewChatMessage: {peer.m_rpc.GetSocket().GetHostName()}");
        ChatMessageDetail chatMessageDetail = ChatMessageDetail.FromZPackage(pkg);
        DiscordConnectorPlugin.StaticLogger.LogDebug(
            $"Pkg: {chatMessageDetail}");
    }
    
    [HarmonyPatch(typeof(ZNet), nameof(ZNet.Awake))]
    public static class RegisterCustomRpc
    {
        [HarmonyPostfix]
        private static void Postfix(ZNet __instance)
        {
            if (!__instance.IsServer())
            {
                return;
            }
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Registering RPC_OnNewChatMessage as '{Common.RPC_OnNewChatMessage}'");
            
            ZRoutedRpc.instance.Register<ZPackage>(Common.RPC_OnNewChatMessage, RPC_OnNewChatMessage);
            
            // Register OnNewChatMessage RPC
            //ZRoutedRpc.instance.Register<long, ZPackage>(Common.RPC_OnNewChatMessage, new Action<ZPackage>(RPC_OnNewChatMessage));

        }
    }
}
