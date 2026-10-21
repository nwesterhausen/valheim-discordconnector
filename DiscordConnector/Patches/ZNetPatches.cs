using System.Collections.Generic;

using HarmonyLib;

using UnityEngine;

namespace DiscordConnector.Patches;

internal class ZNetPatches
{
    private static ZNet? loadedWorld;
    private static ZNet? registeredWorld;
    private static object? registeredSession;
    private static bool startPending;
    private static float startDeadline;

    private static void SendServerStart()
    {
        startPending = false;
        DiscordApi.SendMessage(
            Webhook.Event.ServerStart,
            MessageTransformer.FormatServerMessage(DiscordConnectorPlugin.StaticConfig.LoadedMessage)
        );
    }

    private static void TrySendServerStart()
    {
        if (!startPending) return;
        if (loadedWorld != ZNet.instance || loadedWorld == null || loadedWorld.HaveStopped ||
            !DiscordConnectorPlugin.StaticConfig.LoadedMessageEnabled ||
            ZNet.m_onlineBackend != OnlineBackendType.PlayFab)
        {
            startPending = false;
            return;
        }

        if (Time.realtimeSinceStartup >= startDeadline)
        {
            startPending = false;
            DiscordConnectorPlugin.StaticLogger.LogWarning(
                "ServerStart suppressed: the current crossplay session did not become active with a join code within 180 seconds.");
            return;
        }

        var matchmaking = ZPlayFabMatchmaking.instance;
        var session = matchmaking.m_serverData;
        if (registeredWorld != loadedWorld || session == null || !ReferenceEquals(registeredSession, session) ||
            matchmaking.m_state != ZPlayFabMatchmaking.State.Active ||
            session.worldName != ZNet.World.m_seedName || string.IsNullOrEmpty(session.lobbyId) ||
            string.IsNullOrEmpty(session.joinCode) || session.joinCode != ZPlayFabMatchmaking.JoinCode)
        {
            return;
        }

        SendServerStart();
    }

    [HarmonyPatch(typeof(ZNet), nameof(ZNet.LoadWorld))]
    internal class LoadWorld
    {
        private static void Postfix(ZNet __instance)
        {
            if (__instance != ZNet.instance || loadedWorld == __instance) return;
            loadedWorld = __instance;
            startPending = DiscordConnectorPlugin.StaticConfig.LoadedMessageEnabled;
            startDeadline = Time.realtimeSinceStartup + 180f;
            if (startPending)
            {
                if (ZNet.m_onlineBackend == OnlineBackendType.PlayFab)
                {
                    TrySendServerStart();
                }
                else
                {
                    SendServerStart();
                }
            }

            if (DiscordConnectorPlugin.IsHeadless())
            {
                DiscordConnectorPlugin.StaticEventWatcher.Activate();
            }
        }
    }

    [HarmonyPatch(typeof(ZPlayFabMatchmaking), nameof(ZPlayFabMatchmaking.RegisterServer))]
    internal class RegisterServer
    {
        private static void Prefix(ZPlayFabMatchmaking __instance, out object? __state)
        {
            __state = __instance.m_serverData;
        }

        private static void Postfix(ZPlayFabMatchmaking __instance, object? __state)
        {
            // RegisterServer can defer until login; only bind a newly created session to this world.
            if (!ReferenceEquals(__instance, ZPlayFabMatchmaking.instance) || ZNet.instance == null ||
                !ZNet.instance.IsServer() || ZNet.instance.HaveStopped ||
                ZNet.m_onlineBackend != OnlineBackendType.PlayFab || __instance.m_serverData == null ||
                ReferenceEquals(__state, __instance.m_serverData) ||
                __instance.m_serverData.worldName != ZNet.World.m_seedName) return;

            registeredWorld = ZNet.instance;
            registeredSession = __instance.m_serverData;
            TrySendServerStart();
        }
    }

    [HarmonyPatch(typeof(ZPlayFabMatchmaking), nameof(ZPlayFabMatchmaking.OnSessionUpdated))]
    internal class SessionUpdated
    {
        private static void Postfix(ZPlayFabMatchmaking __instance)
        {
            if (ReferenceEquals(__instance, ZPlayFabMatchmaking.instance)) TrySendServerStart();
        }
    }

    [HarmonyPatch(typeof(ZNet), nameof(ZNet.Update))]
    internal class Update
    {
        private static void Postfix(ZNet __instance)
        {
            if (__instance == loadedWorld) TrySendServerStart();
        }
    }

    private static void CancelServerStart(ZNet world)
    {
        if (world == loadedWorld) startPending = false;
        if (world == registeredWorld)
        {
            registeredWorld = null;
            registeredSession = null;
        }
    }

    [HarmonyPatch(typeof(ZNet), nameof(ZNet.StopAll))]
    internal class StopAll
    {
        private static void Prefix(ZNet __instance) => CancelServerStart(__instance);
    }

    [HarmonyPatch(typeof(ZNet), nameof(ZNet.OnDestroy))]
    internal class OnDestroy
    {
        private static void Prefix(ZNet __instance) => CancelServerStart(__instance);
    }

    [HarmonyPatch(typeof(ZNet), nameof(ZNet.SaveWorld))]
    internal class SaveWorld
    {
        private static void Postfix(ref bool sync)
        {
            if (DiscordConnectorPlugin.StaticConfig.WorldSaveMessageEnabled)
            {
                DiscordApi.SendMessage(
                    Webhook.Event.ServerSave,
                    MessageTransformer.FormatServerMessage(DiscordConnectorPlugin.StaticConfig.SaveMessage)
                );
            }
        }
    }

    [HarmonyPatch(typeof(ZNet), nameof(ZNet.RPC_CharacterID))]
    internal class RPC_CharacterID
    {
        private static List<long> joinedPlayers = new();

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
