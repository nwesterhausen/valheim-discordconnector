
using System.Collections.Generic;
using LiteDB;
using UnityEngine;

namespace DiscordConnector.Records
{
    internal class Database
    {
        private const string DB_NAME = "records.db";
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
            DbPath = System.IO.Path.Combine(rootStorePath, DB_NAME);
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

        private void InsertSimpleStatRecord(ILiteCollection<SimpleStat> collection, string playerName, ulong steamId, Vector3 pos)
        {
            var newRecord = new SimpleStat(
                playerName,
                steamId,
                pos.x, pos.y, pos.z
            );
            collection.Insert(newRecord);

            collection.EnsureIndex(x => x.Name);
            collection.EnsureIndex(x => x.SteamId);
        }
        private void InsertSimpleStatRecord(ILiteCollection<SimpleStat> collection, string playerName, ulong steamId)
        {
            InsertSimpleStatRecord(collection, playerName, steamId, Vector3.zero);
        }

        private int CountOfRecordsByName(ILiteCollection<SimpleStat> collection, string playerName)
        {
            return collection.Query()
                .Where(x => x.Name.Equals(playerName))
                .Count();
        }

        private int CountOfRecordsBySteamId(ILiteCollection<SimpleStat> collection, ulong steamId)
        {
            return collection.Query()
                .Where(x => x.SteamId == steamId)
                .Count();
        }

        private int CountOfRecordsByNameAndSteamId(ILiteCollection<SimpleStat> collection, string playerName, ulong steamId)
        {
            return collection.Query()
                .Where(x => (x.Name.Equals(playerName) && x.SteamId == steamId))
                .Count();
        }

        private List<CountResult> CountAllRecordsGrouped(ILiteCollection<SimpleStat> collection)
        {
            Plugin.StaticLogger.LogDebug($"CountAllRecordsGrouped {Plugin.StaticConfig.RecordRetrievalDiscernmentMethod}");
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
                        .GroupBy("{Name,SteamId}")
                        .Select("{NameSteam: @Key, Count: COUNT(*)}")
                        .ToList()
                );
            }
            else // Leaving only Config.RetrievalDiscernmentMethods.BySteamID
            {

                return ConvertBsonDocumentCountToDotNet(
                    collection.Query()
                        .GroupBy("SteamId")
                        .Select("{Steam: @Key, Count: COUNT(*)}")
                        .ToList()
                );
            }
        }

        private string GetLatestNameForSteamId(ulong steamId)
        {
            Plugin.StaticLogger.LogDebug($"GetLatestNameForSteamId {steamId} begin");
            var nameQuery = JoinCollection.Query()
                .Where(x => x.SteamId == steamId)
                .OrderByDescending("Date");
            if (nameQuery.Count() == 0)
            {
                Plugin.StaticLogger.LogDebug($"GetLatestNameForSteamId {steamId} result = NONE");
                return "undefined";
            }
            SimpleStat result = nameQuery
                .First();
            Plugin.StaticLogger.LogDebug($"GetLatestNameForSteamId {steamId} result = {result}");
            return result.Name;
        }

        private List<CountResult> ConvertBsonDocumentCountToDotNet(List<BsonDocument> bsonDocuments)
        {
            List<CountResult> results = new List<CountResult>();

            Plugin.StaticLogger.LogDebug($"ConvertBsonDocumentCountToDotNet r={bsonDocuments.Count}");
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
                else if (doc.ContainsKey("NameSteam"))
                {
                    results.Add(new CountResult(
                        doc["NameSteam"]["Name"].AsString,
                        doc["Count"].AsInt32
                    ));
                }
                else if (doc.ContainsKey("Steam"))
                {
                    if (doc["Steam"].AsInt64 >= STEAMID_LOWERBOUND)
                    {
                        results.Add(new CountResult(
                            GetLatestNameForSteamId((ulong)doc["Steam"].AsInt64),
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

        public int CountOfRecordsBySteamId(string key, ulong steamId)
        {
            if (!Plugin.StaticConfig.CollectStatsEnabled)
            {
                return -1;
            }
            switch (key)
            {
                case Categories.Death:
                    return CountOfRecordsBySteamId(DeathCollection, steamId);
                case Categories.Join:
                    return CountOfRecordsBySteamId(JoinCollection, steamId);
                case Categories.Leave:
                    return CountOfRecordsBySteamId(LeaveCollection, steamId);
                case Categories.Ping:
                    return CountOfRecordsBySteamId(PingCollection, steamId);
                case Categories.Shout:
                    return CountOfRecordsBySteamId(ShoutCollection, steamId);
                default:
                    Plugin.StaticLogger.LogDebug($"CountOfRecordsBySteamId, invalid key '{key}'");
                    return -2;
            }
        }

        public void InsertSimpleStatRecord(string key, string playerName, ulong steamId, Vector3 pos)
        {

            switch (key)
            {
                case Categories.Death:
                    InsertSimpleStatRecord(DeathCollection, playerName, steamId, pos);
                    break;
                case Categories.Join:
                    InsertSimpleStatRecord(JoinCollection, playerName, steamId, pos);
                    break;
                case Categories.Leave:
                    InsertSimpleStatRecord(LeaveCollection, playerName, steamId, pos);
                    break;
                case Categories.Ping:
                    InsertSimpleStatRecord(PingCollection, playerName, steamId, pos);
                    break;
                case Categories.Shout:
                    InsertSimpleStatRecord(ShoutCollection, playerName, steamId, pos);
                    break;
                default:
                    Plugin.StaticLogger.LogDebug($"InsertSimpleStatRecord, invalid key '{key}'");
                    break;
            }
        }
        public void InsertSimpleStatRecord(string key, string playerName, ulong steamId)
        {
            InsertSimpleStatRecord(key, playerName, steamId, Vector3.zero);
        }

    }
}
