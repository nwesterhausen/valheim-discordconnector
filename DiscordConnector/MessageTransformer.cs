using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;

namespace DiscordConnector;

/// <summary>
///     Handles the transformation of messages for Discord, including variables replacement
///     and formatting for both plain text and embed-based message formats.
/// </summary>
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
    private const string TIMESTAMP = "%TIMESTAMP%";
    private const string TIMESINCE = "%TIMESINCE%";
    private const string UNIX_TIMESTAMP = "%UNIX_TIMESTAMP%";

    private static readonly Regex OpenCaretRegex = new(@"<[\w=]+>");
    private static readonly Regex CloseCaretRegex = new(@"</[\w]+>");

    /// <summary>
    ///     Replace the static variables in the message. These are variables that are set in the config file.
    ///     Replaces:
    ///     - `%VAR1%` through `%VAR10%` with the user variables
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="subtractOne">Subtract one from the number of online players</param>
    private static string ReplaceVariables(string rawMessage, bool subtractOne = false)
    {
        string customVariablesReplaced = rawMessage
            .Replace(VAR, DiscordConnectorPlugin.StaticConfig.UserVariable)
            .Replace(VAR_1, DiscordConnectorPlugin.StaticConfig.UserVariable1)
            .Replace(VAR_2, DiscordConnectorPlugin.StaticConfig.UserVariable2)
            .Replace(VAR_3, DiscordConnectorPlugin.StaticConfig.UserVariable3)
            .Replace(VAR_4, DiscordConnectorPlugin.StaticConfig.UserVariable4)
            .Replace(VAR_5, DiscordConnectorPlugin.StaticConfig.UserVariable5)
            .Replace(VAR_6, DiscordConnectorPlugin.StaticConfig.UserVariable6)
            .Replace(VAR_7, DiscordConnectorPlugin.StaticConfig.UserVariable7)
            .Replace(VAR_8, DiscordConnectorPlugin.StaticConfig.UserVariable8)
            .Replace(VAR_9, DiscordConnectorPlugin.StaticConfig.UserVariable9);

        return ReplaceDynamicVariables(customVariablesReplaced, subtractOne);
    }

    /// <summary>
    ///     Replace dynamic variables in the message. These are variables that are not static and need to be calculated at
    ///     runtime.
    ///     Replaces:
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
        dynamicReplacedMessage = ReplaceTimestamp(dynamicReplacedMessage);
        dynamicReplacedMessage = ReplaceTimesince(dynamicReplacedMessage);
        dynamicReplacedMessage = ReplaceUnixTimestamp(dynamicReplacedMessage);

        return dynamicReplacedMessage;
    }

    /// <summary>
    ///     Replace the timestamp in the message. This inserts the current time as a unix timestamp in a format that
    ///     discord replaces with a human readable time, adjusted to the user's timezone.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <returns>Message with the timestamp replaced</returns>
    private static string ReplaceTimestamp(string rawMessage)
    {
        return rawMessage
            .Replace(TIMESTAMP, $"<t:{(int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds}>");
    }

    /// <summary>
    ///     Replace the timestamp in the message. This inserts the current time as a unix timestamp in a format that
    ///     discord replaces with a human readable time, showing a countdown to the time. (e.g. "in 5 minutes" or "5 minutes
    ///     ago")
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <returns>Message with the timestamp replaced</returns>
    private static string ReplaceTimesince(string rawMessage)
    {
        return rawMessage
            .Replace(TIMESINCE, $"<t:{(int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds}:R>");
    }

    /// <summary>
    ///     Replace the unix timestamp in the message. This inserts the current time as a unix timestamp. This does not
    ///     get replaced on its own by Discord, but could be used to customize a `<t:UNIX_TIMESTAMP>` tag for Discord.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <returns>Message with the timestamp replaced</returns>
    private static string ReplaceUnixTimestamp(string rawMessage)
    {
        return rawMessage
            .Replace(UNIX_TIMESTAMP, ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString());
    }

    /// <summary>
    ///     Replace the join code in the message. Uses the ZPlayFabMatchmaking class to get the join code.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    private static string ReplaceJoinCode(string rawMessage)
    {
        return rawMessage
            .Replace(JOIN_CODE, ZPlayFabMatchmaking.JoinCode);
    }

    /// <summary>
    ///     Replace the day number in the message. Uses the EnvMan instance to get the current day.
    ///     This will only replace `%DAY_NUMBER%` with the current day if the EnvMan instance is available.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    private static string ReplaceDayNumber(string rawMessage)
    {
        // as written, if no EnvMan instance is available, it will return the raw message with `%DAY_NUMBER%` still in it.
        return rawMessage
            .Replace(DAY_NUMBER, EnvMan.instance != null ? EnvMan.instance.GetCurrentDay().ToString() : DAY_NUMBER);
    }

    /// <summary>
    ///     Replace the number of players in the message. Uses the ZNet instance to get the number of players.
    ///     This will only replace `%NUM_PLAYERS%` with the number of players if the ZNet instance is available.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="subtractOne">Subtract one from the number of online players</param>
    private static string ReplaceNumPlayers(string rawMessage, bool subtractOne = false)
    {
        if (subtractOne)
        {
            return rawMessage
                .Replace(NUM_PLAYERS,
                    ZNet.instance != null ? (ZNet.instance.GetNrOfPlayers() - 1).ToString() : NUM_PLAYERS);
        }

        return rawMessage
            .Replace(NUM_PLAYERS, ZNet.instance != null ? ZNet.instance.GetNrOfPlayers().ToString() : NUM_PLAYERS);
    }

    /// <summary>
    ///     Replace the public IP in the message. Uses the ZNet instance to get the public IP.
    ///     Note that if this fails, it will return an empty string. Also some occasions, the ZNet class may fail to get
    ///     the public IP address. This is out of scope for this plugin, to avoid making our own assumptions about the
    ///     network configuration of the server.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    private static string ReplacePublicIp(string rawMessage)
    {
        return rawMessage
            .Replace(PUBLIC_IP, DiscordConnectorPlugin.PublicIpAddress);
    }

    /// <summary>
    ///     Replace the world name in the message. Uses the ZNet instance to get the world name.
    ///     This will only replace `%WORLD_NAME%` with the world name if the ZNet instance is available.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    private static string ReplaceWorldName(string rawMessage)
    {
        return rawMessage
            .Replace(WORLD_NAME, ZNet.instance != null ? ZNet.instance.GetWorldName() : WORLD_NAME);
    }

    /// <summary>
    ///     Format a server message using the config values.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    public static string FormatServerMessage(string rawMessage)
    {
        return ReplaceVariables(rawMessage);
    }

    /// <summary>
    ///     Format a server message using the config values, with extra player information.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="playerName">Name of the player</param>
    /// <param name="playerId">ID of the player</param>
    /// <param name="subtractOne">(Optional) Subtract one from the number of online players</param>
    public static string FormatPlayerMessage(string rawMessage, string playerName, string playerId,
        bool subtractOne = false)
    {
        return ReplaceVariables(rawMessage, subtractOne)
            .Replace(PLAYER_STEAMID, playerId)
            .Replace(PLAYER_ID, playerId)
            .Replace(PLAYER_NAME, playerName);
    }

    /// <summary>
    ///     Format a server message using the config values, with extra player information.
    ///     Specifically, this version includes the player's position in the message.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="playerName">Name of the player</param>
    /// <param name="playerId">ID of the player</param>
    /// <param name="pos">Position of the player</param>
    /// <param name="subtractOne">(Optional) Subtract one from the number of online players</param>
    public static string FormatPlayerMessage(string rawMessage, string playerName, string playerId, Vector3 pos,
        bool subtractOne = false)
    {
        return FormatPlayerMessage(rawMessage, playerName, playerId, subtractOne)
            .Replace(POS, $"{pos}");
    }

    /// <summary>
    ///     Format a server message using the config values, with extra player information.
    ///     Specifically, this version includes the shout message in the message.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="playerName">Name of the player</param>
    /// <param name="playerId">ID of the player</param>
    /// <param name="shout">Shout message</param>
    /// <param name="subtractOne">(Optional) Subtract one from the number of online players</param>
    public static string FormatPlayerMessage(string rawMessage, string playerName, string playerId, string shout,
        bool subtractOne = false)
    {
        return FormatPlayerMessage(rawMessage, playerName, playerId, subtractOne)
            .Replace(SHOUT, shout);
    }

    /// <summary>
    ///     Format a server message using the config values, with extra player information.
    ///     Specifically, this version includes the shout message and the player's position in the message.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="playerName">Name of the player</param>
    /// <param name="playerSteamId">Steam ID of the player</param>
    /// <param name="shout">Shout message</param>
    /// <param name="pos">Position of the player</param>
    /// <param name="subtractOne">(Optional) Subtract one from the number of online players</param>
    public static string FormatPlayerMessage(string rawMessage, string playerName, string playerSteamId, string shout,
        Vector3 pos, bool subtractOne = false)
    {
        return FormatPlayerMessage(rawMessage, playerName, playerSteamId, pos, subtractOne)
            .Replace(SHOUT, shout);
    }

    /// <summary>
    ///     Format an event message using the config values, with extra event information.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventStartMsg">Event start message</param>
    /// <param name="eventEndMsg">Event end message</param>
    public static string FormatEventMessage(string rawMessage, string eventStartMsg, string eventEndMsg)
    {
        return ReplaceVariables(rawMessage)
            .Replace(EVENT_START_MSG, eventStartMsg)
            .Replace(EVENT_END_MSG, eventEndMsg);
        //.Replace(EVENT_PLAYERS, players); //! Removed until re can reliably poll player locations
    }

    /// <summary>
    ///     Format an event message using the config values, with extra event information.
    ///     Specifically, this version includes the event position in the message.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventStartMsg">Event start message</param>
    /// <param name="eventEndMsg">Event end message</param>
    /// <param name="pos">Position of the event</param>
    public static string FormatEventMessage(string rawMessage, string eventStartMsg, string eventEndMsg, Vector3 pos)
    {
        return FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg)
            .Replace(POS, $"{pos}");
    }

    /// <summary>
    ///     Format an event start message. This will only replace the event message with the start message.
    /// </summary>
    public static string FormatEventStartMessage(string rawMessage, string eventStartMsg, string eventEndMsg)
    {
        return FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg)
            .Replace(EVENT_MSG, eventStartMsg);
    }

    /// <summary>
    ///     Format an event end message. This will only replace the event message with the end message.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventStartMsg">Event start message</param>
    /// <param name="eventEndMsg">Event end message</param>
    /// <param name="pos">Position of the event</param>
    public static string FormatEventEndMessage(string rawMessage, string eventStartMsg, string eventEndMsg)
    {
        return FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg)
            .Replace(EVENT_MSG, eventEndMsg);
    }

    /// <summary>
    ///     Format an event start message. This will only replace the event message with the start message.
    ///     Specifically, this version includes the event position in the message.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventStartMsg">Event start message</param>
    /// <param name="eventEndMsg">Event end message</param>
    /// <param name="pos">Position of the event</param>
    public static string FormatEventStartMessage(string rawMessage, string eventStartMsg, string eventEndMsg,
        Vector3 pos)
    {
        return FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg, pos)
            .Replace(EVENT_MSG, eventStartMsg);
    }

    /// <summary>
    ///     Format an event end message. This will only replace the event message with the end message.
    ///     Specifically, this version includes the event position in the message.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventStartMsg">Event start message</param>
    /// <param name="eventEndMsg">Event end message</param>
    /// <param name="pos">Position of the event</param>
    public static string FormatEventEndMessage(string rawMessage, string eventStartMsg, string eventEndMsg, Vector3 pos)
    {
        return FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg, pos)
            .Replace(EVENT_MSG, eventEndMsg);
    }

    /// <summary>
    ///     Format the header of the leaderboard header using the config values.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    public static string FormatLeaderBoardHeader(string rawMessage)
    {
        return ReplaceVariables(rawMessage);
    }

    /// <summary>
    ///     Format the header of the leaderboard header using the config values.
    ///     Specifically, this version includes the number of items in the leaderboard.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="n">Number of items in the leaderboard</param>
    public static string FormatLeaderBoardHeader(string rawMessage, int n)
    {
        return ReplaceVariables(rawMessage)
            .Replace(N, n.ToString());
    }

    /// <summary>
    ///     Remove caret formatting from a string. This is used to strip special color codes away from user names.
    ///     For example, some mods can send messages as shouts in the game. They may try to color the name of the user:
    ///     `<color= cyan>[Admin]</color> vadmin`
    ///     This function strips away any caret formatting, making the string "plain text"
    ///     `[Admin] vadmin`
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
    ///     Format a vector3 position into the formatted version used by discord connector
    /// </summary>
    /// <param name="vec3">Position vector to turn into string</param>
    /// <returns>String following the formatting laid out in the variable config file.</returns>
    public static string FormatVector3AsPos(Vector3 vec3)
    {
        return DiscordConnectorPlugin.StaticConfig.PosVarFormat
            .Replace("%X%", vec3.x.ToString("F1"))
            .Replace("%Y%", vec3.y.ToString("F1"))
            .Replace("%Z%", vec3.z.ToString("F1"));
    }

    /// <summary>
    ///     Format the appended position data using the config values.
    /// </summary>
    /// <param name="vec3">Position vector to include</param>
    /// <returns>String to append with the position information</returns>
    public static string FormatAppendedPos(Vector3 vec3)
    {
        string posStr = FormatVector3AsPos(vec3);
        return DiscordConnectorPlugin.StaticConfig.AppendedPosFormat
            .Replace(POS, posStr);
    }
    
    #region Embed Transformation Methods
    
    /// <summary>
    ///     Creates an embed for a server message using the appropriate template and formatting.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventType">The event type for color selection and template</param>
    /// <returns>A configured EmbedBuilder instance</returns>
    public static EmbedBuilder CreateServerMessageEmbed(string rawMessage, Webhook.Event eventType)
    {
        string formattedMessage = FormatServerMessage(rawMessage);
        string worldName = ZNet.instance != null ? ZNet.instance.GetWorldName() : "Unknown World";
        string serverName = DiscordConnectorPlugin.StaticConfig.ServerName;
        
        return EmbedTemplates.ServerLifecycle(eventType, formattedMessage, worldName, serverName);
    }
    
    /// <summary>
    ///     Creates an embed for a player death event using the appropriate template and formatting.
    /// </summary>
    /// <param name="peer">The player's ZNetPeer instance</param>
    /// <param name="message">The formatted death message</param>
    /// <param name="eventType">The event type for color selection and template</param>
    /// <returns>A configured EmbedBuilder instance for a death event</returns>
    public static EmbedBuilder CreateDeathEmbed(ZNetPeer peer, string message, Webhook.Event eventType)
    {
        string playerName = peer.m_playerName;
        string playerHostName = peer.m_socket.GetHostName();
        Vector3 position = peer.m_refPos;
        string worldName = ZNet.instance != null ? ZNet.instance.GetWorldName() : "Unknown World";
        string serverName = DiscordConnectorPlugin.StaticConfig.ServerName;
        
        // Create a death-specific embed with custom styling
        EmbedBuilder embedBuilder = new EmbedBuilder()
            .SetColorForEvent(eventType)
            .SetDescription(message)
            .SetTimestamp()
            .SetTitle("💀 Player Death")
            // Always use server name (Valheim) as the author instead of player name
            .SetAuthor(serverName, null, DiscordConnectorPlugin.StaticConfig.EmbedAuthorIconUrl)
            .SetFooter($"World: {worldName} • Today at {DateTime.Now:HH:mm}");
        
        // Set thumbnail if enabled in config
        if (DiscordConnectorPlugin.StaticConfig.EmbedThumbnailEnabled)
        {
            embedBuilder.SetThumbnail(DiscordConnectorPlugin.StaticConfig.EmbedThumbnailUrl);
        }
            
        // Add position field if enabled in config
        if (DiscordConnectorPlugin.StaticConfig.PlayerDeathPosEnabled)
        {
            embedBuilder.AddField("Death Location", FormatVector3AsPos(position), true);
        }
        
        // Add player ID field if enabled in config
        if (DiscordConnectorPlugin.StaticConfig.ShowPlayerIds)
        {
            embedBuilder.AddField("Player ID", playerHostName, true);
        }
        
        // Always add player name field
        embedBuilder.AddField("Player", playerName, true);
        
        return embedBuilder;
    }
    
    /// <summary>
    ///     Creates an embed for a shout event from a specific player.
    /// </summary>
    /// <param name="peer">The player's ZNetPeer instance</param>
    /// <param name="text">The shout message text</param>
    /// <param name="eventType">The event type for color selection and template</param>
    /// <returns>A configured EmbedBuilder instance for a shout event</returns>
    public static EmbedBuilder CreateShoutEmbed(ZNetPeer peer, string text, Webhook.Event eventType)
    {
        string playerName = peer.m_playerName;
        string playerHostName = peer.m_socket.GetHostName();
        string worldName = ZNet.instance != null ? ZNet.instance.GetWorldName() : "Unknown World";
        
        // Capitalize the shout if configuration requires it
        string shoutText = DiscordConnectorPlugin.StaticConfig.ChatShoutAllCaps ? text.ToUpper() : text;
        
        return EmbedTemplates.ChatMessage(eventType, shoutText, playerName, null, worldName);
    }
    
    /// <summary>
    ///     Creates an embed for a player-related message using the appropriate template and formatting.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventType">The event type for color selection and template</param>
    /// <param name="playerName">Name of the player</param>
    /// <param name="playerId">ID of the player</param>
    /// <param name="subtractOne">Whether to subtract one from player count</param>
    /// <returns>A configured EmbedBuilder instance</returns>
    public static EmbedBuilder CreatePlayerMessageEmbed(string rawMessage, Webhook.Event eventType, 
                                                      string playerName, string playerId, bool subtractOne = false)
    {
        string formattedMessage = FormatPlayerMessage(rawMessage, playerName, playerId, subtractOne);
        string worldName = ZNet.instance != null ? ZNet.instance.GetWorldName() : "Unknown World";
        
        return EmbedTemplates.PlayerEvent(eventType, formattedMessage, playerName, null, worldName);
    }
    
    /// <summary>
    ///     Creates an embed for a player-related message with position using the appropriate template and formatting.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventType">The event type for color selection and template</param>
    /// <param name="playerName">Name of the player</param>
    /// <param name="playerId">ID of the player</param>
    /// <param name="position">Position of the player</param>
    /// <param name="subtractOne">Whether to subtract one from player count</param>
    /// <returns>A configured EmbedBuilder instance</returns>
    public static EmbedBuilder CreatePlayerMessageEmbed(string rawMessage, Webhook.Event eventType, 
                                                      string playerName, string playerId, Vector3 position, 
                                                      bool subtractOne = false)
    {
        string formattedMessage = FormatPlayerMessage(rawMessage, playerName, playerId, position, subtractOne);
        string worldName = ZNet.instance != null ? ZNet.instance.GetWorldName() : "Unknown World";
        
        return EmbedTemplates.PlayerEvent(eventType, formattedMessage, playerName, position, worldName);
    }
    
    /// <summary>
    ///     Creates an embed for a chat/shout message using the appropriate template and formatting.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventType">The event type for color selection and template</param>
    /// <param name="playerName">Name of the player</param>
    /// <param name="playerId">ID of the player</param>
    /// <param name="shout">Shout message</param>
    /// <param name="subtractOne">Whether to subtract one from player count</param>
    /// <returns>A configured EmbedBuilder instance</returns>
    public static EmbedBuilder CreateShoutMessageEmbed(string rawMessage, Webhook.Event eventType, 
                                                     string playerName, string playerId, string shout, 
                                                     bool subtractOne = false)
    {
        // For chat messages, we want to use the actual shout as the description
        // so we'll just process variables in the raw message
        string formattedMessage = FormatPlayerMessage(rawMessage, playerName, playerId, shout, subtractOne);
        string worldName = ZNet.instance != null ? ZNet.instance.GetWorldName() : "Unknown World";
        
        return EmbedTemplates.ChatMessage(eventType, shout, playerName, null, worldName);
    }
    
    /// <summary>
    ///     Creates an embed for a chat/shout message with position using the appropriate template and formatting.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventType">The event type for color selection and template</param>
    /// <param name="playerName">Name of the player</param>
    /// <param name="playerId">ID of the player</param>
    /// <param name="shout">Shout message</param>
    /// <param name="position">Position of the player</param>
    /// <param name="subtractOne">Whether to subtract one from player count</param>
    /// <returns>A configured EmbedBuilder instance</returns>
    public static EmbedBuilder CreateShoutMessageEmbed(string rawMessage, Webhook.Event eventType, 
                                                     string playerName, string playerId, string shout, 
                                                     Vector3 position, bool subtractOne = false)
    {
        // For chat messages, we want to use the actual shout as the description
        // so we'll just process variables in the raw message
        string formattedMessage = FormatPlayerMessage(rawMessage, playerName, playerId, shout, position, subtractOne);
        string worldName = ZNet.instance != null ? ZNet.instance.GetWorldName() : "Unknown World";
        
        return EmbedTemplates.ChatMessage(eventType, shout, playerName, position, worldName);
    }
    
    /// <summary>
    ///     Creates an embed for an event message using the appropriate template and formatting.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventType">The event type for color selection and template</param>
    /// <param name="eventStartMsg">Event start message</param>
    /// <param name="eventEndMsg">Event end message</param>
    /// <returns>A configured EmbedBuilder instance</returns>
    public static EmbedBuilder CreateEventMessageEmbed(string rawMessage, Webhook.Event eventType, 
                                                     string eventStartMsg, string eventEndMsg)
    {
        string formattedMessage = FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg);
        string worldName = ZNet.instance != null ? ZNet.instance.GetWorldName() : "Unknown World";
        string eventName;
        
        // Determine event name based on event type
        if (eventType == Webhook.Event.EventStart)
        {
            eventName = eventStartMsg;
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Creating EventStart embed with name: {eventName}");
        }
        else if (eventType == Webhook.Event.EventStop)
        {
            eventName = eventEndMsg;
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Creating EventStop embed with name: {eventName}");
        }
        else
        {
            eventName = "Game Event";
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Creating other game event embed with name: {eventName} - Event type: {eventType}");
        }
        
        return EmbedTemplates.WorldEvent(eventType, formattedMessage, eventName, worldName);
    }
    
    /// <summary>
    ///     Creates an embed for an event message with position using the appropriate template and formatting.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="eventType">The event type for color selection and template</param>
    /// <param name="eventStartMsg">Event start message</param>
    /// <param name="eventEndMsg">Event end message</param>
    /// <param name="position">Position of the event</param>
    /// <returns>A configured EmbedBuilder instance</returns>
    public static EmbedBuilder CreateEventMessageEmbed(string rawMessage, Webhook.Event eventType, 
                                                     string eventStartMsg, string eventEndMsg, Vector3 position)
    {
        string formattedMessage = FormatEventMessage(rawMessage, eventStartMsg, eventEndMsg, position);
        string worldName = ZNet.instance != null ? ZNet.instance.GetWorldName() : "Unknown World";
        string eventName;
        
        // Determine event name based on event type
        if (eventType == Webhook.Event.EventStart)
        {
            eventName = eventStartMsg;
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Creating EventStart embed with name: {eventName}");
        }
        else if (eventType == Webhook.Event.EventStop)
        {
            eventName = eventEndMsg;
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Creating EventStop embed with name: {eventName}");
        }
        else
        {
            eventName = "Game Event";
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Creating other game event embed with name: {eventName} - Event type: {eventType}");
        }
        
        var embed = EmbedTemplates.WorldEvent(eventType, formattedMessage, eventName, worldName);
        embed.AddPositionField(position);
        return embed;
    }
    
    /// <summary>
    ///     Creates an embed for a leaderboard message using the appropriate template and formatting.
    /// </summary>
    /// <param name="headerMessage">Raw header message to format</param>
    /// <param name="eventType">The event type for color selection</param>
    /// <param name="entries">Leaderboard entries as name/value tuples</param>
    /// <returns>A configured EmbedBuilder instance</returns>
    public static EmbedBuilder CreateLeaderboardEmbed(string headerMessage, Webhook.Event eventType, 
                                                    List<Tuple<string, string>> entries)
    {
        string formattedHeader = FormatLeaderBoardHeader(headerMessage, entries.Count);
        string worldName = ZNet.instance != null ? ZNet.instance.GetWorldName() : "Unknown World";
        
        // Extract a title from the header, limited to the first 50 characters
        string title = "Leaderboard";
        string description = formattedHeader;
        
        // If the header is long enough, try to split it into title and description
        if (formattedHeader.Length > 10 && formattedHeader.Contains("\n"))
        {
            var parts = formattedHeader.Split(new[] { '\n' }, 2);
            if (parts.Length > 1)
            {
                title = parts[0].Trim();
                description = parts[1].Trim();
            }
        }
        
        return EmbedTemplates.LeaderboardEmbed(title, entries, worldName);
    }
    
    /// <summary>
    ///     Creates an embed for a position message using the appropriate template and formatting.
    /// </summary>
    /// <param name="rawMessage">Raw message to format</param>
    /// <param name="playerName">Name of the player</param>
    /// <param name="position">Position of the player</param>
    /// <param name="eventType">The event type to determine title formatting</param>
    /// <returns>A configured EmbedBuilder instance</returns>
    public static EmbedBuilder CreatePositionEmbed(string rawMessage, string playerName, Vector3 position, Webhook.Event eventType)
    {
        string formattedMessage = FormatServerMessage(rawMessage);
        string worldName = ZNet.instance != null ? ZNet.instance.GetWorldName() : "Unknown World";
        
        return EmbedTemplates.PositionMessage(eventType, formattedMessage, playerName, position, worldName);
    }
    
    /// <summary>
    ///     Extracts player information for embed author usage.
    /// </summary>
    /// <param name="playerName">Name of the player</param>
    /// <param name="eventType">Event type (not used for icon selection anymore)</param>
    /// <returns>Tuple with author name, url and icon url (both null)</returns>
    public static Tuple<string, string?, string?> GetPlayerAuthorInfo(string playerName, Webhook.Event eventType)
    {
        string authorName = playerName;
        string? authorUrl = null;
        string? iconUrl = null;
        
        // No longer using icons for player author information
        // Just return the player name with null for URL and icon URL
        
        return new Tuple<string, string?, string?>(authorName, authorUrl, iconUrl);
    }
    
    /// <summary>
    ///     Creates field content for embed usage, applying proper formatting and line breaks.
    /// </summary>
    /// <param name="content">Raw content to format</param>
    /// <param name="maxLength">Maximum length for the field value</param>
    /// <returns>Formatted field content</returns>
    public static string FormatFieldContent(string content, int maxLength = 1024)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }
        
        // Process Discord formatting (like code blocks, bold, etc.)
        string formattedContent = content;
        
        // Ensure content doesn't exceed maximum length
        if (formattedContent.Length > maxLength)
        {
            formattedContent = formattedContent.Substring(0, maxLength - 3) + "...";
        }
        
        return formattedContent;
    }
    
    #endregion
}
