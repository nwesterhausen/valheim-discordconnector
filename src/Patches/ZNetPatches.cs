using System;
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

        [HarmonyPatch(typeof(ZNet), nameof(ZNet.SetServer))]
        internal class SetServer
        {
            private static void Postfix(ref bool server, ref bool openServer, ref bool publicServer, ref string serverName, ref string password, ref World world)
            {
                try
                {
                    Plugin.StaticServerSetup[Plugin.ServerSetup.IsServer] = server == true;
                    Plugin.StaticServerSetup[Plugin.ServerSetup.IsOpenServer] = openServer == true;
                    Plugin.StaticServerSetup[Plugin.ServerSetup.IsPublicServer] = publicServer == true;
                    Plugin.StaticServerInfo[Plugin.ServerInfo.WorldName] = $"{world.m_name}";
                    Plugin.StaticServerInfo[Plugin.ServerInfo.WorldSeed] = $"{world.m_seed}";
                    Plugin.StaticServerInfo[Plugin.ServerInfo.WorldSeedName] = $"{world.m_seedName}";
                }
                catch (NullReferenceException e)
                {
                    Plugin.StaticLogger.LogInfo($"Skipped loading world details due to {e.Message}");
                    Plugin.StaticLogger.LogDebug(e);
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
