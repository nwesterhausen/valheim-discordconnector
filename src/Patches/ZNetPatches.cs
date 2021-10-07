using System.Collections.Generic;
using HarmonyLib;

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
                    if (peer.m_characterID.id != 0 || !Plugin.StaticConfig.PlayerDeathMessageEnabled)
                    {
                        return;
                    }
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
                    if (Plugin.StaticConfig.StatsDeathEnabled)
                    {
                        Plugin.StaticRecords.Store(Categories.Death, peer.m_playerName, 1);
                    }
                }
                else if (Plugin.StaticConfig.PlayerJoinMessageEnabled)
                {
                    // PLAYER JOINED
                    joinedPlayers.Add(peer.m_uid);
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
                if (!Plugin.StaticConfig.PlayerLeaveMessageEnabled)
                {
                    return;
                }
                ZNetPeer peer = ZNet.instance.GetPeer(rpc);
                if (peer != null && peer.m_uid != 0)
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
                    if (Plugin.StaticConfig.StatsLeaveEnabled)
                    {
                        Plugin.StaticRecords.Store(Categories.Leave, peer.m_playerName, 1);
                    }
                }
            }
        }
    }
}