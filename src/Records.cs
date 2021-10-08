using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordConnector
{
    internal static class Categories
    {
        internal const string Death = "death";
        internal const string Join = "join";
        internal const string Leave = "leave";
        internal const string Ping = "ping";
        internal const string Shout = "shout";

        internal static string[] All = new string[] {
            Death,
            Join,
           Leave,
            Ping,
            Shout
        };
    }
    internal class RecordValue
    {
        public string PlayerName { get; set; }
        public int Value { get; set; }
    }
    internal class Record
    {
        public string Category { get; set; }
        public List<RecordValue> Values { get; set; }
    }
    class Records
    {
        private static string filename = "records.json";
        private static string storepath;
        private List<Record> recordCache;


        public Records(string basepath)
        {
            storepath = $"{basepath}{(basepath.EndsWith($"{Path.DirectorySeparatorChar}") ? "" : Path.DirectorySeparatorChar)}{filename}";
            PopulateCache();
            if (!Plugin.StaticConfig.CollectStatsEnabled)
            {
                Plugin.StaticLogger.LogInfo("Saving stats is disabled, nothing will be recorded.");
            }
        }

        public void Store(string key, string playername, int value)
        {
            if (Plugin.StaticConfig.CollectStatsEnabled)
            {
                if (Array.IndexOf<string>(Categories.All, key) >= 0)
                {
                    foreach (Record r in recordCache)
                    {
                        if (r.Category.Equals(key))
                        {
                            bool stored = false;
                            foreach (RecordValue v in r.Values)
                            {
                                if (v.PlayerName.Equals(playername))
                                {
                                    v.Value += value;
                                    stored = true;
                                }
                            }
                            if (!stored)
                            {
                                r.Values.Add(new RecordValue()
                                {
                                    PlayerName = playername,
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
        public int Retrieve(string key, string playername)
        {
            if (!Plugin.StaticConfig.CollectStatsEnabled)
            {
                return -1;
            }
            if (Array.IndexOf<string>(Categories.All, key) >= 0)
            {
                foreach (Record r in recordCache)
                {
                    if (r.Category.Equals(key))
                    {
                        foreach (RecordValue v in r.Values)
                        {
                            if (v.PlayerName.Equals(playername))
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

        public Tuple<string, int> Retrieve(string key)
        {
            if (!Plugin.StaticConfig.CollectStatsEnabled)
            {
                return Tuple.Create("not allowed", -1);
            }

            if (Array.IndexOf<string>(Categories.All, key) >= 0)
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
                                player = v.PlayerName;
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

        private async Task FlushCache()
        {
            if (Plugin.StaticConfig.CollectStatsEnabled)
            {
                string jsonString = JsonSerializer.Serialize(recordCache);

                using (var stream = new StreamWriter(@storepath, false))
                {
                    await stream.WriteAsync(jsonString);
                }

                Plugin.StaticLogger.LogDebug($"Flushed cached stats to {storepath}");
            }
        }

        private void PopulateCache()
        {
            if (File.Exists(storepath))
            {
                string jsonString = File.ReadAllText(@storepath);
                recordCache = JsonSerializer.Deserialize<List<Record>>(jsonString);
                Plugin.StaticLogger.LogInfo($"Read existing stats from disk {storepath}");
            }
            else
            {
                Plugin.StaticLogger.LogInfo($"Unable to find existing stats data at {storepath}. Creating new {filename}");
                recordCache = new List<Record>();
                foreach (string category in Categories.All)
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
}
