using UnityEngine;

namespace DiscordConnector
{
    internal static class MessageTransformer
    {
        private const string PUBLIC_IP = "%PUBLICIP%";
        private const string VAR = "%VAR1%";
        private const string VAR_1 = "%VAR2%";
        private const string VAR_2 = "%VAR3%";
        private const string VAR_3 = "%VAR4%";
        private const string VAR_4 = "%VAR5%";
        private const string VAR_5 = "%VAR6%";
        private const string VAR_6 = "%VAR7%";
        private const string VAR_7 = "%VAR8%";
        private const string VAR_8 = "%VAR9%";
        private const string VAR_9 = "%VAR10%";
        private const string PLAYER_NAME = "%PLAYER_NAME%";
        private const string PLAYER_STEAMID = "%PLAYER_STEAMID%";
        private const string PLAYER_ID = "%PLAYER_ID%";
        private const string SHOUT = "%SHOUT%";
        private const string POS = "%POS%";
        private const string EVENT_START_MSG = "%EVENT_START_MSG%";
        private const string EVENT_END_MSG = "%EVENT_END_MSG%";
        private const string EVENT_MSG = "%EVENT_MSG%";
        private const string EVENT_PLAYERS = "%PLAYERS%";
        private const string N = "%N%";
        private static string ReplaceVariables(string rawMessage)
        {
            return rawMessage
                .Replace(VAR, Plugin.StaticConfig.UserVariable)
                .Replace(VAR_1, Plugin.StaticConfig.UserVariable1)
                .Replace(VAR_2, Plugin.StaticConfig.UserVariable2)
                .Replace(VAR_3, Plugin.StaticConfig.UserVariable3)
                .Replace(VAR_4, Plugin.StaticConfig.UserVariable4)
                .Replace(VAR_5, Plugin.StaticConfig.UserVariable5)
                .Replace(VAR_6, Plugin.StaticConfig.UserVariable6)
                .Replace(VAR_7, Plugin.StaticConfig.UserVariable7)
                .Replace(VAR_8, Plugin.StaticConfig.UserVariable8)
                .Replace(VAR_9, Plugin.StaticConfig.UserVariable9)
                .Replace(PUBLIC_IP, Plugin.PublicIpAddress);
        }
        public static string FormatServerMessage(string rawMessage)
        {
            return MessageTransformer.ReplaceVariables(rawMessage);
        }

        public static string FormatPlayerMessage(string rawMessage, string playerName, string playerId)
        {
            return MessageTransformer.ReplaceVariables(rawMessage)
                .Replace(PLAYER_STEAMID, playerId)
                .Replace(PLAYER_ID, playerId)
                .Replace(PLAYER_NAME, playerName);
        }

        public static string FormatPlayerMessage(string rawMessage, string playerName, string playerId, Vector3 pos)
        {
            return MessageTransformer.FormatPlayerMessage(rawMessage, playerName, playerId)
                .Replace(POS, $"{pos}");
        }
        public static string FormatPlayerMessage(string rawMessage, string playerName, string playerId, string shout)
        {
            return MessageTransformer.FormatPlayerMessage(rawMessage, playerName, playerId)
                .Replace(SHOUT, shout);
        }
        public static string FormatPlayerMessage(string rawMessage, string playerName, string playerSteamId, string shout, Vector3 pos)
        {
            return MessageTransformer.FormatPlayerMessage(rawMessage, playerName, playerSteamId, pos)
                .Replace(SHOUT, shout);
        }
        public static string FormatEventMessage(string rawMessage, string eventStartMsg, string eventEndMsg)
        {
            return MessageTransformer.ReplaceVariables(rawMessage)
                .Replace(EVENT_START_MSG, eventStartMsg)
                .Replace(EVENT_END_MSG, eventEndMsg);
            //.Replace(EVENT_PLAYERS, players); //! Removed until re can reliably poll player locations
        }
        public static string FormatEventMessage(string rawMessage, string eventStartMsg, string eventEndMsg, Vector3 pos)
        {
            return MessageTransformer.FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg)
                .Replace(POS, $"{pos}");
        }
        public static string FormatEventStartMessage(string rawMessage, string eventStartMsg, string eventEndMsg)
        {
            return MessageTransformer.FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg)
                .Replace(EVENT_MSG, eventStartMsg);
        }
        public static string FormatEventEndMessage(string rawMessage, string eventStartMsg, string eventEndMsg)
        {
            return MessageTransformer.FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg)
                .Replace(EVENT_MSG, eventEndMsg);
        }
        public static string FormatEventStartMessage(string rawMessage, string eventStartMsg, string eventEndMsg, Vector3 pos)
        {
            return MessageTransformer.FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg, pos)
                .Replace(EVENT_MSG, eventStartMsg);
        }
        public static string FormatEventEndMessage(string rawMessage, string eventStartMsg, string eventEndMsg, Vector3 pos)
        {
            return MessageTransformer.FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg, pos)
                .Replace(EVENT_MSG, eventEndMsg);
        }
        public static string FormatLeaderBoardHeader(string rawMessage)
        {
            return MessageTransformer.ReplaceVariables(rawMessage);
        }

        public static string FormatLeaderBoardHeader(string rawMessage, int n)
        {
            return MessageTransformer.ReplaceVariables(rawMessage)
                .Replace(N, n.ToString());
        }
    }
}
