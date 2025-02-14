using System;
using System.Collections.Generic;
using DiscordConnector.Leaderboards;

namespace DiscordConnector.Records;

/// <summary>
///     These are categories used when keeping track of values in the record system. It is a simple system
///     that currently only supports storing string:integer pairings underneath one of these categories.
/// </summary>
public static class Categories
{
    /// <summary>
    ///     Count of player deaths
    /// </summary>
    public const string Death = "death";

    /// <summary>
    ///     Count of times player has joined
    /// </summary>
    public const string Join = "join";

    /// <summary>
    ///     Count of times player has left
    /// </summary>
    public const string Leave = "leave";

    /// <summary>
    ///     Count of times player has pinged the map
    /// </summary>
    public const string Ping = "ping";

    /// <summary>
    ///     Count of times player has shouted
    /// </summary>
    public const string Shout = "shout";

    /// <summary>
    ///     Sum of time between player join and leaves
    /// </summary>
    public const string TimeOnline = "time_online";

    /// <summary>
    ///     Categories that are known how to be queried using the <see cref="Helper" />
    /// </summary>
    /// <value>Valid category to query</value>
    public static readonly string[] All = new[] { Death, Join, Leave, Ping, Shout, TimeOnline };
}

/// <summary>
///     This class provides some convenience methods for querying the database, allowing for queries of all records or
///     records within a date range.
/// </summary>
public static class Helper
{
    /// <summary>
    ///     Retrieve the top <paramref name="n" /> players for the <paramref name="key" /> category.
    ///     This returns the values in descending (most to least) order.
    /// </summary>
    /// <param name="key">One of <see cref="Categories.All" /></param>
    /// <param name="n">Number of results to return</param>
    /// <returns>A list of <paramref name="n" /> players and their totals for the <paramref name="key" />, in descending order.</returns>
    public static List<CountResult> TopNResultForCategory(string key, int n)
    {
        return TopNResultForCategory(key, n, DateHelper.DummyDateTime, DateHelper.DummyDateTime);
    }

    /// <summary>
    ///     Retrieve the top <paramref name="n" /> players for the <paramref name="key" /> category.
    ///     This returns the values in descending (most to least) order.
    ///     Returns an empty list if the provided <paramref name="key" /> is invalid.
    /// </summary>
    /// <param name="key">One of <see cref="Categories.All" /></param>
    /// <param name="n">Number of results to return</param>
    /// <param name="startDate">Earliest valid date for the stat records used to gather the results</param>
    /// <param name="endDate">Latest valid date for the stat records used to gather the results</param>
    /// <returns>A list of <paramref name="n" /> players and their totals for the <paramref name="key" />, in descending order.</returns>
    public static List<CountResult> TopNResultForCategory(string key, int n, DateTime startDate, DateTime endDate)
    {
        // Validate key
        if (Array.IndexOf<string>(Categories.All, key) == -1)
        {
            DiscordConnectorPlugin.StaticLogger.LogWarning($"Invalid key \"{key}\" when getting top {n} results.");
            DiscordConnectorPlugin.StaticLogger.LogDebug("Empty list returned because of invalid key.");
            return new List<CountResult>();
        }

        List<CountResult> queryResults;

        // Determine if we are getting ALL or being limited by start and end dates.
        if (startDate != DateHelper.DummyDateTime && endDate != DateHelper.DummyDateTime)
        {
            // Get records from database for category 'key' between dates
            queryResults =
                DiscordConnectorPlugin.StaticDatabase.CountRecordsBetweenDatesGrouped(key, startDate, endDate);
        }
        else
        {
            // Get all records from database for category 'key'
            queryResults = DiscordConnectorPlugin.StaticDatabase.CountAllRecordsGrouped(key);
        }

        // Check if we are debugging all database calls, and print debug
        if (DiscordConnectorPlugin.StaticConfig.DebugDatabaseMethods)
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug(
                $"TopNResultForCategory {key} n={n}, results={queryResults.Count}");
        }

        // If the amount of results is 0, no need to process further, just return.
        if (queryResults.Count == 0)
        {
            return queryResults;
        }

        // Perform sorting of results
        queryResults.Sort(CountResult.CompareByCount); // sorts lowest to highest
        queryResults.Reverse(); // Now high --> low

        // If the amount of results is less than the number desired, just return it
        if (queryResults.Count <= n)
        {
            return queryResults;
        }

        // Return results limited ot the number desired
        return queryResults.GetRange(0, n);
    }

    /// <summary>
    ///     Retrieve the bottom <paramref name="n" /> players for the <paramref name="key" /> category.
    ///     This returns the values in ascending (least to most) order.
    ///     Returns an empty list if the provided <paramref name="key" /> is invalid.
    /// </summary>
    /// <param name="key">One of <see cref="Categories.All" /></param>
    /// <param name="n">Number of results to return</param>
    /// <returns>A list of <paramref name="n" /> players and their totals for the <paramref name="key" />, in ascending order.</returns>
    public static List<CountResult> BottomNResultForCategory(string key, int n)
    {
        return BottomNResultForCategory(key, n, DateHelper.DummyDateTime, DateHelper.DummyDateTime);
    }

    /// <summary>
    ///     Retrieve the bottom <paramref name="n" /> players for the <paramref name="key" /> category.
    ///     This returns the values in ascending (least to most) order.
    /// </summary>
    /// <param name="key">One of <see cref="Categories.All" /></param>
    /// <param name="n">Number of results to return</param>
    /// <param name="startDate">Earliest valid date for the stat records used to gather the results</param>
    /// <param name="endDate">Latest valid date for the stat records used to gather the results</param>
    /// <returns>A list of <paramref name="n" /> players and their totals for the <paramref name="key" />, in ascending order.</returns>
    public static List<CountResult> BottomNResultForCategory(string key, int n, DateTime startDate, DateTime endDate)
    {
        // Validate key
        if (Array.IndexOf<string>(Categories.All, key) == -1)
        {
            DiscordConnectorPlugin.StaticLogger.LogWarning($"Invalid key \"{key}\" when getting bottom {n} results.");
            DiscordConnectorPlugin.StaticLogger.LogDebug("Empty list returned because of invalid key.");
            return new List<CountResult>();
        }

        List<CountResult> queryResults;

        // Determine if we are getting ALL or being limited by start and end dates.
        if (startDate != DateHelper.DummyDateTime && endDate != DateHelper.DummyDateTime)
        {
            // Get records from database for category 'key' between dates
            queryResults =
                DiscordConnectorPlugin.StaticDatabase.CountRecordsBetweenDatesGrouped(key, startDate, endDate);
        }
        else
        {
            // Get all records from database for category 'key'
            queryResults = DiscordConnectorPlugin.StaticDatabase.CountAllRecordsGrouped(key);
        }

        // Check if we are debugging all database calls, and print debug
        if (DiscordConnectorPlugin.StaticConfig.DebugDatabaseMethods)
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug(
                $"BottomNResultForCategory {key} n={n}, results={queryResults.Count}");
        }

        // If the amount of results is 0, no need to process further, just return.
        if (queryResults.Count == 0)
        {
            return queryResults;
        }

        // Perform sorting of results
        queryResults.Sort(CountResult.CompareByCount); // sorts lowest to highest

        // If the amount of results is less than the number desired, just return it
        if (queryResults.Count <= n)
        {
            return queryResults;
        }

        // Return results limited ot the number desired
        return queryResults.GetRange(0, n);
    }

    /// <summary>
    ///     Query the database for a count of all values for the category <paramref name="key" />, limited to
    ///     <paramref name="timeRange" />
    /// </summary>
    /// <param name="key">Which category to get count from</param>
    /// <param name="timeRange">Time range to restrict count to</param>
    /// <returns>Number of records that fit category within the time range</returns>
    internal static int CountUniquePlayers(string key, TimeRange timeRange)
    {
        // Validate key
        if (Array.IndexOf<string>(Categories.All, key) == -1)
        {
            DiscordConnectorPlugin.StaticLogger.LogWarning($"Invalid key \"{key}\" when getting total unique players.");
            DiscordConnectorPlugin.StaticLogger.LogDebug("Zero returned because of invalid key.");
            return 0;
        }

        // Simplify the all time record gathering
        if (timeRange == TimeRange.AllTime)
        {
            List<CountResult> results = DiscordConnectorPlugin.StaticDatabase.CountAllRecordsGrouped(key);
            return results.Count;
        }

        // All others expand out the time to a range for querying
        Tuple<DateTime, DateTime> dates = DateHelper.StartEndDatesForTimeRange(timeRange);
        return CountUniquePlayers(key, dates.Item1, dates.Item2);
    }

    /// <summary>
    ///     Query the database for a count of all values for the category <paramref name="key" />, limited to
    ///     <paramref name="timeRange" />
    /// </summary>
    /// <param name="key">Which category to get count from</param>
    /// <param name="startDate">Earliest valid date for the stat records used to gather the results</param>
    /// <param name="endDate">Latest valid date for the stat records used to gather the results</param>
    /// <returns>Number of records that fit category within the time range</returns>
    internal static int CountUniquePlayers(string key, DateTime startDate, DateTime endDate)
    {
        // Validate key
        if (Array.IndexOf<string>(Categories.All, key) == -1)
        {
            DiscordConnectorPlugin.StaticLogger.LogWarning($"Invalid key \"{key}\" when getting total unique players.");
            DiscordConnectorPlugin.StaticLogger.LogDebug("Zero returned because of invalid key.");
            return 0;
        }

        List<CountResult> allCounted =
            DiscordConnectorPlugin.StaticDatabase.CountRecordsBetweenDatesGrouped(key, startDate, endDate);

        return allCounted.Count;
    }
}
