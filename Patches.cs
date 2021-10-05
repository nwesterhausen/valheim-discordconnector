using HarmonyLib;
using UnityEngine;
using System;

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
            private static void Postfix(ZRpc rpc, ZDOID characterID)
            {
                if (Plugin.StaticConfig.PlayerJoinMessageEnabled)
                {
                    ZNetPeer peer = ZNet.instance.GetPeer(rpc);
                    if (peer != null)
                    {
                        DiscordApi.SendMessage(
                          $"{peer.m_playerName} {Plugin.StaticConfig.JoinMessage}"
                        );
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
                        DiscordApi.SendMessage(
                          $"{peer.m_playerName} {Plugin.StaticConfig.LeaveMessage}"
                        );
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
                    switch (type)
                    {
                        case Talker.Type.Ping:
                            if (Plugin.StaticConfig.ChatPingEnabled)
                            {
                                DiscordApi.SendMessage(
                                    $"{user} pings the map at ${pos}"
                                );
                            }
                            break;
                        case Talker.Type.Shout:
                            if (text.Equals("I have arrived!"))
                            {
                                Plugin.StaticLogger.LogDebug(
                                    $"{user} shouts 'I have arrived!'"
                                );
                            }
                            else if (Plugin.StaticConfig.ChatShoutEnabled)
                            {

                                if (Plugin.StaticConfig.ChatShoutPosEnabled)
                                {
                                    DiscordApi.SendMessage(
                                        $"{user} shouts {text} from {pos}!"
                                    );
                                }
                                else
                                {
                                    DiscordApi.SendMessage(
                                        $"{user} shouts {text}!"
                                    );
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

    internal class EventPatches
    {
        [HarmonyPatch(typeof(RandEventSystem), nameof(RandEventSystem.RPC_SetEvent))]
        internal class RPC_SetEvent
        {
            private static void Postfix(ref long sender, ref string eventName, ref float time, ref Vector3 pos)
            {
                if (!String.IsNullOrEmpty(eventName))
                {
                    Plugin.StaticLogger.LogInfo(
                        $"Random event details: {eventName}{Environment.NewLine}sender: {sender}; time: {time} at {pos}"
                    );

                }
            }
        }
    }
}