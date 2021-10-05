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
                        DiscordApi.SendMessage(
                          $"{peer.m_playerName} {Plugin.StaticConfig.LeaveMessage}"
                        );
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
                    switch (type)
                    {
                        case Talker.Type.Ping:
                            if (Plugin.StaticConfig.ChatPingEnabled)
                            {
                                DiscordApi.SendMessage(
                                    $"{user} pings the map at {pos}"
                                );
                                if (Plugin.StaticConfig.StatsPingEnabled)
                                {
                                    Plugin.StaticRecords.Store(Categories.Ping, user, 1);
                                }
                            }
                            break;
                        case Talker.Type.Shout:
                            if (text.Equals("I have arrived!"))
                            {
                                if (!BepInEx.Paths.ProcessName.Equals("valheim_server.exe"))
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

                                if (Plugin.StaticConfig.ChatShoutPosEnabled)
                                {
                                    DiscordApi.SendMessage(
                                        $"{user} shouts {text} from {pos}!"
                                    );
                                    if (Plugin.StaticConfig.StatsShoutEnabled)
                                    {
                                        Plugin.StaticRecords.Store(Categories.Shout, user, 1);
                                    }
                                }
                                else
                                {
                                    DiscordApi.SendMessage(
                                        $"{user} shouts {text}!"
                                    );
                                    if (Plugin.StaticConfig.StatsShoutEnabled)
                                    {
                                        Plugin.StaticRecords.Store(Categories.Shout, user, 1);
                                    }
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