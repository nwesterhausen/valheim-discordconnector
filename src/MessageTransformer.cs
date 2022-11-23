using System.Text.RegularExpressions;
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
        private const string WORLD_NAME = "%WORLD_NAME%";
        private const string WORLD_SEED_NAME = "%WORLD_SEED_NAME%";
        private const string WORLD_SEED = "%WORLD_SEED%";

        private static Regex OpenCaretRegex = new Regex(@"<[\w=]+>");
        private static Regex CloseCaretRegex = new Regex(@"</[\w]+>");

        private static string ReplaceVariables(string rawMessage)
        {
            return ReplaceDynamicVariables(rawMessage)
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
        private static string ReplaceDynamicVariables(string rawMessage)
        {
            string world_name = "";
            Plugin.StaticServerInfo.TryGetValue(Plugin.ServerInfo.WorldName, out world_name);

            return rawMessage
                .Replace(WORLD_NAME, world_name);
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
        public static string FormatPlayerMessage(string rawMessage, string playerName, string playerid, string shout)
        {
            return MessageTransformer.FormatPlayerMessage(rawMessage, playerName, playerid)
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
        public static string FormatLeaderboardHeader(string rawMessage)
        {
            return MessageTransformer.ReplaceVariables(rawMessage);
        }

        public static string FormatLeaderboardHeader(string rawMessage, int n)
        {
            return MessageTransformer.ReplaceVariables(rawMessage)
                .Replace(N, n.ToString());
        }

        /// <summary>
        /// Remove caret formatting from a string. This is used to strip special color codes away from user names.
        /// 
        /// For example, some mods can send messages as shouts in the game. They may try to color the name of the user:
        ///     `<color=cyan>[Admin]</color> vadmin`
        /// This function strips away any caret formatting, making the string "plain text"
        ///     `[Admin] vadmin`
        /// 
        /// </summary>
        /// <param name="str">String to strip caret formatting from</param>
        /// <returns>Same string but without the caret formatting</returns>
        public static string CleanCaretFormatting(string str)
        {
            // regex.Replace(input, sub, 1);
            string result = OpenCaretRegex.Replace(str, @"", 1);
            result = CloseCaretRegex.Replace(result, @"", 1);

            return result;
        }

        /// <summary>
        /// Format a vector3 position into the formatted version used by discord connector
        /// </summary>
        /// <param name="vec3">Position vector to turn into string</param>
        /// <returns>String following the formatting laid out in the variable config file.</returns>
        public static string FormatVector3AsPos(Vector3 vec3)
        {
            return Plugin.StaticConfig.PosVarFormat
                .Replace("%X%", vec3.x.ToString("F1"))
                .Replace("%Y%", vec3.y.ToString("F1"))
                .Replace("%Z%", vec3.z.ToString("F1"));
        }

        /// <summary>
        /// Format the appended position data using the config values.
        /// </summary>
        /// <param name="vec3">Position vector to include</param>
        /// <returns>String to append with the position information</returns>
        public static string FormatAppendedPos(Vector3 vec3)
        {
            string posStr = FormatVector3AsPos(vec3);
            return Plugin.StaticConfig.AppendedPosFormat
                .Replace(POS, posStr);
        }
    }
}
