using HarmonyLib;
using UnityEngine;

namespace DiscordConnector.Patches;

internal class ChatPatches
{
	internal const string ArrivalShout = "I have arrived!";

	[HarmonyPatch(typeof(Chat), nameof(Chat.RPC_ChatMessage))]
	internal class RpcChatMessagePatch
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
		private static void Prefix(long sender, Vector3 position, int type, UserInfo userInfo, string text)

		{
			DiscordConnectorPlugin.StaticLogger.LogDebug(
				$"User details: name:{userInfo.Name}  DisplayName():{userInfo.GetDisplayName()} senderID:{sender}  type:{(Talker.Type)type}  text:{text}");

			var userName = userInfo.Name;
			if (string.IsNullOrEmpty(userName))
			{
				DiscordConnectorPlugin.StaticLogger.LogInfo("Ignored shout from invalid user (null reference)");
				return;
			}

			if (DiscordConnectorPlugin.StaticConfig.MutedPlayers.IndexOf(userName) >= 0 ||
			    DiscordConnectorPlugin.StaticConfig.MutedPlayersRegex.IsMatch(userName))
			{
				DiscordConnectorPlugin.StaticLogger.LogInfo(
					$"Ignored shout from user on muted list. User: {userName} Shout: {text}.");
				return;
			}

			var peer = ZNet.instance.GetPeerByPlayerName(userName);

			// If peer is null, the message wasn't sent from a player
			if (peer == null)
			{
				Handlers.NonPlayerChat((Talker.Type)type, userName, text);
				return;
			}

			DiscordConnectorPlugin.StaticLogger.LogDebug($"ChatMessage fell through: {userName} {text} {peer.GetType()} {peer.m_playerName}");
		}
	}
}
