using System.Collections.Generic;
using DiscordConnector.Records;
using UnityEngine;

namespace DiscordConnector;

internal static class Handlers
{
    /// <summary>
    ///     Track players on the server using their host names, e.g. Steam_109248510103 or XBox_2091010148
    /// </summary>
    public static HashSet<string> joinedPlayers = new();

    /// <summary>
    ///     Perform the necessary steps for a player joining the server.
    /// </summary>
    public static void Join(ZNetPeer peer)
    {
        if (peer == null)
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug("Handler:Join - Guarded against null peer");
            return;
        }

        // - If it's their first time:
        //     a. If first join announcements are enabled:
        //         i. Send a first join announcement to Discord
        //     b. (else) If player join messages are enabled:
        //         i. Send a message to discord
        // - If recording player join stats is enabled
        //     a. Save a record of the player joining
        // - Add player to the player list
        Webhook.Event ev = Webhook.Event.PlayerJoin;

        // Get the player's hostname to use for record keeping and logging
        string playerHostName = $"{peer.m_socket.GetHostName()}";

        // Try adding the player to the joinedPlayers list. If we are not able to add them, check if it's a dead player before doing nothing.
        if (!joinedPlayers.Add(playerHostName))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug($"{playerHostName} already exists in list of joined players.");

            // Seems that player is dead if character ZDOID id is 0
            // m_characterID id=0 means dead, user_id always matches peer.m_uid
            if (peer.m_characterID.ID != 0)
            {
                Death(peer);
            }

            return;
        }

        DiscordConnectorPlugin.StaticLogger.LogDebug(
            $"Added player {playerHostName} peer_id:{peer.m_uid} ({peer.m_playerName}) to joined player list.");

        // Create basic message pre-formatting
        string preFormattedMessage = "";
        // If first-time joining announcements are enabled and we have no record of the player joining, set the message content to the first time join announcement
        if (DiscordConnectorPlugin.StaticConfig.AnnouncePlayerFirstJoinEnabled &&
            DiscordConnectorPlugin.StaticDatabase.CountOfRecordsByName(Categories.Join, peer.m_playerName) == 0)
        {
            preFormattedMessage = DiscordConnectorPlugin.StaticConfig.PlayerFirstJoinMessage;
            ev = Webhook.Event.PlayerFirstJoin;
        }
        // If sending messages for players joining is enabled
        else if (DiscordConnectorPlugin.StaticConfig.PlayerJoinMessageEnabled)
        {
            preFormattedMessage = DiscordConnectorPlugin.StaticConfig.JoinMessage;
        }


        // If recording player join statistics is enabled, save a record of player joining
        if (DiscordConnectorPlugin.StaticConfig.StatsJoinEnabled)
        {
            DiscordConnectorPlugin.StaticDatabase.InsertSimpleStatRecord(Categories.Join, peer.m_playerName,
                playerHostName, peer.m_refPos);
        }


        // If sending messages for players joining is completely disabled
        if (string.IsNullOrEmpty(preFormattedMessage))
        {
            return;
        }

        FinalizeFormattingAndSend(peer, playerHostName, preFormattedMessage,
            DiscordConnectorPlugin.StaticConfig.PlayerJoinPosEnabled, ev);
    }

    /// <summary>
    ///     Perform the necessary steps for a player leaving the server.
    /// </summary>
    public static void Leave(ZNetPeer peer)
    {
        if (peer == null)
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug("Handler:Leave - Guarded against null peer");
            return;
        }

        // - If it's their first time:
        //     a. If first leave announcements are enabled:
        //         i. Send a first leave announcement to Discord
        //     b. (else) If player leave messages are enabled:
        //         i. Send a message to discord
        // - If recording player leave stats is enabled
        //     a. Save a record of the player leaving
        // - Remove player from the player list
        Webhook.Event ev = Webhook.Event.PlayerLeave;

        // Get the player's hostname to use for record keeping and logging
        string playerHostName = $"{peer.m_socket.GetHostName()}";

        // Try removing the player to the joinedPlayers list. If we couldn't remove them, then do nothing.
        if (!joinedPlayers.Remove(playerHostName))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug(
                $"{playerHostName} did not exist in the list of joined players!");
            return;
        }

        DiscordConnectorPlugin.StaticLogger.LogDebug(
            $"Removed player {playerHostName} peer_id:{peer.m_uid} ({peer.m_playerName}) from joined player list.");

        // Create basic message pre-formatting
        string preFormattedMessage = "";
        // If first-time leaving announcements are enabled and we have no record of the player leaving, set the message content to the first time leave announcement
        if (DiscordConnectorPlugin.StaticConfig.AnnouncePlayerFirstLeaveEnabled &&
            DiscordConnectorPlugin.StaticDatabase.CountOfRecordsByName(Categories.Leave, peer.m_playerName) == 0)
        {
            preFormattedMessage = DiscordConnectorPlugin.StaticConfig.PlayerFirstLeaveMessage;
            ev = Webhook.Event.PlayerFirstLeave;
        }
        // If sending messages for players leaving is enabled
        else if (DiscordConnectorPlugin.StaticConfig.PlayerLeaveMessageEnabled)
        {
            preFormattedMessage = DiscordConnectorPlugin.StaticConfig.LeaveMessage;
        }


        // If recording player leave statistics is enabled, save a record of player leaving
        if (DiscordConnectorPlugin.StaticConfig.StatsLeaveEnabled)
        {
            DiscordConnectorPlugin.StaticDatabase.InsertSimpleStatRecord(Categories.Leave, peer.m_playerName,
                playerHostName, peer.m_refPos);
        }


        // If sending messages for players leaving is completely disabled
        if (string.IsNullOrEmpty(preFormattedMessage))
        {
            return;
        }

        FinalizeFormattingAndSend(peer, playerHostName, preFormattedMessage,
            DiscordConnectorPlugin.StaticConfig.PlayerLeavePosEnabled, ev);
    }

    /// <summary>
    ///     Perform the necessary steps for a player dying on the server.
    /// </summary>
    public static void Death(ZNetPeer peer)
    {
        if (peer == null)
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug("Handler:Death - Guarded against null peer");
            return;
        }

        // - If it's their first time:
        //     a. If first death announcements are enabled:
        //         i. Send a first death announcement to Discord
        //     b. (else) If player death messages are enabled:
        //         i. Send a message to discord
        // - If recording player death stats is enabled
        //     a. Save a record of the player dying
        // - Remove player from the player list
        Webhook.Event ev = Webhook.Event.PlayerDeath;

        // Get the player's hostname to use for record keeping and logging
        string playerHostName = $"{peer.m_socket.GetHostName()}";

        // Create basic message pre-formatting
        string preFormattedMessage = "";
        // If first-time dying announcements are enabled and we have no record of the player dying, set the message content to the first time death announcement
        if (DiscordConnectorPlugin.StaticConfig.AnnouncePlayerFirstDeathEnabled &&
            DiscordConnectorPlugin.StaticDatabase.CountOfRecordsByName(Categories.Death, peer.m_playerName) == 0)
        {
            preFormattedMessage = DiscordConnectorPlugin.StaticConfig.PlayerFirstDeathMessage;
            ev = Webhook.Event.PlayerFirstDeath;
        }
        // If sending messages for players dying is enabled
        else if (DiscordConnectorPlugin.StaticConfig.PlayerDeathMessageEnabled)
        {
            preFormattedMessage = DiscordConnectorPlugin.StaticConfig.DeathMessage;
        }

        if (DiscordConnectorPlugin.StaticConfig.StatsDeathEnabled)
        {
            DiscordConnectorPlugin.StaticDatabase.InsertSimpleStatRecord(Categories.Death, peer.m_playerName,
                playerHostName, peer.m_refPos);
        }


        // If sending messages for players dying is completely disabled
        if (string.IsNullOrEmpty(preFormattedMessage))
        {
            return;
        }

        FinalizeFormattingAndSend(peer, playerHostName, preFormattedMessage,
            DiscordConnectorPlugin.StaticConfig.PlayerDeathPosEnabled, ev);
    }

    /// <summary>
    ///     Perform the necessary steps for a player pinging on the server.
    /// </summary>
    public static void Ping(ZNetPeer peer, Vector3 pos)
    {
        if (peer == null)
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug("Handler:Ping - Guarded against null peer");
            return;
        }

        // - If it's their first time:
        //     a. If first ping announcements are enabled:
        //         i. Send a first ping announcement to Discord
        //     b. (else) If player ping messages are enabled:
        //         i. Send a message to discord
        // - If recording player ping stats is enabled
        //     a. Save a record of the player pinging
        // - Remove player from the player list
        Webhook.Event ev = Webhook.Event.PlayerPing;

        // Get the player's hostname to use for record keeping and logging
        string playerHostName = $"{peer.m_socket.GetHostName()}";

        // Create basic message pre-formatting
        string preFormattedMessage = "";
        // If first-time pinging announcements are enabled and we have no record of the player pinging, set the message content to the first time ping announcement
        if (DiscordConnectorPlugin.StaticConfig.AnnouncePlayerFirstPingEnabled &&
            DiscordConnectorPlugin.StaticDatabase.CountOfRecordsByName(Categories.Ping, peer.m_playerName) == 0)
        {
            preFormattedMessage = DiscordConnectorPlugin.StaticConfig.PlayerFirstPingMessage;
            ev = Webhook.Event.PlayerFirstPing;
        }
        // If sending messages for players pinging is enabled
        else if (DiscordConnectorPlugin.StaticConfig.ChatPingEnabled)
        {
            preFormattedMessage = DiscordConnectorPlugin.StaticConfig.PingMessage;
        }

        if (DiscordConnectorPlugin.StaticConfig.StatsPingEnabled)
        {
            DiscordConnectorPlugin.StaticDatabase.InsertSimpleStatRecord(Categories.Ping, peer.m_playerName,
                playerHostName, pos);
        }


        // If sending messages for players pinging is completely disabled
        if (string.IsNullOrEmpty(preFormattedMessage))
        {
            return;
        }

        FinalizeFormattingAndSend(peer, playerHostName, preFormattedMessage,
            DiscordConnectorPlugin.StaticConfig.ChatPingPosEnabled, pos, ev);
    }


    /// <summary>
    ///     Perform the necessary steps for a player shouting on the server.
    /// </summary>
    public static void Shout(ZNetPeer peer, Vector3 pos, string text)
    {
        if (peer == null)
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug("Handler:Shout - Guarded against null peer");
            return;
        }

        // - If it's their first time:
        //     a. If first shout announcements are enabled:
        //         i. Send a first shout announcement to Discord
        //     b. (else) If player shout messages are enabled:
        //         i. Send a message to discord
        // - If recording player shout stats is enabled
        //     a. Save a record of the player shouting
        Webhook.Event ev = Webhook.Event.PlayerShout;

        // Get the player's hostname to use for record keeping and logging
        string playerHostName = $"{peer.m_socket.GetHostName()}";

        // Create basic message pre-formatting
        string preFormattedMessage = "";
        // If first-time shouting announcements are enabled and we have no record of the player shouting, set the message content to the first time ping announcement
        if (DiscordConnectorPlugin.StaticConfig.AnnouncePlayerFirstShoutEnabled &&
            DiscordConnectorPlugin.StaticDatabase.CountOfRecordsByName(Categories.Shout, peer.m_playerName) == 0)
        {
            preFormattedMessage = DiscordConnectorPlugin.StaticConfig.PlayerFirstShoutMessage;
            ev = Webhook.Event.PlayerFirstShout;
        }
        // If sending messages for players shouting is enabled
        else if (DiscordConnectorPlugin.StaticConfig.ChatShoutEnabled)
        {
            preFormattedMessage = DiscordConnectorPlugin.StaticConfig.ShoutMessage;
        }

        if (DiscordConnectorPlugin.StaticConfig.StatsShoutEnabled)
        {
            DiscordConnectorPlugin.StaticDatabase.InsertSimpleStatRecord(Categories.Shout, peer.m_playerName,
                playerHostName, pos);
        }

        // If sending messages for players shouting is completely disabled
        if (string.IsNullOrEmpty(preFormattedMessage))
        {
            return;
        }

        // Capitalize the entire shout if enabled
        if (DiscordConnectorPlugin.StaticConfig.ChatShoutAllCaps)
        {
            text = text.ToUpper();
        }

        FinalizeFormattingAndSend(peer, playerHostName, preFormattedMessage,
            DiscordConnectorPlugin.StaticConfig.ChatShoutPosEnabled, pos, text, ev);
    }

    /// <summary>
    ///     Finish formatting the message based on the positional data (if allowed) and dispatch it to the Discord webhook.
    /// </summary>
    /// <param name="peer">Player peer reference</param>
    /// <param name="playerHostName">Player host name</param>
    /// <param name="preFormattedMessage">Raw message to format for sending to discord</param>
    /// <param name="posEnabled">If we are allowed to include the position data</param>
    private static void FinalizeFormattingAndSend(ZNetPeer peer, string playerHostName, string preFormattedMessage,
        bool posEnabled, Webhook.Event ev)
    {
        FinalizeFormattingAndSend(peer, playerHostName, preFormattedMessage, posEnabled, peer.m_refPos, ev);
    }

    /// <summary>
    ///     Finish formatting the message based on the positional data (if allowed) and dispatch it to the Discord webhook.
    ///     Enhanced to support rich Discord embeds for player events.
    /// </summary>
    /// <param name="peer">Player peer reference</param>
    /// <param name="playerHostName">Player host name</param>
    /// <param name="preFormattedMessage">Raw message to format for sending to discord</param>
    /// <param name="posEnabled">If we are allowed to include the position data</param>
    /// <param name="pos">Positional data to use in formatting</param>
    /// <param name="ev">The event type that triggered this message</param>
    private static void FinalizeFormattingAndSend(ZNetPeer peer, string playerHostName, string preFormattedMessage,
        bool posEnabled, Vector3 pos, Webhook.Event ev)
    {
        // Format the message accordingly, depending if it has the %POS% variable or not
        string finalMessage;
        bool isPlayerLeaving = ev == Webhook.Event.PlayerLeave || ev == Webhook.Event.PlayerFirstLeave;
        if (preFormattedMessage.Contains("%POS%"))
        {
            if (!posEnabled)
            {
                preFormattedMessage = preFormattedMessage.Replace("%POS%", "");
            }

            finalMessage = MessageTransformer.FormatPlayerMessage(preFormattedMessage, peer.m_playerName,
                playerHostName, pos, isPlayerLeaving);
        }
        else
        {
            finalMessage = MessageTransformer.FormatPlayerMessage(preFormattedMessage, peer.m_playerName,
                playerHostName, isPlayerLeaving);
        }

        // Determine whether to use embeds based on configuration
        if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled)
        {
            // Create the appropriate embed based on event type and available data
            EmbedBuilder embedBuilder;
            
            // Determine which type of embed to create based on event category
            if (Webhook.PlayerDeathEvents.Contains(ev))
            {
                // For death events, use a specialized death embed
                embedBuilder = MessageTransformer.CreateDeathEmbed(peer, finalMessage, ev);
            }
            else if (posEnabled)
            {
                // For events with position data, use player message embed with position
                embedBuilder = MessageTransformer.CreatePlayerMessageEmbed(finalMessage, ev, peer.m_playerName, playerHostName, pos, isPlayerLeaving);
            }
            else
            {
                // For standard player events without position
                embedBuilder = MessageTransformer.CreatePlayerMessageEmbed(finalMessage, ev, peer.m_playerName, playerHostName, isPlayerLeaving);
            }
            
            // Send the embed using the enhanced API
            DiscordApi.SendEmbed(ev, embedBuilder);
        }
        else
        {
            // Legacy behavior for non-embed mode
            if (posEnabled)
            {
                // Send with position if enabled
                DiscordApi.SendMessage(ev, finalMessage, pos);
            }
            else
            {
                // Send without position
                DiscordApi.SendMessage(ev, finalMessage);
            }
        }
    }


    /// <summary>
    ///     Finish formatting the message based on the positional data (if allowed) and dispatch it to the Discord webhook.
    ///     This method handles the text from shouts and other chat-adjacent things.
    ///     Enhanced to support rich Discord embeds for chat messages.
    /// </summary>
    /// <param name="peer">Player peer reference</param>
    /// <param name="playerHostName">Player host name</param>
    /// <param name="preFormattedMessage">Raw message to format for sending to discord</param>
    /// <param name="posEnabled">If we are allowed to include the position data</param>
    /// <param name="pos">Positional data to use in formatting</param>
    /// <param name="text">Text that was sent</param>
    /// <param name="ev">Event type</param>
    private static void FinalizeFormattingAndSend(ZNetPeer peer, string playerHostName, string preFormattedMessage,
        bool posEnabled, Vector3 pos, string text, Webhook.Event ev)
    {
        if (preFormattedMessage.Contains("%SHOUT%"))
        {
            if (string.IsNullOrEmpty(text))
            {
                preFormattedMessage = preFormattedMessage.Replace("%SHOUT%", "");
            }
        }
        
        // Format the message accordingly, depending if it has the %POS% variable or not
        string finalMessage;
        if (preFormattedMessage.Contains("%POS%"))
        {
            if (!posEnabled)
            {
                preFormattedMessage = preFormattedMessage.Replace("%POS%", "");
            }

            finalMessage =
                MessageTransformer.FormatPlayerMessage(preFormattedMessage, peer.m_playerName, playerHostName, text,
                    pos);
        }
        else
        {
            finalMessage =
                MessageTransformer.FormatPlayerMessage(preFormattedMessage, peer.m_playerName, playerHostName, text);
        }

        // Determine whether to use embeds based on configuration
        if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled)
        {
            // Create the appropriate embed based on event type and available data
            EmbedBuilder embedBuilder;
            
            // Determine which type of embed to create based on event category
            if (Webhook.PlayerShoutEvents.Contains(ev))
            {
                // For shout events, use a specialized shout embed with the chat message
                embedBuilder = MessageTransformer.CreateShoutMessageEmbed(finalMessage, ev, peer.m_playerName, playerHostName, text);
                
                // Add position field if enabled
                if (posEnabled)
                {
                    embedBuilder.AddField("Position", MessageTransformer.FormatVector3AsPos(pos), true);
                }
            }
            else
            {
                // For other chat-adjacent events
                if (posEnabled)
                {
                    // Include position data in the embed
                    embedBuilder = MessageTransformer.CreatePlayerMessageEmbed(finalMessage, ev, peer.m_playerName, playerHostName, pos);
                }
                else
                {
                    // Standard player message embed without position
                    embedBuilder = MessageTransformer.CreatePlayerMessageEmbed(finalMessage, ev, peer.m_playerName, playerHostName);
                }
            }
            
            // Send the embed using the enhanced API
            DiscordApi.SendEmbed(ev, embedBuilder);
        }
        else
        {
            // Legacy behavior for non-embed mode
            if (posEnabled)
            {
                // Send message with position data
                DiscordApi.SendMessage(ev, finalMessage, pos);
            }
            else
            {
                // Send standard message without position
                DiscordApi.SendMessage(ev, finalMessage);
            }
        }
    }

    /// <summary>
    ///     Handle a non-player chat message. Currently only works for shouts.
    ///     ///
    ///     If allowed in the configuration, this will send a message to discord as if a player shouted.
    /// </summary>
    /// <param name="type">Type of chat message</param>
    /// <param name="user">Listed username of sender</param>
    /// <param name="text">Text sent to chat</param>
    internal static void NonPlayerChat(Talker.Type type, string user, string text)
    {
        // Check if we allow non-player shouts
        if (DiscordConnectorPlugin.StaticConfig.AllowNonPlayerShoutLogging)
        {
            // Guard against chats that aren't shouts by non-players
            if (type != Talker.Type.Shout)
            {
                DiscordConnectorPlugin.StaticLogger.LogDebug($"Ignored ping/join/leave from non-player {user}");
                return;
            }

            string nonPlayerHostName = "";
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Sending shout from '{user}' to discord: '{text}'");

            // Only if we are sending shouts per the config should we send the shout
            if (DiscordConnectorPlugin.StaticConfig.ChatShoutEnabled)
            {
                // Clean any "formatting" done to the username. This includes coloring via <color=x> tags.
                string userCleaned = MessageTransformer.CleanCaretFormatting(user);
                // Format the message into the shout format as defined in the config files
                string message = MessageTransformer.FormatPlayerMessage(
                    DiscordConnectorPlugin.StaticConfig.ShoutMessage, userCleaned, nonPlayerHostName, text);

                // Non-players shouldn't have a position, so disregard any position in the message formatting
                if (message.Contains("%POS%"))
                {
                    message.Replace("%POS%", "");
                }

                DiscordApi.SendMessage(Webhook.Event.PlayerShout, message);
            }

            // Exit the function since we sent the message
            return;
        }

        DiscordConnectorPlugin.StaticLogger.LogInfo($"Ignored shout from {user} because they aren't a real player");
    }
}
