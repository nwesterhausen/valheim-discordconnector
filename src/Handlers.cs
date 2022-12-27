using System.Collections.Generic;
using UnityEngine;

namespace DiscordConnector;
internal static class Handlers
{
    /// <summary>
    /// Track players on the server using their host names, e.g. Steam_109248510103 or XBox_2091010148
    /// </summary>
    public static HashSet<string> joinedPlayers = new HashSet<string>();

    /// <summary>
    /// Perform the necessary steps for a player joining the server.
    /// </summary>
    public static void Join(ZNetPeer peer)
    {
        // - If it's their first time:
        //     a. If first join announcements are enabled:
        //         i. Send a first join announcement to Discord
        //     b. (else) If player join messages are enabled:
        //         i. Send a message to discord
        // - If recording player join stats is enabled
        //     a. Save a record of the player joining
        // - Add player to the player list

        // Get the player's hostname to use for record keeping and logging
        string playerHostName = $"{peer.m_socket.GetHostName()}";

        // Try adding the player to the joinedPlayers list. If we are not able to add them, check if it's a dead player before doing nothing.
        if (!joinedPlayers.Add(playerHostName))
        {
            Plugin.StaticLogger.LogDebug($"{playerHostName} already exists in list of joined players.");

            // Seems that player is dead if character ZDOID id is 0
            // m_characterID id=0 means dead, user_id always matches peer.m_uid
            if (peer.m_characterID.id != 0)
            {
                Handlers.Death(peer);
            }

            return;
        }

        Plugin.StaticLogger.LogDebug($"Added player {playerHostName} peer_id:{peer.m_uid} ({peer.m_playerName}) to joined player list.");

        // Create basic message pre-formatting
        string preFormattedMessage = "";
        // If first-time joining announcements are enabled and we have no record of the player joining, set the message content to the first time join announcement
        if (Plugin.StaticConfig.AnnouncePlayerFirstJoinEnabled && Plugin.StaticDatabase.CountOfRecordsByName(Records.Categories.Join, peer.m_playerName) == 0)
        {
            preFormattedMessage = Plugin.StaticConfig.PlayerFirstJoinMessage;
        }
        // If sending messages for players joining is enabled
        else if (Plugin.StaticConfig.PlayerJoinMessageEnabled)
        {
            preFormattedMessage = Plugin.StaticConfig.JoinMessage;
        }


        // If recording player join statistics is enabled, save a record of player joining
        if (Plugin.StaticConfig.StatsJoinEnabled)
        {
            Plugin.StaticDatabase.InsertSimpleStatRecord(Records.Categories.Join, peer.m_playerName, playerHostName, peer.m_refPos);
        }


        // If sending messages for players joining is completely disabled
        if (string.IsNullOrEmpty(preFormattedMessage))
        {
            return;
        }

        FinalizeFormattingAndSend(peer, playerHostName, preFormattedMessage, Plugin.StaticConfig.PlayerJoinPosEnabled);
    }

    /// <summary>
    /// Perform the necessary steps for a player leaving the server.
    /// </summary>
    public static void Leave(ZNetPeer peer)
    {
        // - If it's their first time:
        //     a. If first leave announcements are enabled:
        //         i. Send a first leave announcement to Discord
        //     b. (else) If player leave messages are enabled:
        //         i. Send a message to discord
        // - If recording player leave stats is enabled
        //     a. Save a record of the player leaving
        // - Remove player from the player list

        // Get the player's hostname to use for record keeping and logging
        string playerHostName = $"{peer.m_socket.GetHostName()}";

        // Try removing the player to the joinedPlayers list. If we couldn't remove them, then do nothing.
        if (!joinedPlayers.Remove(playerHostName))
        {
            Plugin.StaticLogger.LogDebug($"{playerHostName} did not exist in the list of joined players!");
            return;
        }

        Plugin.StaticLogger.LogDebug($"Removed player {playerHostName} peer_id:{peer.m_uid} ({peer.m_playerName}) from joined player list.");

        // Create basic message pre-formatting
        string preFormattedMessage = "";
        // If first-time leaving announcements are enabled and we have no record of the player leaving, set the message content to the first time leave announcement
        if (Plugin.StaticConfig.AnnouncePlayerFirstLeaveEnabled && Plugin.StaticDatabase.CountOfRecordsByName(Records.Categories.Leave, peer.m_playerName) == 0)
        {
            preFormattedMessage = Plugin.StaticConfig.PlayerFirstLeaveMessage;
        }
        // If sending messages for players leaving is enabled
        else if (Plugin.StaticConfig.PlayerLeaveMessageEnabled)
        {
            preFormattedMessage = Plugin.StaticConfig.LeaveMessage;
        }


        // If recording player leave statistics is enabled, save a record of player leaving
        if (Plugin.StaticConfig.StatsLeaveEnabled)
        {
            Plugin.StaticDatabase.InsertSimpleStatRecord(Records.Categories.Leave, peer.m_playerName, playerHostName, peer.m_refPos);
        }


        // If sending messages for players leaving is completely disabled
        if (string.IsNullOrEmpty(preFormattedMessage))
        {
            return;
        }

        FinalizeFormattingAndSend(peer, playerHostName, preFormattedMessage, Plugin.StaticConfig.PlayerLeavePosEnabled);
    }

    /// <summary>
    /// Perform the necessary steps for a player dying on the server.
    /// </summary>
    public static void Death(ZNetPeer peer)
    {
        // - If it's their first time:
        //     a. If first death announcements are enabled:
        //         i. Send a first death announcement to Discord
        //     b. (else) If player death messages are enabled:
        //         i. Send a message to discord
        // - If recording player death stats is enabled
        //     a. Save a record of the player dying
        // - Remove player from the player list

        // Get the player's hostname to use for record keeping and logging
        string playerHostName = $"{peer.m_socket.GetHostName()}";

        // Create basic message pre-formatting
        string preFormattedMessage = "";
        // If first-time dying announcements are enabled and we have no record of the player dying, set the message content to the first time death announcement
        if (Plugin.StaticConfig.AnnouncePlayerFirstDeathEnabled && Plugin.StaticDatabase.CountOfRecordsByName(Records.Categories.Death, peer.m_playerName) == 0)
        {
            preFormattedMessage = Plugin.StaticConfig.PlayerFirstDeathMessage;
        }
        // If sending messages for players dying is enabled
        else if (Plugin.StaticConfig.PlayerDeathMessageEnabled)
        {
            preFormattedMessage = Plugin.StaticConfig.DeathMessage;
        }

        if (Plugin.StaticConfig.StatsDeathEnabled)
        {
            Plugin.StaticDatabase.InsertSimpleStatRecord(Records.Categories.Death, peer.m_playerName, playerHostName, peer.m_refPos);
        }



        // If sending messages for players dying is completely disabled
        if (string.IsNullOrEmpty(preFormattedMessage))
        {
            return;
        }

        FinalizeFormattingAndSend(peer, playerHostName, preFormattedMessage, Plugin.StaticConfig.PlayerDeathPosEnabled);
    }

    /// <summary>
    /// Perform the necessary steps for a player pinging on the server.
    /// </summary>
    public static void Ping(ZNetPeer peer, Vector3 pos)
    {
        // - If it's their first time:
        //     a. If first ping announcements are enabled:
        //         i. Send a first ping announcement to Discord
        //     b. (else) If player ping messages are enabled:
        //         i. Send a message to discord
        // - If recording player ping stats is enabled
        //     a. Save a record of the player pinging
        // - Remove player from the player list

        // Get the player's hostname to use for record keeping and logging
        string playerHostName = $"{peer.m_socket.GetHostName()}";

        // Create basic message pre-formatting
        string preFormattedMessage = "";
        // If first-time pinging announcements are enabled and we have no record of the player pinging, set the message content to the first time ping announcement
        if (Plugin.StaticConfig.AnnouncePlayerFirstPingEnabled && Plugin.StaticDatabase.CountOfRecordsByName(Records.Categories.Ping, peer.m_playerName) == 0)
        {
            preFormattedMessage = Plugin.StaticConfig.PlayerFirstPingMessage;
        }
        // If sending messages for players pinging is enabled
        else if (Plugin.StaticConfig.ChatPingEnabled)
        {
            preFormattedMessage = Plugin.StaticConfig.PingMessage;
        }

        if (Plugin.StaticConfig.StatsPingEnabled)
        {
            Plugin.StaticDatabase.InsertSimpleStatRecord(Records.Categories.Ping, peer.m_playerName, playerHostName, pos);
        }



        // If sending messages for players pinging is completely disabled
        if (string.IsNullOrEmpty(preFormattedMessage))
        {
            return;
        }

        FinalizeFormattingAndSend(peer, playerHostName, preFormattedMessage, Plugin.StaticConfig.ChatPingPosEnabled, pos);
    }


    /// <summary>
    /// Perform the necessary steps for a player shouting on the server.
    /// </summary>
    public static void Shout(ZNetPeer peer, Vector3 pos, string text)
    {
        // - If it's their first time:
        //     a. If first shout announcements are enabled:
        //         i. Send a first shout announcement to Discord
        //     b. (else) If player shout messages are enabled:
        //         i. Send a message to discord
        // - If recording player shout stats is enabled
        //     a. Save a record of the player shouting

        // Get the player's hostname to use for record keeping and logging
        string playerHostName = $"{peer.m_socket.GetHostName()}";

        // Create basic message pre-formatting
        string preFormattedMessage = "";
        // If first-time shouting announcements are enabled and we have no record of the player shouting, set the message content to the first time ping announcement
        if (Plugin.StaticConfig.AnnouncePlayerFirstShoutEnabled && Plugin.StaticDatabase.CountOfRecordsByName(Records.Categories.Shout, peer.m_playerName) == 0)
        {
            preFormattedMessage = Plugin.StaticConfig.PlayerFirstShoutMessage;
        }
        // If sending messages for players shouting is enabled
        else if (Plugin.StaticConfig.ChatShoutEnabled)
        {
            preFormattedMessage = Plugin.StaticConfig.ShoutMessage;
        }

        if (Plugin.StaticConfig.StatsShoutEnabled)
        {
            Plugin.StaticDatabase.InsertSimpleStatRecord(Records.Categories.Shout, peer.m_playerName, playerHostName, pos);
        }



        // If sending messages for players shouting is completely disabled
        if (string.IsNullOrEmpty(preFormattedMessage))
        {
            return;
        }

        FinalizeFormattingAndSend(peer, playerHostName, preFormattedMessage, Plugin.StaticConfig.ChatShoutPosEnabled, pos, text);
    }

    /// <summary>
    /// Finish formatting the message based on the positional data (if allowed) and dispatch it to the Discord webhook.
    /// </summary>
    /// <param name="peer">Player peer reference</param>
    /// <param name="playerHostName">Player host name</param>
    /// <param name="preFormattedMessage">Raw message to format for sending to discord</param>
    /// <param name="posEnabled">If we are allowed to include the position data</param>
    private static void FinalizeFormattingAndSend(ZNetPeer peer, string playerHostName, string preFormattedMessage, bool posEnabled)
    {
        FinalizeFormattingAndSend(peer, playerHostName, preFormattedMessage, posEnabled, peer.m_refPos);
    }

    /// <summary>
    /// Finish formatting the message based on the positional data (if allowed) and dispatch it to the Discord webhook.
    /// </summary>
    /// <param name="peer">Player peer reference</param>
    /// <param name="playerHostName">Player host name</param>
    /// <param name="preFormattedMessage">Raw message to format for sending to discord</param>
    /// <param name="posEnabled">If we are allowed to include the position data</param>
    /// <param name="pos">Positional data to use in formatting</param>
    private static void FinalizeFormattingAndSend(ZNetPeer peer, string playerHostName, string preFormattedMessage, bool posEnabled, Vector3 pos)
    {
        // Format the message accordingly, depending if it has the %POS% variable or not
        string finalMessage;
        if (preFormattedMessage.Contains("%POS%"))
        {
            if (!posEnabled)
            {
                preFormattedMessage.Replace("%POS%", "");
            }

            finalMessage = MessageTransformer.FormatPlayerMessage(preFormattedMessage, peer.m_playerName, playerHostName, pos);

        }
        else
        {
            finalMessage = MessageTransformer.FormatPlayerMessage(preFormattedMessage, peer.m_playerName, playerHostName);
        }

        // If sending the position with the player join message is enabled
        if (posEnabled)
        {
            // If "fancier" discord messages are enabled OR if the message we intend to send DOES NOT contain the %POS% variable
            if (Plugin.StaticConfig.DiscordEmbedsEnabled || !finalMessage.Contains("%POS%"))
            {
                // Send the message to discord with an auto-appended POS (or as a POS embed if "fancier" discord messages are enabled)
                DiscordApi.SendMessage(finalMessage, pos);
                return;
            }
        }

        // Sending position data is not allowed OR the message doesn't contain the %POS% variable
        DiscordApi.SendMessage(finalMessage);
    }



    /// <summary>
    /// Finish formatting the message based on the positional data (if allowed) and dispatch it to the Discord webhook.
    /// 
    /// This method handles the text from shouts and other chat-adjacent things
    /// </summary>
    /// <param name="peer">Player peer reference</param>
    /// <param name="playerHostName">Player host name</param>
    /// <param name="preFormattedMessage">Raw message to format for sending to discord</param>
    /// <param name="posEnabled">If we are allowed to include the position data</param>
    /// <param name="pos">Positional data to use in formatting</param>
    /// <param name="text">Text that was sent</param>
    private static void FinalizeFormattingAndSend(ZNetPeer peer, string playerHostName, string preFormattedMessage, bool posEnabled, Vector3 pos, string text)
    {
        // Format the message accordingly, depending if it has the %POS% variable or not
        string finalMessage;
        if (preFormattedMessage.Contains("%POS%"))
        {
            if (!posEnabled)
            {
                preFormattedMessage.Replace("%POS%", "");
            }

            finalMessage = MessageTransformer.FormatPlayerMessage(preFormattedMessage, peer.m_playerName, playerHostName, text, pos);

        }
        else
        {
            finalMessage = MessageTransformer.FormatPlayerMessage(preFormattedMessage, peer.m_playerName, playerHostName, text);
        }

        // If sending the position with the player join message is enabled
        if (posEnabled)
        {
            // If "fancier" discord messages are enabled OR if the message we intend to send DOES NOT contain the %POS% variable
            if (Plugin.StaticConfig.DiscordEmbedsEnabled || !finalMessage.Contains("%POS%"))
            {
                // Send the message to discord with an auto-appended POS (or as a POS embed if "fancier" discord messages are enabled)
                DiscordApi.SendMessage(finalMessage, pos);
                return;
            }
        }

        // Sending position data is not allowed OR the message doesn't contain the %POS% variable
        DiscordApi.SendMessage(finalMessage);
    }

    /// <summary>
    /// Handle a non-player chat message. Currently only works for shouts.
    /// 
    /// If allowed in the configuration, this will send a message to discord as if a player shouted. 
    /// </summary>
    /// <param name="type">Type of chat message</param>
    /// <param name="user">Listed username of sender</param>
    /// <param name="text">Text sent to chat</param>
    internal static void NonPlayerChat(Talker.Type type, string user, string text)
    {
        // Check if we allow non-player shouts
        if (Plugin.StaticConfig.AllowNonPlayerShoutLogging)
        {
            // Guard against chats that aren't shouts by non-players
            if (type != Talker.Type.Shout)
            {
                Plugin.StaticLogger.LogDebug($"Ignored ping/join/leave from non-player {user}");
                return;
            }

            string nonPlayerHostName = "";
            Plugin.StaticLogger.LogDebug($"Sending shout from '{user}' to discord: '{text}'");

            // Only if we are sending shouts per the config should we send the shout
            if (Plugin.StaticConfig.ChatShoutEnabled)
            {
                // Clean any "formatting" done to the username. This includes coloring via <color=x> tags.
                string userCleaned = MessageTransformer.CleanCaretFormatting(user);
                // Format the message into the shout format as defined in the config files
                string message = MessageTransformer.FormatPlayerMessage(Plugin.StaticConfig.ShoutMessage, userCleaned, nonPlayerHostName, text);

                // Non-players shouldn't have a position, so disregard any position in the message formatting
                if (message.Contains("%POS%"))
                {
                    message.Replace("%POS%", "");
                }

                DiscordApi.SendMessage(message);
            }
            // Exit the function since we sent the message
            return;
        }

        Plugin.StaticLogger.LogInfo($"Ignored shout from {user} because they aren't a real player");
    }
}
