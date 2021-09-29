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

        internal static void SendSerializedJson(string serializedJson)
        {
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
}