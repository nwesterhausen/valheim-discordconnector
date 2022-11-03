
using System.Collections.Generic;
using LiteDB;
using UnityEngine;

namespace DiscordConnector.Records
{
    internal class Database
    {
        private const string DB_NAME = $"{PluginInfo.PLUGIN_ID}-records.db";
        private static string DbPath;
        private LiteDatabase db;
        private ILiteCollection<SimpleStat> DeathCollection;
        private ILiteCollection<SimpleStat> JoinCollection;
        private ILiteCollection<SimpleStat> LeaveCollection;
        private ILiteCollection<SimpleStat> ShoutCollection;
        private ILiteCollection<SimpleStat> PingCollection;

        public Database(string rootStorePath)
        {
            DbPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, DB_NAME);

            Initialize();
        }

        public void Initialize()
        {
            try
            {
                db = new LiteDatabase(DbPath);
                Plugin.StaticLogger.LogDebug($"LiteDB Connection Established to {DbPath}");
                DeathCollection = db.GetCollection<SimpleStat>("deaths");
                JoinCollection = db.GetCollection<SimpleStat>("joins");
                LeaveCollection = db.GetCollection<SimpleStat>("leaves");
                ShoutCollection = db.GetCollection<SimpleStat>("shouts");
                PingCollection = db.GetCollection<SimpleStat>("pings");
            }
            catch (System.IO.IOException e)
            {
                Plugin.StaticLogger.LogError($"Unable to acquire un-shared access to {DbPath}");
                Plugin.StaticLogger.LogDebug(e);
            }
        }

        public void Dispose()
        {
            Plugin.StaticLogger.LogDebug("Closing LiteDB connection");
            db.Dispose();
        }

        private void InsertSimpleStatRecord(ILiteCollection<SimpleStat> collection, string playerName, string playerHostName, Vector3 pos)
        {
            var newRecord = new SimpleStat(
                playerName,
                playerHostName,
                pos.x, pos.y, pos.z
            );
            collection.Insert(newRecord);

            collection.EnsureIndex(x => x.Name);
            collection.EnsureIndex(x => x.PlayerId);
        }
        private void InsertSimpleStatRecord(ILiteCollection<SimpleStat> collection, string playerName, string playerHostName)
        {
            InsertSimpleStatRecord(collection, playerName, playerHostName, Vector3.zero);
        }

        private int CountOfRecordsByName(ILiteCollection<SimpleStat> collection, string playerName)
        {
            return collection.Query()
                .Where(x => x.Name.Equals(playerName))
                .Count();
        }

        private int CountOfRecordsByCharacterId(ILiteCollection<SimpleStat> collection, string playerHostName)
        {
            return collection.Query()
                .Where(x => x.PlayerId == playerHostName)
                .Count();
        }

        private int CountOfRecordsByNameAndCharacterId(ILiteCollection<SimpleStat> collection, string playerName, string playerHostName)
        {
            return collection.Query()
                .Where(x => (x.Name.Equals(playerName) && x.PlayerId == playerHostName))
                .Count();
        }

        private List<CountResult> CountAllRecordsGrouped(ILiteCollection<SimpleStat> collection)
        {
            if (Plugin.StaticConfig.DebugDatabaseMethods) { Plugin.StaticLogger.LogDebug($"CountAllRecordsGrouped {Plugin.StaticConfig.RecordRetrievalDiscernmentMethod}"); }
            //! if treat name as character, each name combined regardless
            if (Plugin.StaticConfig.RecordRetrievalDiscernmentMethod.Equals(Config.RetrievalDiscernmentMethods.ByName))
            {
                return ConvertBsonDocumentCountToDotNet(
                    collection.Query()
                        .GroupBy("Name")
                        .Select("{Name: @Key, Count: COUNT(*)}")
                        .ToList()
                );
            }
            else if (Plugin.StaticConfig.RecordRetrievalDiscernmentMethod.Equals(Config.RetrievalDiscernmentMethods.ByNameAndSteamID))
            {
                return ConvertBsonDocumentCountToDotNet(
                    collection.Query()
                        .GroupBy("{Name,PlayerId}")
                        .Select("{NamePlayer: @Key, Count: COUNT(*)}")
                        .ToList()
                );
            }
            else // Leaving only Config.RetrievalDiscernmentMethods.BySteamID
            {

                return ConvertBsonDocumentCountToDotNet(
                    collection.Query()
                        .GroupBy("PlayerId")
                        .Select("{Player: @Key, Count: COUNT(*)}")
                        .ToList()
                );
            }
        }

        private string GetLatestNameForCharacterId(string playerHostName)
        {
            if (Plugin.StaticConfig.DebugDatabaseMethods) { Plugin.StaticLogger.LogDebug($"GetLatestNameForCharacterId {playerHostName} begin"); }
            var nameQuery = JoinCollection.Query()
                .Where(x => x.PlayerId == playerHostName)
                .OrderByDescending("Date")
                .Select("$.Name")
                .ToList();
            if (nameQuery.Count == 0)
            {
                if (Plugin.StaticConfig.DebugDatabaseMethods) { Plugin.StaticLogger.LogDebug($"GetLatestNameForCharacterId {playerHostName} result = NONE"); }
                return "undefined";
            }
            if (Plugin.StaticConfig.DebugDatabaseMethods) { Plugin.StaticLogger.LogDebug($"nameQuery has {nameQuery.Count} results"); }
            var result = nameQuery[0];
            if (Plugin.StaticConfig.DebugDatabaseMethods) { Plugin.StaticLogger.LogDebug($"GetLatestNameForCharacterId {playerHostName} result = {result}"); }
            return result["Name"].AsString;
        }

        private List<CountResult> ConvertBsonDocumentCountToDotNet(List<BsonDocument> bsonDocuments)
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
                            GetLatestNameForCharacterId(doc["Player"]),
                            doc["Count"].AsInt32
                        ));
                    }
                }
            }
            return results;
        }

        public List<CountResult> CountAllRecordsGrouped(string key)
        {
            switch (key)
            {
                case Categories.Death:
                    return CountAllRecordsGrouped(DeathCollection);
                case Categories.Join:
                    return CountAllRecordsGrouped(JoinCollection);
                case Categories.Leave:
                    return CountAllRecordsGrouped(LeaveCollection);
                case Categories.Ping:
                    return CountAllRecordsGrouped(PingCollection);
                case Categories.Shout:
                    return CountAllRecordsGrouped(ShoutCollection);
                default:
                    Plugin.StaticLogger.LogDebug($"RetrieveAllRecordsGroupByName, invalid key '{key}'");
                    return new List<CountResult>();
            }
        }

        public int CountOfRecordsByName(string key, string playerName)
        {
            if (!Plugin.StaticConfig.CollectStatsEnabled)
            {
                return -1;
            }
            switch (key)
            {
                case Categories.Death:
                    return CountOfRecordsByName(DeathCollection, playerName);
                case Categories.Join:
                    return CountOfRecordsByName(JoinCollection, playerName);
                case Categories.Leave:
                    return CountOfRecordsByName(LeaveCollection, playerName);
                case Categories.Ping:
                    return CountOfRecordsByName(PingCollection, playerName);
                case Categories.Shout:
                    return CountOfRecordsByName(ShoutCollection, playerName);
                default:
                    Plugin.StaticLogger.LogDebug($"CountOfRecordsBySteamId, invalid key '{key}'");
                    return -2;
            }
        }

        public int CountOfRecordsBySteamId(string key, string playerHostName)
        {
            if (!Plugin.StaticConfig.CollectStatsEnabled)
            {
                return -1;
            }
            switch (key)
            {
                case Categories.Death:
                    return CountOfRecordsByCharacterId(DeathCollection, playerHostName);
                case Categories.Join:
                    return CountOfRecordsByCharacterId(JoinCollection, playerHostName);
                case Categories.Leave:
                    return CountOfRecordsByCharacterId(LeaveCollection, playerHostName);
                case Categories.Ping:
                    return CountOfRecordsByCharacterId(PingCollection, playerHostName);
                case Categories.Shout:
                    return CountOfRecordsByCharacterId(ShoutCollection, playerHostName);
                default:
                    Plugin.StaticLogger.LogDebug($"CountOfRecordsBySteamId, invalid key '{key}'");
                    return -2;
            }
        }

        public void InsertSimpleStatRecord(string key, string playerName, string playerHostName, Vector3 pos)
        {

            switch (key)
            {
                case Categories.Death:
                    InsertSimpleStatRecord(DeathCollection, playerName, playerHostName, pos);
                    break;
                case Categories.Join:
                    InsertSimpleStatRecord(JoinCollection, playerName, playerHostName, pos);
                    break;
                case Categories.Leave:
                    InsertSimpleStatRecord(LeaveCollection, playerName, playerHostName, pos);
                    break;
                case Categories.Ping:
                    InsertSimpleStatRecord(PingCollection, playerName, playerHostName, pos);
                    break;
                case Categories.Shout:
                    InsertSimpleStatRecord(ShoutCollection, playerName, playerHostName, pos);
                    break;
                default:
                    Plugin.StaticLogger.LogDebug($"InsertSimpleStatRecord, invalid key '{key}'");
                    break;
            }
        }
        public void InsertSimpleStatRecord(string key, string playerName, string characterId)
        {
            InsertSimpleStatRecord(key, playerName, characterId, Vector3.zero);
        }

    }
}
