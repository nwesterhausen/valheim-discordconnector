using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

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
        internal static string filename = "records.json";
        internal static string storepath;
        internal List<Record> recordCache;
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

        //TODO: WriteCache to disk

        //TODO: Read file from disk to populate initial cache
    }
}