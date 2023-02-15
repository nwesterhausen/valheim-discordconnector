using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordConnector;
class DiscordApi
{
    /// <summary>
    /// Send a <paramref name="message"/> and a <paramref name="pos"/> to Discord.
    /// </summary>
    /// <param name="ev">The event which triggered this message</param>
    /// <param name="message">A string optionally formatted with Discord-approved markdown syntax.</param>
    /// <param name="pos">A 3-dimensional vector representing a position</param>
    public static void SendMessage(Webhook.Event ev, string message, UnityEngine.Vector3 pos)
    {
        if (Plugin.StaticConfig.DiscordEmbedsEnabled)
        {
            SendMessageWithFields(ev, message, new List<Tuple<string, string>> {
                    Tuple.Create("Coordinates",MessageTransformer.FormatVector3AsPos(pos))
                });
        }
        else
        {
            SendMessage(ev, $"{message} {MessageTransformer.FormatAppendedPos(pos)}");
        }
    }
    /// <summary>
    /// Sends a <paramref name="message"/> to Discord.
    /// </summary>
    /// <param name="ev">The event which triggered this message</param>
    /// <param name="message">A string optionally formatted with Discord-approved markdown syntax.</param>
    public static void SendMessage(Webhook.Event ev, string message)
    {
        // A simple string message
        var payload = new DiscordSimpleWebhook
        {
            content = message
        };

        string payloadString = JsonConvert.SerializeObject(payload);

        SendSerializedJson(ev, payloadString);
    }

    /// <summary>
    /// Send a <paramref name="message"/> with <paramref name="fields"/> to Discord.
    /// </summary>
    /// <param name="ev">The event which triggered this message</param>
    /// <param name="content">A string optionally formatted with Discord-approved markdown syntax.</param>
    /// <param name="fields">Discord fields as defined in the API, as Tuples (field name, value)</param>
    public static void SendMessageWithFields(Webhook.Event ev, string content = null, List<Tuple<string, string>> fields = null)
    {
        // Guard against null/empty calls
        if (string.IsNullOrEmpty(content) && fields == null)
        {
            content = "Uh-oh! An unexpectedly empty message was sent!";
        }

        // Begin the payload object
        string payloadString = "{";
        // If we have fields at all, put them as embedded fields
        if (fields != null)
        {
            // Fields go under embed as array
            payloadString += "\"embeds\":[{\"fields\":[";

            // Convert the fields into JSON Strings
            List<string> fieldStrings = new List<string>();
            foreach (Tuple<string, string> t in fields)
            {
                fieldStrings.Add(JsonConvert.SerializeObject(new DiscordField
                {
                    name = t.Item1,
                    value = t.Item2
                }));
            }

            if (fieldStrings.Count > 0)
            {
                // Put the field JSON strings into our payload object 
                // Fields go under embed as array
                payloadString += "\"embeds\":[{\"fields\":[";
                payloadString += string.Join(",", fieldStrings.ToArray());
                payloadString += "]}]";

                // Cautiously put a comma if there is content to add to the payload as well
                if (content != null)
                {
                    payloadString += ",";
                }
            }
        }

        // If there is any content
        if (content != null)
        {
            // Append the content to the payload
            payloadString += $"\"content\":\"{content}\"";
        }

        // Finish the payload JSON 
        payloadString += "}";

        // Use our pre-existing method to send serialized JSON to discord
        SendSerializedJson(ev, payloadString);
    }

    /// <summary>
    /// Sends <paramref name="serializedJson"/> to the webhook specified in configuration.
    /// </summary>
    /// <param name="ev">The event which triggered this message</param>
    /// <param name="serializedJson">Body data for the webhook as JSON serialized into a string</param>
    private static void SendSerializedJson(Webhook.Event ev, string serializedJson)
    {
        Plugin.StaticLogger.LogDebug($"Trying webhook with payload: {serializedJson} (event: {ev})");

        if (ev == Webhook.Event.Other)
        {
            Plugin.StaticLogger.LogInfo($"Dispatching webhook for 3rd party plugin (configured as 'Other' in WebHook config)");
        }

        // Guard against unset webhook or empty serialized json
        if ((string.IsNullOrEmpty(Plugin.StaticConfig.PrimaryWebhook.Url) && string.IsNullOrEmpty(Plugin.StaticConfig.SecondaryWebhook.Url)) || string.IsNullOrEmpty(serializedJson))
        {
            return;
        }

        // Responsible for sending a JSON string to the webhook.
        byte[] byteArray = Encoding.UTF8.GetBytes(serializedJson);

        if (Plugin.StaticConfig.PrimaryWebhook.HasEvent(ev))
        {
            Plugin.StaticLogger.LogDebug($"Sending {ev} message to Primary Webhook");
            DispatchRequest(Plugin.StaticConfig.PrimaryWebhook, byteArray);
        }
        if (Plugin.StaticConfig.SecondaryWebhook.HasEvent(ev))
        {
            Plugin.StaticLogger.LogDebug($"Sending {ev} message to Secondary Webhook");
            DispatchRequest(Plugin.StaticConfig.SecondaryWebhook, byteArray);
        }
    }

    /// <summary>
    /// Send a web request to discord.
    /// </summary>
    /// <param name="webhook">The webhook to use for the request</param>
    /// <param name="byteArray">The payload as a byte array</param>
    private static void DispatchRequest(WebhookEntry webhook, byte[] byteArray)
    {

        // Create a web request to send the payload to discord
        WebRequest request = WebRequest.Create(webhook.Url);
        request.Method = "POST";
        request.ContentType = "application/json";
        request.ContentLength = byteArray.Length;

        // Dispatch the request to discord and the response processing to an async task
        Task.Run(() =>
        {
            // We have to write the data to the request
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            // Wait for a response to the web request
            WebResponse response = request.GetResponse();
            if (Plugin.StaticConfig.DebugHttpRequestResponse)
            {
                Plugin.StaticLogger.LogDebug($"Request Response Short Code: {((HttpWebResponse)response).StatusDescription}");
            }

            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.
            using (dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                if (Plugin.StaticConfig.DebugHttpRequestResponse)
                {
                    Plugin.StaticLogger.LogDebug($"Full response: {responseFromServer}");
                }
            }

            // Close the response.
            response.Close();
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Send a <paramref name="message"/> and a <paramref name="pos"/> to Discord.
    /// </summary>
    /// <param name="message">A string optionally formatted with Discord-approved markdown syntax.</param>
    /// <param name="pos">A 3-dimensional vector representing a position</param>
    public static void SendMessage(string message, UnityEngine.Vector3 pos)
    {
        SendMessage(Webhook.Event.Other, message, pos);
    }
    /// <summary>
    /// Sends a <paramref name="message"/> to Discord.
    /// </summary>
    /// <param name="message">A string optionally formatted with Discord-approved markdown syntax.</param>
    public static void SendMessage(string message)
    {
        SendMessage(Webhook.Event.Other, message);
    }
    /// <summary>
    /// Send a <paramref name="message"/> with <paramref name="fields"/> to Discord.
    /// </summary>
    /// <param name="content">A string optionally formatted with Discord-approved markdown syntax.</param>
    /// <param name="fields">Discord fields as defined in the API, as Tuples (field name, value)</param>
    public static void SendMessageWithFields(string content = null, List<Tuple<string, string>> fields = null)
    {
        SendMessageWithFields(Webhook.Event.Other, content, fields);
    }
}

internal class DiscordSimpleWebhook
{
    public string content { get; set; }
}
internal class DiscordComplexWebhook
{
    public DiscordEmbed embeds { get; set; }
}
internal class DiscordEmbed
{

#nullable enable
    public string? title { get; set; }
    public string? description { get; set; }
    public List<DiscordField>? fields { get; set; }
#nullable restore
}
internal class DiscordField
{
    public string name { get; set; }
    public string value { get; set; }
}
