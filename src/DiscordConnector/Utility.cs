using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

namespace DiscordConnectorLite
{
    internal static class IpifyAPI
    {
        private const string ENDPOINT = "https://api.ipify.org";

        /// <summary>
        /// Get your public IP address (either IPv4 or IPv6, preferring IPv4) according to https://api64.ipify.org
        /// </summary>
        /// <returns>Your public IP address</returns>
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

    internal static class Utility
    {
        /// <summary>
        /// Generate a random string <paramref name="length"/> characters long. Characters will be chosen randomly from
        /// the list "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz"
        /// </summary>
        /// <param name="length">Desired length of the random string.</param>
        /// <returns>A random alphanumeric string.</returns>
        public static string RandomAlphanumericString(int length)
        {
            const string allowed_chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            Plugin.StaticLogger.LogDebug("Random string generation start.");
            var random = new CryptoRandom();
            string randomString = new string(Enumerable.Repeat(allowed_chars, length)
                                                    .Select(s => s[random.Next(s.Length)]).ToArray());

            Plugin.StaticLogger.LogDebug("Random string generation complete.");
            return randomString;
        }
        /// <summary>
        /// A More "Random" Random from .NET Matters 10/02/2019. https://docs.microsoft.com/en-us/archive/msdn-magazine/2007/september/net-matters-tales-from-the-cryptorandom
        /// An implementation of System.Random using RNGCryptoServiceProvider.
        /// </summary>
        private class CryptoRandom : System.Random
        {
            private RNGCryptoServiceProvider _rng =
                new RNGCryptoServiceProvider();
            private byte[] _uint32Buffer = new byte[4];

            public CryptoRandom() { }
            public CryptoRandom(Int32 ignoredSeed) { }

            public override Int32 Next()
            {
                _rng.GetBytes(_uint32Buffer);
                return BitConverter.ToInt32(_uint32Buffer, 0) & 0x7FFFFFFF;
            }

            public override Int32 Next(Int32 maxValue)
            {
                if (maxValue < 0)
                    throw new ArgumentOutOfRangeException("maxValue");
                return Next(0, maxValue);
            }

            public override Int32 Next(Int32 minValue, Int32 maxValue)
            {
                if (minValue > maxValue)
                    throw new ArgumentOutOfRangeException("minValue");
                if (minValue == maxValue) return minValue;
                Int64 diff = maxValue - minValue;
                while (true)
                {
                    _rng.GetBytes(_uint32Buffer);
                    UInt32 rand = BitConverter.ToUInt32(_uint32Buffer, 0);

                    Int64 max = (1 + (Int64)UInt32.MaxValue);
                    Int64 remainder = max % diff;
                    if (rand < max - remainder)
                    {
                        return (Int32)(minValue + (rand % diff));
                    }
                }
            }

            public override double NextDouble()
            {
                _rng.GetBytes(_uint32Buffer);
                UInt32 rand = BitConverter.ToUInt32(_uint32Buffer, 0);
                return rand / (1.0 + UInt32.MaxValue);
            }

            public override void NextBytes(byte[] buffer)
            {
                if (buffer == null) throw new ArgumentNullException("buffer");
                _rng.GetBytes(buffer);
            }
        }
    }
}
