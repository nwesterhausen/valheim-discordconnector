using System.Collections.Generic;
using HarmonyLib;

namespace DiscordConnector.Patches
{
    internal class ZNetPatches
    {

        [HarmonyPatch(typeof(ZNet), nameof(ZNet.LoadWorld))]
        internal class LoadWorld
        {
            private static void Postfix()
            {
                if (Plugin.StaticConfig.LoadedMessageEnabled)
                {
                    DiscordApi.SendMessage(
                        MessageTransformer.FormatServerMessage(Plugin.StaticConfig.LoadedMessage)
                    );
                }

                if (Plugin.IsHeadless())
                {
                    Plugin.StaticEventWatcher.Activate();
                }
            }
        }

        [HarmonyPatch(typeof(ZNet), nameof(ZNet.SaveWorld))]
        internal class SaveWorld
        {
            private static void Postfix(ref bool sync)
            {
                if (Plugin.StaticConfig.WorldSaveMessageEnabled)
                {
                    DiscordApi.SendMessage(
                        MessageTransformer.FormatServerMessage(Plugin.StaticConfig.SaveMessage)
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

                // Get the player's hostname to use for record keeping and logging
                string playerHostName = $"{peer.m_socket.GetHostName()}";

                if (joinedPlayers.IndexOf(peer.m_uid) >= 0)
                {
                    // Seems that player is dead if character ZDOID id is 0
                    // m_characterID id=0 means dead, user_id always matches peer.m_uid
                    if (peer.m_characterID.id != 0)
                    {
                        Plugin.StaticLogger.LogDebug($"Player \"join\" from someone already on the server. {playerHostName} peer_id:{peer.m_uid} ({peer.m_playerName})");
                        return;
                    }
                    if (Plugin.StaticConfig.PlayerDeathMessageEnabled)
                    {
                        if (Plugin.StaticConfig.AnnouncePlayerFirstDeathEnabled && Plugin.StaticDatabase.CountOfRecordsByName(Records.Categories.Death, peer.m_playerName) == 0)
                        {
                            string firstDeathMessage = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PlayerFirstDeathMessage, peer.m_playerName, playerHostName);
                            if (Plugin.StaticConfig.PlayerDeathPosEnabled)
                            {
                                if (Plugin.StaticConfig.DiscordEmbedsEnabled || !firstDeathMessage.Contains("%POS%"))
                                {
                                    DiscordApi.SendMessage(firstDeathMessage, peer.m_refPos);
                                }
                                else
                                {
                                    DiscordApi.SendMessage(MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PlayerFirstDeathMessage, peer.m_playerName, playerHostName, peer.m_refPos));
                                }
                            }
                            else
                            {
                                DiscordApi.SendMessage(firstDeathMessage);
                            }
                        }
                        else
                        {
                            string message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.DeathMessage, peer.m_playerName, playerHostName);
                            if (Plugin.StaticConfig.PlayerDeathPosEnabled)
                            {
                                if (Plugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                                {
                                    DiscordApi.SendMessage(message, peer.m_refPos);
                                }
                                else
                                {
                                    message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.DeathMessage, peer.m_playerName, playerHostName, peer.m_refPos);
                                    DiscordApi.SendMessage(message);
                                }
                            }
                            else
                            {
                                DiscordApi.SendMessage(message);
                            }
                        }
                    }

                    if (Plugin.StaticConfig.StatsDeathEnabled)
                    {
                        Plugin.StaticDatabase.InsertSimpleStatRecord(Records.Categories.Death, peer.m_playerName, playerHostName, peer.m_refPos);
                    }
                }
                else
                {
                    // PLAYER JOINED
                    joinedPlayers.Add(peer.m_uid);
                    Plugin.StaticLogger.LogDebug($"Added player {playerHostName} peer_id:{peer.m_uid} ({peer.m_playerName}) to joined player list.");
                    if (Plugin.StaticConfig.PlayerJoinMessageEnabled)
                    {
                        if (Plugin.StaticConfig.AnnouncePlayerFirstJoinEnabled && Plugin.StaticDatabase.CountOfRecordsByName(Records.Categories.Join, peer.m_playerName) == 0)
                        {
                            string firstJoinMessage = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PlayerFirstJoinMessage, peer.m_playerName, playerHostName);
                            if (Plugin.StaticConfig.PlayerJoinPosEnabled)
                            {
                                if (Plugin.StaticConfig.DiscordEmbedsEnabled || !firstJoinMessage.Contains("%POS%"))
                                {
                                    DiscordApi.SendMessage(firstJoinMessage, peer.m_refPos);
                                }
                                else
                                {
                                    firstJoinMessage = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.JoinMessage, peer.m_playerName, playerHostName, peer.m_refPos);
                                    DiscordApi.SendMessage(firstJoinMessage);
                                }
                            }
                            else
                            {
                                DiscordApi.SendMessage(
                                MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PlayerFirstJoinMessage, peer.m_playerName, playerHostName));
                            }
                        }
                        string message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.JoinMessage, peer.m_playerName, playerHostName);
                        if (Plugin.StaticConfig.PlayerJoinPosEnabled)
                        {
                            if (Plugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                            {
                                DiscordApi.SendMessage(message, peer.m_refPos);
                            }
                            else
                            {
                                message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.JoinMessage, peer.m_playerName, playerHostName, peer.m_refPos);
                                DiscordApi.SendMessage(message);
                            }
                        }
                        else
                        {
                            DiscordApi.SendMessage(message);
                        }
                    }

                    if (Plugin.StaticConfig.StatsJoinEnabled)
                    {
                        Plugin.StaticDatabase.InsertSimpleStatRecord(Records.Categories.Join, peer.m_playerName, playerHostName, peer.m_refPos);
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
                    // Get the player's hostname to use for record keeping and logging
                    string playerHostName = $"{peer.m_socket.GetHostName()}";
                    if (Plugin.StaticConfig.PlayerLeaveMessageEnabled)
                    {
                        if (Plugin.StaticConfig.AnnouncePlayerFirstLeaveEnabled && Plugin.StaticDatabase.CountOfRecordsByName(Records.Categories.Leave, peer.m_playerName) == 0)
                        {
                            string firstLeaveMessage = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PlayerFirstLeaveMessage, peer.m_playerName, playerHostName);
                            if (Plugin.StaticConfig.PlayerLeavePosEnabled)
                            {
                                if (Plugin.StaticConfig.DiscordEmbedsEnabled || !firstLeaveMessage.Contains("%POS%"))
                                {
                                    DiscordApi.SendMessage(firstLeaveMessage, peer.m_refPos);
                                }
                                else
                                {
                                    firstLeaveMessage = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PlayerFirstLeaveMessage, peer.m_playerName, playerHostName, peer.m_refPos);
                                    DiscordApi.SendMessage(firstLeaveMessage);
                                }
                            }
                            else
                            {
                                DiscordApi.SendMessage(firstLeaveMessage);
                            }
                        }

                        string message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.LeaveMessage, peer.m_playerName, playerHostName);
                        if (Plugin.StaticConfig.PlayerLeavePosEnabled)
                        {
                            if (Plugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                            {
                                DiscordApi.SendMessage(message, peer.m_refPos);
                            }
                            else
                            {
                                message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.LeaveMessage, peer.m_playerName, playerHostName, peer.m_refPos);
                                DiscordApi.SendMessage(message);
                            }

                        }
                        else
                        {
                            DiscordApi.SendMessage(message);
                        }
                    }

                    if (Plugin.StaticConfig.StatsLeaveEnabled)
                    {
                        Plugin.StaticDatabase.InsertSimpleStatRecord(Records.Categories.Leave, peer.m_playerName, playerHostName, peer.m_refPos);
                    }
                }
            }
        }
    }
}
