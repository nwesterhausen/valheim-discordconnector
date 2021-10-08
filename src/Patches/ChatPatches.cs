using HarmonyLib;
using UnityEngine;

namespace DiscordConnector.Patches
{
    internal class ChatPatches
    {

        [HarmonyPatch(typeof(Chat), nameof(Chat.OnNewChatMessage))]
        internal class OnNewChatMessage
        {
            private static void Prefix(ref GameObject go, ref long senderID, ref Vector3 pos, ref Talker.Type type, ref string user, ref string text)
            {
                if (Plugin.StaticConfig.MutedPlayers.IndexOf(user) >= 0)
                {
                    Plugin.StaticLogger.LogInfo($"Ignored shout from user on muted list. User: {user} Shout: {text}. Index {Plugin.StaticConfig.MutedPlayers.IndexOf(user)}");
                    return;
                }
                switch (type)
                {
                    case Talker.Type.Ping:
                        if (Plugin.StaticConfig.ChatPingEnabled)
                        {
                            string message = Plugin.StaticConfig.PingMessage.Replace("%PLAYER_NAME%", user);
                            if (Plugin.StaticConfig.ChatPingPosEnabled)
                            {
                                DiscordApi.SendMessage(
                                    message,
                                    pos
                                );
                            }
                            else
                            {
                                DiscordApi.SendMessage(message);
                            }
                        }
                        if (Plugin.StaticConfig.AnnouncePlayerFirstPingEnabled && Plugin.StaticRecords.Retrieve(Categories.Ping, user) == 0)
                        {
                            DiscordApi.SendMessage(Plugin.StaticConfig.PlayerFirstPingMessage.Replace("%PLAYER_NAME%", user));
                        }
                        if (Plugin.StaticConfig.StatsPingEnabled)
                        {
                            Plugin.StaticRecords.Store(Categories.Ping, user, 1);
                        }
                        break;
                    case Talker.Type.Shout:
                        if (text.Equals("I have arrived!"))
                        {
                            if (!Plugin.IsHeadless())
                            {
                                if (Plugin.StaticConfig.PlayerJoinMessageEnabled)
                                {
                                    string message = Plugin.StaticConfig.JoinMessage.Replace("%PLAYER_NAME%", user);
                                    if (Plugin.StaticConfig.PlayerJoinPosEnabled)
                                    {
                                        DiscordApi.SendMessage(
                                        message,
                                        pos
                                        );
                                    }
                                    else
                                    {
                                        DiscordApi.SendMessage(message);
                                    }
                                }
                                if (Plugin.StaticConfig.AnnouncePlayerFirstJoinEnabled && Plugin.StaticRecords.Retrieve(Categories.Join, user) == 0)
                                {
                                    DiscordApi.SendMessage(Plugin.StaticConfig.PlayerFirstJoinMessage.Replace("%PLAYER_NAME%", user));
                                }
                                if (Plugin.StaticConfig.StatsJoinEnabled)
                                {
                                    Plugin.StaticRecords.Store(Categories.Join, user, 1);
                                }
                            }
                            Plugin.StaticLogger.LogDebug(
                                $"{user} shouts 'I have arrived!'"
                            );
                        }
                        else
                        {
                            if (Plugin.StaticConfig.ChatShoutEnabled)
                            {
                                string message = Plugin.StaticConfig.ShoutMessage.Replace("%PLAYER_NAME%", user).Replace("%SHOUT%", text);
                                if (Plugin.StaticConfig.ChatShoutPosEnabled)
                                {
                                    DiscordApi.SendMessage(
                                        message,
                                        pos
                                    );
                                }
                                else
                                {
                                    DiscordApi.SendMessage(message);
                                }
                            }
                            if (Plugin.StaticConfig.AnnouncePlayerFirstShoutEnabled && Plugin.StaticRecords.Retrieve(Categories.Shout, user) == 0)
                            {
                                DiscordApi.SendMessage(Plugin.StaticConfig.PlayerFirstShoutMessage.Replace("%PLAYER_NAME%", user).Replace("%SHOUT%", text));
                            }
                            if (Plugin.StaticConfig.StatsShoutEnabled)
                            {
                                Plugin.StaticRecords.Store(Categories.Shout, user, 1);
                            }
                        }
                        break;
                    default:
                        Plugin.StaticLogger.LogDebug(
                            $"Unmatched chat message. [{type}] {user}: {text} at {pos}"
                        );
                        break;
                }

            }
        }
    }
}
