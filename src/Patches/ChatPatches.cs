﻿using HarmonyLib;
using UnityEngine;

namespace DiscordConnector.Patches;
internal class ChatPatches
{
    private const string ArrivalShout = "I have arrived!";

    [HarmonyPatch(typeof(Chat), nameof(Chat.OnNewChatMessage))]
    internal class OnNewChatMessage
    {
        /// <summary>
        /// Look into the chat message and perform some actions depending on what it is. No modifications are made to the messages being sent in game.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     DiscordConnector is concerned with Shouts and Pings, as the other types of chat messages are not broadcast to the server where DiscordConnector operates.
        ///     In the special case of shouting <see cref="ArrivalShout"/> in a server hosted from the client version of Valheim, some player has joined the game.
        /// </para>
        /// <para>
        ///     The implementation here passes details of the chat message to one of the <see cref="Handlers"/> functions.
        /// </para>
        /// </remarks>
        private static void Prefix(ref GameObject go, ref long senderID, ref Vector3 pos, ref Talker.Type type, ref UserInfo user, ref string text, ref string senderNetworkUserId)
        {
            Plugin.StaticLogger.LogDebug($"User details: name:{user.Name} gamerTag:{user.Gamertag} networkUserId:{user.NetworkUserId} DisplayName():{user.GetDisplayName(user.NetworkUserId)}");

            string userName = user.Name;
            if (string.IsNullOrEmpty(userName))
            {
                Plugin.StaticLogger.LogInfo("Ignored shout from invalid user (null reference)");
                return;
            }
            if (Plugin.StaticConfig.MutedPlayers.IndexOf(userName) >= 0 || Plugin.StaticConfig.MutedPlayersRegex.IsMatch(userName))
            {
                Plugin.StaticLogger.LogInfo($"Ignored shout from user on muted list. User: {userName} Shout: {text}.");
                return;
            }

            ZNetPeer peer = ZNet.instance.GetPeerByPlayerName(userName);

            // If peer or the peer socket is null, the message wasn't sent from a player
            if (peer == null || peer.m_socket == null)
            {
                Handlers.NonPlayerChat(type, userName, text);
                return;
            }

            // Get the player's hostname to use for record keeping and logging
            string playerHostName = peer.m_socket.GetHostName();

            switch (type)
            {
                case Talker.Type.Ping:
                    Handlers.Ping(peer, pos);
                    break;
                case Talker.Type.Shout:
                    if (text.Equals(ArrivalShout))
                    {
                        if (Plugin.IsHeadless())
                        {
                            return;
                        }

                        // On servers hosted from the client version, the host player shouts instead of joining
                        Handlers.Join(peer);
                    }
                    else
                    {
                        Handlers.Shout(peer, pos, text);
                    }
                    break;
                default:
                    Plugin.StaticLogger.LogDebug(
                        $"Unmatched chat message. [{type}] {userName}: {text} at {pos}"
                    );
                    break;
            }

        }
    }
}
