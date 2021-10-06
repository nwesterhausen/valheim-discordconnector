using System;
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
                if (peer != null)
                {
                    if (joinedPlayers.IndexOf(peer.m_uid) >= 0)
                    {
                        // Seems that player is dead if character ZDOID id is 0
                        // m_characterID id=0 means dead, user_id always matches peer.m_uid
                        if (peer.m_characterID.id == 0 && Plugin.StaticConfig.PlayerDeathMessageEnabled)
                        {
                            string message = $"{peer.m_playerName} {Plugin.StaticConfig.DeathMessage}";
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
                            if (Plugin.StaticConfig.StatsDeathEnabled)
                            {
                                Plugin.StaticRecords.Store(Categories.Death, peer.m_playerName, 1);
                            }
                        }
                    }
                    else if (Plugin.StaticConfig.PlayerJoinMessageEnabled)
                    {
                        // PLAYER JOINED
                        joinedPlayers.Add(peer.m_uid);
                        string message = $"{peer.m_playerName} {Plugin.StaticConfig.JoinMessage}";
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
                        if (Plugin.StaticConfig.StatsJoinEnabled)
                        {
                            Plugin.StaticRecords.Store(Categories.Join, peer.m_playerName, 1);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(ZNet), nameof(ZNet.RPC_Disconnect))]
        internal class RPC_Disconnect
        {
            private static void Prefix(ZRpc rpc)
            {
                if (Plugin.StaticConfig.PlayerLeaveMessageEnabled)
                {
                    ZNetPeer peer = ZNet.instance.GetPeer(rpc);
                    if (peer != null)
                    {
                        string message = $"{peer.m_playerName} {Plugin.StaticConfig.LeaveMessage}";
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
                        if (Plugin.StaticConfig.StatsLeaveEnabled)
                        {
                            Plugin.StaticRecords.Store(Categories.Leave, peer.m_playerName, 1);
                        }
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
                if (Plugin.StaticConfig.ChatMessageEnabled)
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
                                string message = $"{user} pings the map!";
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
                                if (Plugin.StaticConfig.StatsPingEnabled)
                                {
                                    Plugin.StaticRecords.Store(Categories.Ping, user, 1);
                                }
                            }
                            break;
                        case Talker.Type.Shout:
                            if (text.Equals("I have arrived!"))
                            {
                                if (!BepInEx.Paths.ProcessName.Equals("valheim_server"))
                                {
                                    DiscordApi.SendMessage(
                                        $"{user} {Plugin.StaticConfig.JoinMessage}"
                                    );
                                }
                                Plugin.StaticLogger.LogDebug(
                                    $"{user} shouts 'I have arrived!'"
                                );
                            }
                            else if (Plugin.StaticConfig.ChatShoutEnabled)
                            {
                                string message = $"{user} shouts: **{text}**!";
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
                                if (Plugin.StaticConfig.StatsShoutEnabled)
                                {
                                    Plugin.StaticRecords.Store(Categories.Shout, user, 1);
                                }
                            }
                            break;
                        default:
                            Plugin.StaticLogger.LogInfo(
                                $"Ignoring chat message. [{type}] {user}: {text} at {pos}"
                            );
                            break;
                    }
                }
            }
        }
    }
}