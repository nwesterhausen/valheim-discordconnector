using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DiscordConnector.Records;
using LiteDB;

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

    internal LiteDbMigrator()
    {
        sqliteConnection = Plugin.StaticDatabaseNEW.GetSQLiteConnection();

        liteDbPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, PluginInfo.PLUGIN_ID, LITE_DB_NAME);
        migratedLiteDbPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, PluginInfo.PLUGIN_ID, MIGRATED_LITE_DB_NAME);

        // Check if the records.db exists. If it doesn't then we don't migrate.
        if (!System.IO.File.Exists(liteDbPath))
        {
            Plugin.StaticLogger.LogDebug("liteDB migrate: did not detect liteDB database.");
            return;
        }
        liteDb = new LiteDatabase(liteDbPath);
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

        // Iterate through LiteDB records
        foreach (var record in currentCollection.FindAll())
        {
            string playerName = record["Name"].AsString;
            string playerHostName = record["PlayerId"].AsString;

            string playerKey = $"{playerHostName}:{playerName}";

            // Check if player key exists in dictionary
            if (!playerHostnamesToIds.ContainsKey(playerKey))
            {
                // Insert player into SQLite 'players' table and get playerId
                int playerId = Plugin.StaticDatabaseNEW.InsertPlayerIfNotExists(playerName, playerHostName);

                // Guard against bad inserts
                if (playerId == 0)
                {
                    Plugin.StaticLogger.LogWarning($"liteDb migrate: failure inserting '{collectionName}' record on bad player info {playerKey}");
                    continue;
                }

                // Add player to the dictionary
                playerHostnamesToIds[playerKey] = playerId;
            }

            // Get playerId from the dictionary
            int current_playerId = playerHostnamesToIds[playerKey];

            // Extract position and time information from the record
            double x = record["Pos"]["x"].AsDouble;
            double y = record["Pos"]["y"].AsDouble;
            double z = record["Pos"]["z"].AsDouble;
            DateTime timestamp = record["Date"].AsDateTime;

            // Prepare the SQL command
            using (var command = new SQLiteCommand(sqliteConnection))
            {
                command.CommandType = CommandType.Text;
                command.CommandText = $@"
                INSERT INTO {tableName} (player_id, x, y, z, {timestampColumn})
                VALUES (@playerId, @x, @y, @z, @timestamp);
            ";
                // Modify command if destination is the 'pings' table
                if (tableName.Equals("pings"))
                {
                    command.CommandText = $@"
                INSERT INTO {tableName} (player_id, x, y, z, ping_x, ping_y, ping_z, {timestampColumn})
                VALUES (@playerId, 0.0, 0.0, 0.0, @x, @y, @z, @timestamp);
            ";
                }
                else if (tableName.Equals("shouts"))
                {
                    command.CommandText = $@"
                INSERT INTO {tableName} (player_id, x, y, z, text, {timestampColumn})
                VALUES (@playerId, @x, @y, @z, @shoutText, @timestamp);
            ";
                    command.Parameters.AddWithValue("@shoutText", "");
                }

                command.Parameters.AddWithValue("@playerId", current_playerId);
                command.Parameters.AddWithValue("@x", x);
                command.Parameters.AddWithValue("@y", y);
                command.Parameters.AddWithValue("@z", z);
                command.Parameters.AddWithValue("@timestamp", timestamp);

                command.ExecuteNonQuery();
            }
        }
    }

}