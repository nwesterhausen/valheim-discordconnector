using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DiscordConnector;

internal static class Hashing
{
    public static string GetMD5Checksum(string filename)
    {
        using MD5 md5 = MD5.Create();
        using FileStream stream = File.OpenRead(filename);
        byte[] hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "");
    }
}

internal static class Strings
{
    public static string HumanReadableMs(double ms)
    {
        TimeSpan t = TimeSpan.FromMilliseconds(ms);

        if (t.Milliseconds == t.Seconds && t.Seconds == 0)
        {
            return string.Format(
                "{0:D2}h:{1:D2}m",
                t.Hours,
                t.Minutes);
        }

        return string.Format(
            "{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
            t.Hours,
            t.Minutes,
            t.Seconds,
            t.Milliseconds);
    }
}

internal static class PublicIPChecker
{
    /// <summary>
    /// Efficient handler for HTTP requests.
    /// </summary>
    private static readonly HttpClient httpClient = new HttpClient();

    /// <summary>
    /// Asynchronously gets the public IP address of the server from https://api.ipify.org
    /// </summary>
    /// <remarks>
    /// In the Plugins main Awake() method, you must have this line:
    /// ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    /// </remarks>   
    /// <returns>A Task that resolves to the public IP address string, or an empty string on failure.</returns>
    public static async Task<string> GetPublicIPAsync()
    {
        DiscordConnectorPlugin.StaticLogger.LogDebug("Getting public IP address.");
        string address = string.Empty;
        try
        {
            address = await httpClient.GetStringAsync("https://api.ipify.org").ConfigureAwait(false);
            address = address.Trim();
        }
        catch (Exception e)
        {
            await DiscordConnectorPlugin.StaticLogger.LogErrorAsync(
                $"Failed to get public IP address, an empty string will be used. Details:\n{e}")
                .ConfigureAwait(false);
        }

        await DiscordConnectorPlugin.StaticLogger.LogDebugAsync($"Public IP address is '{address}'")
            .ConfigureAwait(false);
        return address;
    }
}

internal static class GuidHelper
{
    public static string GenerateShortHexGuid(int byteCount = 4)
    {
        if (byteCount <= 0 || byteCount > 16)
        {
            throw new ArgumentException("Byte count must be between 1 and 16.");
        }

        Guid guid = Guid.NewGuid();
        byte[] bytes = guid.ToByteArray();
        string hexString = BitConverter.ToString(bytes, 0, byteCount).Replace("-", "").ToLower();
        return hexString;
    }
}
