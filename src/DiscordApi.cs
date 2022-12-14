using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace DiscordConnector
{
    class DiscordApi
    {
        /// <summary>
        /// Send a <paramref name="message"/> and a <paramref name="pos"/> to Discord.
        /// </summary>
        /// <param name="message">A string optionally formatted with Discord-approved markdown syntax.</param>
        /// <param name="pos">A 3-dimensional vector representing a position</param>
        public static void SendMessage(string message, Vector3 pos)
        {
            if (Plugin.StaticConfig.DiscordEmbedsEnabled)
            {
                SendMessageWithFields(message, new List<Tuple<string, string>> {
                    Tuple.Create("Coordinates",MessageTransformer.FormatVector3AsPos(pos))
                });
            }
            else
            {
                SendMessage($"{message} {MessageTransformer.FormatAppendedPos(pos)}");
            }
        }
        /// <summary>
        /// Sends a <paramref name="message"/> to Discord.
        /// </summary>
        /// <param name="message">A string optionally formatted with Discord-approved markdown syntax.</param>
        public static void SendMessage(string message)
        {
            // A simple string message
            var payload = new DiscordSimpleWebhook
            {
                content = message
            };

            string payloadString = JsonConvert.SerializeObject(payload);

            SendSerializedJson(payloadString);
        }

        /// <summary>
        /// Send a <paramref name="message"/> with <paramref name="fields"/> to Discord.
        /// </summary>
        /// <param name="content">A string optionally formatted with Discord-approved markdown syntax.</param>
        /// <param name="fields">Discord fields as defined in the API, as Tuples (field name, value)</param>
        public static void SendMessageWithFields(string content = null, List<Tuple<string, string>> fields = null)
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

                // Put the field JSON strings into our payload object 
                payloadString += string.Join(",", fieldStrings.ToArray());
                payloadString += "]}]";

                // Cautiously put a comma if there is content to add to the payload as well
                if (content != null)
                {
                    payloadString += ",";
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
            SendSerializedJson(payloadString);
        }

        /// <summary>
        /// Sends <paramref name="serializedJson"/> to the webhook specified in configuration.
        /// </summary>
        /// <param name="serializedJson">Body data for the webhook as JSON serialized into a string</param>
        internal static void SendSerializedJson(string serializedJson)
        {
            Plugin.StaticLogger.LogDebug($"Trying webhook with payload: {serializedJson}");

            // Guard against unset webhook or empty serialized json
            if (string.IsNullOrEmpty(Plugin.StaticConfig.WebHookURL) || string.IsNullOrEmpty(serializedJson))
            {
                return;
            }

            // Create a web request to send the payload to discord
            WebRequest request = WebRequest.Create(Plugin.StaticConfig.WebHookURL);
            request.Method = "POST";
            request.ContentType = "application/json";

            request.GetRequestStreamAsync().ContinueWith(requestStreamTask =>
            {
                // Write JSON payload into request
                using StreamWriter writer = new(requestStreamTask.Result);
                writer.WriteAsync(serializedJson).ContinueWith(_ =>
                {
                    request.GetResponseAsync();
                });
            });
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
}
