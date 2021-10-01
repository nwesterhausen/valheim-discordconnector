using HarmonyLib;
using UnityEngine;

namespace DiscordConnector.Patches
{
    internal class ZNetPatches
    {

        [HarmonyPatch(typeof(ZNet), "LoadWorld")]
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

        [HarmonyPatch(typeof(ZNet), "Shutdown")]
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
    }

    internal class ChatPatches
    {

        [HarmonyPatch(typeof(Chat), "OnNewChatMessage")]
        internal class OnNewChatMessage
        {
            private static void Prefix(ref GameObject go, ref long senderID, ref Vector3 pos, ref Talker.Type type, ref string user, ref string text)
            {
                if (Plugin.StaticConfig.ChatMessageEnabled)
                {
                    switch (type)
                    {
                        case Talker.Type.Ping:
                            if (Plugin.StaticConfig.ChatPingEnabled)
                            {
                                DiscordApi.SendMessage(
                                    $"{user} pings the map at ${pos}"
                                );
                            }
                            break;
                        case Talker.Type.Shout:
                            if (text.Equals("I have arrived!") && Plugin.StaticConfig.ChatArrivalEnabled)
                            {
                                if (Plugin.StaticConfig.ChatArrivalPosEnabled)
                                {
                                    DiscordApi.SendMessage(
                                        $"{user} has arrived at {pos}!"
                                    );
                                }
                                else
                                {
                                    DiscordApi.SendMessage(
                                        $"{user} has arrived!"
                                    );
                                }
                            }
                            else if (Plugin.StaticConfig.ChatShoutEnabled)
                            {

                                if (Plugin.StaticConfig.ChatShoutPosEnabled)
                                {
                                    DiscordApi.SendMessage(
                                        $"{user} shouts {text} from {pos}!"
                                    );
                                }
                                else
                                {
                                    DiscordApi.SendMessage(
                                        $"{user} shouts {text}!"
                                    );
                                }
                            }
                            break;
                        default:
                            Plugin.StaticLogger.LogInfo(
                                $"Ignoring chat message. [{type}] {user}: {text} at {pos}"
                            );
                            break;
                    }
                }
            }
        }
    }
}