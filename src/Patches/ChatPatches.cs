using HarmonyLib;
using UnityEngine;

namespace DiscordConnector.Patches
{
    internal class ChatPatches
    {

        [HarmonyPatch(typeof(Chat), nameof(Chat.OnNewChatMessage))]
        internal class OnNewChatMessage
        {
            private static void Prefix(ref GameObject go, ref long senderID, ref Vector3 pos, ref Talker.Type type, ref string user, ref string text)
            {
                if (Plugin.StaticConfig.MutedPlayers.IndexOf(user) >= 0)
                {
                    Plugin.StaticLogger.LogInfo($"Ignored shout from user on muted list. User: {user} Shout: {text}. Index {Plugin.StaticConfig.MutedPlayers.IndexOf(user)}");
                    return;
                }
                ulong peerSteamID = 0;
                ZNetPeer peerInstance = ZNet.instance.GetPeerByPlayerName(user);
                if (peerInstance != null)
                {
                    peerSteamID = ((ZSteamSocket)peerInstance.m_socket).GetPeerID().m_SteamID; // Get the SteamID from peer.
                }
                switch (type)
                {
                    case Talker.Type.Ping:
                        if (Plugin.StaticConfig.AnnouncePlayerFirstPingEnabled && Plugin.StaticDatabase.CountOfRecordsByName(Records.Categories.Ping, user) == 0)
                        {
                            DiscordApi.SendMessage(
                                MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PlayerFirstPingMessage, user)
                            );
                        }
                        if (Plugin.StaticConfig.StatsPingEnabled)
                        {
                            Plugin.StaticDatabase.InsertSimpleStatRecord(Records.Categories.Ping, user, peerSteamID, pos);
                        }
                        if (Plugin.StaticConfig.ChatPingEnabled)
                        {
                            string message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PingMessage, user);
                            if (Plugin.StaticConfig.ChatPingPosEnabled)
                            {
                                if (Plugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                                {
                                    DiscordApi.SendMessage(
                                        message,
                                        pos
                                    );
                                    break;
                                }
                                message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PingMessage, user, pos);
                            }
                            if (message.Contains("%POS%"))
                            {
                                message.Replace("%POS%", "");
                            }
                            DiscordApi.SendMessage(message);
                        }
                        break;
                    case Talker.Type.Shout:
                        if (text.Equals("I have arrived!"))
                        {
                            if (!Plugin.IsHeadless())
                            {
                                if (Plugin.StaticConfig.AnnouncePlayerFirstJoinEnabled && Plugin.StaticDatabase.CountOfRecordsByName(Records.Categories.Join, user) == 0)
                                {
                                    DiscordApi.SendMessage(
                                        MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PlayerFirstJoinMessage, user)
                                    );
                                }
                                if (Plugin.StaticConfig.StatsJoinEnabled)
                                {
                                    Plugin.StaticDatabase.InsertSimpleStatRecord(Records.Categories.Join, user, peerSteamID, pos);
                                }
                                if (Plugin.StaticConfig.PlayerJoinMessageEnabled)
                                {
                                    string message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.JoinMessage, user);
                                    if (Plugin.StaticConfig.PlayerJoinPosEnabled)
                                    {
                                        if (Plugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                                        {
                                            DiscordApi.SendMessage(
                                                message,
                                                pos
                                            );
                                            break;
                                        }
                                        message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.JoinMessage, user, pos);
                                    }
                                    if (message.Contains("%POS%"))
                                    {
                                        message.Replace("%POS%", "");
                                    }
                                    DiscordApi.SendMessage(message);
                                }
                            }
                        }
                        else
                        {
                            if (Plugin.StaticConfig.AnnouncePlayerFirstShoutEnabled && Plugin.StaticDatabase.CountOfRecordsByName(Records.Categories.Shout, user) == 0)
                            {
                                DiscordApi.SendMessage(
                                    MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PlayerFirstShoutMessage, user, text)
                                );
                            }
                            if (Plugin.StaticConfig.StatsShoutEnabled)
                            {
                                Plugin.StaticDatabase.InsertSimpleStatRecord(Records.Categories.Shout, user, peerSteamID, pos);
                            }
                            if (Plugin.StaticConfig.ChatShoutEnabled)
                            {
                                string message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.ShoutMessage, user, text);
                                if (Plugin.StaticConfig.ChatShoutPosEnabled)
                                {
                                    if (Plugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                                    {
                                        DiscordApi.SendMessage(
                                            message,
                                            pos
                                        );
                                        break;
                                    }
                                    message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.ShoutMessage, user, text, pos);
                                }
                                if (message.Contains("%POS%"))
                                {
                                    message.Replace("%POS%", "");
                                }
                                DiscordApi.SendMessage(message);
                            }
                        }
                        break;
                    default:
                        Plugin.StaticLogger.LogDebug(
                            $"Unmatched chat message. [{type}] {user}: {text} at {pos}"
                        );
                        break;
                }

            }
        }
    }
}
