using System.Collections.Generic;

namespace DiscordConnector.Records
{
    /// <summary>
    /// These are categories used when keeping track of values in the record system. It is a simple system
    /// that currently only supports storing string:integer pairings underneath one of these categories.
    /// </summary>
    public static class Categories
    {
        public const string Death = "death";
        public const string Join = "join";
        public const string Leave = "leave";
        public const string Ping = "ping";
        public const string Shout = "shout";

        public readonly static string[] All = new string[] {
            Death,
            Join,
            Leave,
            Ping,
            Shout
        };
    }

    public static class Helper
    {
        public static List<CountResult> TopNResultForCategory(string key, int n)
        {
            List<CountResult> queryResults = Plugin.StaticDatabase.CountAllRecordsGrouped(key);
            if (Plugin.StaticConfig.DebugDatabaseMethods) { Plugin.StaticLogger.LogDebug($"TopNResultForCategory {key} n={n}, results={queryResults.Count}"); }
            if (queryResults.Count == 0)
            {
                return queryResults;
            }

            queryResults.Sort(CountResult.CompareByCount); // sorts lowest to highest
            queryResults.Reverse(); // Now high --> low

            if (queryResults.Count <= n)
            {
                return queryResults;
            }

            return queryResults.GetRange(0, n);
        }

        public static CountResult TopResultForCategory(string key)
        {
            var results = Helper.TopNResultForCategory(key, 1);
            if (results.Count == 0)
            {
                return new CountResult("", 0);
            }
            return results[0];
        }

        public static List<CountResult> BottomNResultForCategory(string key, int n)
        {

            List<CountResult> queryResults = Plugin.StaticDatabase.CountAllRecordsGrouped(key);
            if (Plugin.StaticConfig.DebugDatabaseMethods) { Plugin.StaticLogger.LogDebug($"BottomNResultForCategory {key} n={n}, results={queryResults.Count}"); }
            if (queryResults.Count == 0)
            {
                return queryResults;
            }

            queryResults.Sort(CountResult.CompareByCount); // sorts lowest to highest

            if (queryResults.Count <= n)
            {
                return queryResults;
            }

            return queryResults.GetRange(0, n);
        }

        public static CountResult BottomResultForCategory(string key)
        {
            var results = Helper.BottomNResultForCategory(key, 1);
            if (results.Count == 0)
            {
                return new CountResult("", 0);
            }
            return results[0];
        }
    }
}
