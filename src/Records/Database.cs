
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB;
using UnityEngine;

namespace DiscordConnector.Records
{
    /// <summary>
    /// The database class holds the connection to the database, and so all the databse-accessing methods are contained within.
    /// </summary>
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
        private ILiteCollection<PlayerToName> PlayerToNameCollection;

        /// <summary>
        /// Set's up the database using the compiled string `"${PluginInfo.PLUGIN_ID}-records.db"`, which in this
        /// case is probably `games.nwest.valheim.discordconnector-records.db`. This method needs to know where to
        /// store the database, since that is something that is only known at runtime.
        /// </summary>
        /// <param name="rootStorePath">Directory to save the LiteDB database in.</param>
        public Database(string rootStorePath)
        {
            DbPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, DB_NAME);

            Initialize();
        }

        /// <summary>
        /// Method to initialize the database reference and content.
        /// 
        /// This method creates the database file (if it doesn't exist) and opens it for reading.
        /// 
        /// Because of how LiteDB works (it creates tables as needed), this method simply creates the collection handles
        /// used later on as records get added to the database. 
        /// </summary>
        public void Initialize()
        {
            Task.Run(() =>
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
                    PlayerToNameCollection = db.GetCollection<PlayerToName>("player_name");
                }
                catch (System.IO.IOException e)
                {
                    Plugin.StaticLogger.LogError($"Unable to acquire un-shared access to {DbPath}");
                    Plugin.StaticLogger.LogDebug(e);
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Closes the database connection nicely.
        /// </summary>
        public void Dispose()
        {
            Plugin.StaticLogger.LogDebug("Closing LiteDB connection");
            db.Dispose();
        }

        /// <summary>
        /// Insert a "simple stat" record into the database for the provided collection.
        /// </summary>
        /// <param name="collection">Collection to insert the record into</param>
        /// <param name="playerName">Character name</param>
        /// <param name="playerHostName">Player connection id/hostname</param>
        /// <param name="pos">Position</param>
        private void InsertSimpleStatRecord(ILiteCollection<SimpleStat> collection, string playerName, string playerHostName, Vector3 pos)
        {
            Task.Run(() =>
            {
                var newRecord = new SimpleStat(
                    playerName,
                    playerHostName,
                    pos.x, pos.y, pos.z
                );
                collection.Insert(newRecord);

                collection.EnsureIndex(x => x.Name);
                collection.EnsureIndex(x => x.PlayerId);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Inserts a record with the player hostname and id into the databse, to simplify retrieval at a later date.
        /// </summary>
        /// <param name="characterName">Player character's name</param>
        /// <param name="playerHostName">Player's connection hostname</param>
        private void EnsurePlayerNameRecorded(string characterName, string playerHostName)
        {
            Task.Run(() =>
            {
                if (PlayerToNameCollection.Exists(x => x.PlayerId.Equals(playerHostName) && x.CharacterName.Equals(characterName)))
                {
                    // If the player record exists with both playerHostName and characterName, do nothing.
                    return;
                }

                if (PlayerToNameCollection.Exists(x => x.PlayerId.Equals(playerHostName)))
                {
                    // If the player record exists but only with the playerHostName, a new "latest" name is here
                    Plugin.StaticLogger.LogDebug($"Multiple characters from {playerHostName}, latest is {characterName}");

                }
                // Insert the player name record if it doesn't exist
                PlayerToName newPlayer = new PlayerToName(characterName, playerHostName);
                PlayerToNameCollection.Insert(newPlayer);

                PlayerToNameCollection.EnsureIndex(x => x.PlayerId);
                PlayerToNameCollection.EnsureIndex(x => x.CharacterName);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert a "simple stat" record without the position data.
        /// </summary>
        /// <param name="collection">Collection to insert the record into</param>
        /// <param name="playerName">Character name</param>
        /// <param name="playerHostName">Player connection id/hostname</param>
        private void InsertSimpleStatRecord(ILiteCollection<SimpleStat> collection, string playerName, string playerHostName)
        {
            InsertSimpleStatRecord(collection, playerName, playerHostName, Vector3.zero);
        }

        /// <summary>
        /// Insert a simple stat record with position for the provided key into the LiteDB database.
        /// </summary>
        /// <param name="key">What kind of record to insert</param>
        /// <param name="playerName">Player character name</param>
        /// <param name="playerHostName">Player connection host name (e.g. `Steam_{steamid}`)</param>
        /// <param name="pos">World position to record with the stat</param>
        public void InsertSimpleStatRecord(string key, string playerName, string playerHostName, Vector3 pos)
        {
            switch (key)
            {
                case Categories.Death:
                    InsertSimpleStatRecord(DeathCollection, playerName, playerHostName, pos);
                    break;
                case Categories.Join:
                    EnsurePlayerNameRecorded(playerName, playerHostName);
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

        /// <summary>
        /// Insert a simple stat record without position for the provided key into the LiteDB database.
        /// 
        /// This method simply wraps the positional InsertSimpleStatRecord method.
        /// </summary>
        /// <param name="key">What kind of record to insert</param>
        /// <param name="playerName">Player character name</param>
        /// <param name="playerHostName">Player connection host name (e.g. `Steam_{steamid}`)</param>
        public void InsertSimpleStatRecord(string key, string playerName, string playerHostName)
        {
            InsertSimpleStatRecord(key, playerName, playerHostName, Vector3.zero);
        }

        /// <summary>
        /// Returns the latest known character name for the given player identifier.
        /// 
        /// This first tries to find the name in the player_name table, and failing that references the join table with a query.
        /// If it has to use the join table to find the name, if it does find a valid name, it adds a record to the player_name
        /// table to make future lookups faster.
        /// </summary>
        /// <param name="playerHostName">The player's connection host name</param>
        /// <returns>Last known character name of the player</returns>
        internal string GetLatestCharacterNameForPlayer(string playerHostName)
        {
            if (Plugin.StaticConfig.DebugDatabaseMethods)
            {
                Plugin.StaticLogger.LogDebug($"GetLatestNameForCharacterId {playerHostName} begin");
            }

            if (PlayerToNameCollection.Exists(x => x.PlayerId.Equals(playerHostName)))
            {
                try
                {
                    PlayerToName playerInfo = PlayerToNameCollection
                        .Query()
                        // Select only the player we're looking for
                        .Where(x => x.PlayerId.Equals(playerHostName))
                        // Get the most recent name at the top
                        .OrderByDescending(x => x.InsertedDate)
                        .First();
                    return playerInfo.CharacterName;
                }
                catch (System.InvalidOperationException)
                {
                    // We should never not find the record, since we check for exists above!
                    Plugin.StaticLogger.LogWarning($"Should have found {playerHostName} in player_name table but did not!");
                }
            }

            // Some manual query business to grab the name from the Join table. This section should only get reached for old records,
            // where the player has not logged in for a while.
            var nameQuery = JoinCollection.Query()
                .Where(x => x.PlayerId.Equals(playerHostName))
                .OrderByDescending("Date")
                .Select("$.Name")
                .ToList();
            if (nameQuery.Count == 0)
            {
                if (Plugin.StaticConfig.DebugDatabaseMethods)
                {
                    Plugin.StaticLogger.LogDebug($"GetLatestNameForCharacterId {playerHostName} result = NONE");
                }
                return "undefined";
            }
            if (Plugin.StaticConfig.DebugDatabaseMethods)
            {
                Plugin.StaticLogger.LogDebug($"nameQuery has {nameQuery.Count} results");
            }
            var result = nameQuery[0];
            if (Plugin.StaticConfig.DebugDatabaseMethods)
            {
                Plugin.StaticLogger.LogDebug($"GetLatestNameForCharacterId {playerHostName} result = {result}");
            }

            Task.Run(() =>
            {
                // Insert the playername, since we didn't have it in the player_name database!
                EnsurePlayerNameRecorded(result["Name"], playerHostName);
            }).ConfigureAwait(false);

            return result["Name"].AsString;
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
                    Plugin.StaticLogger.LogDebug($"CountAllRecordsGrouped, invalid key '{key}'");
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

        public int CountOfRecordsByPlayerId(string key, string playerHostName)
        {
            if (!Plugin.StaticConfig.CollectStatsEnabled)
            {
                return -1;
            }
            switch (key)
            {
                case Categories.Death:
                    return CountOfRecordsByPlayerId(DeathCollection, playerHostName);
                case Categories.Join:
                    return CountOfRecordsByPlayerId(JoinCollection, playerHostName);
                case Categories.Leave:
                    return CountOfRecordsByPlayerId(LeaveCollection, playerHostName);
                case Categories.Ping:
                    return CountOfRecordsByPlayerId(PingCollection, playerHostName);
                case Categories.Shout:
                    return CountOfRecordsByPlayerId(ShoutCollection, playerHostName);
                default:
                    Plugin.StaticLogger.LogDebug($"CountOfRecordsBySteamId, invalid key '{key}'");
                    return -2;
            }
        }

        public List<CountResult> CountTodaysRecordsGrouped(string key)
        {
            switch (key)
            {
                case Categories.Death:
                    return CountAllTodaysRecordsGrouped(DeathCollection);
                case Categories.Join:
                    return CountAllTodaysRecordsGrouped(JoinCollection);
                case Categories.Leave:
                    return CountAllTodaysRecordsGrouped(LeaveCollection);
                case Categories.Ping:
                    return CountAllTodaysRecordsGrouped(PingCollection);
                case Categories.Shout:
                    return CountAllTodaysRecordsGrouped(ShoutCollection);
                default:
                    Plugin.StaticLogger.LogDebug($"CountTodaysRecordsGrouped, invalid key '{key}'");
                    return new List<CountResult>();
            }
        }



        internal int CountOfRecordsByName(ILiteCollection<SimpleStat> collection, string playerName)
        {
            try
            {
                return collection.Query()
                    .Where(x => x.Name.Equals(playerName))
                    .Count();
            }
            catch
            {
                Plugin.StaticLogger.LogDebug($"Error when trying to find {playerName} to count!");
                return 0;
            }
        }

        internal int CountOfRecordsByPlayerId(ILiteCollection<SimpleStat> collection, string playerHostName)
        {
            try
            {
                return collection.Query()
                    .Where(x => x.PlayerId.Equals(playerHostName))
                    .Count();
            }
            catch
            {
                Plugin.StaticLogger.LogDebug($"Error when trying to find {playerHostName} to count!");
                return 0;
            }
        }

        internal int CountOfRecordsByNameAndPlayerId(ILiteCollection<SimpleStat> collection, string playerName, string playerHostName)
        {
            try
            {
                return collection.Query()
                    .Where(x => (x.Name.Equals(playerName) && x.PlayerId.Equals(playerHostName)))
                    .Count();
            }
            catch
            {
                Plugin.StaticLogger.LogDebug($"Error when trying to find {playerHostName} as {playerName} to count!");
                return 0;
            }
        }

        /// <summary>
        /// Return a list of leaders for the collection.
        /// 
        /// This looks through the provided simple stat table and counts up results for each player using the method defined in the config.
        /// </summary>
        /// <param name="collection">Simple stat collection to count player totals in</param>
        /// <returns>List of counts with CharacterName and Total (x) for the provided SimpleStat colleciton.</returns>
        internal List<CountResult> CountAllRecordsGrouped(ILiteCollection<SimpleStat> collection)
        {
            if (Plugin.StaticConfig.DebugDatabaseMethods)
            {
                Plugin.StaticLogger.LogDebug($"CountAllRecordsGrouped {Plugin.StaticConfig.RecordRetrievalDiscernmentMethod}");
            }

            if (Plugin.StaticConfig.RecordRetrievalDiscernmentMethod.Equals(Config.RetrievalDiscernmentMethods.ByName))
            {
                return CountResult.ConvertFromBsonDocuments(
                    collection.Query()
                        .GroupBy("Name")
                        .Select("{Name: @Key, Count: COUNT(*)}")
                        .ToList()
                );
            }
            else if (Plugin.StaticConfig.RecordRetrievalDiscernmentMethod.Equals(Config.RetrievalDiscernmentMethods.ByNameAndSteamID))
            {
                return CountResult.ConvertFromBsonDocuments(
                    collection.Query()
                        .GroupBy("{Name,PlayerId}")
                        .Select("{NamePlayer: @Key, Count: COUNT(*)}")
                        .ToList()
                );
            }
            else // Leaving only Config.RetrievalDiscernmentMethods.BySteamID
            {

                return CountResult.ConvertFromBsonDocuments(
                    collection.Query()
                        .GroupBy("PlayerId")
                        .Select("{Player: @Key, Count: COUNT(*)}")
                        .ToList()
                );
            }
        }


        /// <summary>
        /// For records from today, return a list of counts with players and totals
        /// 
        /// This looks through the provided simple stat table and counts up results for each player using the method defined in the config.
        /// </summary>
        /// <param name="collection">Simple stat collection to count player totals in</param>
        /// <returns>List of counts with CharacterName and Total (x) for the provided SimpleStat colleciton.</returns>
        internal List<CountResult> CountAllTodaysRecordsGrouped(ILiteCollection<SimpleStat> collection)
        {
            if (Plugin.StaticConfig.DebugDatabaseMethods)
            {
                Plugin.StaticLogger.LogDebug($"CountAllTodaysRecordsGrouped {Plugin.StaticConfig.RecordRetrievalDiscernmentMethod}");
            }

            System.DateTime today = System.DateTime.Today;
            if (Plugin.StaticConfig.RecordRetrievalDiscernmentMethod.Equals(Config.RetrievalDiscernmentMethods.ByName))
            {
                return CountResult.ConvertFromBsonDocuments(
                    collection.Query()
                        // Filter to today by date!
                        .Where(x => x.Date.Year == today.Date.Year
                            && x.Date.Month == today.Date.Month
                            && x.Date.Day == today.Date.Day)
                        .GroupBy("Name")
                        .Select("{Name: @Key, Count: COUNT(*)}")
                        .ToList()
                );
            }
            else if (Plugin.StaticConfig.RecordRetrievalDiscernmentMethod.Equals(Config.RetrievalDiscernmentMethods.ByNameAndSteamID))
            {
                return CountResult.ConvertFromBsonDocuments(
                    collection.Query()
                        // Filter to today by date!
                        .Where(x => x.Date.Year == today.Date.Year
                            && x.Date.Month == today.Date.Month
                            && x.Date.Day == today.Date.Day)
                        .GroupBy("{Name,PlayerId}")
                        .Select("{NamePlayer: @Key, Count: COUNT(*)}")
                        .ToList()
                );
            }
            else // Leaving only Config.RetrievalDiscernmentMethods.BySteamID
            {

                return CountResult.ConvertFromBsonDocuments(
                    collection.Query()
                        // Filter to today by date!
                        .Where(x => x.Date.Year == today.Date.Year
                            && x.Date.Month == today.Date.Month
                            && x.Date.Day == today.Date.Day)
                        .GroupBy("PlayerId")
                        .Select("{Player: @Key, Count: COUNT(*)}")
                        .ToList()
                );
            }
        }


    }
}
