
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

        public void InsertDeathRecord(string playerName, ulong steamId, Vector3 pos)
        {
            InsertSimpleStatRecord(DeathCollection, playerName, steamId, pos);
        }
        public void InsertJoinRecord(string playerName, ulong steamId, Vector3 pos)
        {
            InsertSimpleStatRecord(JoinCollection, playerName, steamId, pos);
        }
        public void InsertLeaveRecord(string playerName, ulong steamId, Vector3 pos)
        {
            InsertSimpleStatRecord(LeaveCollection, playerName, steamId, pos);
        }
        public void InsertShoutRecord(string playerName, ulong steamId, Vector3 pos)
        {
            InsertSimpleStatRecord(ShoutCollection, playerName, steamId, pos);
        }
        public void InsertPingRecord(string playerName, ulong steamId, Vector3 pos)
        {
            InsertSimpleStatRecord(PingCollection, playerName, steamId, pos);
        }
        public void InsertDeathRecord(string playerName, ulong steamId)
        {
            InsertSimpleStatRecord(DeathCollection, playerName, steamId);
        }
        public void InsertJoinRecord(string playerName, ulong steamId)
        {
            InsertSimpleStatRecord(JoinCollection, playerName, steamId);
        }
        public void InsertLeaveRecord(string playerName, ulong steamId)
        {
            InsertSimpleStatRecord(LeaveCollection, playerName, steamId);
        }
        public void InsertShoutRecord(string playerName, ulong steamId)
        {
            InsertSimpleStatRecord(ShoutCollection, playerName, steamId);
        }
        public void InsertPingRecord(string playerName, ulong steamId)
        {
            InsertSimpleStatRecord(PingCollection, playerName, steamId);
        }

        public int GetNumberDeaths(string playerName)
        {
            return CountOfRecordsByName(DeathCollection, playerName);
        }
        public int GetNumberDeaths(ulong steamId)
        {
            return CountOfRecordsBySteamId(DeathCollection, steamId);
        }
        public int GetNumberJoins(string playerName)
        {
            return CountOfRecordsByName(JoinCollection, playerName);
        }
        public int GetNumberJoins(ulong steamId)
        {
            return CountOfRecordsBySteamId(JoinCollection, steamId);
        }
        public int GetNumberLeaves(string playerName)
        {
            return CountOfRecordsByName(LeaveCollection, playerName);
        }
        public int GetNumberLeaves(ulong steamId)
        {
            return CountOfRecordsBySteamId(LeaveCollection, steamId);
        }
        public int GetNumberShouts(string playerName)
        {
            return CountOfRecordsByName(ShoutCollection, playerName);
        }
        public int GetNumberShouts(ulong steamId)
        {
            return CountOfRecordsBySteamId(ShoutCollection, steamId);
        }
        public int GetNumberPings(string playerName)
        {
            return CountOfRecordsByName(PingCollection, playerName);
        }
        public int GetNumberPings(ulong steamId)
        {
            return CountOfRecordsBySteamId(PingCollection, steamId);
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
    }
}
