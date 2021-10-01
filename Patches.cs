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
        }

        [HarmonyPatch(typeof(ZNet),  nameof(ZNet.Shutdown))]
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
}