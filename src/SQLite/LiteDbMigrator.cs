using System;
using System.Collections.Generic;
using System.Data;
using DiscordConnector.SQLite.Models;
using DiscordConnector.SQLite.Repositories;
using LiteDB;
using SQLite;

namespace DiscordConnector.SQLite;

/// <summary>
/// Class to facilitate the migration from LiteDB to SQLite.
/// 
/// Contains the "PerformLiteDbMigration" method which:
/// 
/// 1. Checks for the liteDB database under the expected name
/// 2. Opens it
/// 3. 
/// </summary>
internal class LiteDbMigrator
{
    private const string LITE_DB_NAME = "records.db";
    private const string MIGRATED_LITE_DB_NAME = "records.db.migrated";
    private SQLiteConnection sqliteConnection;
    private string liteDbPath;
    private string migratedLiteDbPath;
    private LiteDatabase liteDb;

    private PlayerRepository playerRepository;
    private JoinRepository joinRepository;
    private LeaveRepository leaveRepository;
    private DeathRepository deathRepository;
    private PingRepository pingRepository;
    private ShoutRepository shoutRepository;

    internal LiteDbMigrator(SQLiteConnection connection)
    {
        sqliteConnection = connection;

        liteDbPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, PluginInfo.PLUGIN_ID, LITE_DB_NAME);
        migratedLiteDbPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, PluginInfo.PLUGIN_ID, MIGRATED_LITE_DB_NAME);

        // Check if the records.db exists. If it doesn't then we don't migrate.
        if (!System.IO.File.Exists(liteDbPath))
        {
            Plugin.StaticLogger.LogDebug("liteDB migrate: did not detect liteDB database.");
            return;
        }
        liteDb = new LiteDatabase(liteDbPath);

        playerRepository = new PlayerRepository(connection);
        joinRepository = new JoinRepository(connection);
        leaveRepository = new LeaveRepository(connection);
        deathRepository = new DeathRepository(connection);
        pingRepository = new PingRepository(connection);
        shoutRepository = new ShoutRepository(connection);
    }
    public void Migrate()
    {
        // Check if the records.db exists. If it doesn't then we don't migrate.
        if (!System.IO.File.Exists(liteDbPath))
        {
            Plugin.StaticLogger.LogDebug("liteDB migrate: did not detect liteDB database.");
            return;
        }

        // Get the LiteDB 'player_name' collection
        var playerNameCollection = liteDb.GetCollection<BsonDocument>("player_name");

        // Dictionary to store playerName and playerHostName => playerId mapping
        var playerHostnamesToIds = new Dictionary<string, int>();

        // Populate SQLite 'players' table and create the player mapping dictionary
        foreach (var playerRecord in playerNameCollection.FindAll())
        {
            string playerName = playerRecord["CharacterName"].AsString;
            string playerHostName = playerRecord["PlayerId"].AsString;

            // Insert player into SQLite 'players' table and get playerId
            int playerId = Plugin.StaticDatabaseNEW.InsertPlayerIfNotExists(playerName, playerHostName);

            // Add player to the dictionary
            string playerKey = $"{playerHostName}:{playerName}";
            playerHostnamesToIds[playerKey] = playerId;
        }
        Plugin.StaticLogger.LogInfo($"Migrated {playerHostnamesToIds.Count} player identity values");

        // Migrate each collection from LiteDB to SQLite
        MigrateCollection(playerHostnamesToIds, "joins", "joins", "joined_at");
        MigrateCollection(playerHostnamesToIds, "deaths", "deaths", "died_at");
        MigrateCollection(playerHostnamesToIds, "leaves", "leaves", "left_at");
        MigrateCollection(playerHostnamesToIds, "shouts", "shouts", "shouted_at");
        MigrateCollection(playerHostnamesToIds, "pings", "pings", "pinged_at");

        // Close the database connections (the SQLite connection is borrowed)
        liteDb.Dispose();

        // Move the liteDB database
        System.IO.File.Move(liteDbPath, migratedLiteDbPath);

        Plugin.StaticLogger.LogInfo("Completed migration of data from LiteDB to SQLite.");
    }

    /// <summary>
    /// Migrate a collection of SimpleStat records into the new SQLite database.
    /// </summary>
    /// <param name="playerHostnamesToIds">A dictionary of player name:hostnames to player IDs.</param>
    /// <param name="collectionName">The name of the collection in the LiteDB database.</param>
    /// <param name="tableName">The name of the table in the SQLite database.</param>
    /// <param name="timestampColumn">The name of the column that contains the timestamp in the SQLite database.</param>
    private void MigrateCollection(Dictionary<string, int> playerHostnamesToIds, string collectionName, string tableName, string timestampColumn)
    {
        // Get the LiteDB collection
        var currentCollection = liteDb.GetCollection<BsonDocument>(collectionName);

        Plugin.StaticLogger.LogInfo($"Migrating {currentCollection.Count()} {collectionName} records from LiteDB to SQLite");

        // Iterate through LiteDB records
        foreach (var record in currentCollection.FindAll())
        {
            string playerName = record["Name"].AsString;
            string playerHostName = record["PlayerId"].AsString;

            // Get playerId
            int current_playerId = playerRepository.GetIdByNameAndHostname(playerName, playerHostName);

            // Guard against bad inserts
            if (current_playerId == 0)
            {
                Plugin.StaticLogger.LogWarning($"liteDb migrate: failure inserting '{collectionName}' record on bad player info {playerHostName}:{playerName}");
                continue;
            }

            // Extract position and time information from the record
            double x = record["Pos"]["x"].AsDouble;
            double y = record["Pos"]["y"].AsDouble;
            double z = record["Pos"]["z"].AsDouble;
            DateTime timestamp = record["Date"].AsDateTime;

            // Insert data into SQLite using the repositories
            switch (tableName)
            {
                case "joins":
                    joinRepository.Insert(new Join
                    {
                        PlayerId = current_playerId,
                        X = x,
                        Y = y,
                        Z = z,
                        JoinedAt = timestamp
                    });
                    break;
                case "leaves":
                    leaveRepository.Insert(new Leave
                    {
                        PlayerId = current_playerId,
                        X = x,
                        Y = y,
                        Z = z,
                        LeftAt = timestamp
                    });
                    break;
                case "deaths":
                    deathRepository.Insert(new Death
                    {
                        PlayerId = current_playerId,
                        X = x,
                        Y = y,
                        Z = z,
                        DiedAt = timestamp
                    });
                    break;
                case "pings":
                    pingRepository.Insert(new Ping
                    {
                        PlayerId = current_playerId,
                        X = 0,
                        Y = 0,
                        Z = 0,
                        PingX = x,
                        PingY = y,
                        PingZ = z,
                        PingedAt = timestamp
                    });
                    break;
                case "shouts":
                    shoutRepository.Insert(new Shout
                    {
                        PlayerId = current_playerId,
                        X = x,
                        Y = y,
                        Z = z,
                        Text = "",
                        ShoutedAt = timestamp
                    });
                    break;
                default:
                    Plugin.StaticLogger.LogDebug($"litedb migrate: Ignored invalid table name {tableName}");
                    break;
            }
        }

        Plugin.StaticLogger.LogInfo($"Migration of {collectionName} completed.");
    }

}