using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

namespace DiscordConnector
{
    internal static class IpifyAPI
    {
        private const string ENDPOINT = "https://api64.ipify.org";

        /// <summary>
        /// Get your public IP address (either IPv4 or IPv6, preferring IPv4) according to https://api64.ipify.org
        /// </summary>
        /// <returns>Your public IP address</returns>
        public static string PublicIpAddress()
        {
            string ipAddress = "";

            WebRequest request = WebRequest.Create(ENDPOINT);
            request.Method = "GET";

            // Wrap firing the response in a TRY/CATCH and on an exception, abandon trying to retreive the public IP
            try
            {
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
            catch (Exception e)
            {
                Plugin.StaticLogger.LogWarning("Failure to get public IP from ipify.org");
                Plugin.StaticLogger.LogWarning(e);
            }
            return "::";
        }

    }
}
