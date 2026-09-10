using System;
using System.Collections.Generic;

using UnityEngine;

namespace DiscordConnector.RPC;

internal static class InGameChatRelay
{
    private const float NormalDistance = 15f;
    private const float WhisperDistance = 4f;
    private static readonly char[] s_idSeparators = [';', ',', ' ', '\r', '\n', '\t'];

    internal static void TryRelay(ZNetPeer senderPeer, ChatMessageDetail chatMessageDetail)
    {
        if (!DiscordConnectorPlugin.StaticConfig.RelaySelectedPlayerChatToGame)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(chatMessageDetail.Text) ||
            chatMessageDetail.Text == ChatMessageDetail.EmptyTextMessage)
        {
            return;
        }

        string hostName = senderPeer.m_socket?.GetHostName() ?? "";
        if (!IsRelayedPlayer(hostName))
        {
            return;
        }

        if (!TryCreateSenderUserInfo(senderPeer, out UserInfo senderUserInfo))
        {
            DiscordConnectorPlugin.StaticLogger.LogWarning(
                $"In-game chat relay skipped for {senderPeer.m_playerName}/{hostName}: player info was not available");
            return;
        }

        int sent = 0;
        foreach (ZNet.PlayerInfo playerInfo in ZNet.instance.GetPlayerList())
        {
            if (playerInfo.m_characterID == ZDOID.None)
            {
                continue;
            }

            long targetPeerId = playerInfo.m_characterID.UserID;
            if (targetPeerId == senderPeer.m_uid)
            {
                continue;
            }

            ZNetPeer targetPeer = ZNet.instance.GetPeer(targetPeerId);
            if (targetPeer == null || !targetPeer.IsReady())
            {
                continue;
            }

            if (!ShouldSendToTarget(chatMessageDetail.TalkerType, chatMessageDetail.Pos, targetPeer.GetRefPos()))
            {
                continue;
            }

            ZRoutedRpc.instance.InvokeRoutedRPC(targetPeerId,
                "ChatMessage",
                chatMessageDetail.Pos,
                (int)chatMessageDetail.TalkerType,
                senderUserInfo,
                chatMessageDetail.Text);
            sent++;
        }

        DiscordConnectorPlugin.StaticLogger.LogInfo(
            $"Relayed {chatMessageDetail.TalkerType} chat from {senderPeer.m_playerName}/{hostName} to {sent} player(s)");
    }

    private static bool IsRelayedPlayer(string hostName)
    {
        if (string.IsNullOrWhiteSpace(hostName))
        {
            return false;
        }

        HashSet<string> relayedIds = GetRelayedPlayerIds();
        return relayedIds.Contains(hostName);
    }

    private static HashSet<string> GetRelayedPlayerIds()
    {
        HashSet<string> relayedIds = new(StringComparer.Ordinal);
        string configuredIds = DiscordConnectorPlugin.StaticConfig.RelayedPlayerChatSteamIds;
        if (string.IsNullOrWhiteSpace(configuredIds))
        {
            return relayedIds;
        }

        foreach (string rawId in configuredIds.Split(s_idSeparators, StringSplitOptions.RemoveEmptyEntries))
        {
            string id = rawId.Trim();
            if (!string.IsNullOrWhiteSpace(id))
            {
                relayedIds.Add(id);
            }
        }

        return relayedIds;
    }

    private static bool TryCreateSenderUserInfo(ZNetPeer senderPeer, out UserInfo senderUserInfo)
    {
        foreach (ZNet.PlayerInfo playerInfo in ZNet.instance.GetPlayerList())
        {
            if (playerInfo.m_characterID != senderPeer.m_characterID)
            {
                continue;
            }

            senderUserInfo = new UserInfo
            {
                Name = playerInfo.m_name,
                UserId = playerInfo.m_userInfo.m_id
            };
            return true;
        }

        senderUserInfo = new UserInfo();
        return false;
    }

    private static bool ShouldSendToTarget(Talker.Type talkerType, Vector3 sourcePosition, Vector3 targetPosition)
    {
        return talkerType switch
        {
            Talker.Type.Shout => true,
            Talker.Type.Normal => Vector3.Distance(sourcePosition, targetPosition) <= NormalDistance,
            Talker.Type.Whisper => Vector3.Distance(sourcePosition, targetPosition) <= WhisperDistance,
            _ => false
        };
    }
}
