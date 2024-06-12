using System.Text.RegularExpressions;

namespace DiscordConnector;
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
    private const string DAY_NUMBER = "%DAY_NUMBER%";
    private const string NUM_PLAYERS = "%NUM_PLAYERS%";
    private const string JOIN_CODE = "%JOIN_CODE%";

    private static readonly Regex OpenCaretRegex = new(@"<[\w=]+>");
    private static readonly Regex CloseCaretRegex = new(@"</[\w]+>");

    /// <summary>
    /// Replace the static variables in the message. These are variables that are set in the config file.
    /// Replaces:
    ///     - `%VAR1%` through `%VAR10%` with the user variables
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="subtractOne">Subtract one from the number of online players</param>
    private static string ReplaceVariables(string rawMessage, bool subtractOne = false)
    {
        string customVariablesReplaced = rawMessage
            .Replace(VAR, Plugin.StaticConfig.UserVariable)
            .Replace(VAR_1, Plugin.StaticConfig.UserVariable1)
            .Replace(VAR_2, Plugin.StaticConfig.UserVariable2)
            .Replace(VAR_3, Plugin.StaticConfig.UserVariable3)
            .Replace(VAR_4, Plugin.StaticConfig.UserVariable4)
            .Replace(VAR_5, Plugin.StaticConfig.UserVariable5)
            .Replace(VAR_6, Plugin.StaticConfig.UserVariable6)
            .Replace(VAR_7, Plugin.StaticConfig.UserVariable7)
            .Replace(VAR_8, Plugin.StaticConfig.UserVariable8)
            .Replace(VAR_9, Plugin.StaticConfig.UserVariable9);

        return ReplaceDynamicVariables(customVariablesReplaced, subtractOne);
    }

    /// <summary>
    /// Replace dynamic variables in the message. These are variables that are not static and need to be calculated at runtime.
    /// Replaces:
    ///     - `%PUBLICIP%` with the public IP of the server
    ///     - `%WORLD_NAME%` with the name of the world
    ///     - `%DAY_NUMBER%` with the current day number
    ///     - `%NUM_PLAYERS%` with the number of players in the server
    ///     - `%JOIN_CODE%` with the join code of the server
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="subtractOne">Subtract one from the number of online players</param>
    private static string ReplaceDynamicVariables(string rawMessage, bool subtractOne = false)
    {
        // additionally add any other dynamic variables here..
        string dynamicReplacedMessage = ReplacePublicIp(rawMessage);
        dynamicReplacedMessage = ReplaceWorldName(dynamicReplacedMessage);
        dynamicReplacedMessage = ReplaceDayNumber(dynamicReplacedMessage);
        dynamicReplacedMessage = ReplaceNumPlayers(dynamicReplacedMessage, subtractOne);
        dynamicReplacedMessage = ReplaceJoinCode(dynamicReplacedMessage);

        return dynamicReplacedMessage;
    }

    /// <summary>
    /// Replace the join code in the message. Uses the ZPlayFabMatchmaking class to get the join code.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    private static string ReplaceJoinCode(string rawMessage)
    {
        return rawMessage
            .Replace(JOIN_CODE, ZPlayFabMatchmaking.JoinCode);
    }

    /// <summary>
    /// Replace the day number in the message. Uses the EnvMan instance to get the current day.
    /// This will only replace `%DAY_NUMBER%` with the current day if the EnvMan instance is available.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    private static string ReplaceDayNumber(string rawMessage)
    {
        // as written, if no EnvMan instance is available, it will return the raw message with `%DAY_NUMBER%` still in it.
        return rawMessage
            .Replace(DAY_NUMBER, EnvMan.instance != null ? EnvMan.instance.GetCurrentDay().ToString() : DAY_NUMBER);
    }

    /// <summary>
    /// Replace the number of players in the message. Uses the ZNet instance to get the number of players.
    /// This will only replace `%NUM_PLAYERS%` with the number of players if the ZNet instance is available.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="subtractOne">Subtract one from the number of online players</param>
    private static string ReplaceNumPlayers(string rawMessage, bool subtractOne = false)
    {
        if (subtractOne)
        {
            return rawMessage
                .Replace(NUM_PLAYERS, ZNet.instance != null ? (ZNet.instance.GetNrOfPlayers() - 1).ToString() : NUM_PLAYERS);
        }

        return rawMessage
            .Replace(NUM_PLAYERS, ZNet.instance != null ? ZNet.instance.GetNrOfPlayers().ToString() : NUM_PLAYERS);
    }

    /// <summary>
    /// Replace the public IP in the message. Uses the ZNet instance to get the public IP.
    ///
    /// Note that if this fails, it will return an empty string. Also some occasions, the ZNet class may fail to get
    /// the public IP address. This is out of scope for this plugin, to avoid making our own assumptions about the
    /// network configuration of the server.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    private static string ReplacePublicIp(string rawMessage)
    {
        return rawMessage
            .Replace(PUBLIC_IP, Plugin.PublicIpAddress);
    }

    /// <summary>
    /// Replace the world name in the message. Uses the ZNet instance to get the world name.
    /// This will only replace `%WORLD_NAME%` with the world name if the ZNet instance is available.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    private static string ReplaceWorldName(string rawMessage)
    {
        return rawMessage
            .Replace(WORLD_NAME, ZNet.instance != null ? ZNet.instance.GetWorldName() : WORLD_NAME);
    }

    /// <summary>
    /// Format a server message using the config values.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    public static string FormatServerMessage(string rawMessage)
    {
        return MessageTransformer.ReplaceVariables(rawMessage);
    }

    /// <summary>
    /// Format a server message using the config values, with extra player information.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="playerName">Name of the player</param>
    /// <param name="playerId">ID of the player</param>
    /// <param name="subtractOne">(Optional) Subtract one from the number of online players</param>
    public static string FormatPlayerMessage(string rawMessage, string playerName, string playerId, bool subtractOne = false)
    {
        return MessageTransformer.ReplaceVariables(rawMessage, subtractOne)
            .Replace(PLAYER_STEAMID, playerId)
            .Replace(PLAYER_ID, playerId)
            .Replace(PLAYER_NAME, playerName);
    }

    /// <summary>
    /// Format a server message using the config values, with extra player information.
    /// Specifically, this version includes the player's position in the message.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="playerName">Name of the player</param>
    /// <param name="playerId">ID of the player</param>
    /// <param name="pos">Position of the player</param>
    /// <param name="subtractOne">(Optional) Subtract one from the number of online players</param>
    public static string FormatPlayerMessage(string rawMessage, string playerName, string playerId, UnityEngine.Vector3 pos, bool subtractOne = false)
    {
        return MessageTransformer.FormatPlayerMessage(rawMessage, playerName, playerId, subtractOne)
            .Replace(POS, $"{pos}");
    }
    /// <summary>
    /// Format a server message using the config values, with extra player information.
    /// Specifically, this version includes the shout message in the message.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="playerName">Name of the player</param>
    /// <param name="playerId">ID of the player</param>
    /// <param name="shout">Shout message</param>
    /// <param name="subtractOne">(Optional) Subtract one from the number of online players</param>
    public static string FormatPlayerMessage(string rawMessage, string playerName, string playerId, string shout, bool subtractOne = false)
    {
        return MessageTransformer.FormatPlayerMessage(rawMessage, playerName, playerId, subtractOne)
            .Replace(SHOUT, shout);
    }
    /// <summary>
    /// Format a server message using the config values, with extra player information.
    /// Specifically, this version includes the shout message and the player's position in the message.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="playerName">Name of the player</param>
    /// <param name="playerSteamId">Steam ID of the player</param>
    /// <param name="shout">Shout message</param>
    /// <param name="pos">Position of the player</param>
    /// <param name="subtractOne">(Optional) Subtract one from the number of online players</param>
    public static string FormatPlayerMessage(string rawMessage, string playerName, string playerSteamId, string shout, UnityEngine.Vector3 pos, bool subtractOne = false)
    {
        return MessageTransformer.FormatPlayerMessage(rawMessage, playerName, playerSteamId, pos, subtractOne)
            .Replace(SHOUT, shout);
    }
    /// <summary>
    /// Format an event message using the config values, with extra event information.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventStartMsg">Event start message</param>
    /// <param name="eventEndMsg">Event end message</param>
    public static string FormatEventMessage(string rawMessage, string eventStartMsg, string eventEndMsg)
    {
        return MessageTransformer.ReplaceVariables(rawMessage)
            .Replace(EVENT_START_MSG, eventStartMsg)
            .Replace(EVENT_END_MSG, eventEndMsg);
        //.Replace(EVENT_PLAYERS, players); //! Removed until re can reliably poll player locations
    }
    /// <summary>
    /// Format an event message using the config values, with extra event information.
    /// Specifically, this version includes the event position in the message.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventStartMsg">Event start message</param>
    /// <param name="eventEndMsg">Event end message</param>
    /// <param name="pos">Position of the event</param>
    public static string FormatEventMessage(string rawMessage, string eventStartMsg, string eventEndMsg, UnityEngine.Vector3 pos)
    {
        return MessageTransformer.FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg)
            .Replace(POS, $"{pos}");
    }
    /// <summary>
    /// Format an event start message. This will only replace the event message with the start message.
    /// </summary>
    public static string FormatEventStartMessage(string rawMessage, string eventStartMsg, string eventEndMsg)
    {
        return MessageTransformer.FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg)
            .Replace(EVENT_MSG, eventStartMsg);
    }
    /// <summary>
    /// Format an event end message. This will only replace the event message with the end message.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventStartMsg">Event start message</param>
    /// <param name="eventEndMsg">Event end message</param>
    /// <param name="pos">Position of the event</param>
    public static string FormatEventEndMessage(string rawMessage, string eventStartMsg, string eventEndMsg)
    {
        return MessageTransformer.FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg)
            .Replace(EVENT_MSG, eventEndMsg);
    }
    /// <summary>
    /// Format an event start message. This will only replace the event message with the start message.
    /// Specifically, this version includes the event position in the message.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventStartMsg">Event start message</param>
    /// <param name="eventEndMsg">Event end message</param>
    /// <param name="pos">Position of the event</param>
    public static string FormatEventStartMessage(string rawMessage, string eventStartMsg, string eventEndMsg, UnityEngine.Vector3 pos)
    {
        return MessageTransformer.FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg, pos)
            .Replace(EVENT_MSG, eventStartMsg);
    }
    /// <summary>
    /// Format an event end message. This will only replace the event message with the end message.
    /// Specifically, this version includes the event position in the message.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventStartMsg">Event start message</param>
    /// <param name="eventEndMsg">Event end message</param>
    /// <param name="pos">Position of the event</param>
    public static string FormatEventEndMessage(string rawMessage, string eventStartMsg, string eventEndMsg, UnityEngine.Vector3 pos)
    {
        return MessageTransformer.FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg, pos)
            .Replace(EVENT_MSG, eventEndMsg);
    }
    /// <summary>
    /// Format the header of the leaderboard header using the config values.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    public static string FormatLeaderBoardHeader(string rawMessage)
    {
        return MessageTransformer.ReplaceVariables(rawMessage);
    }
    /// <summary>
    /// Format the header of the leaderboard header using the config values.
    /// Specifically, this version includes the number of items in the leaderboard.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="n">Number of items in the leaderboard</param>
    public static string FormatLeaderBoardHeader(string rawMessage, int n)
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
    public static string FormatVector3AsPos(UnityEngine.Vector3 vec3)
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
    public static string FormatAppendedPos(UnityEngine.Vector3 vec3)
    {
        string posStr = FormatVector3AsPos(vec3);
        return Plugin.StaticConfig.AppendedPosFormat
            .Replace(POS, posStr);
    }
}
