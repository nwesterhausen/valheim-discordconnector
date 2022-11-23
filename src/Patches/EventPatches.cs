using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace DiscordConnector.Patches
{
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
                Plugin.StaticLogger.LogDebug(
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
                Plugin.StaticLogger.LogDebug(
                    $"Involved players in event: {(string.Join(",", involvedPlayers.ToArray()))}"
                );

                if (__instance.m_time > 0)
                {
                    if (Plugin.StaticConfig.EventResumedMessageEnabled)
                    {
                        string message = MessageTransformer.FormatEventMessage(
                            Plugin.StaticConfig.EventResumedMessage,
                            Localization.instance.Localize(__instance.m_endMessage),
                            Localization.instance.Localize(__instance.m_startMessage)
                        // string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
                        );
                        if (!Plugin.StaticConfig.EventResumedPosEnabled)
                        {
                            DiscordApi.SendMessage(message);
                            return;
                        }
                        if (Plugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                        {
                            DiscordApi.SendMessage(message, pos);
                        }
                        else
                        {
                            message = MessageTransformer.FormatEventMessage(
                                Plugin.StaticConfig.EventResumedMessage,
                                Localization.instance.Localize(__instance.m_endMessage),
                                Localization.instance.Localize(__instance.m_startMessage),
                                // string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
                                pos
                            );
                            DiscordApi.SendMessage(message);
                        }
                    }
                }
                else
                {
                    if (Plugin.StaticConfig.EventStartMessageEnabled)
                    {
                        string message = MessageTransformer.FormatEventStartMessage(
                            Plugin.StaticConfig.EventResumedMessage,
                            Localization.instance.Localize(__instance.m_endMessage),
                            Localization.instance.Localize(__instance.m_startMessage)
                        // string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
                        );
                        if (!Plugin.StaticConfig.EventStartPosEnabled)
                        {
                            DiscordApi.SendMessage(message);
                            return;
                        }
                        if (Plugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                        {
                            DiscordApi.SendMessage(message, pos);
                        }
                        else
                        {
                            message = MessageTransformer.FormatEventStartMessage(
                                Plugin.StaticConfig.EventResumedMessage,
                                Localization.instance.Localize(__instance.m_endMessage),
                                Localization.instance.Localize(__instance.m_startMessage),
                                // string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
                                pos
                            );
                            DiscordApi.SendMessage(message);
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
                Plugin.StaticLogger.LogDebug(
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
                Plugin.StaticLogger.LogDebug(
                    $"Involved players in event: {(string.Join(",", involvedPlayers.ToArray()))}"
                );

                if (!end)
                {
                    if (Plugin.StaticConfig.EventPausedMessageEnabled)
                    {
                        string message = MessageTransformer.FormatEventMessage(
                            Plugin.StaticConfig.EventPausedMessage,
                            Localization.instance.Localize(__instance.m_endMessage),
                            Localization.instance.Localize(__instance.m_startMessage)
                        // string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
                        );
                        if (!Plugin.StaticConfig.EventPausedPosEnabled)
                        {
                            DiscordApi.SendMessage(message);
                            return;
                        }
                        if (Plugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                        {
                            DiscordApi.SendMessage(message, pos);
                        }
                        else
                        {
                            message = MessageTransformer.FormatEventMessage(
                                Plugin.StaticConfig.EventPausedMessage,
                                Localization.instance.Localize(__instance.m_endMessage),
                                Localization.instance.Localize(__instance.m_startMessage),
                                // string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
                                pos
                            );
                            DiscordApi.SendMessage(message);
                        }
                    }
                }
                else
                {
                    if (Plugin.StaticConfig.EventStopMessageEnabled)
                    {
                        string message = MessageTransformer.FormatEventEndMessage(
                            Plugin.StaticConfig.EventStopMessage,
                            Localization.instance.Localize(__instance.m_endMessage),
                            Localization.instance.Localize(__instance.m_startMessage)
                        // string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
                        );
                        if (!Plugin.StaticConfig.EventStopPosEnabled)
                        {
                            DiscordApi.SendMessage(message);
                            return;
                        }
                        if (Plugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                        {
                            DiscordApi.SendMessage(message, pos);
                        }
                        else
                        {
                            message = MessageTransformer.FormatEventEndMessage(
                                Plugin.StaticConfig.EventStopMessage,
                                Localization.instance.Localize(__instance.m_endMessage),
                                Localization.instance.Localize(__instance.m_startMessage),
                                // string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
                                pos
                            );
                            DiscordApi.SendMessage(message);
                        }
                    }
                }
            }
        }
    }
}
