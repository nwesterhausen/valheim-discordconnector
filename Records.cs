using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordConnector
{
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
        public static string[] CATEGORIES = new string[] {
            "death",
            "join",
            "leave",
            "ping",
            "shout"
        };

        public Records(string basepath)
        {
            storepath = $"{basepath}{(basepath.EndsWith($"{Path.PathSeparator}") ? "" : Path.PathSeparator)}{filename}";
            recordCache = new List<Record>() { };
        }

        public void Store(string key, string playername, int value)
        {
            if (Array.IndexOf<string>(CATEGORIES, key) >= 0)
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
                FlushCache().Start();
            }
            else
            {
                Plugin.StaticLogger.LogWarning($"Unable to store record of {key} for player {playername} - not considered a valid category.");
            }
        }
        public int Retrieve(string key, string playername)
        {
            if (Array.IndexOf<string>(CATEGORIES, key) >= 0)
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

        private async Task FlushCache()
        {
            string jsonString = JsonSerializer.Serialize(recordCache);

            using (var stream = new StreamWriter(@storepath, false))
            {
                await stream.WriteAsync(jsonString);
            }

            Plugin.StaticLogger.LogInfo($"Flushed cached stats to {storepath}");
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
                Plugin.StaticLogger.LogInfo($"Unable to find existing stats data at {storepath}");
                recordCache = new List<Record>();
                foreach (string category in CATEGORIES)
                {
                    recordCache.Add(new Record
                    {
                        Category = category,
                        Values = new List<RecordValue>()
                    });
                }
            }
        }
    }
}