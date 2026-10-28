using DiscordConnector.Patches;

namespace DiscordConnector.RPC;

/// <summary>
///     Receives chat messages forwarded by the optional DiscordConnector client plugin.
///     The server also sees shouts and pings on its own (see <see cref="ChatRelay" />); the client plugin is still
///     useful for shouts from a player who is alone on the server, which the game never sends to the server.
/// </summary>
public static class Server
{
    internal static void RPC_OnNewChatMessage(long sender, ZPackage pkg)
    {
        DiscordConnectorPlugin.StaticLogger.LogDebug($"Server Received {Common.RPC_OnNewChatMessage} from {sender}");

        if (sender == 0)
        {
            DiscordConnectorPlugin.StaticLogger.LogWarning($"Server.{Common.RPC_OnNewChatMessage}: Sender is 0");
            return;
        }

        ZNetPeer peer = ZNet.instance.GetPeer(sender);
        if (peer == null)
        {
            DiscordConnectorPlugin.StaticLogger.LogWarning($"Server.{Common.RPC_OnNewChatMessage}: Peer is null");
            return;
        }

        ChatMessageDetail chatMessageDetail;
        try
        {
            chatMessageDetail = ChatMessageDetail.FromZPackage(pkg);
        }
        catch (System.Exception e)
        {
            DiscordConnectorPlugin.StaticLogger.LogWarning(
                $"Server.{Common.RPC_OnNewChatMessage}: could not read message from {peer.m_playerName}: {e.Message}");
            return;
        }

        DiscordConnectorPlugin.StaticLogger.LogDebug($"Pkg: {chatMessageDetail}");

        if (chatMessageDetail.TalkerType == Talker.Type.Shout)
        {
            if (chatMessageDetail.Text == ChatMessageDetail.EmptyTextMessage)
            {
                return;
            }

            if (chatMessageDetail.Text == ChatPatches.ArrivalShout)
            {
                // Arrives around the same time as the character ID; Join ignores whichever comes second.
                Handlers.Join(peer);
            }
        }

        ChatRelay.Report(peer, chatMessageDetail.TalkerType, chatMessageDetail.Pos, chatMessageDetail.Text,
            ChatRelay.ClientPluginTarget, 0);
    }
}
