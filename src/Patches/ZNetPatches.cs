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
                if (Plugin.StaticServerSetup.ContainsKey(Plugin.ServerSetup.IsServer))
                {
                    Plugin.StaticServerSetup[Plugin.ServerSetup.IsServer] = server == true;
                }
                else
                {
                    Plugin.StaticServerSetup.Add(Plugin.ServerSetup.IsServer, server == true);
                }

                if (Plugin.StaticServerSetup.ContainsKey(Plugin.ServerSetup.IsOpenServer))
                {
                    Plugin.StaticServerSetup[Plugin.ServerSetup.IsOpenServer] = openServer == true;
                }
                else
                {
                    Plugin.StaticServerSetup.Add(Plugin.ServerSetup.IsOpenServer, openServer == true);
                }

                if (Plugin.StaticServerSetup.ContainsKey(Plugin.ServerSetup.IsPublicServer))
                {
                    Plugin.StaticServerSetup[Plugin.ServerSetup.IsPublicServer] = publicServer == true;
                }
                else
                {
                    Plugin.StaticServerSetup.Add(Plugin.ServerSetup.IsPublicServer, publicServer == true);
                }

                if (Plugin.StaticServerInfo.ContainsKey(Plugin.ServerInfo.WorldName))
                {
                    Plugin.StaticServerInfo[Plugin.ServerInfo.WorldName] = $"{world.m_name}";
                }
                else
                {
                    Plugin.StaticServerInfo.Add(Plugin.ServerInfo.WorldName, $"{world.m_name}");
                }

                if (Plugin.StaticServerInfo.ContainsKey(Plugin.ServerInfo.WorldSeed))
                {
                    Plugin.StaticServerInfo[Plugin.ServerInfo.WorldSeed] = $"{world.m_seed}";
                }
                else
                {
                    Plugin.StaticServerInfo.Add(Plugin.ServerInfo.WorldSeed, $"{world.m_seed}");
                }

                if (Plugin.StaticServerInfo.ContainsKey(Plugin.ServerInfo.WorldSeedName))
                {
                    Plugin.StaticServerInfo[Plugin.ServerInfo.WorldSeedName] = $"{world.m_seedName}";
                }
                else
                {
                    Plugin.StaticServerInfo.Add(Plugin.ServerInfo.WorldSeedName, $"{world.m_seedName}");
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
