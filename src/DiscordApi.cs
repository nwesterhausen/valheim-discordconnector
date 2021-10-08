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
