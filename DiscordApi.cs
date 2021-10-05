using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace DiscordConnector
{
    class DiscordApi
    {
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

        public static void SendComplexMessage(string title = null, string description = null, List<Tuple<string, string>> fields = null)
        {
            // A complex message with embedded fields (optional) and a title and description (optional)
            var payload = new DiscordComplexWebhook
            {
                embeds = new DiscordEmbed()
            };
            if (!string.IsNullOrEmpty(title))
            {
                payload.embeds.title = title;
            }
            if (!string.IsNullOrEmpty(description))
            {
                payload.embeds.description = description;
            }
            if (fields != null)
            {
                payload.embeds.fields = new List<DiscordField>();
                foreach (Tuple<string, string> pair in fields)
                {
                    payload.embeds.fields.Add(new DiscordField
                    {
                        name = pair.Item1,
                        value = pair.Item2
                    });
                }
            }
            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(description) && fields == null)
            {
                payload.embeds.description = "Uh-oh! An unexpectedly empty message was sent!";
            }

            // Previously attempted to serialize the string automatically but discord doesn't like having
            // fields set to null when posting to the webhook. Here's a manual serialization..
            string payloadString = "{\"embeds\":[{";
            if (payload.embeds.title != null)
            {
                payloadString += $"\"title\":\"{payload.embeds.title}\"";
                if (payload.embeds.description != null || payload.embeds.fields != null)
                {
                    payloadString += ",";
                }
            }
            if (payload.embeds.description != null)
            {
                payloadString += $"\"description\":\"{payload.embeds.description}\"";
                if (payload.embeds.fields != null)
                {
                    payloadString += ",";
                }
            }
            if (payload.embeds.fields != null)
            {
                payloadString += "\"fields\":[";
                foreach (DiscordField f in payload.embeds.fields)
                {
                    payloadString += JsonSerializer.Serialize(f);
                }
                payloadString += "]";
            }
            payloadString += "}]}";

            SendSerializedJson(payloadString);
        }

        internal static void SendSerializedJson(string serializedJson)
        {
            Plugin.StaticLogger.LogInfo($"Trying webhook with payload: {serializedJson}");
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
            Plugin.StaticLogger.LogDebug(((HttpWebResponse)response).StatusDescription);

            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.
            using (dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                Plugin.StaticLogger.LogDebug(responseFromServer);
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