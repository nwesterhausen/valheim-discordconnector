using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace DiscordConnector.Patches
{
    internal class ZNetPatches
    {

        [HarmonyPatch(typeof(ZNet), nameof(ZNet.LoadWorld))]
        internal class LoadWorld
        {
            private static void Postfix(ref ZNet __instance)
            {
                if (Plugin.StaticConfig.LoadedMessageEnabled)
                {
                    DiscordApi.SendMessage(
                        Plugin.StaticConfig.LoadedMessage
                    );
                }
            }
            private static void Prefix(ref ZNet __instance)
            {
                if (Plugin.StaticConfig.LaunchMessageEnabled)
                {
                    DiscordApi.SendMessage(
                        Plugin.StaticConfig.LaunchMessage
                    );
                }
            }
        }

        [HarmonyPatch(typeof(ZNet), nameof(ZNet.Shutdown))]
        internal class Shutdown
        {
            private static void Prefix(ref ZNet __instance)
            {
                if (Plugin.StaticConfig.StopMessageEnabled)
                {
                    DiscordApi.SendMessage(
                        Plugin.StaticConfig.StopMessage
                        );
                }
            }
        }

        [HarmonyPatch(typeof(ZNet), nameof(ZNet.RPC_CharacterID))]
        internal class RPC_CharacterID
        {
            private static List<long> joinedPlayers = new List<long>();
            private static void Postfix(ZRpc rpc, ZDOID characterID)
            {
                ZNetPeer peer = ZNet.instance.GetPeer(rpc);
                if (peer == null)
                {
                    return;
                }
                if (joinedPlayers.IndexOf(peer.m_uid) >= 0)
                {
                    // Seems that player is dead if character ZDOID id is 0
                    // m_characterID id=0 means dead, user_id always matches peer.m_uid
                    if (peer.m_characterID.id != 0)
                    {
                        return;
                    }
                    if (Plugin.StaticConfig.PlayerDeathMessageEnabled)
                    {
                        string message = Plugin.StaticConfig.DeathMessage.Replace("%PLAYER_NAME%", peer.m_playerName);
                        if (Plugin.StaticConfig.PlayerDeathPosEnabled)
                        {
                            DiscordApi.SendMessage(
                            message,
                            peer.m_refPos
                            );
                        }
                        else
                        {
                            DiscordApi.SendMessage(message);
                        }
                    }
                    if (Plugin.StaticConfig.StatsDeathEnabled)
                    {
                        Plugin.StaticRecords.Store(Categories.Death, peer.m_playerName, 1);
                    }
                }
                else
                {
                    // PLAYER JOINED
                    joinedPlayers.Add(peer.m_uid);
                    if (Plugin.StaticConfig.PlayerJoinMessageEnabled)
                    {
                        string message = Plugin.StaticConfig.JoinMessage.Replace("%PLAYER_NAME%", peer.m_playerName);
                        if (Plugin.StaticConfig.PlayerJoinPosEnabled)
                        {
                            DiscordApi.SendMessage(
                            message,
                            peer.m_refPos
                            );
                        }
                        else
                        {
                            DiscordApi.SendMessage(message);
                        }
                    }
                    if (Plugin.StaticConfig.StatsJoinEnabled)
                    {
                        Plugin.StaticRecords.Store(Categories.Join, peer.m_playerName, 1);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(ZNet), nameof(ZNet.RPC_Disconnect))]
        internal class RPC_Disconnect
        {
            private static void Prefix(ZRpc rpc)
            {
                ZNetPeer peer = ZNet.instance.GetPeer(rpc);
                if (peer != null && peer.m_uid != 0)
                {
                    if (Plugin.StaticConfig.PlayerLeaveMessageEnabled)
                    {
                        string message = Plugin.StaticConfig.LeaveMessage.Replace("%PLAYER_NAME%", peer.m_playerName);
                        if (Plugin.StaticConfig.PlayerLeavePosEnabled)
                        {
                            DiscordApi.SendMessage(
                            message,
                            peer.m_refPos
                            );
                        }
                        else
                        {
                            DiscordApi.SendMessage(
                            message
                            );
                        }
                    }
                    if (Plugin.StaticConfig.StatsLeaveEnabled)
                    {
                        Plugin.StaticRecords.Store(Categories.Leave, peer.m_playerName, 1);
                    }
                }
            }
        }
    }

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
                switch (type)
                {
                    case Talker.Type.Ping:
                        if (Plugin.StaticConfig.ChatPingEnabled)
                        {
                            string message = Plugin.StaticConfig.PingMessage.Replace("%PLAYER_NAME%", user);
                            if (Plugin.StaticConfig.ChatPingPosEnabled)
                            {
                                DiscordApi.SendMessage(
                                    message,
                                    pos
                                );
                            }
                            else
                            {
                                DiscordApi.SendMessage(message);
                            }
                        }
                        if (Plugin.StaticConfig.StatsPingEnabled)
                        {
                            Plugin.StaticRecords.Store(Categories.Ping, user, 1);
                        }
                        break;
                    case Talker.Type.Shout:
                        if (text.Equals("I have arrived!"))
                        {
                            if (!Plugin.IsHeadless())
                            {
                                if (Plugin.StaticConfig.PlayerJoinMessageEnabled)
                                {
                                    string message = Plugin.StaticConfig.JoinMessage.Replace("%PLAYER_NAME%", user);
                                    if (Plugin.StaticConfig.PlayerJoinPosEnabled)
                                    {
                                        DiscordApi.SendMessage(
                                        message,
                                        pos
                                        );
                                    }
                                    else
                                    {
                                        DiscordApi.SendMessage(message);
                                    }
                                }
                                if (Plugin.StaticConfig.StatsJoinEnabled)
                                {
                                    Plugin.StaticRecords.Store(Categories.Join, user, 1);
                                }
                            }
                            Plugin.StaticLogger.LogDebug(
                                $"{user} shouts 'I have arrived!'"
                            );
                        }
                        else
                        {
                            if (Plugin.StaticConfig.ChatShoutEnabled)
                            {
                                string message = Plugin.StaticConfig.ShoutMessage.Replace("%PLAYER_NAME%", user).Replace("%SHOUT%", text);
                                if (Plugin.StaticConfig.ChatShoutPosEnabled)
                                {
                                    DiscordApi.SendMessage(
                                        message,
                                        pos
                                    );
                                }
                                else
                                {
                                    DiscordApi.SendMessage(message);
                                }
                            }
                            if (Plugin.StaticConfig.StatsShoutEnabled)
                            {
                                Plugin.StaticRecords.Store(Categories.Shout, user, 1);
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