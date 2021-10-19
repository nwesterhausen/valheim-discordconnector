using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordConnector
{
    /// <summary>
    /// These are categories used when keeping track of values in the record system. It is a simple system
    /// that currently only supports storing string:integer pairings underneath one of these categories.
    /// </summary>
    public static class RecordCategories
    {
        public const string Death = "death";
        public const string Join = "join";
        public const string Leave = "leave";
        public const string Ping = "ping";
        public const string Shout = "shout";

        public static string[] All = new string[] {
            Death,
            Join,
            Leave,
            Ping,
            Shout
        };
    }
    /// <summary>
    /// The individual key:value pairings used in the record system. It supports only string:int pairings.
    /// </summary>
    internal class RecordValue
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }
    /// <summary>
    /// A collation of zero to many key:value pairings to a single category. 
    /// </summary>
    internal class Record
    {
        public string Category { get; set; }
        public List<RecordValue> Values { get; set; }
    }
    class Records
    {
        private static string DEFAULT_FILENAME = "records.json";
        private string storepath;
        private bool saveEnabled;
        private List<Record> recordCache;

        /// <summary>
        /// Creates an instance of the record keeper. It will store a json file using the default filename at <paramref name="basePath"/>.
        /// Specify <paramref name="fileName"/> to change the name of the json file from the default of "records.json"
        /// </summary>
        /// <param name="basePath">The directory to store the json file of records in.</param>
        /// <param name="fileName">The name of the file to use for storage.</param>
        public Records(string basePath, string fileName = null)
        {
            if (fileName == null)
            {
                fileName = DEFAULT_FILENAME;
            }
            storepath = Path.Combine(basePath, fileName);
            saveEnabled = true;
            PopulateCache();
            if (!Plugin.StaticConfig.CollectStatsEnabled)
            {
                Plugin.StaticLogger.LogInfo("Saving stats is disabled, nothing will be recorded.");
            }
        }

        /// <summary>
        /// Add <paramref name="value"/> to a record for <paramref name="playername"/> under <paramref name="key"/> in the records database. 
        /// This will not save the record if the <paramref name="key"/> is not one defined in RecordCategories.
        /// </summary>
        /// <param name="key">RecordCategories category to store the value under</param>
        /// <param name="playername">The player's name.</param>
        /// <param name="value">How much to increase current stored value by.</param>
        public void Store(string key, string playername, int value)
        {
            if (Plugin.StaticConfig.CollectStatsEnabled)
            {
                if (Array.IndexOf<string>(RecordCategories.All, key) >= 0)
                {
                    foreach (Record r in recordCache)
                    {
                        if (r.Category.Equals(key))
                        {
                            bool stored = false;
                            foreach (RecordValue v in r.Values)
                            {
                                if (v.Key.Equals(playername))
                                {
                                    v.Value += value;
                                    stored = true;
                                }
                            }
                            if (!stored)
                            {
                                r.Values.Add(new RecordValue()
                                {
                                    Key = playername,
                                    Value = value
                                });
                            }
                        }
                    }
                    // After adding new data, flush data to disk.
                    FlushCache().ContinueWith(
                        t => Plugin.StaticLogger.LogWarning(t.Exception),
                        TaskContinuationOptions.OnlyOnFaulted);
                }
                else
                {
                    Plugin.StaticLogger.LogWarning($"Unable to store record of {key} for player {playername} - not considered a valid category.");
                }
            }
        }
        /// <summary>
        /// Get the value stored under <paramref name="key"/> at <paramref name="playername"/>.
        /// </summary>
        /// <param name="key">The RecordCategories category the value is stored under</param>
        /// <param name="playername">The name of the player</param>
        /// <returns>This will return 0 if there is no record found for that player. It will return -1 if the category is invalid.</returns>
        public int Retrieve(string key, string playername)
        {
            if (!Plugin.StaticConfig.CollectStatsEnabled)
            {
                return -1;
            }
            if (Array.IndexOf<string>(RecordCategories.All, key) >= 0)
            {
                foreach (Record r in recordCache)
                {
                    if (r.Category.Equals(key))
                    {
                        foreach (RecordValue v in r.Values)
                        {
                            if (v.Key.Equals(playername))
                            {
                                return v.Value;
                            }
                        }
                    }
                }
            }
            Plugin.StaticLogger.LogWarning($"No stored record for player {playername} under {key}, returning default of 0.");
            return 0;
        }

        /// <summary>
        /// Retrieve all stored values under <paramref name="key"/>.
        /// </summary>
        /// <param name="key">RecordCategories category to retrieve stored values from</param>
        /// <returns>A list of (playername, value) tuples. The list will have length 0 if there are no stored records.</returns>
        public Tuple<string, int> Retrieve(string key)
        {
            if (!Plugin.StaticConfig.CollectStatsEnabled)
            {
                return Tuple.Create("not allowed", -1);
            }

            if (Array.IndexOf<string>(RecordCategories.All, key) >= 0)
            {
                string player = "no result";
                int records = -1;
                foreach (Record r in recordCache)
                {
                    if (r.Category.Equals(key))
                    {
                        foreach (RecordValue v in r.Values)
                        {
                            if (v.Value > records)
                            {
                                player = v.Key;
                                records = v.Value;
                            }
                        }
                    }
                }
                return Tuple.Create(player, records);
            }
            else
            {
                return Tuple.Create($"not recording for {key}", -1);
            }
        }

        /// <summary>
        /// (Asynchronous) Writes the in-memory cache of records to disk. 
        /// </summary>
        private async Task FlushCache()
        {
            if (!saveEnabled)
            {
                Plugin.StaticLogger.LogDebug("Saving records is disabled due to an error at load time.");
                return;
            }
            if (Plugin.StaticConfig.CollectStatsEnabled)
            {
                string jsonString = JsonConvert.SerializeObject(recordCache);

                using (var stream = new StreamWriter(@storepath, false))
                {
                    await stream.WriteAsync(jsonString);
                }

                Plugin.StaticLogger.LogDebug($"Flushed cached stats to {storepath}");
            }
        }

        /// <summary>
        /// Builds the in-memory cache by reading from disk.
        /// </summary>
        private void PopulateCache()
        {
            if (File.Exists(storepath))
            {
                string jsonString = File.ReadAllText(@storepath);
                try
                {
                    recordCache = JsonConvert.DeserializeObject<List<Record>>(jsonString);
                    Plugin.StaticLogger.LogInfo($"Read existing stats from disk {storepath}");
                }
                catch (ArgumentNullException)
                {
                    Plugin.StaticLogger.LogWarning($"No content found when reading {storepath} to read saved records. We will start with default values for all records.");
                    Plugin.StaticLogger.LogDebug("File contained null and threw ArgumentNullException");
                    InitializeEmptyCache();
                }
                catch (JsonException)
                {
                    Plugin.StaticLogger.LogError($"Unable to parse the contents of {storepath} as JSON.");
                    Plugin.StaticLogger.LogError("No records will be recorded to disk until existing file is moved, renamed, or deleted.");
                    saveEnabled = false;
                }
            }
            else
            {
                Plugin.StaticLogger.LogInfo($"Unable to find existing stats data at {storepath}. Creating new {DEFAULT_FILENAME}");
                InitializeEmptyCache();
            }
        }

        private void InitializeEmptyCache()
        {
            recordCache = new List<Record>();
            foreach (string category in RecordCategories.All)
            {
                recordCache.Add(new Record
                {
                    Category = category,
                    Values = new List<RecordValue>()
                });
            }
            FlushCache().ContinueWith(
                t => Plugin.StaticLogger.LogWarning(t.Exception),
                TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
