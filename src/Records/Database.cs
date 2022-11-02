
using System.Collections.Generic;
using System.IO;
using LiteDB;
using UnityEngine;

namespace DiscordConnector.Records
{
    internal class Database
    {
        private const string OLD_DB_NAME = "records.db";
        private const string DB_NAME = $"{PluginInfo.PLUGIN_ID}-records2.db";
        private static string DbPath;
        private LiteDatabase db;
        private ILiteCollection<SimpleStat> DeathCollection;
        private ILiteCollection<SimpleStat> JoinCollection;
        private ILiteCollection<SimpleStat> LeaveCollection;
        private ILiteCollection<SimpleStat> ShoutCollection;
        private ILiteCollection<SimpleStat> PingCollection;

        private const long STEAMID_LOWERBOUND = 100;

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

        private void InsertSimpleStatRecord(ILiteCollection<SimpleStat> collection, string playerName, string characterId, Vector3 pos)
        {
            var newRecord = new SimpleStat(
                playerName,
                characterId,
                pos.x, pos.y, pos.z
            );
            collection.Insert(newRecord);

            collection.EnsureIndex(x => x.Name);
            collection.EnsureIndex(x => x.CharacterId);
        }
        private void InsertSimpleStatRecord(ILiteCollection<SimpleStat> collection, string playerName, string characterId)
        {
            InsertSimpleStatRecord(collection, playerName, characterId, Vector3.zero);
        }

        private int CountOfRecordsByName(ILiteCollection<SimpleStat> collection, string playerName)
        {
            return collection.Query()
                .Where(x => x.Name.Equals(playerName))
                .Count();
        }

        private int CountOfRecordsByCharacterId(ILiteCollection<SimpleStat> collection, string characterId)
        {
            return collection.Query()
                .Where(x => x.CharacterId == characterId)
                .Count();
        }

        private int CountOfRecordsByNameAndCharacterId(ILiteCollection<SimpleStat> collection, string playerName, string characterId)
        {
            return collection.Query()
                .Where(x => (x.Name.Equals(playerName) && x.CharacterId == characterId))
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
                        .GroupBy("{Name,CharacterId}")
                        .Select("{NameCharacter: @Key, Count: COUNT(*)}")
                        .ToList()
                );
            }
            else // Leaving only Config.RetrievalDiscernmentMethods.BySteamID
            {

                return ConvertBsonDocumentCountToDotNet(
                    collection.Query()
                        .GroupBy("CharacterId")
                        .Select("{CharacterId: @Key, Count: COUNT(*)}")
                        .ToList()
                );
            }
        }

        private string GetLatestNameForCharacterId(string characterId)
        {
            if (Plugin.StaticConfig.DebugDatabaseMethods) { Plugin.StaticLogger.LogDebug($"GetLatestNameForCharacterId {characterId} begin"); }
            var nameQuery = JoinCollection.Query()
                .Where(x => x.CharacterId == characterId)
                .OrderByDescending("Date")
                .Select("$.Name")
                .ToList();
            if (nameQuery.Count == 0)
            {
                if (Plugin.StaticConfig.DebugDatabaseMethods) { Plugin.StaticLogger.LogDebug($"GetLatestNameForCharacterId {characterId} result = NONE"); }
                return "undefined";
            }
            if (Plugin.StaticConfig.DebugDatabaseMethods) { Plugin.StaticLogger.LogDebug($"nameQuery has {nameQuery.Count} results"); }
            var result = nameQuery[0];
            if (Plugin.StaticConfig.DebugDatabaseMethods) { Plugin.StaticLogger.LogDebug($"GetLatestNameForCharacterId {characterId} result = {result}"); }
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
                else if (doc.ContainsKey("NameCharacter"))
                {
                    results.Add(new CountResult(
                        doc["NameCharacter"]["Name"].AsString,
                        doc["Count"].AsInt32
                    ));
                }
                else if (doc.ContainsKey("CharacterId"))
                {
                    if (!doc["CharacterId"].IsNull)
                    {
                        results.Add(new CountResult(
                            GetLatestNameForCharacterId(doc["CharacterId"]),
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

        public int CountOfRecordsBySteamId(string key, string characterId)
        {
            if (!Plugin.StaticConfig.CollectStatsEnabled)
            {
                return -1;
            }
            switch (key)
            {
                case Categories.Death:
                    return CountOfRecordsByCharacterId(DeathCollection, characterId);
                case Categories.Join:
                    return CountOfRecordsByCharacterId(JoinCollection, characterId);
                case Categories.Leave:
                    return CountOfRecordsByCharacterId(LeaveCollection, characterId);
                case Categories.Ping:
                    return CountOfRecordsByCharacterId(PingCollection, characterId);
                case Categories.Shout:
                    return CountOfRecordsByCharacterId(ShoutCollection, characterId);
                default:
                    Plugin.StaticLogger.LogDebug($"CountOfRecordsBySteamId, invalid key '{key}'");
                    return -2;
            }
        }

        public void InsertSimpleStatRecord(string key, string playerName, string characterId, Vector3 pos)
        {

            switch (key)
            {
                case Categories.Death:
                    InsertSimpleStatRecord(DeathCollection, playerName, characterId, pos);
                    break;
                case Categories.Join:
                    InsertSimpleStatRecord(JoinCollection, playerName, characterId, pos);
                    break;
                case Categories.Leave:
                    InsertSimpleStatRecord(LeaveCollection, playerName, characterId, pos);
                    break;
                case Categories.Ping:
                    InsertSimpleStatRecord(PingCollection, playerName, characterId, pos);
                    break;
                case Categories.Shout:
                    InsertSimpleStatRecord(ShoutCollection, playerName, characterId, pos);
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
