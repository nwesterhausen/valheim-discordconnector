using System;
using System.Collections.Generic;
using UnityEngine;
using DiscordConnector.Config;

namespace DiscordConnector;

/// <summary>
///     Provides pre-configured templates for common Discord embed message types.
///     Each template is optimized for a specific event type with appropriate styling and fields.
/// </summary>
internal static class EmbedTemplates
{
    // We no longer need constants - will use config values from MainConfig
    /// <summary>
    ///     Creates a server lifecycle event embed (launch, start, stop, shutdown).
    /// </summary>
    /// <param name="eventType">The specific server event type</param>
    /// <param name="message">The main message content</param>
    /// <param name="worldName">The name of the server world</param>
    /// <param name="serverName">The name of the server</param>
    /// <returns>A configured EmbedBuilder instance</returns>
    public static EmbedBuilder ServerLifecycle(Webhook.Event eventType, string message, string worldName, string serverName)
    {
        var variables = new Dictionary<string, string>
        {
            {"worldName", worldName},
            {"serverName", serverName},
            {"timestamp", DateTime.UtcNow.ToString("s")}
        };
        
        var builder = new EmbedBuilder()
            .SetColorForEvent(eventType)
            .SetAuthor(serverName, null, DiscordConnectorPlugin.StaticConfig.EmbedAuthorIconUrl) // Default Valheim icon
            .SetTimestamp();
            
        // Set the default thumbnail for server events
        if (DiscordConnectorPlugin.StaticConfig.EmbedThumbnailEnabled)
        {
            builder.SetThumbnail(DiscordConnectorPlugin.StaticConfig.EmbedThumbnailUrl);
        }
            
        // Set different titles based on the event type
        if (Webhook.ServerLaunchEvents.Contains(eventType))
        {
            builder.SetTitle("🚀 Server Launching");
        }
        else if (Webhook.ServerStartEvents.Contains(eventType))
        {
            builder.SetTitle("✅ Server Started");
        }
        else if (Webhook.ServerStopEvents.Contains(eventType))
        {
            builder.SetTitle("🛑 Server Stopping");
        }
        else if (Webhook.ServerShutdownEvents.Contains(eventType))
        {
            builder.SetTitle("⛔ Server Shutdown");
        }
        else if (Webhook.ServerSaveEvents.Contains(eventType))
        {
            builder.SetTitle("💾 World Saved");
        }
        
        // Add the message as description
        builder.SetDescription(message);
        
        // Add world information as fields
        builder.AddInlineField("World", worldName);
        builder.AddInlineField("Status", GetStatusForEvent(eventType));
        
        // Set footer with world info
        builder.SetFooterFromTemplate(variables);
        
        // Set URL if configured
        builder.SetUrlFromTemplate(variables);
        
        return builder;
    }
    
    /// <summary>
    ///     Creates a player event embed (join, leave, death).
    /// </summary>
    /// <param name="eventType">The specific player event type</param>
    /// <param name="message">The main message content</param>
    /// <param name="playerName">The name of the player</param>
    /// <param name="position">The player's position (optional)</param>
    /// <param name="worldName">The name of the server world</param>
    /// <returns>A configured EmbedBuilder instance</returns>
    public static EmbedBuilder PlayerEvent(Webhook.Event eventType, string message, string playerName, 
                                          Vector3? position = null, string worldName = "", string playerHostName = "")
    {
        var variables = new Dictionary<string, string>
        {
            {"worldName", worldName},
            {"playerName", playerName},
            {"timestamp", DateTime.UtcNow.ToString("s")}
        };
        
        // Always use server name (Valheim) as the author name for consistency
        string serverName = DiscordConnectorPlugin.StaticConfig.ServerName;
        
        var builder = new EmbedBuilder()
            .SetColorForEvent(eventType)
            .SetAuthor(serverName, null, DiscordConnectorPlugin.StaticConfig.EmbedAuthorIconUrl) // Valheim icon
            .SetTimestamp();
            
        // Set different titles based on the event type
        if (Webhook.PlayerJoinEvents.Contains(eventType))
        {
            builder.SetTitle("👋 Player Joined");
            // Use the thumbnail from config
            if (DiscordConnectorPlugin.StaticConfig.EmbedThumbnailEnabled)
            {
                builder.SetThumbnail(DiscordConnectorPlugin.StaticConfig.EmbedThumbnailUrl);
            }
        }
        else if (Webhook.PlayerLeaveEvents.Contains(eventType))
        {
            builder.SetTitle("🚶 Player Left");
            // Use the thumbnail from config
            if (DiscordConnectorPlugin.StaticConfig.EmbedThumbnailEnabled)
            {
                builder.SetThumbnail(DiscordConnectorPlugin.StaticConfig.EmbedThumbnailUrl);
            }
        }
        else if (Webhook.PlayerDeathEvents.Contains(eventType))
        {
            builder.SetTitle("💀 Player Death");
            // Use the thumbnail from config
            if (DiscordConnectorPlugin.StaticConfig.EmbedThumbnailEnabled)
            {
                builder.SetThumbnail(DiscordConnectorPlugin.StaticConfig.EmbedThumbnailUrl);
            }
        }
        
        // Add the message as description
        builder.SetDescription(message);
        
        // Add player name as a field
        builder.AddInlineField("Player", playerName);
        
        // Add player ID field if enabled in config and a player ID is provided
        if (DiscordConnectorPlugin.StaticConfig.ShowPlayerIds && !string.IsNullOrEmpty(playerHostName))
        {
            builder.AddInlineField("Player ID", playerHostName);
        }
        
        // Add position field if available
        if (position.HasValue)
        {
            builder.AddPositionField(position.Value);
        }
        
        // Set footer with world info
        builder.SetFooterFromTemplate(variables);
        
        // Set URL if configured
        builder.SetUrlFromTemplate(variables);
        
        return builder;
    }
    
    /// <summary>
    ///     Creates a world event embed (random events, day changes).
    /// </summary>
    /// <param name="eventType">The specific world event type</param>
    /// <param name="message">The main message content</param>
    /// <param name="eventName">The name of the world event</param>
    /// <param name="worldName">The name of the server world</param>
    /// <returns>A configured EmbedBuilder instance</returns>
    public static EmbedBuilder WorldEvent(Webhook.Event eventType, string message, string eventName, string worldName)
    {
        var variables = new Dictionary<string, string>
        {
            {"worldName", worldName},
            {"eventName", eventName},
            {"timestamp", DateTime.UtcNow.ToString("s")}
        };
        
        var serverName = DiscordConnectorPlugin.StaticConfig.ServerName;
        var builder = new EmbedBuilder()
            .SetColorForEvent(eventType)
            .SetAuthor(serverName, null, DiscordConnectorPlugin.StaticConfig.EmbedAuthorIconUrl) // Always use Valheim as author
            .SetTimestamp();
            
        // Set different titles based on the event type
        if (Webhook.Event.EventStart == eventType)
        {
            builder.SetTitle($"🌩️ Event Started: {eventName}");
            if (DiscordConnectorPlugin.StaticConfig.EmbedThumbnailEnabled)
            {
                builder.SetThumbnail(DiscordConnectorPlugin.StaticConfig.EmbedThumbnailUrl);
            }
        }
        else if (Webhook.Event.EventStop == eventType)
        {
            builder.SetTitle($"☀️ Event Ended: {eventName}");
            if (DiscordConnectorPlugin.StaticConfig.EmbedThumbnailEnabled)
            {
                builder.SetThumbnail(DiscordConnectorPlugin.StaticConfig.EmbedThumbnailUrl);
            }
        }
        else if (Webhook.Event.NewDayNumber == eventType)
        {
            builder.SetTitle($"🌅 New Day: {eventName}");
            // Use a bright, vibrant color for day number display
            builder.SetColor("#FFD700"); // Gold color for better visibility
            // Add thumbnail for day number events
            if (DiscordConnectorPlugin.StaticConfig.EmbedThumbnailEnabled)
            {
                builder.SetThumbnail(DiscordConnectorPlugin.StaticConfig.EmbedThumbnailUrl);
            }
        }
        else if (Webhook.Event.ServerSave == eventType)
        {
            builder.SetTitle($"💾 World Saved");
            // Use a vibrant blue for world save events
            builder.SetColor("#1D8BF1");
        }
        
        // Add the message as description
        builder.SetDescription(message);
        
        // Set footer with world info
        builder.SetFooterFromTemplate(variables);
        
        // Set URL if configured
        builder.SetUrlFromTemplate(variables);
        
        return builder;
    }
    
    /// <summary>
    ///     Creates a chat/shout message embed.
    /// </summary>
    /// <param name="eventType">The specific chat event type</param>
    /// <param name="message">The chat message content</param>
    /// <param name="playerName">The name of the player who sent the message</param>
    /// <param name="position">The player's position (optional)</param>
    /// <param name="worldName">The name of the server world</param>
    /// <returns>A configured EmbedBuilder instance</returns>
    public static EmbedBuilder ChatMessage(Webhook.Event eventType, string message, string playerName, 
                                          Vector3? position = null, string worldName = "")
    {
        var variables = new Dictionary<string, string>
        {
            {"worldName", worldName},
            {"playerName", playerName},
            {"timestamp", DateTime.UtcNow.ToString("s")}
        };
        
        // Use the server name from config for consistency across all embeds
        string serverName = DiscordConnectorPlugin.StaticConfig.ServerName;
        
        var builder = new EmbedBuilder()
            .SetColorForEvent(eventType)
            .SetAuthor(serverName, null, DiscordConnectorPlugin.StaticConfig.EmbedAuthorIconUrl) // Valheim icon
            .SetTimestamp();
            
        // Add player name as a field
        builder.AddInlineField("Player", playerName);
            
        // Set title based on message type
        if (Webhook.PlayerShoutEvents.Contains(eventType))
        {
            builder.SetTitle("📣 Shout Message");
            if (DiscordConnectorPlugin.StaticConfig.EmbedThumbnailEnabled)
            {
                builder.SetThumbnail(DiscordConnectorPlugin.StaticConfig.EmbedThumbnailUrl);
            }
        }
        else
        {
            builder.SetTitle("💬 Chat Message");
            if (DiscordConnectorPlugin.StaticConfig.EmbedThumbnailEnabled)
            {
                builder.SetThumbnail(DiscordConnectorPlugin.StaticConfig.EmbedThumbnailUrl);
            }
        }
        
        // Add the message as description with quotes
        builder.SetDescription($"> {message}");
        
        // Add position field if available
        if (position.HasValue)
        {
            builder.AddPositionField(position.Value);
        }
        
        // Set footer with world info
        builder.SetFooterFromTemplate(variables);
        
        // Set URL if configured
        builder.SetUrlFromTemplate(variables);
        
        return builder;
    }
    
    /// <summary>
    ///     Creates a position message embed.
    /// </summary>
    /// <param name="eventType">The specific event type</param>
    /// <param name="message">The main message content</param>
    /// <param name="playerName">The name of the player</param>
    /// <param name="position">The player's position</param>
    /// <param name="worldName">The name of the server world</param>
    /// <returns>A configured EmbedBuilder instance</returns>
    public static EmbedBuilder PositionMessage(Webhook.Event eventType, string message, string playerName, Vector3 position, string worldName = "")
    {
        var variables = new Dictionary<string, string>
        {
            {"worldName", worldName},
            {"playerName", playerName},
            {"timestamp", DateTime.UtcNow.ToString("s")}
        };
        
        // Use server name for consistency across all embeds
        string serverName = DiscordConnectorPlugin.StaticConfig.ServerName;
        
        // Determine the title based on the event type
        string title;
        if (Webhook.Event.EventStop == eventType)
        {
            title = "Event Stopped";
        }
        else if (Webhook.Event.EventStart == eventType)
        {
            title = "Event Started";
        }
        else
        {
            // Default title if it's another event type
            title = "Event Update";
        }
        
        var builder = new EmbedBuilder()
            .SetColor("#3498DB") // Use a bright blue color for position messages
            .SetAuthor(serverName, null, DiscordConnectorPlugin.StaticConfig.EmbedAuthorIconUrl) // Always use server name (Valheim) as author
            .SetTitle(title) // Use a cleaner title format without duplication
            .SetThumbnail(DiscordConnectorPlugin.StaticConfig.EmbedThumbnailEnabled ? 
                        DiscordConnectorPlugin.StaticConfig.EmbedThumbnailUrl : null)
            .SetDescription(message)
            .SetTimestamp();
            
        // Always add the position for position messages
        builder.AddField("Coordinates", MessageTransformer.FormatVector3AsPos(position));
        
        // Set footer with world info
        builder.SetFooterFromTemplate(variables);
        
        // Set URL if configured
        builder.SetUrlFromTemplate(variables);
        
        return builder;
    }
    
    /// <summary>
    ///     Creates a leaderboard message embed.
    /// </summary>
    /// <param name="eventType">The specific leaderboard event type</param>
    /// <param name="title">The leaderboard title</param>
    /// <param name="description">The leaderboard description</param>
    /// <param name="entries">The leaderboard entries as name/value tuples</param>
    /// <param name="worldName">The name of the server world</param>
    /// <returns>A configured EmbedBuilder instance</returns>
    public static EmbedBuilder LeaderboardMessage(Webhook.Event eventType, string title, string description, 
                                                 List<Tuple<string, string>> entries, string worldName = "")
    {
        var variables = new Dictionary<string, string>
        {
            {"worldName", worldName},
            {"timestamp", DateTime.UtcNow.ToString("s")}
        };
        
        var builder = new EmbedBuilder()
            .SetColor("#9B59B6") // Use a vibrant purple color for leaderboards
            .SetAuthor("Leaderboard", null, DiscordConnectorPlugin.StaticConfig.EmbedAuthorIconUrl) // Trophy icon
            .SetTitle(title)
            .SetDescription(description)
            .SetThumbnail(DiscordConnectorPlugin.StaticConfig.EmbedThumbnailEnabled ? 
                        DiscordConnectorPlugin.StaticConfig.EmbedThumbnailUrl : null)
            .SetTimestamp();
            
        // Add all entries as inline fields
        builder.AddInlineFields(entries);
        
        // Set footer with world info
        builder.SetFooterFromTemplate(variables);
        
        // Set URL if configured
        builder.SetUrlFromTemplate(variables);
        
        return builder;
    }
    
    /// <summary>
    ///     Gets the status text for a server event.
    /// </summary>
    /// <param name="eventType">The server event type</param>
    /// <returns>A status text appropriate for the event</returns>
    private static string GetStatusForEvent(Webhook.Event eventType)
    {
        if (Webhook.ServerLaunchEvents.Contains(eventType))
        {
            return "Launching";
        }
        else if (Webhook.ServerStartEvents.Contains(eventType))
        {
            return "Online";
        }
        else if (Webhook.ServerStopEvents.Contains(eventType))
        {
            return "Stopping";
        }
        else if (Webhook.ServerShutdownEvents.Contains(eventType))
        {
            return "Offline";
        }
        else if (eventType == Webhook.Event.ServerSave)
        {
            return "Online";
        }
        else if (eventType == Webhook.Event.NewDayNumber)
        {
            return "Online";
        }
        
        return "Unknown";
    }
}
