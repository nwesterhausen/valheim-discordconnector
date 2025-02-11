using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace DiscordConnector.Patches;
internal class RandEventPatches
{

    [HarmonyPatch(typeof(RandomEvent), nameof(RandomEvent.OnActivate))]
    internal class OnActivate
    {

        private static void Prefix(ref RandomEvent __instance)
        {
            bool active = __instance.m_active;
            float duration = __instance.m_duration;
            string name = __instance.m_name;
            float time = __instance.m_time;
            float remaining = duration - time;
            Vector3 pos = __instance.m_pos;
            DiscordConnectorPlugin.StaticLogger.LogDebug(
                $"Random event OnActivate {name}: {active} for {duration} at {pos}. (time: {time})"
            );



            List<String> involvedPlayers = new List<string>();
            foreach (ZNet.PlayerInfo playerInfo in ZNet.instance.GetPlayerList())
            {
                if (RandEventSystem.instance.IsInsideRandomEventArea(__instance, playerInfo.m_position))
                {
                    involvedPlayers.Add(playerInfo.m_name);
                }
            }
            DiscordConnectorPlugin.StaticLogger.LogDebug(
                $"Involved players in event: {(string.Join(",", involvedPlayers.ToArray()))}"
            );

            if (__instance.m_time > 0)
            {
                if (DiscordConnectorPlugin.StaticConfig.EventResumedMessageEnabled)
                {
                    string message = MessageTransformer.FormatEventMessage(
                        DiscordConnectorPlugin.StaticConfig.EventResumedMessage,
                        Localization.instance.Localize(__instance.m_endMessage),
                        Localization.instance.Localize(__instance.m_startMessage)
                    // string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
                    );
                    if (!DiscordConnectorPlugin.StaticConfig.EventResumedPosEnabled)
                    {
                        DiscordApi.SendMessage(Webhook.Event.EventResumed, message);
                        return;
                    }
                    if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                    {
                        DiscordApi.SendMessage(Webhook.Event.EventResumed, message, pos);
                    }
                    else
                    {
                        message = MessageTransformer.FormatEventMessage(
                            DiscordConnectorPlugin.StaticConfig.EventResumedMessage,
                            Localization.instance.Localize(__instance.m_endMessage),
                            Localization.instance.Localize(__instance.m_startMessage),
                            // string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
                            pos
                        );
                        DiscordApi.SendMessage(Webhook.Event.EventResumed, message);
                    }
                }
            }
            else
            {
                if (DiscordConnectorPlugin.StaticConfig.EventStartMessageEnabled)
                {
                    string message = MessageTransformer.FormatEventStartMessage(
                        DiscordConnectorPlugin.StaticConfig.EventResumedMessage,
                        Localization.instance.Localize(__instance.m_endMessage),
                        Localization.instance.Localize(__instance.m_startMessage)
                    // string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
                    );
                    if (!DiscordConnectorPlugin.StaticConfig.EventStartPosEnabled)
                    {
                        DiscordApi.SendMessage(Webhook.Event.EventStart, message);
                        return;
                    }
                    if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                    {
                        DiscordApi.SendMessage(Webhook.Event.EventStart, message, pos);
                    }
                    else
                    {
                        message = MessageTransformer.FormatEventStartMessage(
                            DiscordConnectorPlugin.StaticConfig.EventResumedMessage,
                            Localization.instance.Localize(__instance.m_endMessage),
                            Localization.instance.Localize(__instance.m_startMessage),
                            // string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
                            pos
                        );
                        DiscordApi.SendMessage(Webhook.Event.EventStart, message);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(RandomEvent), nameof(RandomEvent.OnDeactivate))]
    internal class OnDeactivate
    {

        private static void Prefix(ref RandomEvent __instance, ref bool end)
        {
            bool active = __instance.m_active;
            float duration = __instance.m_duration;
            string name = __instance.m_name;
            float time = __instance.m_time;
            Vector3 pos = __instance.m_pos;
            DiscordConnectorPlugin.StaticLogger.LogDebug(
                $"Random event OnDeactivate {name}: End?{active} for {duration} at {pos}. (time: {time})"
            );

            List<String> involvedPlayers = new List<string>();
            foreach (ZNet.PlayerInfo playerInfo in ZNet.instance.GetPlayerList())
            {
                if (RandEventSystem.instance.IsInsideRandomEventArea(__instance, playerInfo.m_position))
                {
                    involvedPlayers.Add(playerInfo.m_name);
                }
            }
            DiscordConnectorPlugin.StaticLogger.LogDebug(
                $"Involved players in event: {(string.Join(",", involvedPlayers.ToArray()))}"
            );

            if (!end)
            {
                if (DiscordConnectorPlugin.StaticConfig.EventPausedMessageEnabled)
                {
                    string message = MessageTransformer.FormatEventMessage(
                        DiscordConnectorPlugin.StaticConfig.EventPausedMessage,
                        Localization.instance.Localize(__instance.m_endMessage),
                        Localization.instance.Localize(__instance.m_startMessage)
                    // string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
                    );
                    if (!DiscordConnectorPlugin.StaticConfig.EventPausedPosEnabled)
                    {
                        DiscordApi.SendMessage(Webhook.Event.EventPaused, message);
                        return;
                    }
                    if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                    {
                        DiscordApi.SendMessage(Webhook.Event.EventPaused, message, pos);
                    }
                    else
                    {
                        message = MessageTransformer.FormatEventMessage(
                            DiscordConnectorPlugin.StaticConfig.EventPausedMessage,
                            Localization.instance.Localize(__instance.m_endMessage),
                            Localization.instance.Localize(__instance.m_startMessage),
                            // string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
                            pos
                        );
                        DiscordApi.SendMessage(Webhook.Event.EventPaused, message);
                    }
                }
            }
            else
            {
                if (DiscordConnectorPlugin.StaticConfig.EventStopMessageEnabled)
                {
                    string message = MessageTransformer.FormatEventEndMessage(
                        DiscordConnectorPlugin.StaticConfig.EventStopMessage,
                        Localization.instance.Localize(__instance.m_endMessage),
                        Localization.instance.Localize(__instance.m_startMessage)
                    // string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
                    );
                    if (!DiscordConnectorPlugin.StaticConfig.EventStopPosEnabled)
                    {
                        DiscordApi.SendMessage(Webhook.Event.EventStop, message);
                        return;
                    }
                    if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                    {
                        DiscordApi.SendMessage(Webhook.Event.EventStop, message, pos);
                    }
                    else
                    {
                        message = MessageTransformer.FormatEventEndMessage(
                            DiscordConnectorPlugin.StaticConfig.EventStopMessage,
                            Localization.instance.Localize(__instance.m_endMessage),
                            Localization.instance.Localize(__instance.m_startMessage),
                            // string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
                            pos
                        );
                        DiscordApi.SendMessage(Webhook.Event.EventStop, message);
                    }
                }
            }
        }
    }
}
