
using System.Collections.Generic;
using LiteDB;

namespace DiscordConnector.Records
{
    /// <summary>
    /// Holds the name of the collection and value for it.
    /// </summary>
    public class CountResult
    {
        public string Name { get; }
        public int Count { get; }

        [BsonCtor]
        public CountResult(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public static int CompareByCount(CountResult cr1, CountResult cr2)
        {
            return cr1.Count.CompareTo(cr2.Count);
        }

        public static int CompareByName(CountResult cr1, CountResult cr2)
        {
            return cr1.Name.CompareTo(cr2.Name);
        }

        /// <summary>
        /// Converts a list of BSON documents with "player" and "count" values into our CountResult value.
        /// </summary>
        /// <param name="bsonDocuments">List of BSON with player and count values.</param>
        /// <returns>List of count results</returns>
        public static List<CountResult> ConvertFromBsonDocuments(List<BsonDocument> bsonDocuments)
        {
            List<CountResult> results = new List<CountResult>();

            if (Plugin.StaticConfig.DebugDatabaseMethods) { Plugin.StaticLogger.LogDebug($"ConvertBsonDocumentCountToDotNet r={bsonDocuments.Count}"); }
            foreach (BsonDocument doc in bsonDocuments)
            {
                if (!doc.ContainsKey("Count"))
                {
                    continue;
                }

                if (doc.ContainsKey("Name"))
                {
                    results.Add(new CountResult(
                        doc["Name"].AsString,
                        doc["Count"].AsInt32
                    ));
                }
                else if (doc.ContainsKey("NamePlayer"))
                {
                    results.Add(new CountResult(
                        doc["NamePlayer"]["Name"].AsString,
                        doc["Count"].AsInt32
                    ));
                }
                else if (doc.ContainsKey("Player"))
                {
                    if (!doc["Player"].IsNull)
                    {
                        results.Add(new CountResult(
                            Plugin.StaticDatabase.GetLatestCharacterNameForPlayer(doc["Player"]),
                            doc["Count"].AsInt32
                        ));
                    }
                }
            }
            return results;
        }
    }
}
