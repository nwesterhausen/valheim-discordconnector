using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using UnityEngine;

namespace DiscordConnector
{
    static class IpifyAPI
    {
        private const string ENDPOINT = "https://api64.ipify.org";
        public static string PublicIpAddress()
        {
            string ipAddress = "";

            WebRequest request = WebRequest.Create(ENDPOINT);
            request.Method = "GET";

            WebResponse response = request.GetResponse();
            Plugin.StaticLogger.LogDebug($"Response Short Code (ipify.org): {((HttpWebResponse)response).StatusDescription}");

            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.
            using (Stream dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                ipAddress = reader.ReadToEnd();
                // Display the content.
                Plugin.StaticLogger.LogDebug($"Full response (ipify): {ipAddress}");
            }

            // Close the response.
            response.Close();

            return ipAddress;
        }

    }
}
