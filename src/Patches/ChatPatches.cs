using HarmonyLib;
using UnityEngine;

namespace DiscordConnector.Patches
{
    internal class ChatPatches
    {

        [HarmonyPatch(typeof(Chat), nameof(Chat.OnNewChatMessage))]
        internal class OnNewChatMessage
        {
            private static void Prefix(ref GameObject go, ref long senderID, ref Vector3 pos, ref Talker.Type type, ref string user, ref string text, ref string senderNetworkUserId)
            {
                if (Plugin.StaticConfig.MutedPlayers.IndexOf(user) >= 0 || Plugin.StaticConfig.MutedPlayersRegex.IsMatch(user))
                {
                    Plugin.StaticLogger.LogInfo($"Ignored shout from user on muted list. User: {user} Shout: {text}.");
                    return;
                }
                ZNetPeer peerInstance = ZNet.instance.GetPeerByPlayerName(user);

                // Player steam ID is no longer guarenteed. Instead use the characterId (if possible)
                string playerCharacterId = $"{peerInstance.m_characterID}"; // Player SteamID is not guarenteed, so a work-around is needed.

                switch (type)
                {
                    case Talker.Type.Ping:
                        if (Plugin.StaticConfig.AnnouncePlayerFirstPingEnabled && Plugin.StaticDatabase.CountOfRecordsByName(Records.Categories.Ping, user) == 0)
                        {
                            DiscordApi.SendMessage(
                                MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PlayerFirstPingMessage, user, playerCharacterId.ToString())
                            );
                        }
                        if (Plugin.StaticConfig.StatsPingEnabled)
                        {
                            Plugin.StaticDatabase.InsertSimpleStatRecord(Records.Categories.Ping, user, playerCharacterId, pos);
                        }
                        if (Plugin.StaticConfig.ChatPingEnabled)
                        {
                            string message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PingMessage, user, playerCharacterId.ToString());
                            if (Plugin.StaticConfig.ChatPingPosEnabled)
                            {
                                if (Plugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                                {
                                    DiscordApi.SendMessage(
                                        message,
                                        pos
                                    );
                                    break;
                                }
                                message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PingMessage, user, playerCharacterId.ToString(), pos);
                            }
                            if (message.Contains("%POS%"))
                            {
                                message.Replace("%POS%", "");
                            }
                            DiscordApi.SendMessage(message);
                        }
                        break;
                    case Talker.Type.Shout:
                        if (text.Equals("I have arrived!"))
                        {
                            if (!Plugin.IsHeadless())
                            {
                                if (Plugin.StaticConfig.AnnouncePlayerFirstJoinEnabled && Plugin.StaticDatabase.CountOfRecordsByName(Records.Categories.Join, user) == 0)
                                {
                                    DiscordApi.SendMessage(
                                        MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PlayerFirstJoinMessage, user, playerCharacterId.ToString())
                                    );
                                }
                                if (Plugin.StaticConfig.StatsJoinEnabled)
                                {
                                    Plugin.StaticDatabase.InsertSimpleStatRecord(Records.Categories.Join, user, playerCharacterId, pos);
                                }
                                if (Plugin.StaticConfig.PlayerJoinMessageEnabled)
                                {
                                    string message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.JoinMessage, user, playerCharacterId.ToString());
                                    if (Plugin.StaticConfig.PlayerJoinPosEnabled)
                                    {
                                        if (Plugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                                        {
                                            DiscordApi.SendMessage(
                                                message,
                                                pos
                                            );
                                            break;
                                        }
                                        message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.JoinMessage, user, playerCharacterId.ToString(), pos);
                                    }
                                    if (message.Contains("%POS%"))
                                    {
                                        message.Replace("%POS%", "");
                                    }
                                    DiscordApi.SendMessage(message);
                                }
                            }
                        }
                        else
                        {
                            if (Plugin.StaticConfig.AnnouncePlayerFirstShoutEnabled && Plugin.StaticDatabase.CountOfRecordsByName(Records.Categories.Shout, user) == 0)
                            {
                                DiscordApi.SendMessage(
                                    MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.PlayerFirstShoutMessage, user, playerCharacterId.ToString(), text)
                                );
                            }
                            if (Plugin.StaticConfig.StatsShoutEnabled)
                            {
                                Plugin.StaticDatabase.InsertSimpleStatRecord(Records.Categories.Shout, user, playerCharacterId, pos);
                            }
                            if (Plugin.StaticConfig.ChatShoutEnabled)
                            {
                                string message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.ShoutMessage, user, playerCharacterId.ToString(), text);
                                if (Plugin.StaticConfig.ChatShoutPosEnabled)
                                {
                                    if (Plugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                                    {
                                        DiscordApi.SendMessage(
                                            message,
                                            pos
                                        );
                                        break;
                                    }
                                    message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.ShoutMessage, user, playerCharacterId.ToString(), text, pos);
                                }
                                if (message.Contains("%POS%"))
                                {
                                    message.Replace("%POS%", "");
                                }
                                DiscordApi.SendMessage(message);
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
