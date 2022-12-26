using System;

namespace DiscordConnector;
internal static class Hashing
{
    public static string GetMD5Checksum(string filename)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        using var stream = System.IO.File.OpenRead(filename);
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
