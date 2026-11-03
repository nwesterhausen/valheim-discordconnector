using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using DiscordConnector.Records;

using UnityEngine;

namespace DiscordConnector.Tests;

/// <summary>
///     Talks to tools/WebhookCapture, which stands in for Discord during tests.
/// </summary>
internal static class Capture
{
    // Read the capture tool's log file rather than calling it over HTTP. The game's HTTP connection pool is
    // shared with the mod, so an HTTP call here could queue behind the mod's own webhook requests.
    private static string LogPath => Environment.GetEnvironmentVariable("DCTEST_CAPTURE_LOG");

    /// <summary>Captured webhook entries (one JSON object per line) that contain <paramref name="text" />.</summary>
    public static List<string> Lines(string text)
    {
        if (string.IsNullOrEmpty(LogPath) || !File.Exists(LogPath))
        {
            return new List<string>();
        }

        using FileStream fs = new(LogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using StreamReader reader = new(fs);
        string all = reader.ReadToEnd();
        // The capture tool escapes apostrophes as ' in JSON; match either form.
        string escaped = text.Replace("'", "\\u0027");
        return all.Split('\n')
            .Where(l => l.Length > 0 && (l.Contains(text) || l.Contains(escaped)))
            .ToList();
    }

    public static string Search(string text)
    {
        return string.Join("\n", Lines(text).ToArray());
    }

    public static int Count(string text, string webhook = "primary")
    {
        return Lines(text).Count(l => l.Contains($"\"webhook\":\"{webhook}\""));
    }
}

internal static class Check
{
    public static void True(bool condition, string message)
    {
        if (!condition)
        {
            throw new AssertionException(message);
        }
    }

    public static void Equal<T>(T expected, T actual, string what)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new AssertionException($"{what}: expected {expected}, got {actual}");
        }
    }

    /// <summary>
    ///     Polls <paramref name="condition" /> until it is true or the timeout passes, then fails with a message
    ///     built by <paramref name="describe" />. Use as: foreach (var y in Check.Eventually(...)) yield return y;
    ///     Needed because DiscordConnector writes records and sends webhooks on background tasks.
    /// </summary>
    public static IEnumerable<object> Eventually(Func<bool> condition, Func<string> describe, float timeout = 10f)
    {
        float until = Time.realtimeSinceStartup + timeout;
        while (!Safe(condition))
        {
            if (Time.realtimeSinceStartup > until)
            {
                throw new AssertionException($"timed out after {timeout}s: {describe()}");
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    private static bool Safe(Func<bool> condition)
    {
        try
        {
            return condition();
        }
        catch
        {
            return false;
        }
    }
}

internal static class Db
{
    public static Database Instance => DiscordConnectorPlugin.StaticDatabase;

    public static int Count(string category, string playerName)
    {
        return Instance.CountOfRecordsByName(category, playerName);
    }

    /// <summary>Count for one player in a grouped result, or -1 if the player is missing.</summary>
    public static int Lookup(List<CountResult> results, string playerName)
    {
        CountResult match = results.FirstOrDefault(r => r.Name == playerName);
        return match?.Count ?? -1;
    }

    public static string Describe(List<CountResult> results)
    {
        return results.Count == 0 ? "(empty)" : string.Join(", ", results.Select(r => r.ToString()).ToArray());
    }
}
