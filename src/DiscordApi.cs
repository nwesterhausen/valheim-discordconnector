using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
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
                    Tuple.Create("Coordinates",$"{pos}")
                });
            }
            else
            {
                SendMessage($"{message} Coords: {pos}");
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

            string payloadString = JsonSerializer.Serialize(payload);

            SendSerializedJson(payloadString);
        }

        /// <summary>
        /// Send a <paramref name="message"/> with <paramref name="fields"/> to Discord.
        /// </summary>
        /// <param name="content">A string optionally formatted with Discord-approved markdown syntax.</param>
        /// <param name="fields">Discord fields as defined in the API, as Tuples (fieldname, value)</param>
        public static void SendMessageWithFields(string content = null, List<Tuple<string, string>> fields = null)
        {

            if (string.IsNullOrEmpty(content) && fields == null)
            {
                content = "Uh-oh! An unexpectedly empty message was sent!";
            }

            string payloadString = "{";
            if (fields != null)
            {
                payloadString += "\"embeds\":[{\"fields\":[";
                List<string> fieldStrings = new List<string>();
                foreach (Tuple<string, string> t in fields)
                {
                    fieldStrings.Add(JsonSerializer.Serialize(new DiscordField
                    {
                        name = t.Item1,
                        value = t.Item2
                    }));
                }
                payloadString += string.Join(",", fieldStrings.ToArray());
                payloadString += "]}]";
                if (content != null)
                {
                    payloadString += ",";
                }
            }
            if (content != null)
            {
                payloadString += $"\"content\":\"{content}\"";
            }
            payloadString += "}";

            SendSerializedJson(payloadString);
        }

        /// <summary>
        /// Sends <paramref name="serializedJson"/> to the webhook specified in configuration.
        /// </summary>
        /// <param name="serializedJson">Body data for the webhook as JSON serialized into a string</param>
        internal static void SendSerializedJson(string serializedJson)
        {
            Plugin.StaticLogger.LogDebug($"Trying webhook with payload: {serializedJson}");
            if (string.IsNullOrEmpty(Plugin.StaticConfig.WebHookURL))
            {
                Plugin.StaticLogger.LogInfo("No webhook set, not sending message.");
                return;
            }
            // Responsible for sending a JSON string to the webhook.
            byte[] byteArray = Encoding.UTF8.GetBytes(serializedJson);


            WebRequest request = WebRequest.Create(Plugin.StaticConfig.WebHookURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            Plugin.StaticLogger.LogDebug($"Request Response Short Code: {((HttpWebResponse)response).StatusDescription}");

            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.
            using (dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                Plugin.StaticLogger.LogDebug($"Full response: {responseFromServer}");
            }

            // Close the response.
            response.Close();
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
