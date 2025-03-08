using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DiscordConnector.Config;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace DiscordConnector;

internal class DiscordApi
{
    /// <summary>
    ///     Send a <paramref name="message" /> and a <paramref name="pos" /> to Discord.
    ///     Uses embeds if enabled in configuration.
    /// </summary>
    /// <param name="ev">The event which triggered this message</param>
    /// <param name="message">A string optionally formatted with Discord-approved markdown syntax.</param>
    /// <param name="pos">A 3-dimensional vector representing a position</param>
    public static void SendMessage(Webhook.Event ev, string message, Vector3 pos)
    {
        try
        {
            if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled)
            {
                try
                {
                    // Create a position embed using the enhanced MessageTransformer
                    string serverName = DiscordConnectorPlugin.StaticConfig.ServerName; // Use server name (Valheim) for consistency
                    EmbedBuilder embedBuilder = MessageTransformer.CreatePositionEmbed(message, 
                                                                   serverName, // Use server name instead of generic "Player" for all position embeds
                                                                   pos,
                                                                   ev); // Pass the event type directly to control the title
                    
                    // Build the embed and send it
                    SendEmbed(ev, embedBuilder);
                }
                catch (Exception ex)
                {
                    // Log embed creation error and fall back to plain text
                    DiscordConnectorPlugin.StaticLogger.LogError($"Failed to create position embed: {ex.Message}");
                    DiscordConnectorPlugin.StaticLogger.LogDebug(ex.StackTrace);
                    
                    // Fall back to plain text message
                    SendMessage(ev, $"{message} {MessageTransformer.FormatAppendedPos(pos)}");
                }
            }
            else
            {
                SendMessage(ev, $"{message} {MessageTransformer.FormatAppendedPos(pos)}");
            }
        }
        catch (Exception ex)
        {
            // Catch any unexpected errors to prevent crashes
            DiscordConnectorPlugin.StaticLogger.LogError($"Error sending message with position: {ex.Message}");
            DiscordConnectorPlugin.StaticLogger.LogDebug(ex.StackTrace);
        }
    }

    /// <summary>
    ///     Sends a <paramref name="message" /> to Discord.
    ///     Uses embeds if enabled in configuration.
    /// </summary>
    /// <param name="ev">The event which triggered this message</param>
    /// <param name="message">A string optionally formatted with Discord-approved markdown syntax.</param>
    public static void SendMessage(Webhook.Event ev, string message)
    {
        try
        {
            // Validate input to avoid null messages
            if (string.IsNullOrEmpty(message))
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning("Attempted to send empty message");
                message = "[Empty Message]"; // Use placeholder for empty messages
            }

            if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled)
            {
                try
                {
                    // Create a server message embed using the enhanced MessageTransformer
                    EmbedBuilder embedBuilder = MessageTransformer.CreateServerMessageEmbed(message, ev);
                    
                    // Send the embed
                    SendEmbed(ev, embedBuilder);
                }
                catch (Exception ex)
                {
                    // Log embed creation error and fall back to plain text
                    DiscordConnectorPlugin.StaticLogger.LogError($"Failed to create server message embed: {ex.Message}");
                    DiscordConnectorPlugin.StaticLogger.LogDebug(ex.StackTrace);
                    
                    // Fall back to plain text message
                    DiscordExecuteWebhook payload = new() { content = message };
                    payload.SendFor(ev);
                }
            }
            else
            {
                // Classic plain-text message
                DiscordExecuteWebhook payload = new() { content = message };
                payload.SendFor(ev);
            }
        }
        catch (Exception ex)
        {
            // Catch any unexpected errors to prevent crashes
            DiscordConnectorPlugin.StaticLogger.LogError($"Error sending message: {ex.Message}");
            DiscordConnectorPlugin.StaticLogger.LogDebug(ex.StackTrace);
        }
    }

    /// <summary>
    ///     Send a message with fields to Discord.
    ///     Uses enhanced embed formatting if embeds are enabled in configuration.
    /// </summary>
    /// <param name="ev">The event which triggered this message</param>
    /// <param name="content">A string optionally formatted with Discord-approved Markdown syntax.</param>
    /// <param name="fields">Discord fields as defined in the API, as Tuples (field name, value)</param>
    public static void SendMessageWithFields(Webhook.Event ev, string? content = null,
        List<Tuple<string, string>>? fields = null)
    {
        try
        {
            // Guard against null/empty calls
            if (string.IsNullOrEmpty(content) && (fields == null || fields.Count == 0))
            {
                content = "Uh-oh! An unexpectedly empty message was sent!";
                DiscordConnectorPlugin.StaticLogger.LogWarning("Attempted to send message with neither content nor fields");
            }

            // Validate fields to remove any null or empty entries
            if (fields != null)
            {
                fields = fields
                    .Where(f => f != null && !string.IsNullOrEmpty(f.Item1) && !string.IsNullOrEmpty(f.Item2))
                    .ToList();
                
                if (fields.Count == 0)
                {
                    fields = null;
                }
            }

            if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled)
            {
                try
                {
                    // Create an embed builder
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                        .SetColorForEvent(ev)
                        .SetDescription(content)
                        .SetTimestamp();
                    
                    // Add all fields as inline where appropriate
                    if (fields != null && fields.Count > 0)
                    {
                        try
                        {
                            if (fields.Count <= 3)
                            {
                                // Format fields as inline for better appearance when we have few fields
                                embedBuilder.AddInlineFields(fields);
                            }
                            else
                            {
                                // For many fields, use a mix of inline and non-inline for better readability
                                embedBuilder.AddFields(fields);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log field processing error, but continue with basic embed without fields
                            DiscordConnectorPlugin.StaticLogger.LogError($"Error processing fields for embed: {ex.Message}");
                            DiscordConnectorPlugin.StaticLogger.LogDebug(ex.StackTrace);
                        }
                    }
                    
                    // Send the enhanced embed
                    SendEmbed(ev, embedBuilder);
                }
                catch (Exception ex)
                {
                    // Log embed creation error and fall back to plain text
                    DiscordConnectorPlugin.StaticLogger.LogError($"Failed to create embed with fields: {ex.Message}");
                    DiscordConnectorPlugin.StaticLogger.LogDebug(ex.StackTrace);
                    
                    // Fall back to legacy format
                    FallbackToLegacyFormat(ev, content, fields);
                }
            }
            else
            {
                FallbackToLegacyFormat(ev, content, fields);
            }
        }
        catch (Exception ex)
        {
            // Catch any unexpected errors to prevent crashes
            DiscordConnectorPlugin.StaticLogger.LogError($"Unexpected error in SendMessageWithFields: {ex.Message}");
            DiscordConnectorPlugin.StaticLogger.LogDebug(ex.StackTrace);
            
            try
            {
                // Ultimate fallback - just try to send a simple error message
                DiscordExecuteWebhook payload = new() { content = "Error processing message with fields. See logs for details." };
                payload.SendFor(ev);
            }
            catch
            {
                // Nothing more we can do if even the fallback fails
                DiscordConnectorPlugin.StaticLogger.LogError("Failed to send fallback error message");
            }
        }
    }
    
    /// <summary>
    ///     Helper method to send a message using the legacy embed format
    /// </summary>
    /// <param name="ev">The event which triggered this message</param>
    /// <param name="content">A string optionally formatted with Discord-approved Markdown syntax</param>
    /// <param name="fields">Discord fields as defined in the API, as Tuples (field name, value)</param>
    private static void FallbackToLegacyFormat(Webhook.Event ev, string? content, List<Tuple<string, string>>? fields)
    {
        try
        {
            // Legacy embed format
            DiscordExecuteWebhook payload = new() { content = content };

            // If we have fields at all, put them as embedded fields
            if (fields != null && fields.Count > 0)
            {
                payload.embeds = [];
                List<DiscordField> discordFields = [];
                
                foreach (Tuple<string, string> t in fields)
                {
                    if (t != null && !string.IsNullOrEmpty(t.Item1) && !string.IsNullOrEmpty(t.Item2))
                    {
                        discordFields.Add(new DiscordField
                        {
                            name = Config.EmbedConfigValidator.ValidateFieldName(t.Item1),
                            value = Config.EmbedConfigValidator.ValidateFieldValue(t.Item2)
                        });
                    }
                }

                // Add the fields to the payload if we have any valid ones
                if (discordFields.Count > 0)
                {
                    payload.embeds.Add(new DiscordEmbed { fields = discordFields });
                }
            }

            payload.SendFor(ev);
        }
        catch (Exception ex)
        {
            DiscordConnectorPlugin.StaticLogger.LogError($"Error in fallback message format: {ex.Message}");
            DiscordConnectorPlugin.StaticLogger.LogDebug(ex.StackTrace);
            
            // Last resort fallback with just the content
            try
            {
                DiscordExecuteWebhook simplePayload = new() { content = content ?? "Error processing message" };
                simplePayload.SendFor(ev);
            }
            catch
            {
                // Nothing more we can do at this point
                DiscordConnectorPlugin.StaticLogger.LogError("Failed to send simple fallback message");
            }
        }
    }

    /// <summary>
    ///     Send an embed message to Discord using the provided EmbedBuilder.
    /// </summary>
    /// <param name="ev">The event which triggered this message</param>
    /// <param name="embedBuilder">EmbedBuilder instance to build and send</param>
    public static void SendEmbed(Webhook.Event ev, EmbedBuilder embedBuilder)
    {
        if (embedBuilder == null)
        {
            LogDiscordError("Null EmbedBuilder provided to SendEmbed", eventType: ev);
            // Send a fallback message
            DiscordExecuteWebhook fallbackPayload = new() { content = "Error: Cannot create embed with null builder." };
            fallbackPayload.SendFor(ev);
            return;
        }

        try
        {
            // Attempt to build the embed and handle any validation errors that might occur
            DiscordEmbed embed;
            try
            {
                embed = embedBuilder.Build();
                
                if (embed == null)
                {
                    throw new InvalidOperationException("Built embed was null");
                }
            }
            catch (Exception ex)
            {
                LogDiscordError("Error building embed", ex, ev);
                
                // Try to extract just the description for a fallback message
                string fallbackContent = "Error creating embed message. Please check logs.";
                try
                {
                    var description = embedBuilder.GetDescriptionForFallback();
                    if (!string.IsNullOrEmpty(description))
                    {
                        fallbackContent = description!;
                    }
                }
                catch (Exception fallbackEx)
                {
                    // Log and ignore any errors in fallback extraction
                    LogDiscordError("Failed to extract fallback description", fallbackEx, ev, false);
                }
                
                // Send a fallback plain text message
                DiscordExecuteWebhook descriptionFallback = new() { content = fallbackContent };
                descriptionFallback.SendFor(ev);
                return;
            }
            
            // If we have a valid embed, validate it and send it
            try
            {
                // Validate the embed with our comprehensive validator and auto-fix issues if possible
                bool isValid = Config.EmbedConfigValidator.ValidateEmbed(embed, true, true);
                
                if (!isValid)
                {
                    DiscordConnectorPlugin.StaticLogger.LogWarning("Embed validation failed and could not be automatically fixed");
                    
                    // Try to extract just the description for a fallback message
                    string fallbackContent = embedBuilder.GetDescriptionForFallback() ?? string.Empty;
                    if (string.IsNullOrEmpty(fallbackContent))
                    {
                        fallbackContent = "Message could not be sent to Discord due to validation issues. Check logs for details.";
                    }
                    
                    // Send a fallback plain text message
                    DiscordConnectorPlugin.StaticLogger.LogInfo("Sending plain text fallback message instead of invalid embed");
                    DiscordExecuteWebhook descriptionFallback = new() { content = fallbackContent };
                    descriptionFallback.SendFor(ev);
                    return;
                }
                
                // At this point, embed has been validated and fixed if needed
                
                // Create payload with the validated embed
                DiscordExecuteWebhook payload = new() { embeds = [embed] };
                
                // Send the payload
                payload.SendFor(ev);
            }
            catch (Exception ex)
            {
                LogDiscordError("Error sending embed payload", ex, ev);
                throw new Exception($"Error sending embed payload: {ex.Message}", ex);
            }
        }
        catch (Exception ex)
        {
            // Log error and fall back to plain text if anything fails
            LogDiscordError("Failed to send embed", ex, ev);
            
            try
            {
                // Send a fallback plain text message directly to avoid potential recursion
                DiscordExecuteWebhook fallbackPayload = new() { content = "Error sending Discord message. Please check logs." };
                fallbackPayload.SendFor(ev);
            }
            catch (Exception fallbackEx)
            {
                // Last resort logging if we can't even send the fallback
                LogDiscordError("Complete failure sending Discord message - even fallback failed", fallbackEx, ev, false);
            }
        }
    }
    
    /// <summary>
    ///     Helper method to log errors consistently across Discord API operations.
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="ex">The exception that occurred, if any</param>
    /// <param name="eventType">The event type that was being processed, if any</param>
    /// <param name="logStackTrace">Whether to log the stack trace (default: true)</param>
    private static void LogDiscordError(string message, Exception? ex = null, Webhook.Event? eventType = null, bool logStackTrace = true)
    {
        try
        {
            var eventInfo = eventType.HasValue ? $" for event {eventType}" : string.Empty;
            
            if (ex != null)
            {
                DiscordConnectorPlugin.StaticLogger.LogError($"{message}{eventInfo}: {ex.Message}");
                
                if (logStackTrace && ex.StackTrace != null)
                {
                    DiscordConnectorPlugin.StaticLogger.LogDebug(ex.StackTrace);
                }
                
                // Log inner exception if present
                if (ex.InnerException != null)
                {
                    DiscordConnectorPlugin.StaticLogger.LogError($"Inner exception: {ex.InnerException.Message}");
                }
            }
            else
            {
                DiscordConnectorPlugin.StaticLogger.LogError($"{message}{eventInfo}");
            }
        }
        catch
        {
            // Last resort fallback if even logging fails
            try
            {
                DiscordConnectorPlugin.StaticLogger.LogError("Failed to log Discord error details due to exception in logging");
            }
            catch
            {
                // Nothing more we can do
            }
        }
    }
    
    /// <summary>
    ///     Sends <paramref name="serializedJson" /> to the webhook specified in configuration.
    /// </summary>
    /// <param name="ev">The event which triggered this message</param>
    /// <param name="serializedJson">Body data for the webhook as JSON serialized into a string</param>
    public static void SendSerializedJson(Webhook.Event ev, string serializedJson)
    {
        DiscordConnectorPlugin.StaticLogger.LogDebug($"Finding webhooks for event: (event: {ev})");

        if (ev == Webhook.Event.Other)
        {
            DiscordConnectorPlugin.StaticLogger.LogInfo(
                "Dispatching webhook for 3rd party plugin (configured as 'Other' in WebHook config)");
        }

        // Guard against unset webhook or empty serialized json
        if ((string.IsNullOrEmpty(DiscordConnectorPlugin.StaticConfig.PrimaryWebhook.Url) &&
             string.IsNullOrEmpty(DiscordConnectorPlugin.StaticConfig.SecondaryWebhook.Url)) ||
            string.IsNullOrEmpty(serializedJson))
        {
            return;
        }

        // Responsible for sending a JSON string to the webhook.
        byte[] byteArray = Encoding.UTF8.GetBytes(serializedJson);

        if (DiscordConnectorPlugin.StaticConfig.PrimaryWebhook.HasEvent(ev))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Sending {ev} message to Primary Webhook");
            DispatchRequest(DiscordConnectorPlugin.StaticConfig.PrimaryWebhook, byteArray);
        }

        if (DiscordConnectorPlugin.StaticConfig.SecondaryWebhook.HasEvent(ev))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Sending {ev} message to Secondary Webhook");
            DispatchRequest(DiscordConnectorPlugin.StaticConfig.SecondaryWebhook, byteArray);
        }

        // Check for any extra webhooks that should be sent to
        foreach (WebhookEntry webhook in DiscordConnectorPlugin.StaticConfig.ExtraWebhooks)
        {
            if (webhook.HasEvent(ev))
            {
                DiscordConnectorPlugin.StaticLogger.LogDebug($"Sending {ev} message to Extra Webhook: {webhook.Url}");
                DispatchRequest(webhook, byteArray);
            }
        }
    }

    /// <summary>
    ///     Sends serialized JSON to the <paramref name="webhook" />.
    /// </summary>
    /// <param name="webhook">Webhook definition to receive the JSON</param>
    /// <param name="serializedJson">Serialized JSON of the request to make</param>
    public static void SendSerializedJson(WebhookEntry webhook, string serializedJson)
    {
        DiscordConnectorPlugin.StaticLogger.LogDebug($"Trying webhook with payload: {serializedJson}");

        // Guard against unset webhook or empty serialized json
        if (string.IsNullOrEmpty(webhook.Url) || string.IsNullOrEmpty(serializedJson))
        {
            return;
        }

        // Responsible for sending a JSON string to the webhook.
        byte[] byteArray = Encoding.UTF8.GetBytes(serializedJson);

        DispatchRequest(webhook, byteArray);
    }

    /// <summary>
    ///     Send a web request to discord.
    /// </summary>
    /// <param name="webhook">The webhook to use for the request</param>
    /// <param name="byteArray">The payload as a byte array</param>
    private static void DispatchRequest(WebhookEntry webhook, byte[] byteArray)
    {
        if (string.IsNullOrEmpty(webhook.Url))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug("Dispatch attempted with empty webhook - ignoring");
            return;
        }

        // Create an identifier for the request
        string requestId = GuidHelper.GenerateShortHexGuid();
        DiscordConnectorPlugin.StaticLogger.LogDebug(
            $"DispatchRequest.{requestId}: sending {byteArray.Length} bytes to Discord");

        // Create a web request to send the payload to discord
        WebRequest request = WebRequest.Create(webhook.Url);
        request.Method = "POST";
        request.ContentType = "application/json";
        request.ContentLength = byteArray.Length;

        // Dispatch the request to discord and the response processing to an async task
        Task.Run(() =>
        {
            try
            {
                // We have to write the data to the request
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                // Wait for a response to the web request
                bool responseExpected = true;
                WebResponse response;
                try
                {
                    response = request.GetResponse();
                    if (DiscordConnectorPlugin.StaticConfig.DebugHttpRequestResponse)
                    {
                        if (response is HttpWebResponse webResponse)
                        {
                            if (webResponse.StatusCode == HttpStatusCode.NoContent)
                            {
                                responseExpected = false;
                            }

                            DiscordConnectorPlugin.StaticLogger.LogDebug(
                                $"DispatchRequest.{requestId}: Response Code: {webResponse.StatusCode}");
                        }
                        else
                        {
                            DiscordConnectorPlugin.StaticLogger.LogDebug(
                                $"DispatchRequest.{requestId}: Response was not an HttpWebResponse");
                        }
                    }
                }
                catch (WebException ex)
                {
                    DiscordConnectorPlugin.StaticLogger.LogError(
                        $"DispatchRequest.{requestId}: Error getting web response: {ex}");
                    return;
                }
                    if (responseExpected)
                    {
                    // Get the stream containing content returned by the server.
                    using (Stream? dataStream = response.GetResponseStream())
                    {
                        if (dataStream == null)
                        {
                            DiscordConnectorPlugin.StaticLogger.LogError(
                                $"DispatchRequest.{requestId}: Response stream is null");
                            return;
                        }

                        // Open the stream using a StreamReader for easy access.
                        using (StreamReader reader = new(dataStream))
                        {
                            // Read the content.
                            string responseFromServer = reader.ReadToEnd();
                            // Display the content.
                            if (DiscordConnectorPlugin.StaticConfig.DebugHttpRequestResponse)
                            {
                                if (responseFromServer.Length > 0)
                                {
                                    DiscordConnectorPlugin.StaticLogger.LogDebug(
                                        $"DispatchRequest.{requestId}: Response from server: {responseFromServer}");
                                }
                                else
                                {
                                    DiscordConnectorPlugin.StaticLogger.LogDebug(
                                        $"DispatchRequest.{requestId}: Empty response from server (normal)");
                                }
                            }
                        }
                    }
                }

                // Close the response.
                response.Close();
            }
            catch (Exception e)
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning($"Error dispatching webhook: {e}");
            }
        }).ConfigureAwait(false);
    }

    /// <summary>
    ///     Send a <paramref name="message" /> and a <paramref name="pos" /> to Discord.
    /// </summary>
    /// <param name="message">A string optionally formatted with Discord-approved markdown syntax.</param>
    /// <param name="pos">A 3-dimensional vector representing a position</param>
    public static void SendMessage(string message, Vector3 pos)
    {
        SendMessage(Webhook.Event.Other, message, pos);
    }

    /// <summary>
    ///     Sends a <paramref name="message" /> to Discord.
    /// </summary>
    /// <param name="message">A string optionally formatted with Discord-approved markdown syntax.</param>
    public static void SendMessage(string message)
    {
        SendMessage(Webhook.Event.Other, message);
    }

    /// <summary>
    ///     Send a <paramref name="content" /> with <paramref name="fields" /> to Discord.
    /// </summary>
    /// <param name="content">A string optionally formatted with Discord-approved markdown syntax.</param>
    /// <param name="fields">Discord fields as defined in the API, as Tuples (field name, value)</param>
    public static void SendMessageWithFields(string? content = null, List<Tuple<string, string>>? fields = null)
    {
        SendMessageWithFields(Webhook.Event.Other, content, fields);
    }

    /// <summary>
    ///     Sends <paramref name="serializedJson" /> to the webhook specified in configuration.
    /// </summary>
    /// <param name="serializedJson">Body data for the webhook as JSON serialized into a string</param>
    private static void SendSerializedJson(string serializedJson)
    {
        SendSerializedJson(Webhook.Event.Other, serializedJson);
    }
}

internal class DiscordExecuteWebhook
{
    /// <summary>
    ///     The message contents (up to 2000 characters). Required if `embeds` is not provided.
    /// </summary>
    public string? content { get; set; }

    /// <summary>
    ///     Override the default username of the webhook.
    /// </summary>
    public string? username { get; set; }

    /// <summary>
    ///     Override the default avatar of the webhook.
    /// </summary>
    public string? avatar_url { get; set; }

    /// <summary>
    ///     An array of up to 10 embed objects. Required if `content` is not provided.
    /// </summary>
    public List<DiscordEmbed>? embeds { get; set; }

    /// <summary>
    ///     Allowed mentions for the message.
    /// </summary>
    public AllowedMentions? allowed_mentions { get; set; }

    /// <summary>
    ///     Create an empty DiscordExecuteWebhook object.
    /// </summary>
    public DiscordExecuteWebhook()
    {
        // allowed mentions are set for all webhooks right now
        allowed_mentions = new AllowedMentions();

        ResetOverrides();
    }

    /// <summary>
    ///     Set the username for the webhook.
    /// </summary>
    /// <param name="name">The username to set for the webhook</param>
    public void SetUsername(string name)
    {
        this.username = name;
    }

    /// <summary>
    ///     Set the avatar URL for the webhook.
    /// </summary>
    /// <param name="url">The avatar URL to set for the webhook</param>
    public void SetAvatarUrl(string url)
    {
        this.avatar_url = url;
    }

    /// <summary>
    ///     Validate that this object is ready to be serialized into JSON.
    ///     This is a simple check to ensure that the object is not empty, and that it has at least one of the required fields.
    /// </summary>
    /// <returns>True if the object is valid, false otherwise</returns>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(content) || embeds != null;
    }

    /// <summary>
    ///     Reset any overrides on the webhook.
    /// </summary>
    public void ResetOverrides()
    {
        username = null;
        avatar_url = null;

        if (!string.IsNullOrEmpty(DiscordConnectorPlugin.StaticConfig.DefaultWebhookUsernameOverride))
        {
            SetUsername(DiscordConnectorPlugin.StaticConfig.DefaultWebhookUsernameOverride);
        }
    }

    /// <summary>
    ///     Send the message to Discord.
    /// </summary>
    /// <param name="ev">The event that triggered this message</param>
    public void SendFor(Webhook.Event ev)
    {
        // If the object is not valid, do not send it
        if (!IsValid())
        {
            DiscordConnectorPlugin.StaticLogger.LogWarning(
                $"Attempted to send an invalid DiscordExecuteWebhook object:\n{this}");
            return;
        }

        // Find any webhook events that match the event
        try
        {
            if (DiscordConnectorPlugin.StaticConfig.PrimaryWebhook.HasEvent(ev))
            {
                DiscordConnectorPlugin.StaticLogger.LogDebug($"Sending {ev} message to Primary Webhook");
                WebhookEntry primaryWebhook = DiscordConnectorPlugin.StaticConfig.PrimaryWebhook;
                ResetOverrides();

                if (primaryWebhook.HasUsernameOverride())
                {
                    SetUsername(primaryWebhook.UsernameOverride);
                }

                if (primaryWebhook.HasAvatarOverride())
                {
                    SetAvatarUrl(primaryWebhook.AvatarOverride);
                }

                DiscordApi.SendSerializedJson(primaryWebhook, JsonConvert.SerializeObject(this));
            }

            if (DiscordConnectorPlugin.StaticConfig.SecondaryWebhook.HasEvent(ev))
            {
                DiscordConnectorPlugin.StaticLogger.LogDebug($"Sending {ev} message to Secondary Webhook");
                WebhookEntry secondaryWebhook = DiscordConnectorPlugin.StaticConfig.SecondaryWebhook;
                ResetOverrides();

                if (secondaryWebhook.HasUsernameOverride())
                {
                    SetUsername(secondaryWebhook.UsernameOverride);
                }

                if (secondaryWebhook.HasAvatarOverride())
                {
                    SetAvatarUrl(secondaryWebhook.AvatarOverride);
                }

                DiscordApi.SendSerializedJson(secondaryWebhook, JsonConvert.SerializeObject(this));
            }

                foreach (WebhookEntry webhook in DiscordConnectorPlugin.StaticConfig.ExtraWebhooks)
                {
                    if (webhook.HasEvent(ev))
                    {
                        DiscordConnectorPlugin.StaticLogger.LogDebug($"Sending {ev} message to an Extra Webhook");
                        ResetOverrides();

                        if (webhook.HasUsernameOverride())
                        {
                            SetUsername(webhook.UsernameOverride);
                        }

                        if (webhook.HasAvatarOverride())
                        {
                            SetAvatarUrl(webhook.AvatarOverride);
                        }

                        DiscordApi.SendSerializedJson(webhook, JsonConvert.SerializeObject(this));
                    }
                }
            
        }
        catch (Exception e)
        {
            DiscordConnectorPlugin.StaticLogger.LogWarning($"Error serializing payload: {e}");
        }
    }
    // ! Any additional fields are also left out, as they are not used in this plugin.
}

internal class AllowedMentions
{
    public AllowedMentions()
    {
        parse = [];
        roles = [];
        users = [];
        replied_user = false;

        // Update from config
        if (DiscordConnectorPlugin.StaticConfig.AllowMentionsHereEveryone)
        {
            AllowEveryone();
        }

        if (DiscordConnectorPlugin.StaticConfig.AllowMentionsAnyRole)
        {
            AllowAnyRoles();
        }

        if (DiscordConnectorPlugin.StaticConfig.AllowMentionsAnyUser)
        {
            AllowAnyUsers();
        }

        if (DiscordConnectorPlugin.StaticConfig.AllowedRoleMentions.Count > 0)
        {
            AllowRoles(DiscordConnectorPlugin.StaticConfig.AllowedRoleMentions);
        }

        if (DiscordConnectorPlugin.StaticConfig.AllowedUserMentions.Count > 0)
        {
            AllowUsers(DiscordConnectorPlugin.StaticConfig.AllowedUserMentions);
        }
    }

    /// <summary>
    ///     An array of allowed mention types to parse from the content.
    ///     Allowed mention types:
    ///     `roles` - Role mentions
    ///     `users` - User mentions
    ///     `everyone` - @everyone/@here mentions
    /// </summary>
    /// <value>empty (none allowed)</value>
    public List<string> parse { get; set; }

    /// <summary>
    ///     Array of role_ids to mention (Max size of 100)
    /// </summary>
    /// <value>empty</value>
    public List<string> roles { get; set; }

    /// <summary>
    ///     Array of user_ids to mention (Max size of 100)
    /// </summary>
    /// <value>empty</value>
    public List<string> users { get; set; }

    /// <summary>
    ///     For replies, whether to mention the user being replied to.
    /// </summary>
    /// <value>false</value>
    public bool replied_user { get; set; }

    /// <summary>
    ///     Enable `@everyone` and `@here` mentions.
    /// </summary>
    public void AllowEveryone()
    {
        if (!parse.Contains("everyone"))
        {
            parse.Add("everyone");
        }
    }

    /// <summary>
    ///     Enable `@everyone` and `@here` mentions.
    /// </summary>
    public void AllowHere()
    {
        if (!parse.Contains("everyone"))
        {
            parse.Add("everyone");
        }
    }

    /// <summary>
    ///     Add a role_id to the allowed mentions.
    /// </summary>
    /// <param name="role_id">The role_id to allow mentions for</param>
    public void AllowRole(string role_id)
    {
        if (!roles.Contains(role_id))
        {
            roles.Add(role_id);
        }
    }

    /// <summary>
    ///     Add a list of role_ids to the allowed mentions.
    /// </summary>
    /// <param name="role_ids">The role_ids to allow mentions for</param>
    public void AllowRoles(List<string> role_ids)
    {
        foreach (string role_id in role_ids)
        {
            AllowRole(role_id);
        }
    }

    /// <summary>
    ///     Remove a role_id from the allowed mentions.
    /// </summary>
    /// <param name="role_id">The role_id to disallow mentions for</param>
    public void DisallowRole(string role_id)
    {
        if (roles.Contains(role_id))
        {
            roles.Remove(role_id);
        }
    }

    /// <summary>
    ///     Add a user_id to the allowed mentions.
    /// </summary>
    /// <param name="user_id">The user_id to allow mentions for</param>
    public void AllowUser(string user_id)
    {
        if (!users.Contains(user_id))
        {
            users.Add(user_id);
        }
    }

    /// <summary>
    ///     Add a list of user_ids to the allowed mentions.
    /// </summary>
    /// <param name="user_ids">The user_ids to allow mentions for</param>
    public void AllowUsers(List<string> user_ids)
    {
        foreach (string user_id in user_ids)
        {
            AllowUser(user_id);
        }
    }

    /// <summary>
    ///     Remove a user_id from the allowed mentions.
    /// </summary>
    /// <param name="user_id">The user_id to disallow mentions for</param>
    public void DisallowUser(string user_id)
    {
        if (users.Contains(user_id))
        {
            users.Remove(user_id);
        }
    }

    /// <summary>
    ///     Allow any role mentions.
    /// </summary>
    public void AllowAnyRoles()
    {
        if (!parse.Contains("roles"))
        {
            parse.Add("roles");
        }
    }

    /// <summary>
    ///     Allow any user mentions.
    /// </summary>
    public void AllowAnyUsers()
    {
        if (!parse.Contains("users"))
        {
            parse.Add("users");
        }
    }

    /// <summary>
    ///     Disallow any role mentions. (Specific role_ids will still be allowed)
    /// </summary>
    public void DisallowAnyRoles()
    {
        if (parse.Contains("roles"))
        {
            parse.Remove("roles");
        }
    }

    /// <summary>
    ///     Disallow any user mentions. (Specific user_ids will still be allowed)
    /// </summary>
    public void DisallowAnyUsers()
    {
        if (parse.Contains("users"))
        {
            parse.Remove("users");
        }
    }
}

/// <summary>
///     A complex Discord message, containing more than just a simple string.
///     Fully implements Discord's embed object structure.
///     See https://discord.com/developers/docs/resources/channel#embed-object
/// </summary>
internal class DiscordEmbed
{
    /// <summary>
    ///     The title of the embed.
    ///     Limited to 256 characters by Discord API.
    /// </summary>
    public string? title { get; set; }

    /// <summary>
    ///     The description of the embed.
    ///     Limited to 4096 characters by Discord API.
    /// </summary>
    public string? description { get; set; }

    /// <summary>
    ///     The URL of the embed. Makes the title into a hyperlink if set.
    /// </summary>
    public string? url { get; set; }

    /// <summary>
    ///     The timestamp to display in the embed footer.
    ///     ISO8601 timestamp format.
    /// </summary>
    public string? timestamp { get; set; }

    /// <summary>
    ///     The color of the embed sidebar.
    ///     Stored as decimal color value (0-16777215).
    /// </summary>
    public int? color { get; set; }

    /// <summary>
    ///     The author information for the embed.
    ///     Displays at the top of the embed.
    /// </summary>
    public DiscordEmbedAuthor? author { get; set; }

    /// <summary>
    ///     The footer information for the embed.
    ///     Displays at the bottom of the embed.
    /// </summary>
    public DiscordEmbedFooter? footer { get; set; }

    /// <summary>
    ///     The thumbnail image to display in the top-right corner of the embed.
    ///     Limited to 80x80 pixels by Discord's UI, but can be larger resolution.
    /// </summary>
    public DiscordEmbedThumbnail? thumbnail { get; set; }

    /// <summary>
    ///     The main image to display within the embed body.
    /// </summary>
    public DiscordEmbedImage? image { get; set; }

    /// <summary>
    ///     A list of fields to include in the embed.
    ///     For leaderboards, each leaderboard entry is typically a field.
    ///     Limited to 25 fields by Discord API.
    /// </summary>
    public List<DiscordField>? fields { get; set; }
}

/// <summary>
///     A field for a Discord embed, which allows for structured content display.
///     When set to inline, fields will display in rows (up to 3 per row).
///     See https://discord.com/developers/docs/resources/channel#embed-object-embed-field-structure
/// </summary>
internal class DiscordField
{
    /// <summary>
    ///     Name of the field.
    ///     These are just titled embedded values, where the name is the title. The value is a content string.
    ///     Limited to 256 characters by Discord API.
    /// </summary>
    public string? name { get; set; }

    /// <summary>
    ///     The string content of the field.
    ///     For example, the leaderboards are a list with `\n` as a separator, so they appear as an ordered list in Discord.
    ///     Limited to 1024 characters by Discord API.
    /// </summary>
    public string? value { get; set; }

    /// <summary>
    ///     Whether or not this field should display inline. 
    ///     When true, fields can be arranged horizontally (2-3 per row).
    ///     When false, fields will take up the full width of the embed.
    /// </summary>
    /// <value>false by default</value>
    public bool? inline { get; set; }
}

/// <summary>
///     Represents the author section of a Discord embed.
///     Appears at the top of the embed with name, icon, and optional URL.
///     See https://discord.com/developers/docs/resources/channel#embed-object-embed-author-structure
/// </summary>
internal class DiscordEmbedAuthor
{
    /// <summary>
    ///     The name of the author.
    ///     Typically used for player name or server name.
    ///     Limited to 256 characters by Discord API.
    /// </summary>
    public string? name { get; set; }

    /// <summary>
    ///     The URL to make the author name clickable.
    ///     Could link to player profile, server info, etc.
    /// </summary>
    public string? url { get; set; }

    /// <summary>
    ///     The URL of the author icon.
    ///     Typically a small image that appears next to the author name.
    ///     Must be a publicly accessible image URL.
    /// </summary>
    public string? icon_url { get; set; }
}

/// <summary>
///     Represents the footer section of a Discord embed.
///     Appears at the bottom of the embed with text, icon, and can be paired with timestamp.
///     See https://discord.com/developers/docs/resources/channel#embed-object-embed-footer-structure
/// </summary>
internal class DiscordEmbedFooter
{
    /// <summary>
    ///     The text for the footer.
    ///     Limited to 2048 characters by Discord API.
    /// </summary>
    public string? text { get; set; }

    /// <summary>
    ///     The URL of the footer icon.
    ///     Must be a publicly accessible image URL.
    /// </summary>
    public string? icon_url { get; set; }
}

/// <summary>
///     Represents a thumbnail image in a Discord embed.
///     Appears in the top-right corner of the embed.
///     See https://discord.com/developers/docs/resources/channel#embed-object-embed-thumbnail-structure
/// </summary>
internal class DiscordEmbedThumbnail
{
    /// <summary>
    ///     The URL of the thumbnail image.
    ///     Must be a publicly accessible image URL.
    /// </summary>
    public string url { get; set; } = "";

    /// <summary>
    ///     The proxy URL of the thumbnail image (used by Discord internally).
    ///     Generally not set manually; Discord will generate this.
    /// </summary>
    public string? proxy_url { get; set; }

    /// <summary>
    ///     The height of the thumbnail image.
    ///     Not typically needed, as Discord will resize images appropriately.
    /// </summary>
    public int? height { get; set; }

    /// <summary>
    ///     The width of the thumbnail image.
    ///     Not typically needed, as Discord will resize images appropriately.
    /// </summary>
    public int? width { get; set; }
}

/// <summary>
///     Represents a main image in a Discord embed.
///     Appears in the body of the embed, below description and above fields.
///     See https://discord.com/developers/docs/resources/channel#embed-object-embed-image-structure
/// </summary>
internal class DiscordEmbedImage
{
    /// <summary>
    ///     The URL of the image.
    ///     Must be a publicly accessible image URL.
    /// </summary>
    public string url { get; set; } = "";

    /// <summary>
    ///     The proxy URL of the image (used by Discord internally).
    ///     Generally not set manually; Discord will generate this.
    /// </summary>
    public string? proxy_url { get; set; }

    /// <summary>
    ///     The height of the image.
    ///     Not typically needed, as Discord will resize images appropriately.
    /// </summary>
    public int? height { get; set; }

    /// <summary>
    ///     The width of the image.
    ///     Not typically needed, as Discord will resize images appropriately.
    /// </summary>
    public int? width { get; set; }
}
