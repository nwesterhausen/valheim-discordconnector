using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace DiscordConnector.SQLite.Migrations;

/// <summary>
/// Interface to facilitate schema migrations.
/// </summary>
interface IMigration
{
    string MigrateDown { get; }
    string MigrateUp { get; }
    int MigrationId { get; }
}

/// <summary>
/// Controller to handle the database schema migrations.
/// </summary>
internal class MigrationController
{
    /// <summary>
    /// Migrates the database to the latest version.
    /// </summary>
    public static void Migrate()
    {
        // Check if versions table exists
        if (!Plugin.StaticDatabaseNEW.TableExists("versions"))
        {
            // Create versions table if it doesn't exist
            Plugin.StaticDatabaseNEW.ExecuteNonQuery("CREATE TABLE versions (id INTEGER PRIMARY KEY, migration_name TEXT NOT NULL, migrated_at TEXT NOT NULL)");
        }

        // Get reference to the db connection to perform migrations
        SQLiteConnection dbConn = Plugin.StaticDatabaseNEW.GetSQLiteConnection();

        // Get the highest migration id from the versions table
        int highestMigrationId = GetHighestMigrationId(dbConn);

        // Get a list of all migrations that need to be applied
        List<IMigration> migrationsToApply = GetMigrationsToApply(highestMigrationId);

        if (migrationsToApply.Count == 0)
        {
            Plugin.StaticLogger.LogInfo("SQLite database up to date at latest schema.");
        }

        // Apply the migrations
        foreach (IMigration migration in migrationsToApply)
        {
            Plugin.StaticLogger.LogInfo($"Applying sqlite migration {migration.MigrationId}");
            Plugin.StaticLogger.LogDebug($"migration sqlite: {migration.MigrateUp}");

            using (var command = new SQLiteCommand(migration.MigrateUp, dbConn))
            {
                command.ExecuteNonQuery();
            }

            // Add migration to the versions table
            using (var command = new SQLiteCommand("INSERT INTO versions (id, migration_name, migrated_at) VALUES (@id, @migration_name, @migrated_at)", dbConn))
            {
                command.Parameters.AddWithValue("@id", migration.MigrationId);
                command.Parameters.AddWithValue("@migration_name", migration.GetType().Name);
                command.Parameters.AddWithValue("@migrated_at", DateTime.Now.ToString());
                command.ExecuteNonQuery();
            }
        }

        Plugin.StaticLogger.LogInfo("All sqlite migrations have been applied.");
    }


    /// <summary>
    /// Get the highest id of applied migrations.
    /// </summary>
    /// <param name="SQLiteConnection">The connection to the database.</param>
    private static int GetHighestMigrationId(SQLiteConnection connection)
    {
        int highestMigrationId = 0;
        Plugin.StaticLogger.LogDebug("sqlite: Checking for applied migrations");
        using (var command = new SQLiteCommand("SELECT MAX(id) FROM versions", connection))
        {
            var result = command.ExecuteScalar();
            if (result != DBNull.Value)
            {
                highestMigrationId = Convert.ToInt32(result);
            }
        }

        Plugin.StaticLogger.LogDebug($"sqlite: Highest migration is {highestMigrationId}");
        return highestMigrationId;
    }

    /// <summary>
    /// Get a list of all the valid migrations to apply. Should be all migrations with ID > `highestMigrationId`.
    /// </summary>
    /// <param name="highestMigrationId">The highest migration id that has been applied to the
    /// database.</param>
    private static List<IMigration> GetMigrationsToApply(int highestMigrationId)
    {
        var migrations = new List<IMigration>();

        // Add all migrations with id greater than the highest migration id
        foreach (var migration in GetAllMigrations())
        {
            if (migration.MigrationId > highestMigrationId)
            {
                migrations.Add(migration);
            }
        }

        // Sort the migrations by id
        migrations.Sort((x, y) => x.MigrationId.CompareTo(y.MigrationId));

        Plugin.StaticLogger.LogDebug($"sqlite: {migrations.Count} valid migrations");
        return migrations;
    }

    /// <summary>
    /// Get a list of all the available migrations
    /// </summary>
    private static List<IMigration> GetAllMigrations()
    {
        var migrations = new List<IMigration>();

        // Add all migrations
        migrations.Add(new Migration_01());


        Plugin.StaticLogger.LogDebug($"sqlite: {migrations.Count} total available migrations");
        return migrations;
    }
}
