using System;
using System.Collections.Generic;
using HarmonyLib;
using System.Linq;

namespace DiscordConnector.Patches
{
    internal class ZNetPatches
    {
        [HarmonyPatch(typeof(ZNet), nameof(ZNet.UpdatePlayerList))]
        internal class UpdatePlayerList
        {
            private static void Postfix()
            {
                // Do nothing if we know of all players
                if (ZNet.instance.m_players.Count == Handlers.joinedPlayers.Count)
                {
                    return;
                }

                // Clear joinedPlayers if there are no players on the server!
                if (ZNet.instance.m_players.Count == 0 && Handlers.joinedPlayers.Count > 0)
                {
                    foreach (String playerHostName in Handlers.joinedPlayers)
                    {
                        // Online players is zero so force all players to "leave"
                        Handlers.ForceLeave(playerHostName);
                    }
                    // Ensure the joined players list is empty
                    Handlers.joinedPlayers.Clear();
                }
                else
                {
                    // If they aren't equal and m_players isn't empty then reconcile.
                    HashSet<string> onlinePlayers = new HashSet<string>();

                    // Fill online players list
                    foreach (ZNetPeer peer in ZNet.instance.m_peers)
                    {
                        onlinePlayers.Add(peer.m_socket.GetHostName());
                    }

                    // Get players that are online but not in joined list
                    HashSet<string> missingOnlinePlayers = new HashSet<string>(onlinePlayers.Except(Handlers.joinedPlayers));
                    // Get players that are in joined list but not online
                    HashSet<string> joinedOfflinePlayers = new HashSet<string>(Handlers.joinedPlayers.Except(onlinePlayers));

                    foreach (string playerHostName in missingOnlinePlayers)
                    {
                        ZNetPeer peer = ZNet.instance.GetPeerByHostName(playerHostName);
                        if (peer != null)
                        {
                            Handlers.Join(peer);
                        }
                    }
                    foreach (string playerHostName in joinedOfflinePlayers)
                    {
                        ZNetPeer peer = ZNet.instance.GetPeerByHostName(playerHostName);
                        if (peer != null)
                        {
                            Handlers.Leave(peer);
                        }
                    }


                }

            }

        }

        [HarmonyPatch(typeof(ZNet), nameof(ZNet.LoadWorld))]
        internal class LoadWorld
        {
            private static void Postfix()
            {
                if (Plugin.StaticConfig.LoadedMessageEnabled)
                {
                    Plugin.StaticLogger.LogDebug("sending message to discord, server reader");
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

                Handlers.Join(peer);

            }

            [HarmonyPatch(typeof(ZNet), nameof(ZNet.RPC_Disconnect))]
            internal class RPC_Disconnect
            {
                private static void Prefix(ZRpc rpc)
                {
                    ZNetPeer peer = ZNet.instance.GetPeer(rpc);
                    if (peer != null && peer.m_uid != 0)
                    {
                        Handlers.Leave(peer);
                    }
                }
            }
        }
    }
}
