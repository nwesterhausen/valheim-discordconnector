using System.Collections.Generic;
using HarmonyLib;

namespace DiscordConnector.Patches
{
    [HarmonyPatch(typeof(ZNet))]
    internal class ZNetPatches
    {

        [HarmonyPatch(nameof(ZNet.LoadWorld))]
        internal class LoadWorld
        {
            private static void Postfix(ref ZNet __instance)
            {
                if (Plugin.StaticConfig.LoadedMessageEnabled)
                {
                    DiscordApi.SendMessage(
                        Plugin.StaticConfig.LoadedMessage.Replace("%PUBLICIP%", Plugin.PublicIpAddress)
                    );
                }
            }
            private static void Prefix(ref ZNet __instance)
            {
                if (Plugin.StaticConfig.LaunchMessageEnabled)
                {
                    DiscordApi.SendMessage(
                        Plugin.StaticConfig.LaunchMessage.Replace("%PUBLICIP%", Plugin.PublicIpAddress)
                    );
                }
            }
        }

        [HarmonyPatch(nameof(ZNet.SaveWorld))]
        internal class SaveWorld
        {
            private static void Postfix(ref bool sync)
            {
                if (Plugin.StaticConfig.WorldSaveMessageEnabled)
                {
                    DiscordApi.SendMessage(
                        Plugin.StaticConfig.SaveMessage.Replace("%PUBLICIP%", Plugin.PublicIpAddress)
                    );
                }
            }
        }

        [HarmonyPatch(nameof(ZNet.Shutdown))]
        internal class Shutdown
        {
            [HarmonyBefore(new string[] { "HackShardGaming.WorldofValheimServerSideCharacters" })]
            private static void Prefix(ref ZNet __instance)
            {
                if (Plugin.StaticConfig.StopMessageEnabled)
                {
                    DiscordApi.SendMessage(
                        Plugin.StaticConfig.StopMessage.Replace("%PUBLICIP%", Plugin.PublicIpAddress)
                        );
                }
            }
            private static void Postfix(ref ZNet __instance)
            {
                if (Plugin.StaticConfig.ShutdownMessageEnabled)
                {
                    DiscordApi.SendMessage(
                        Plugin.StaticConfig.ShutdownMessage.Replace("%PUBLICIP%", Plugin.PublicIpAddress)
                        );
                }
            }
        }

        [HarmonyPatch(nameof(ZNet.RPC_CharacterID))]
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
                        Plugin.StaticLogger.LogDebug($"Player \"join\" from someone already on the server: {peer.m_uid} ({peer.m_playerName})");
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
                    if (Plugin.StaticConfig.AnnouncePlayerFirstDeathEnabled && Plugin.StaticRecords.Retrieve(RecordCategories.Death, peer.m_playerName) == 0)
                    {
                        DiscordApi.SendMessage(Plugin.StaticConfig.PlayerFirstDeathMessage.Replace("%PLAYER_NAME%", peer.m_playerName));
                    }
                    if (Plugin.StaticConfig.StatsDeathEnabled)
                    {
                        Plugin.StaticRecords.Store(RecordCategories.Death, peer.m_playerName, 1);
                    }
                }
                else
                {
                    // PLAYER JOINED
                    joinedPlayers.Add(peer.m_uid);
                    Plugin.StaticLogger.LogDebug($"Added player {peer.m_uid} ({peer.m_playerName}) to joined player list.");
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
                    if (Plugin.StaticConfig.AnnouncePlayerFirstJoinEnabled && Plugin.StaticRecords.Retrieve(RecordCategories.Join, peer.m_playerName) == 0)
                    {
                        DiscordApi.SendMessage(Plugin.StaticConfig.PlayerFirstJoinMessage.Replace("%PLAYER_NAME%", peer.m_playerName));
                    }
                    if (Plugin.StaticConfig.StatsJoinEnabled)
                    {
                        Plugin.StaticRecords.Store(RecordCategories.Join, peer.m_playerName, 1);
                    }
                }
            }
        }

        [HarmonyPatch(nameof(ZNet.RPC_Disconnect))]
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
                    if (Plugin.StaticConfig.AnnouncePlayerFirstLeaveEnabled && Plugin.StaticRecords.Retrieve(RecordCategories.Leave, peer.m_playerName) == 0)
                    {
                        DiscordApi.SendMessage(Plugin.StaticConfig.PlayerFirstLeaveMessage.Replace("%PLAYER_NAME%", peer.m_playerName));
                    }
                    if (Plugin.StaticConfig.StatsLeaveEnabled)
                    {
                        Plugin.StaticRecords.Store(RecordCategories.Leave, peer.m_playerName, 1);
                    }
                }
            }
        }
    }
}
