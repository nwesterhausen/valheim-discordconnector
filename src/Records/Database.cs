
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordConnector.LeaderBoards;
using LiteDB;
using Newtonsoft.Json;
using UnityEngine;

namespace DiscordConnector.Records
{
    /// <summary>
    /// The database class holds the connection to the database, and so all the database-accessing methods are contained within.
    /// </summary>
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
        private ILiteCollection<PlayerToName> PlayerToNameCollection;

        /// <summary>
        /// Set's up the database using the compiled string `"${PluginInfo.PLUGIN_ID}-records.db"`, which in this
        /// case is probably `games.nwest.valheim.discordconnector-records.db`. This method needs to know where to
        /// store the database, since that is something that is only known at runtime.
        /// </summary>
        /// <param name="rootStorePath">Directory to save the LiteDB database in.</param>
        public Database(string rootStorePath)
        {
            DbPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, PluginInfo.PLUGIN_ID, DB_NAME);

            // Check for database in old location and move if necessary
            string oldDatabase = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, $"{PluginInfo.PLUGIN_ID}-records.db");
            if (System.IO.File.Exists(oldDatabase))
            {
                System.IO.File.Move(oldDatabase, DbPath);
            }

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

                    // Grab references to the collections
                    DeathCollection = db.GetCollection<SimpleStat>("deaths");
                    JoinCollection = db.GetCollection<SimpleStat>("joins");
                    LeaveCollection = db.GetCollection<SimpleStat>("leaves");
                    ShoutCollection = db.GetCollection<SimpleStat>("shouts");
                    PingCollection = db.GetCollection<SimpleStat>("pings");
                    PlayerToNameCollection = db.GetCollection<PlayerToName>("player_name");

                    // Ensure indices on the collections
                    Task.Run(() =>
                    {
                        EnsureIndicesOnCollections();
                        Plugin.StaticLogger.LogDebug("Created indices on database collections");
                    }).ConfigureAwait(false);
                }
                catch (System.IO.IOException e)
                {
                    Plugin.StaticLogger.LogError($"Unable to acquire un-shared access to {DbPath}");
                    Plugin.StaticLogger.LogDebug(e);
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs EnsureIndex() on each collection, targeting the columns we index on
        /// </summary>
        private void EnsureIndicesOnCollections()
        {
            DeathCollection.EnsureIndex(x => x.PlayerId);
            DeathCollection.EnsureIndex(x => x.Name);
            JoinCollection.EnsureIndex(x => x.PlayerId);
            JoinCollection.EnsureIndex(x => x.Name);
            LeaveCollection.EnsureIndex(x => x.PlayerId);
            LeaveCollection.EnsureIndex(x => x.Name);
            ShoutCollection.EnsureIndex(x => x.PlayerId);
            ShoutCollection.EnsureIndex(x => x.Name);
            PingCollection.EnsureIndex(x => x.PlayerId);
            PingCollection.EnsureIndex(x => x.Name);
            PlayerToNameCollection.EnsureIndex(x => x.PlayerId);
            PlayerToNameCollection.EnsureIndex(x => x.CharacterName);
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
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Inserts a record with the player hostname and id into the database, to simplify retrieval at a later date.
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
        /// <param name="playerHostName">Player connection host name (e.g. `Steam_{steamId}`)</param>
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
        /// <param name="playerHostName">Player connection host name (e.g. `Steam_{steamId}`)</param>
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
            // simplify results to single record
            var result = nameQuery[0];
            if (Plugin.StaticConfig.DebugDatabaseMethods)
            {
                Plugin.StaticLogger.LogDebug($"GetLatestNameForCharacterId {playerHostName} result = {result}");
            }

            Task.Run(() =>
            {
                // Insert the player name, since we didn't have it in the player_name database!
                EnsurePlayerNameRecorded(result["Name"], playerHostName);
            }).ConfigureAwait(false);

            return result["Name"].AsString;
        }


        /// <summary>
        /// Get totals for each player's tracked records
        /// </summary>
        /// <param name="key">Record type to return sums for</param>
        /// <returns>List of results with character names and totals</returns>
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
                case Categories.TimeOnline:
                    return AllTimeOnlineRecordsGrouped();
                default:
                    Plugin.StaticLogger.LogDebug($"CountAllRecordsGrouped, invalid key '{key}'");
                    return new List<CountResult>();
            }
        }

        private List<CountResult> AllTimeOnlineRecordsGrouped()
        {

            PlayerToName[] players = PlayerToNameCollection.Query().ToArray();
            List<CountResult> results = new List<CountResult>();

            foreach (PlayerToName player in players)
            {
                // Create a spot to record total online time for player
                Tuple<string, string> playerTuple = Tuple.Create(player.PlayerId, player.CharacterName);
                System.TimeSpan onlineTime = System.TimeSpan.FromSeconds(0.0);

                // Grab joins and leaves for player
                SimpleStat[] joins = JoinCollection.Query().Where(x => x.PlayerId == player.PlayerId && x.Name == player.CharacterName).ToArray();
                SimpleStat[] leaves = LeaveCollection.Query().Where(x => x.PlayerId == player.PlayerId && x.Name == player.CharacterName).ToArray();

                // Compare their lengths
                Plugin.StaticLogger.LogDebug($"{player.PlayerId} as {player.CharacterName} has {joins.Length} joins, {leaves.Length}");
                int sessionDifference = joins.Length - leaves.Length;
                if (sessionDifference > 1)
                {
                    Plugin.StaticLogger.LogDebug($"{sessionDifference} more joins than leaves, timeOnline likely to be very inaccurate");
                }
                // Should either be equal to joins.Length or joins.Length - 1 (basically leaves.Length)
                int travelableLength = joins.Length - sessionDifference;

                // Add up time between each one.
                for (int i = 0; i < travelableLength; i++)
                {
                    // Get the dates
                    System.DateTime login = joins[i].Date;
                    System.DateTime logout = leaves[i].Date;

                    // Get the time between them
                    System.TimeSpan difference = logout.Subtract(login);

                    // Add that time to the player's total
                    onlineTime.Add(difference);
                }

                // Total time is then stored
                Plugin.StaticLogger.LogDebug($"{onlineTime.ToString()} total online time.");

                // Append to list
                results.Add(new CountResult(player.CharacterName, (int)onlineTime.TotalSeconds));
            }

            Task.Run(() =>
            {
                DiscordApi.SendMessage(JsonConvert.SerializeObject(results));
            });

            return results;
        }

        /// <summary>
        /// Provides time online in seconds for all players within the date range provided. By default, it includes the start date.
        /// 
        /// This looks through the provided simple stat table and counts up time differences between joins and leaves.
        /// </summary>
        /// <param name="startDate">Date to start including records from</param>
        /// <param name="startDate">Date to stop including records before</param>
        /// <param name="inclusiveStart">Whether to include the start date or not in the returned results. If true, it will use `>=` for the startDate comparison; otherwise `>`.</param>
        /// <param name="inclusiveEnd">Whether to include the end date or not in the returned results. If true, it will use `<=` for the startDate comparison; otherwise `<`.</param>
        /// <returns>List of counts with CharacterName and Total (x) for the provided SimpleStat collection.</returns>
        private List<CountResult> TimeOnlineRecordsWhereDate(System.DateTime startDate, System.DateTime endDate, bool inclusiveStart = true, bool inclusiveEnd = true)
        {

            PlayerToName[] players = PlayerToNameCollection.Query().ToArray();
            List<CountResult> results = new List<CountResult>();

            foreach (PlayerToName player in players)
            {
                // Create a spot to record total online time for player
                Tuple<string, string> playerTuple = Tuple.Create(player.PlayerId, player.CharacterName);
                System.TimeSpan onlineTime = System.TimeSpan.FromSeconds(0.0);

                // Grab joins and leaves for player
                SimpleStat[] joins;
                SimpleStat[] leaves;


                if (inclusiveStart && inclusiveEnd)
                {
                    joins = JoinCollection.Query()
                            // Filter to dates inclusively
                            .Where(x => x.Date.Year >= startDate.Date.Year
                                && x.Date.Month >= startDate.Date.Month
                                && x.Date.Day >= startDate.Date.Day
                                && x.Date.Year <= endDate.Date.Year
                                && x.Date.Month <= endDate.Date.Month
                                && x.Date.Day <= endDate.Date.Day)
                            // Filter to player
                            .Where(x => x.PlayerId == player.PlayerId && x.Name == player.CharacterName)
                            .ToArray();
                    leaves = LeaveCollection.Query()
                            // Filter to dates inclusively
                            .Where(x => x.Date.Year >= startDate.Date.Year
                                && x.Date.Month >= startDate.Date.Month
                                && x.Date.Day >= startDate.Date.Day
                                && x.Date.Year <= endDate.Date.Year
                                && x.Date.Month <= endDate.Date.Month
                                && x.Date.Day <= endDate.Date.Day)
                            // Filter to player
                            .Where(x => x.PlayerId == player.PlayerId && x.Name == player.CharacterName)
                            .ToArray();

                }
                else if (inclusiveEnd)
                {
                    joins = JoinCollection.Query()
                            // Filter to dates: end date inclusively, start date exclusively
                            .Where(x => x.Date.Year > startDate.Date.Year
                                && x.Date.Month > startDate.Date.Month
                                && x.Date.Day > startDate.Date.Day
                                && x.Date.Year <= endDate.Date.Year
                                && x.Date.Month <= endDate.Date.Month
                                && x.Date.Day <= endDate.Date.Day)
                            // Filter to player
                            .Where(x => x.PlayerId == player.PlayerId && x.Name == player.CharacterName)
                            .ToArray();
                    leaves = LeaveCollection.Query()
                            // Filter to dates: end date inclusively, start date exclusively
                            .Where(x => x.Date.Year > startDate.Date.Year
                                && x.Date.Month > startDate.Date.Month
                                && x.Date.Day > startDate.Date.Day
                                && x.Date.Year <= endDate.Date.Year
                                && x.Date.Month <= endDate.Date.Month
                                && x.Date.Day <= endDate.Date.Day)
                            // Filter to player
                            .Where(x => x.PlayerId == player.PlayerId && x.Name == player.CharacterName)
                            .ToArray();
                }
                else if (inclusiveStart)
                {
                    joins = JoinCollection.Query()
                            // Filter to dates: start date inclusively, end date exclusively
                            .Where(x => x.Date.Year >= startDate.Date.Year
                                && x.Date.Month >= startDate.Date.Month
                                && x.Date.Day >= startDate.Date.Day
                                && x.Date.Year < endDate.Date.Year
                                && x.Date.Month < endDate.Date.Month
                                && x.Date.Day < endDate.Date.Day)
                            // Filter to player
                            .Where(x => x.PlayerId == player.PlayerId && x.Name == player.CharacterName)
                            .ToArray();
                    leaves = LeaveCollection.Query()
                            // Filter to dates: start date inclusively, end date exclusively
                            .Where(x => x.Date.Year >= startDate.Date.Year
                                && x.Date.Month >= startDate.Date.Month
                                && x.Date.Day >= startDate.Date.Day
                                && x.Date.Year < endDate.Date.Year
                                && x.Date.Month < endDate.Date.Month
                                && x.Date.Day < endDate.Date.Day)
                            // Filter to player
                            .Where(x => x.PlayerId == player.PlayerId && x.Name == player.CharacterName)
                            .ToArray();
                }
                else
                {
                    joins = JoinCollection.Query()
                            // Filter to dates exclusively
                            .Where(x => x.Date.Year > startDate.Date.Year
                                && x.Date.Month > startDate.Date.Month
                                && x.Date.Day > startDate.Date.Day
                                && x.Date.Year < endDate.Date.Year
                                && x.Date.Month < endDate.Date.Month
                                && x.Date.Day < endDate.Date.Day)
                            // Filter to player
                            .Where(x => x.PlayerId == player.PlayerId && x.Name == player.CharacterName)
                            .ToArray();
                    leaves = LeaveCollection.Query()
                            // Filter to dates exclusively
                            .Where(x => x.Date.Year > startDate.Date.Year
                                && x.Date.Month > startDate.Date.Month
                                && x.Date.Day > startDate.Date.Day
                                && x.Date.Year < endDate.Date.Year
                                && x.Date.Month < endDate.Date.Month
                                && x.Date.Day < endDate.Date.Day)
                            // Filter to player
                            .Where(x => x.PlayerId == player.PlayerId && x.Name == player.CharacterName)
                            .ToArray();
                }


                // Compare their lengths
                Plugin.StaticLogger.LogDebug($"{player.PlayerId} as {player.CharacterName} has {joins.Length} joins, {leaves.Length}");
                int sessionDifference = joins.Length - leaves.Length;
                if (sessionDifference > 1)
                {
                    Plugin.StaticLogger.LogDebug($"{sessionDifference} more joins than leaves, timeOnline likely to be very inaccurate");
                }
                // Should either be equal to joins.Length or joins.Length - 1 (basically leaves.Length)
                int travelableLength = joins.Length - sessionDifference;

                // Add up time between each one.
                for (int i = 0; i < travelableLength; i++)
                {
                    // Get the dates
                    System.DateTime login = joins[i].Date;
                    System.DateTime logout = leaves[i].Date;

                    // Get the time between them
                    System.TimeSpan difference = logout.Subtract(login);

                    // Add that time to the player's total
                    onlineTime.Add(difference);
                }

                // Total time is then stored
                Plugin.StaticLogger.LogDebug($"{onlineTime.ToString()} total online time.");

                // Append to list
                results.Add(new CountResult(player.CharacterName, (int)onlineTime.TotalSeconds));
            }

            Task.Run(() =>
            {
                DiscordApi.SendMessage(JsonConvert.SerializeObject(results));
            });

            return results;
        }

        /// <summary>
        /// Get the total count of records in the category for the player (by character name).
        /// 
        /// This can be used to figure out if its the first action the player has taken.
        /// 
        /// Returns -1 if collecting stats is disabled.
        /// Returns -2 if the specified category key is invalid
        /// Returns -3 if the collection in the database has an issue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="playerName"></param>
        /// <returns></returns>
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

        public List<CountResult> CountAllRecordsGrouped(string key, LeaderBoards.TimeRange timeRange)
        {
            if (timeRange == TimeRange.AllTime)
            {
                return CountAllRecordsGrouped(key);
            }

            var BeginEndDate = DateHelper.StartEndDatesForTimeRange(timeRange);
            return CountRecordsBetweenDatesGrouped(key, BeginEndDate.Item1, BeginEndDate.Item2);
        }

        internal List<CountResult> CountRecordsBetweenDatesGrouped(string key, System.DateTime startDate, System.DateTime endDate, bool inclusiveStart = true, bool inclusiveEnd = true)
        {
            switch (key)
            {
                case Categories.Death:
                    return CountAllRecordsGroupsWhereDate(DeathCollection, startDate, endDate, inclusiveStart, inclusiveEnd);
                case Categories.Join:
                    return CountAllRecordsGroupsWhereDate(JoinCollection, startDate, endDate, inclusiveStart, inclusiveEnd);
                case Categories.Leave:
                    return CountAllRecordsGroupsWhereDate(LeaveCollection, startDate, endDate, inclusiveStart, inclusiveEnd);
                case Categories.Ping:
                    return CountAllRecordsGroupsWhereDate(PingCollection, startDate, endDate, inclusiveStart, inclusiveEnd);
                case Categories.Shout:
                    return CountAllRecordsGroupsWhereDate(ShoutCollection, startDate, endDate, inclusiveStart, inclusiveEnd);
                case Categories.TimeOnline:
                    return TimeOnlineRecordsWhereDate(startDate, endDate, inclusiveStart, inclusiveEnd);
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
                return -3;
            }
        }

        /// <summary>
        /// Return a list of leaders for the collection.
        /// 
        /// This looks through the provided simple stat table and counts up results for each player using the method defined in the config.
        /// </summary>
        /// <param name="collection">Simple stat collection to count player totals in</param>
        /// <returns>List of counts with CharacterName and Total (x) for the provided SimpleStat collection.</returns>
        internal List<CountResult> CountAllRecordsGrouped(ILiteCollection<SimpleStat> collection)
        {
            if (Plugin.StaticConfig.DebugDatabaseMethods)
            {
                Plugin.StaticLogger.LogDebug($"CountAllRecordsGrouped {Plugin.StaticConfig.RecordRetrievalDiscernmentMethod}");
            }

            if (collection.Count() == 0)
            {
                if (Plugin.StaticConfig.DebugDatabaseMethods)
                {
                    Plugin.StaticLogger.LogDebug("Collection is empty, skipping.");
                }
                return new List<CountResult>();
            }

            // Config.RetrievalDiscernmentMethods.BySteamID by default (should be most common), conditionally check for others
            string GroupByClause = "PlayerId";
            string SelectClause = "{Player: @Key, Count: Count(*)}";

            if (Plugin.StaticConfig.RecordRetrievalDiscernmentMethod == Config.MainConfig.RetrievalDiscernmentMethods.NameAndPlayerId)
            {
                GroupByClause = "{Name,PlayerId}";
                SelectClause = "{NamePlayer: @Key, Count: COUNT(*)}";
            }
            if (Plugin.StaticConfig.RecordRetrievalDiscernmentMethod == Config.MainConfig.RetrievalDiscernmentMethods.Name)
            {
                GroupByClause = "Name";
                SelectClause = "{Name: @Key, Count: Count(*)}";
            }

            var result = CountResult.ConvertFromBsonDocuments(
                collection.Query()
                    .GroupBy(GroupByClause)
                    .Select(SelectClause)
                    .ToList()
            );

            if (Plugin.StaticConfig.DebugDatabaseMethods)
            {
                Plugin.StaticLogger.LogDebug($"CountAllRecordsGrouped {result.Count} records returned");
            }

            return result;
        }


        /// <summary>
        /// Provides count summaries for the collection within the date range provided. By default, it includes the start date.
        /// 
        /// This looks through the provided simple stat table and counts up results for each player using the method defined in the config.
        /// </summary>
        /// <param name="collection">Simple stat collection to count player totals in</param>
        /// <param name="startDate">Date to start including records from</param>
        /// <param name="startDate">Date to stop including records before</param>
        /// <param name="inclusiveStart">Whether to include the start date or not in the returned results. If true, it will use `>=` for the startDate comparison; otherwise `>`.</param>
        /// <param name="inclusiveEnd">Whether to include the end date or not in the returned results. If true, it will use `<=` for the startDate comparison; otherwise `<`.</param>
        /// <returns>List of counts with CharacterName and Total (x) for the provided SimpleStat collection.</returns>
        internal List<CountResult> CountAllRecordsGroupsWhereDate(ILiteCollection<SimpleStat> collection, System.DateTime startDate, System.DateTime endDate, bool inclusiveStart = true, bool inclusiveEnd = true)
        {
            if (Plugin.StaticConfig.DebugDatabaseMethods)
            {
                Plugin.StaticLogger.LogDebug($"CountAllRecordsGroupsWhereDate {Plugin.StaticConfig.RecordRetrievalDiscernmentMethod} {startDate} {endDate}");
            }

            if (collection.Count() == 0)
            {
                if (Plugin.StaticConfig.DebugDatabaseMethods)
                {
                    Plugin.StaticLogger.LogDebug("Collection is empty, skipping.");
                }
                return new List<CountResult>();
            }

            // Config.RetrievalDiscernmentMethods.BySteamID by default (should be most common), conditionally check for others
            string GroupByClause = "PlayerId";
            string SelectClause = "{Player: @Key, Count: Count(*)}";

            if (Plugin.StaticConfig.RecordRetrievalDiscernmentMethod == Config.MainConfig.RetrievalDiscernmentMethods.NameAndPlayerId)
            {
                GroupByClause = "{Name,PlayerId}";
                SelectClause = "{NamePlayer: @Key, Count: COUNT(*)}";
            }
            if (Plugin.StaticConfig.RecordRetrievalDiscernmentMethod == Config.MainConfig.RetrievalDiscernmentMethods.Name)
            {
                GroupByClause = "Name";
                SelectClause = "{Name: @Key, Count: Count(*)}";
            }

            List<CountResult> result;
            if (inclusiveStart && inclusiveEnd)
            {
                result = CountResult.ConvertFromBsonDocuments(
                    collection.Query()
                        // Filter to dates inclusively
                        .Where(x => x.Date.Year >= startDate.Date.Year
                            && x.Date.Month >= startDate.Date.Month
                            && x.Date.Day >= startDate.Date.Day
                            && x.Date.Year <= endDate.Date.Year
                            && x.Date.Month <= endDate.Date.Month
                            && x.Date.Day <= endDate.Date.Day)
                        .GroupBy(GroupByClause)
                        .Select(SelectClause)
                        .ToList()
                );
            }
            else if (inclusiveEnd)
            {
                result = CountResult.ConvertFromBsonDocuments(
                    collection.Query()
                        // Filter to dates: end date inclusively, start date exclusively
                        .Where(x => x.Date.Year > startDate.Date.Year
                            && x.Date.Month > startDate.Date.Month
                            && x.Date.Day > startDate.Date.Day
                            && x.Date.Year <= endDate.Date.Year
                            && x.Date.Month <= endDate.Date.Month
                            && x.Date.Day <= endDate.Date.Day)
                        .GroupBy(GroupByClause)
                        .Select(SelectClause)
                        .ToList()
                );
            }
            else if (inclusiveStart)
            {
                result = CountResult.ConvertFromBsonDocuments(
                    collection.Query()
                        // Filter to dates: start date inclusively, end date exclusively
                        .Where(x => x.Date.Year >= startDate.Date.Year
                            && x.Date.Month >= startDate.Date.Month
                            && x.Date.Day >= startDate.Date.Day
                            && x.Date.Year < endDate.Date.Year
                            && x.Date.Month < endDate.Date.Month
                            && x.Date.Day < endDate.Date.Day)
                        .GroupBy(GroupByClause)
                        .Select(SelectClause)
                        .ToList()
                );
            }
            else
            {
                result = CountResult.ConvertFromBsonDocuments(
                    collection.Query()
                        // Filter to dates exclusively
                        .Where(x => x.Date.Year > startDate.Date.Year
                            && x.Date.Month > startDate.Date.Month
                            && x.Date.Day > startDate.Date.Day
                            && x.Date.Year < endDate.Date.Year
                            && x.Date.Month < endDate.Date.Month
                            && x.Date.Day < endDate.Date.Day)
                        .GroupBy(GroupByClause)
                        .Select(SelectClause)
                        .ToList()
                );
            }

            if (Plugin.StaticConfig.DebugDatabaseMethods)
            {
                Plugin.StaticLogger.LogDebug($"CountAllRecordsGroupsWhereDate {result.Count} records returned");
            }

            return result;

        }


    }
}
