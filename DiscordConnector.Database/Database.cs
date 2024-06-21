using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordConnector.Database.Models;
using DiscordConnector.Utilities;
using LiteDB;
using UnityEngine;

namespace DiscordConnector.Database;

public class Database
{
  public const string DB_NAME = "records.db";
  public const string DB_LOG_NAME = "database.log";
  public const string DEATH_COLLECTION = "deaths";
  public const string JOIN_COLLECTION = "joins";
  public const string LEAVE_COLLECTION = "leaves";
  public const string SHOUT_COLLECTION = "shouts";
  public const string PING_COLLECTION = "pings";
  public const string PLAYER_TO_NAME_COLLECTION = "player_name";

  /// <summary>
  /// Available options for sorting the results gathered from the database. This is used when defining the custom leader boards.
  /// </summary>
  public enum Ordering
  {
    [System.ComponentModel.Description("Most to Least (Descending)")]
    Descending,
    [System.ComponentModel.Description("Least to Most (Ascending)")]
    Ascending,
  }

  public enum Tables
  {
    DEATHS,
    JOINS,
    LEAVES,
    SHOUTS,
    PINGS,
    PLAYER_TO_NAME,
    TIME_ONLINE
  }

  public enum DatabaseStatus
  {
    INITIALIZING,
    READY,
    ERROR
  }

  private static string DbPath;
  private static string LogPath;
  private static Config DbConfig;
  private DatabaseStatus Status = DatabaseStatus.INITIALIZING;
  private LiteDatabase db;
  private SimpleLogger logger;
  private ILiteCollection<SimpleStat> DeathCollection;
  private ILiteCollection<SimpleStat> JoinCollection;
  private ILiteCollection<SimpleStat> LeaveCollection;
  private ILiteCollection<SimpleStat> ShoutCollection;
  private ILiteCollection<SimpleStat> PingCollection;
  private ILiteCollection<PlayerToName> PlayerToNameCollection;

  public Database(string storageDirectory, Config config)
  {
    DbPath = System.IO.Path.Combine(storageDirectory, DB_NAME);
    LogPath = System.IO.Path.Combine(storageDirectory, DB_LOG_NAME);
    DbConfig = config;

    logger = new SimpleLogger(LogPath);

    Initialize();
  }
  public Database(string storageDirectory) : this(storageDirectory, new Config())
  {
  }

  public DatabaseStatus GetStatus()
  {
    return Status;
  }

  /// <summary>
  /// Method to initialize the database reference and content.
  ///
  /// This method creates the database file (if it doesn't exist) and opens it for reading.
  ///
  /// Because of how LiteDB works (it creates tables as needed), this method simply creates the collection handles
  /// used later on as records get added to the database.
  /// </summary>
  private void Initialize()
  {
    try
    {
      db = new LiteDatabase(DbPath);

      logger.LogDebug($"LiteDB Connection Established to {DbPath}");

      DeathCollection = db.GetCollection<SimpleStat>(DEATH_COLLECTION);
      JoinCollection = db.GetCollection<SimpleStat>(JOIN_COLLECTION);
      LeaveCollection = db.GetCollection<SimpleStat>(LEAVE_COLLECTION);
      ShoutCollection = db.GetCollection<SimpleStat>(SHOUT_COLLECTION);
      PingCollection = db.GetCollection<SimpleStat>(PING_COLLECTION);
      PlayerToNameCollection = db.GetCollection<PlayerToName>(PLAYER_TO_NAME_COLLECTION);

      logger.LogDebug("Tables Initialized");

      EnsureIndicesOnCollections();

      logger.LogDebug("Created indices on database collections");
    }
    catch (System.IO.IOException e)
    {
      logger.LogError($"Unable to acquire un-shared access to {DbPath}");
      logger.LogDebug(e.ToString());
      Status = DatabaseStatus.ERROR;
    }

    Status = DatabaseStatus.READY;
  }

  /// <summary>
  /// Runs EnsureIndex() on each collection, targeting the columns we index
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
    logger.LogDebug("Closing LiteDB connection");
    db.Dispose();
    logger.LogDebug("LiteDB connection closed");
    logger.Dispose();
  }

  /// <summary>
  /// Inserts a record with the player hostname and id into the database, to simplify retrieval at a later date.
  /// </summary>
  /// <param name="characterName">Player character's name</param>
  /// <param name="playerHostName">Player's connection hostname</param>
  private void EnsurePlayerNameRecorded(string characterName, string playerHostName)
  {
    if (PlayerToNameCollection.Exists(x => x.PlayerId.Equals(playerHostName) && x.CharacterName.Equals(characterName)))
    {
      // If the player record exists with both playerHostName and characterName, do nothing.
      return;
    }

    if (PlayerToNameCollection.Exists(x => x.PlayerId.Equals(playerHostName)))
    {
      // If the player record exists but only with the playerHostName, a new "latest" name is here
      logger.LogDebug($"EnsurePlayerNameRecorded: Multiple characters from {playerHostName}, latest is {characterName}");
    }
    // Insert the player name record if it doesn't exist
    PlayerToName newPlayer = new(characterName, playerHostName);
    PlayerToNameCollection.Insert(newPlayer);
  }

  /// <summary>
  /// Insert a simple stat record with position for the provided key into the LiteDB database.
  /// </summary>
  /// <param name="key">What kind of record to insert</param>
  /// <param name="playerName">Player character name</param>
  /// <param name="playerHostName">Player connection host name (e.g. `Steam_{steamId}`)</param>
  /// <param name="pos">World position to record with the stat</param>
  public void InsertSimpleStatRecord(Tables key, string playerName, string playerHostName, Vector3 pos)
  {
    switch (key)
    {
      case Tables.DEATHS:
        EnsurePlayerNameRecorded(playerName, playerHostName);
        InsertSimpleStatRecord(DeathCollection, playerName, playerHostName, pos);
        break;
      case Tables.JOINS:
        EnsurePlayerNameRecorded(playerName, playerHostName);
        InsertSimpleStatRecord(JoinCollection, playerName, playerHostName, pos);
        break;
      case Tables.LEAVES:
        EnsurePlayerNameRecorded(playerName, playerHostName);
        InsertSimpleStatRecord(LeaveCollection, playerName, playerHostName, pos);
        break;
      case Tables.PINGS:
        EnsurePlayerNameRecorded(playerName, playerHostName);
        InsertSimpleStatRecord(PingCollection, playerName, playerHostName, pos);
        break;
      case Tables.SHOUTS:
        EnsurePlayerNameRecorded(playerName, playerHostName);
        InsertSimpleStatRecord(ShoutCollection, playerName, playerHostName, pos);
        break;
      default:
        logger.LogDebug($"InsertSimpleStatRecord: invalid key '{key}'");
        break;
    }
  }

  /// <summary>
  /// Insert a "simple stat" record without the position data.
  /// </summary>
  /// <param name="key">What kind of record to insert</param>
  /// <param name="playerName">Player character name</param>
  /// <param name="playerHostName">Player connection host name (e.g. `Steam_{steamId}`)</param>
  public void InsertSimpleStatRecord(Tables key, string playerName, string playerHostName)
  {
    InsertSimpleStatRecord(key, playerName, playerHostName, Vector3.zero);
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
      SimpleStat newRecord = new SimpleStat(
              playerName,
              playerHostName,
              new Position(pos.x, pos.y, pos.z)
          );
      collection.Insert(newRecord);
    }).ConfigureAwait(false);
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
    if (DbConfig.DebugDatabaseMethods)
    {
      logger.LogDebug($"GetLatestNameForCharacterId {playerHostName} begin");
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
      catch (InvalidOperationException)
      {
        // We should never not find the record, since we check for exists above!
        logger.LogWarning($"Should have found {playerHostName} in player_name table but did not!");
      }
    }

    // Some manual query business to grab the name from the Join table. This section should only get reached for old records,
    // where the player has not logged in for a while.
    List<BsonDocument> nameQuery = JoinCollection.Query()
        .Where(x => x.PlayerId.Equals(playerHostName))
        .OrderByDescending("Date")
        .Select("$.Name")
        .ToList();
    if (nameQuery.Count == 0)
    {
      if (DbConfig.DebugDatabaseMethods)
      {
        logger.LogDebug($"GetLatestNameForCharacterId {playerHostName} result = NONE");
      }
      return "undefined";
    }
    if (DbConfig.DebugDatabaseMethods)
    {
      logger.LogDebug($"nameQuery has {nameQuery.Count} results");
    }
    // simplify results to single record
    BsonDocument result = nameQuery[0];
    if (DbConfig.DebugDatabaseMethods)
    {
      logger.LogDebug($"GetLatestNameForCharacterId {playerHostName} result = {result}");
    }

    Task.Run(() =>
    {
      // Insert the player name, since we didn't have it in the player_name database!
      EnsurePlayerNameRecorded(result["Name"], playerHostName);
    }).ConfigureAwait(false);

    return result["Name"].AsString;
  }


  private struct JoinLeaveTime
  {
    public DateTime Time;
    public bool IsJoin;
  }

  /// <summary>
  /// Provides time online in seconds for all players within the date range provided. By default, it includes all time.
  ///
  /// This looks through the provided simple stat table and counts up time differences between joins and leaves.
  /// </summary>
  /// <param name="startDate">Date to start including records from</param>
  /// <param name="startDate">Date to stop including records before</param>
  /// <param name="inclusiveStart">Whether to include the start date or not in the returned results. If true, it will use `>=` for the startDate comparison; otherwise `>`.</param>
  /// <param name="inclusiveEnd">Whether to include the end date or not in the returned results. If true, it will use `<=` for the startDate comparison; otherwise `<`.</param>
  /// <returns>List of counts with CharacterName and Total (x) for the provided SimpleStat collection.</returns>
  private List<CountResult> TimeOnlineRecordsGrouped(DateTime? startDate = null, DateTime? endDate = null, bool inclusiveStart = true, bool inclusiveEnd = true)
  {

    PlayerToName[] players = PlayerToNameCollection.Query().ToArray();
    List<CountResult> results = new();

    foreach (PlayerToName player in players)
    {
      // Create a spot to record total online time for player.
      TimeSpan onlineTime = TimeSpan.FromSeconds(0.0);

      var joinsQuery = JoinCollection.Query().Where(x => x.PlayerId.Equals(player.PlayerId) && x.Name.Equals(player.CharacterName));
      var leavesQuery = LeaveCollection.Query().Where(x => x.PlayerId.Equals(player.PlayerId) && x.Name.Equals(player.CharacterName));

      Func<int, bool> startCompare = inclusiveStart ?
          (x) => x >= 0 :
          (x) => x > 0;
      Func<int, bool> endCompare = inclusiveEnd ?
          (x) => x <= 0 :
          (x) => x < 0;
      if (startDate != null)
      {
        joinsQuery = joinsQuery.Where(x => startCompare(x.Date.CompareTo(startDate.GetValueOrDefault())));
      }
      if (endDate != null)
      {
        joinsQuery = joinsQuery.Where(x => endCompare(x.Date.CompareTo(endDate.GetValueOrDefault())));
      }

      // Grab joins and leaves for player and sort them by time.
      JoinLeaveTime[] joins = Array.ConvertAll(joinsQuery.ToArray(),
          stat => new JoinLeaveTime { Time = stat.Date, IsJoin = true });
      JoinLeaveTime[] leaves = Array.ConvertAll(leavesQuery.ToArray(),
          stat => new JoinLeaveTime { Time = stat.Date, IsJoin = false });
      JoinLeaveTime[] sortedJoinLeaves = joins.Concat(leaves).OrderBy(j => j.Time).ToArray();

      logger.LogDebug($"{player.PlayerId} as {player.CharacterName} has {joins.Length} joins, {leaves.Length} leaves");

      DateTime? joinedTime = null;
      foreach (JoinLeaveTime joinLeaveTime in sortedJoinLeaves)
      {
        if (joinedTime == null)
        {
          // Player is not currently joined, so expecting a join.
          if (joinLeaveTime.IsJoin)
          {
            joinedTime = joinLeaveTime.Time;
          }
          else
          {
            logger.LogDebug($"Player {player.CharacterName} left at {joinLeaveTime.Time} but was not joined.");
          }
        }
        else
        {
          // Player is currently joined, expecting a leave.
          if (joinLeaveTime.IsJoin)
          {
            logger.LogDebug($"Player {player.CharacterName} joined at {joinLeaveTime.Time} but was already joined at {joinedTime}");
            joinedTime = joinLeaveTime.Time;
          }
          else
          {
            onlineTime += joinLeaveTime.Time.Subtract(joinedTime ?? joinLeaveTime.Time);
            joinedTime = null;
          }
        }
      }

      // Total time is then stored
      logger.LogDebug($"{onlineTime} total online time.");

      // Append to list
      results.Add(new CountResult(player.CharacterName, (int)onlineTime.TotalSeconds));
    }

    return results;
  }


  /// <summary>
  /// Converts a list of BSON documents with "player" and "count" values into our CountResult value.
  /// </summary>
  /// <param name="bsonDocuments">List of BSON with player and count values.</param>
  /// <returns>List of count results</returns>
  public List<CountResult> ConvertFromBsonDocuments(List<BsonDocument> bsonDocuments)
  {
    List<CountResult> results = [];

    if (DbConfig.DebugDatabaseMethods) { logger.LogDebug($"ConvertBsonDocumentCountToDotNet r={bsonDocuments.Count}"); }
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
              GetLatestCharacterNameForPlayer(doc["Player"]),
              doc["Count"].AsInt32
          ));
        }
      }
    }
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
  public int CountOfRecordsByName(Tables key, string playerName)
  {
    if (!DbConfig.CollectStatsEnabled)
    {
      return -1;
    }
    switch (key)
    {
      case Tables.DEATHS:
        return CountOfRecordsByName(DeathCollection, playerName);
      case Tables.JOINS:
        return CountOfRecordsByName(JoinCollection, playerName);
      case Tables.LEAVES:
        return CountOfRecordsByName(LeaveCollection, playerName);
      case Tables.PINGS:
        return CountOfRecordsByName(PingCollection, playerName);
      case Tables.SHOUTS:
        return CountOfRecordsByName(ShoutCollection, playerName);
      default:
        logger.LogDebug($"CountOfRecordsBySteamId, invalid key '{key}'");
        return -2;
    }
  }

  public List<CountResult> CountAllRecordsGrouped(Tables key, DateHelper.TimeRange timeRange)
  {
    if (timeRange == DateHelper.TimeRange.AllTime)
    {
      return CountAllRecordsGrouped(key);
    }

    Tuple<DateTime, DateTime> BeginEndDate = DateHelper.StartEndDatesForTimeRange(timeRange);
    return CountRecordsBetweenDatesGrouped(key, BeginEndDate.Item1, BeginEndDate.Item2);
  }

  public List<CountResult> CountRecordsBetweenDatesGrouped(Tables key, System.DateTime startDate, System.DateTime endDate, bool inclusiveStart = true, bool inclusiveEnd = true)
  {
    switch (key)
    {
      case Tables.DEATHS:
        return CountAllRecordsGroupsWhereDate(DeathCollection, startDate, endDate, inclusiveStart, inclusiveEnd);
      case Tables.JOINS:
        return CountAllRecordsGroupsWhereDate(JoinCollection, startDate, endDate, inclusiveStart, inclusiveEnd);
      case Tables.LEAVES:
        return CountAllRecordsGroupsWhereDate(LeaveCollection, startDate, endDate, inclusiveStart, inclusiveEnd);
      case Tables.PINGS:
        return CountAllRecordsGroupsWhereDate(PingCollection, startDate, endDate, inclusiveStart, inclusiveEnd);
      case Tables.SHOUTS:
        return CountAllRecordsGroupsWhereDate(ShoutCollection, startDate, endDate, inclusiveStart, inclusiveEnd);
      case Tables.TIME_ONLINE:
        return TimeOnlineRecordsGrouped(startDate, endDate, inclusiveStart, inclusiveEnd);
      default:
        logger.LogDebug($"CountTodaysRecordsGrouped, invalid key '{key}'");
        return [];
    }

  }

  public int CountOfRecordsByName(ILiteCollection<SimpleStat> collection, string playerName)
  {
    try
    {
      return collection.Query()
          .Where(x => x.Name.Equals(playerName))
          .Count();
    }
    catch
    {
      logger.LogDebug($"Error when trying to find {playerName} to count!");
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
  public List<CountResult> CountAllRecordsGrouped(ILiteCollection<SimpleStat> collection)
  {
    if (DbConfig.DebugDatabaseMethods)
    {
      logger.LogDebug($"CountAllRecordsGrouped {DbConfig.RecordRetrievalDiscernmentMethod}");
    }

    if (collection.Count() == 0)
    {
      if (DbConfig.DebugDatabaseMethods)
      {
        logger.LogDebug("Collection is empty, skipping.");
      }
      return [];
    }

    // Config.RetrievalDiscernmentMethods.BySteamID by default (should be most common), conditionally check for others
    string GroupByClause = "PlayerId";
    string SelectClause = "{Player: @Key, Count: Count(*)}";

    if (DbConfig.RecordRetrievalDiscernmentMethod == Config.RetrievalDiscernmentMethods.NameAndPlayerId)
    {
      GroupByClause = "{Name,PlayerId}";
      SelectClause = "{NamePlayer: @Key, Count: COUNT(*)}";
    }
    if (DbConfig.RecordRetrievalDiscernmentMethod == Config.RetrievalDiscernmentMethods.Name)
    {
      GroupByClause = "Name";
      SelectClause = "{Name: @Key, Count: Count(*)}";
    }

    List<CountResult> result = ConvertFromBsonDocuments(
        collection.Query()
            .GroupBy(GroupByClause)
            .Select(SelectClause)
            .ToList()
    );

    if (DbConfig.DebugDatabaseMethods)
    {
      logger.LogDebug($"CountAllRecordsGrouped {result.Count} records returned");
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
  public List<CountResult> CountAllRecordsGroupsWhereDate(ILiteCollection<SimpleStat> collection, System.DateTime startDate, System.DateTime endDate, bool inclusiveStart = true, bool inclusiveEnd = true)
  {
    if (DbConfig.DebugDatabaseMethods)
    {
      logger.LogDebug($"CountAllRecordsGroupsWhereDate {DbConfig.RecordRetrievalDiscernmentMethod} {startDate} {endDate}");
    }

    if (collection.Count() == 0)
    {
      if (DbConfig.DebugDatabaseMethods)
      {
        logger.LogDebug("Collection is empty, skipping.");
      }
      return [];
    }

    // Config.RetrievalDiscernmentMethods.BySteamID by default (should be most common), conditionally check for others
    string GroupByClause = "PlayerId";
    string SelectClause = "{Player: @Key, Count: Count(*)}";

    if (DbConfig.RecordRetrievalDiscernmentMethod == Config.RetrievalDiscernmentMethods.NameAndPlayerId)
    {
      GroupByClause = "{Name,PlayerId}";
      SelectClause = "{NamePlayer: @Key, Count: COUNT(*)}";
    }
    if (DbConfig.RecordRetrievalDiscernmentMethod == Config.RetrievalDiscernmentMethods.Name)
    {
      GroupByClause = "Name";
      SelectClause = "{Name: @Key, Count: Count(*)}";
    }

    List<CountResult> result;
    if (inclusiveStart && inclusiveEnd)
    {
      result = ConvertFromBsonDocuments(
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
      result = ConvertFromBsonDocuments(
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
      result = ConvertFromBsonDocuments(
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
      result = ConvertFromBsonDocuments(
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

    if (DbConfig.DebugDatabaseMethods)
    {
      logger.LogDebug($"CountAllRecordsGroupsWhereDate {result.Count} records returned");
    }

    return result;

  }

  /// <summary>
  /// Get totals for each player's tracked records
  /// </summary>
  /// <param name="key">Record type to return sums for</param>
  /// <returns>List of results with character names and totals</returns>
  public List<CountResult> CountAllRecordsGrouped(Tables key)
  {
    switch (key)
    {
      case Tables.DEATHS:
        return CountAllRecordsGrouped(DeathCollection);
      case Tables.JOINS:
        return CountAllRecordsGrouped(JoinCollection);
      case Tables.LEAVES:
        return CountAllRecordsGrouped(LeaveCollection);
      case Tables.PINGS:
        return CountAllRecordsGrouped(PingCollection);
      case Tables.SHOUTS:
        return CountAllRecordsGrouped(ShoutCollection);
      case Tables.TIME_ONLINE:
        return TimeOnlineRecordsGrouped();
      default:
        logger.LogDebug($"CountAllRecordsGrouped, invalid key '{key}'");
        return [];
    }
  }

}
