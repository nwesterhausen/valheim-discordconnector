using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DiscordConnector
{
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
    class RecordsOld
    {
        private static string DEFAULT_FILENAME = "records.json";
        private const string MIGRATED_FILENAME = "records.json.migrated";
        private string storepath, movepath;
        // private bool saveEnabled;
        private List<Record> recordCache;

        /// <summary>
        /// Creates an instance of the record keeper. It will store a json file using the default filename at <paramref name="basePath"/>.
        /// Specify <paramref name="fileName"/> to change the name of the json file from the default of "records.json"
        /// </summary>
        /// <param name="basePath">The directory to store the json file of records in.</param>
        /// <param name="fileName">The name of the file to use for storage.</param>
        public RecordsOld(string basePath, string fileName = null)
        {
            if (fileName == null)
            {
                fileName = DEFAULT_FILENAME;
            }
            storepath = Path.Combine(basePath, fileName);
            movepath = Path.Combine(basePath, MIGRATED_FILENAME);

            MigrateRecords();
        }

        private void MigrateRecords()
        {
            //! check if storePath exists..
            if (System.IO.File.Exists(storepath))
            {
                Plugin.StaticLogger.LogInfo("Migrating from discovered Records.json to LiteDB");
                //! read all records in from storePath if they exist

                try
                {
                    string jsonString = File.ReadAllText(@storepath);
                    recordCache = JsonConvert.DeserializeObject<List<Record>>(jsonString);
                    Plugin.StaticLogger.LogInfo($"Read existing stats from disk {storepath}");
                }
                catch (ArgumentNullException)
                {
                    Plugin.StaticLogger.LogWarning($"No content found when reading {storepath} to read saved records. We will start with default values for all records.");
                    Plugin.StaticLogger.LogDebug("File contained null and threw ArgumentNullException");
                    return;
                }
                catch (JsonException)
                {
                    Plugin.StaticLogger.LogError($"Unable to parse the contents of {storepath} as JSON.");
                    Plugin.StaticLogger.LogError("No records will be recorded to disk until existing file is moved, renamed, or deleted.");
                    return;
                }

                //! store records into LiteDB
                foreach (Record r in recordCache)
                {
                    int count = 0;
                    foreach (RecordValue v in r.Values)
                    {
                        for (int i = 0; i < v.Value; i++)
                        {
                            Plugin.StaticDatabase.InsertSimpleStatRecord(r.Category, v.Key, 1);
                            count++;
                        }

                    }
                    Plugin.StaticLogger.LogInfo($"Migrated {count} {r.Category} records");

                }

                //! move storePath to a new path with MIGRATED_FILENAME
                System.IO.File.Move(storepath, movepath);
                Plugin.StaticLogger.LogInfo($"Moved existing records.json to {MIGRATED_FILENAME}");
            }
            else
            {
                Plugin.StaticLogger.LogDebug("No records.json found, not migrating.");
            }
        }
    }
}
