using System;

using DiscordConnector.Client;
using DiscordConnector.RPC;

using HarmonyLib;

using UnityEngine;

namespace DiscordConnector.Patches;

internal class ChatPatches
{
    internal const string ArrivalShout = "I have arrived!";

    [HarmonyPatch(typeof(Chat), nameof(Chat.OnNewChatMessage))]
    internal class OnNewChatMessage
    {
        /// <summary>
        ///     Look into the chat message and perform some actions depending on what it is. No modifications are made to the
        ///     messages being sent in game.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         DiscordConnector is concerned with Shouts and Pings, as the other types of chat messages are not broadcast to
        ///         the server where DiscordConnector operates.
        ///         In the special case of shouting <see cref="ArrivalShout" /> in a server hosted from the client version of
        ///         Valheim, some player has joined the game.
        ///     </para>
        ///     <para>
        ///         The implementation here passes details of the chat message to one of the <see cref="Handlers" /> functions.
        ///     </para>
        /// </remarks>
        private static void Prefix(ref GameObject go, ref long senderID, ref Vector3 pos, ref Talker.Type type,
            ref UserInfo sender, ref string text)
        {
            DiscordConnectorClientPlugin.StaticLogger.LogDebug(
                $"User details: name:{sender.Name}  DisplayName():{sender.GetDisplayName()} senderID:{senderID}  type:{type}  text:{text}");

            try
            {
                ChatMessageDetail messageDetail = new(pos, type, text);
                ZPackage package = messageDetail.ToZPackage();

                long serverID = ZRoutedRpc.instance.GetServerPeerID();

                try
                {
                    ZRoutedRpc.instance.InvokeRoutedRPC(
                        serverID,
                        RPC.Common.RPC_OnNewChatMessage,
                        package);

                    DiscordConnectorClientPlugin.StaticLogger.LogDebug(
                        $"Sent encoded chat message to server {messageDetail} in {package.Size()}B");
                }
                catch (Exception ex)
                {
                    DiscordConnectorClientPlugin.StaticLogger.LogError("Failed to send chat message to server");
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                DiscordConnectorClientPlugin.StaticLogger.LogError("Failed to encode chat message");
                DiscordConnectorClientPlugin.StaticLogger.LogDebug(ex.ToString());
            }
        }
    }
}
