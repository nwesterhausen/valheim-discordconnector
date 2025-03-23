using System;
using System.Collections;

using DiscordConnector.Patches;

using HarmonyLib;

namespace DiscordConnector.RPC;

public static class RPCServer
{
    internal static IEnumerator RPC_OnNewChatMessage(long sender, ZPackage pkg)
    {
        DiscordConnectorPlugin.StaticLogger.LogDebug($"Server Received {Common.RPC_OnNewChatMessage} from {sender}");
        
        if (sender == 0)
        {
            DiscordConnectorPlugin.StaticLogger.LogError($"Server.{Common.RPC_OnNewChatMessage}: Sender is 0");
            yield break;
        }
         
        ZNetPeer peer = ZNet.instance.GetPeer(sender);
        if (peer == null)
        {
            DiscordConnectorPlugin.StaticLogger.LogError($"Server.{Common.RPC_OnNewChatMessage}: Peer is null");
            yield break;
        }
                     
        string hostName = peer.m_socket.GetHostName();
        string playerName = peer.m_playerName;
        DiscordConnectorPlugin.StaticLogger.LogDebug(
            $"Server.{Common.RPC_OnNewChatMessage}: Message from {playerName}/{hostName}");
        ChatMessageDetail chatMessageDetail = ChatMessageDetail.FromZPackage(pkg);
        DiscordConnectorPlugin.StaticLogger.LogDebug(
            $"Pkg: {chatMessageDetail}");
        
        if (chatMessageDetail == null)
        {
            DiscordConnectorPlugin.StaticLogger.LogError(
                $"Server.{Common.RPC_OnNewChatMessage}: ChatMessageDetail is null");
            yield break;
        }

        switch (chatMessageDetail.TalkerType)   
        {
            case Talker.Type.Normal:
                // Handlers.PlayerChat(peer, chatMessageDetail.Pos, chatMessageDetail.Text);
                DiscordConnectorPlugin.StaticLogger.LogInfo(
                    $"Server.{Common.RPC_OnNewChatMessage}: Normal message '{chatMessageDetail.Text}' at {chatMessageDetail.Pos}");
                break;
            case Talker.Type.Shout:
                if (chatMessageDetail.Text == ChatMessageDetail.EmptyTextMessage)
                {
                    DiscordConnectorPlugin.StaticLogger.LogDebug($"Skipping shout message '{ChatMessageDetail.EmptyTextMessage}'");
                    yield break;
                }
                if (chatMessageDetail.Text == ChatPatches.ArrivalShout)
                {
                    DiscordConnectorPlugin.StaticLogger.LogInfo(
                        $"Server.{Common.RPC_OnNewChatMessage}: Arrival shout message '{ChatPatches.ArrivalShout}' at {chatMessageDetail.Pos}");
                    Handlers.Join(peer);
                    yield break;
                }
                
                Handlers.Shout(peer, chatMessageDetail.Pos, chatMessageDetail.Text);
                break;
            case Talker.Type.Whisper:
                // Handlers.Whisper(peer, chatMessageDetail.Pos, chatMessageDetail.Text);
                DiscordConnectorPlugin.StaticLogger.LogInfo(
                    $"Server.{Common.RPC_OnNewChatMessage}: Whisper message '{chatMessageDetail.Text}' at {chatMessageDetail.Pos}");
                break;
            case Talker.Type.Ping:
                Handlers.Ping(peer, chatMessageDetail.Pos);
                break;
            default:
                DiscordConnectorPlugin.StaticLogger.LogError(
                    $"Server.{Common.RPC_OnNewChatMessage}: Unknown TalkerType {chatMessageDetail.TalkerType}");
                break;
            
        }
        
    }
}
