
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

        public Database(string rootStorePath)
        {
            DbPath = System.IO.Path.Combine(rootStorePath, DB_NAME);
            Initialize();
        }

        public void Initialize()
        {
            db = new LiteDatabase(DbPath);
            Plugin.StaticLogger.LogDebug($"LiteDB Connection Established to {DbPath}");
            DeathCollection = db.GetCollection<SimpleStat>("deaths");
            JoinCollection = db.GetCollection<SimpleStat>("joins");
            LeaveCollection = db.GetCollection<SimpleStat>("leaves");
            ShoutCollection = db.GetCollection<SimpleStat>("shouts");
            PingCollection = db.GetCollection<SimpleStat>("pings");
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
            return DeathCollection.Query()
                .Where(x => x.Name.Equals(playerName))
                .Count();
        }

        private int CountOfRecordsBySteamId(ILiteCollection<SimpleStat> collection, ulong steamId)
        {
            return DeathCollection.Query()
                .Where(x => x.SteamId == steamId)
                .Count();
        }

        private List<CountResult> CountAllRecordsGroupBySteamId(ILiteCollection<SimpleStat> collection)
        {
            return ConvertBsonDocumentCountToDotNet(
                collection.Query()
                    .GroupBy("SteamId")
                    .Select("{Name: SteamId, Count: COUNT(*)}")
                    .ToList()
            );
        }

        private List<CountResult> RetrieveAllRecordsGroupByName(ILiteCollection<SimpleStat> collection)
        {
            return ConvertBsonDocumentCountToDotNet(
                collection.Query()
                    .GroupBy("Name")
                    .Select("{Name: Name, Count: COUNT(*)}")
                    .ToList()
            );
        }

        private List<CountResult> ConvertBsonDocumentCountToDotNet(List<BsonDocument> bsonDocuments)
        {
            List<CountResult> results = new List<CountResult>();
            foreach (BsonDocument doc in bsonDocuments)
            {
                if (doc.ContainsKey("Name") && doc.ContainsKey("Count"))
                {
                    results.Add(new CountResult (
                        doc["Name"].AsString,
                        doc["Count"].AsInt32
                    ));
                }
            }
            return results;
        }

        public List<CountResult> RetrieveAllRecordsGroupByName(string key)
        {           
            switch (key)
            {
                case Categories.Death:
                    return RetrieveAllRecordsGroupByName(DeathCollection);
                case Categories.Join:
                    return RetrieveAllRecordsGroupByName(JoinCollection);
                case Categories.Leave:
                    return RetrieveAllRecordsGroupByName(LeaveCollection);
                case Categories.Ping:
                    return RetrieveAllRecordsGroupByName(PingCollection);
                case Categories.Shout:
                    return RetrieveAllRecordsGroupByName(ShoutCollection);
                default:
                    Plugin.StaticLogger.LogDebug($"RetrieveAllRecordsGroupByName, invalid key '{key}'");
                    return new List<CountResult>();
            }
        }
        public List<CountResult> CountAllRecordsGroupBySteamId(string key)
        {           
            switch (key)
            {
                case Categories.Death:
                    return CountAllRecordsGroupBySteamId(DeathCollection);
                case Categories.Join:
                    return CountAllRecordsGroupBySteamId(JoinCollection);
                case Categories.Leave:
                    return CountAllRecordsGroupBySteamId(LeaveCollection);
                case Categories.Ping:
                    return CountAllRecordsGroupBySteamId(PingCollection);
                case Categories.Shout:
                    return CountAllRecordsGroupBySteamId(ShoutCollection);
                default:
                    Plugin.StaticLogger.LogDebug($"CountAllRecordsGroupBySteamId, invalid key '{key}'");
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
