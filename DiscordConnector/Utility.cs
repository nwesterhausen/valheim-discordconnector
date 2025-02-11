using System;

namespace DiscordConnector;
internal static class Hashing
{
    public static string GetMD5Checksum(string filename)
    {
        using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        using System.IO.FileStream stream = System.IO.File.OpenRead(filename);
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
    /// Get the public IP address of the server from https://ifconfig.me/ip
    /// </summary>
    /// <returns>The public IP address of the server</returns>
    public static string GetPublicIP()
    {
        DiscordConnectorPlugin.StaticLogger.LogDebug("Getting public IP address.");
        string address = string.Empty;
        try
        {
            using System.Net.WebClient client = new();
            address = client.DownloadString("https://ifconfig.me/ip");
        }
        catch (Exception e)
        {
            DiscordConnectorPlugin.StaticLogger.LogError($"Failed to get public IP address, an empty string will be used: {e.Message}");
        }
        DiscordConnectorPlugin.StaticLogger.LogDebug($"Public IP address is '{address}'");
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
