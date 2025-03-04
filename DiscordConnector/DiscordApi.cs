﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace DiscordConnector;

internal class DiscordApi
{
    /// <summary>
    ///     Send a <paramref name="message" /> and a <paramref name="pos" /> to Discord.
    /// </summary>
    /// <param name="ev">The event which triggered this message</param>
    /// <param name="message">A string optionally formatted with Discord-approved markdown syntax.</param>
    /// <param name="pos">A 3-dimensional vector representing a position</param>
    public static void SendMessage(Webhook.Event ev, string message, Vector3 pos)
    {
        if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled)
        {
            SendMessageWithFields(ev, message, [
                Tuple.Create("Coordinates", MessageTransformer.FormatVector3AsPos(pos))
            ]);
        }
        else
        {
            SendMessage(ev, $"{message} {MessageTransformer.FormatAppendedPos(pos)}");
        }
    }

    /// <summary>
    ///     Sends a <paramref name="message" /> to Discord.
    /// </summary>
    /// <param name="ev">The event which triggered this message</param>
    /// <param name="message">A string optionally formatted with Discord-approved markdown syntax.</param>
    public static void SendMessage(Webhook.Event ev, string message)
    {
        // A simple string message
        DiscordExecuteWebhook payload = new() { content = message };

        payload.SendFor(ev);
    }

    /// <summary>
    ///     Send a message with fields to Discord.
    /// </summary>
    /// <param name="ev">The event which triggered this message</param>
    /// <param name="content">A string optionally formatted with Discord-approved Markdown syntax.</param>
    /// <param name="fields">Discord fields as defined in the API, as Tuples (field name, value)</param>
    public static void SendMessageWithFields(Webhook.Event ev, string? content = null,
        List<Tuple<string, string>>? fields = null)
    {
        // Guard against null/empty calls
        if (string.IsNullOrEmpty(content) && fields == null)
        {
            content = "Uh-oh! An unexpectedly empty message was sent!";
        }

        DiscordExecuteWebhook payload = new() { content = content };

        // If we have fields at all, put them as embedded fields
        if (fields != null)
        {
            payload.embeds = [];
            List<DiscordField> discordFields = [];
            
            foreach (Tuple<string, string> t in fields)
            {
                discordFields.Add(new DiscordField { name = t.Item1, value = t.Item2 });
            }

            // Add the fields to the payload
            payload.embeds.Add(new DiscordEmbed { fields = discordFields });
        }

        payload.SendFor(ev);
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
///     See https://discord.com/developers/docs/resources/channel#embed-object
/// </summary>
internal class DiscordEmbed
{
    /// <summary>
    ///     The title of the message.
    /// </summary>
    public string? title { get; set; }

    /// <summary>
    ///     The description of the message.
    /// </summary>
    public string? description { get; set; }

    /// <summary>
    ///     A list of fields to include in the message.
    ///     For leaderboards, each leaderboard that is included is a field.
    /// </summary>
    public List<DiscordField>? fields { get; set; }
}

/// <summary>
///     A field for a Discord message, which allows for fancy formatting.
///     See https://discord.com/developers/docs/resources/channel#embed-object-embed-field-structure
/// </summary>
internal class DiscordField
{
    /// <summary>
    ///     Name of the field.
    ///     These are just titled embedded values, where the name is the title. The value is a content string.
    /// </summary>
    public string? name { get; set; }

    /// <summary>
    ///     The string content of the field.
    ///     For example, the leaderboards are a list with `\n` as a separator, so they appear as an ordered list in Discord.
    /// </summary>
    public string? value { get; set; }

    /// <summary>
    ///     Whether or not this field should display inline.
    /// </summary>
    /// <value>false</value>
    public bool? inline { get; set; }
}
