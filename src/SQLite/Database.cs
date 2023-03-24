using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace DiscordConnector.SQLite;

internal class Database
{
    private const string DB_NAME = "vdc_db.sqlite";
    private static string DbConnectionString;
    private SQLiteConnection connection;


    /// <summary>
    /// Set's up the database using the compiled string `"${PluginInfo.PLUGIN_ID}-records.db"`, which in this
    /// case is probably `games.nwest.valheim.discordconnector-records.db`. This method needs to know where to
    /// store the database, since that is something that is only known at runtime.
    /// </summary>
    public Database()
    {

        // Setup the connection string
        string dbPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, PluginInfo.PLUGIN_ID, DB_NAME);
        DbConnectionString = $"Data Source={dbPath}";

        // Open database connection
        connection = new SQLiteConnection(DbConnectionString).OpenAndReturn();
        Plugin.StaticLogger.LogInfo("Opened SQLite database connection");
    }

    /// <summary>
    /// Setup the database by validating the migrations. Closes the database if SQLite is disabled.
    /// </summary>
    public void Awake()
    {
        if (!Plugin.StaticConfig.SQLiteEnabled)
        {
            Plugin.StaticLogger.LogInfo("Enable the SQLite database to migrate and enable better record keeping.");
            connection.Close();
            Plugin.StaticLogger.LogInfo("Closed SQLite connection");
            return;
        }

        // Initialize (verify the migration status)
        Migrations.MigrationController.Migrate();
    }


    /// <summary>
    /// Closes the database connection nicely.
    /// </summary>
    public void Dispose()
    {
        if (Plugin.StaticConfig.SQLiteEnabled)
        {
            Plugin.StaticLogger.LogDebug("Closing SQLite connection");
            connection.Close();
        }
    }


    /// <summary>
    /// Executes a SQL statement that does not return a result set.
    /// </summary>
    /// <param name="sql">The SQL statement to execute.</param>
    internal void ExecuteNonQuery(string sql)
    {
        if (!Plugin.StaticConfig.SQLiteEnabled) { return; }

        using (var command = new SQLiteCommand(sql, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Get the direct SQLiteConnection object.
    /// </summary>
    internal SQLiteConnection GetSQLiteConnection()
    {
        return connection;
    }


    /// <summary>
    /// Check if a table exists in the SQLite database.
    /// </summary>
    /// <param name="tableName">The name of the table you want to check for.</param>
    internal bool TableExists(string tableName)
    {
        if (!Plugin.StaticConfig.SQLiteEnabled) { return false; }

        using (var command = new SQLiteCommand($"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'", connection))
        {
            using (var reader = command.ExecuteReader())
            {
                return reader.Read();
            }
        }
    }
}