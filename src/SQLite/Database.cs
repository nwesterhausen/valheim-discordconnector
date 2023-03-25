using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace DiscordConnector.SQLite;

/// <summary>
/// Valid categories to record stats in the database for.
/// </summary>
public enum StatCategory
{
    Join,
    Leave,
    Death,
    Ping,
    Shout,
}

/// <summary>
/// Class used to contain information destined for leaderboards and leaderboard creation.
/// </summary>
public class CountResult
{
    /// <summary>
    /// The player's name.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Total count of the events
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Constructor to create a CountResult (which is designed readonly)
    /// </summary>
    /// <param name="name">The player's name</param>
    /// <param name="count">Count of events</param>
    public CountResult(string name, int count)
    {
        Name = name;
        Count = count;
    }
}

/// <summary>
/// Database class which connects to and handles reading/writing to/from a SQLite database.
/// This is specifically to record player stats used by Discord Connector (joins, deaths, etc).
/// It also records players character names and host names (the PlatformID from UserInfo).
/// These are used to connect all the records together and to retrieve total counts used for leaderboards.
/// </summary>
internal class Database
{
    /// <summary>
    /// Name of the discord connector SQLite database
    /// </summary>
    private const string DB_NAME = "vdc_db.sqlite";
    /// <summary>
    /// Connection string used to connect to the database.
    /// </summary>
    private static string DbConnectionString;
    /// <summary>
    /// Reference for the SQLite database connection.
    /// </summary>
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

    /// <summary>
    /// It returns id of the player in the database given the peer.
    /// </summary>
    /// <param name="ZNetPeer">The peer that you want to get the player ID from.</param>
    internal int PlayerIdRefFromPeer(ZNetPeer peer)
    {
        // Guard against null peer
        if (peer == null) { return -1; }
        // Guard against null socket
        if (peer.m_socket == null) { return -1; }

        string playerHostName = peer.m_socket.GetHostName();
        string playerName = peer.m_playerName;

        return PlayerIdRefFromNameAndHostName(playerName, playerHostName);
    }


    /// <summary>
    /// It returns id of the player in the database given their character name and host name.
    /// </summary>
    /// <param name="playerName">The name of the player you want to get the ID of.</param>
    /// <param name="playerHostName">The hostname of the player.</param>
    internal int PlayerIdRefFromNameAndHostName(string playerName, string playerHostName)
    {
        string idRetrievalSql;

        // Assume is default case of playerId+playerName
        if (Plugin.StaticConfig.RecordRetrievalDiscernmentMethod == Config.MainConfig.RetrievalDiscernmentMethods.PlayerId)
        {
            idRetrievalSql = "SELECT id FROM players WHERE hostname=@hostname";
        }
        else if (Plugin.StaticConfig.RecordRetrievalDiscernmentMethod == Config.MainConfig.RetrievalDiscernmentMethods.Name)
        {
            idRetrievalSql = "SELECT id FROM players WHERE name=@name";
        }
        else
        {
            idRetrievalSql = "SELECT id FROM players WHERE name=@name AND hostname=@hostname";
        }

        // Default to a non-valid player id
        int matchingPlayerId = 0;

        using (var command = new SQLiteCommand(connection))
        {
            command.CommandType = CommandType.Text;
            command.CommandText = idRetrievalSql;

            if (idRetrievalSql.Contains("@name"))
            {
                command.Parameters.AddWithValue("@name", playerName);
            }
            if (idRetrievalSql.Contains("@hostname"))
            {
                command.Parameters.AddWithValue("@hostname", playerHostName);
            }

            var result = command.ExecuteScalar();
            if (result != DBNull.Value)
            {
                matchingPlayerId = Convert.ToInt32(result);
            }
        }

        if (matchingPlayerId == 0)
        {
            Plugin.StaticLogger.LogInfo($"Failure to match a player in the database for {playerHostName}:{playerName}");
            Plugin.StaticLogger.LogInfo($"Inserting new player record for player.");
            InsertPlayerIfNotExists(playerName, playerHostName);
            return PlayerIdRefFromNameAndHostName(playerName, playerHostName);
        }
        Plugin.StaticLogger.LogDebug($"Matched {playerHostName}:{playerName} to id:{matchingPlayerId}");
        return matchingPlayerId;
    }

    /// <summary>
    /// It inserts a join event into the game.
    /// </summary>
    /// <param name="playerId">The player's ID.</param>
    /// <param name="x">The x coordinate of the player</param>
    /// <param name="y">The yaw of the player.</param>
    /// <param name="z">The z-coordinate of the player.</param>
    internal void InsertJoin(int playerId, double x, double y, double z)
    {

        using (var command = new SQLiteCommand(connection))
        {
            command.CommandType = CommandType.Text;
            command.CommandText = @"
                    INSERT INTO joins (player_id, x, y, z)
                    VALUES (@playerId, @x, @y, @z);
                ";

            command.Parameters.AddWithValue("@playerId", playerId);
            command.Parameters.AddWithValue("@x", x);
            command.Parameters.AddWithValue("@y", y);
            command.Parameters.AddWithValue("@z", z);

            command.ExecuteNonQuery();
        }

    }
    /// <summary>
    /// Inserts a leave event into the database
    /// </summary>
    /// <param name="playerId">The player's ID.</param>
    /// <param name="x">The x coordinate of the player's position.</param>
    /// <param name="y">The y coordinate of the player.</param>
    /// <param name="z">The z coordinate of the player.</param>
    internal void InsertLeave(int playerId, double x, double y, double z)
    {

        using (var command = new SQLiteCommand(connection))
        {
            command.CommandType = CommandType.Text;
            command.CommandText = @"
                    INSERT INTO leaves (player_id, x, y, z)
                    VALUES (@playerId, @x, @y, @z);
                ";

            command.Parameters.AddWithValue("@playerId", playerId);
            command.Parameters.AddWithValue("@x", x);
            command.Parameters.AddWithValue("@y", y);
            command.Parameters.AddWithValue("@z", z);

            command.ExecuteNonQuery();
        }

    }
    /// <summary>
    /// Inserts a death event into the database
    /// </summary>
    /// <param name="playerId">The player's ID.</param>
    /// <param name="x">The x coordinate of the player's death</param>
    /// <param name="y">The y coordinate of the player's death.</param>
    /// <param name="z">The z coordinate of the player's death.</param>
    internal void InsertDeath(int playerId, double x, double y, double z)
    {

        using (var command = new SQLiteCommand(connection))
        {
            command.CommandType = CommandType.Text;
            command.CommandText = @"
                    INSERT INTO deaths (player_id, x, y, z)
                    VALUES (@playerId, @x, @y, @z);
                ";

            command.Parameters.AddWithValue("@playerId", playerId);
            command.Parameters.AddWithValue("@x", x);
            command.Parameters.AddWithValue("@y", y);
            command.Parameters.AddWithValue("@z", z);

            command.ExecuteNonQuery();
        }

    }

    /// <summary>
    /// This function inserts a shout event into the database
    /// </summary>
    /// <param name="playerId">The player's ID.</param>
    /// <param name="x">The x coordinate of the player who shouted.</param>
    /// <param name="y">The Y coordinate of the player.</param>
    /// <param name="z">The Z coordinate of the player.</param>
    /// <param name="text">The text of the shout.</param>
    internal void InsertShout(int playerId, double x, double y, double z, string text)
    {

        using (var command = new SQLiteCommand(connection))
        {
            command.CommandType = CommandType.Text;
            command.CommandText = @"
                    INSERT INTO shouts (player_id, x, y, z, text)
                    VALUES (@playerId, @x, @y, @z, @text);
                ";

            command.Parameters.AddWithValue("@playerId", playerId);
            command.Parameters.AddWithValue("@x", x);
            command.Parameters.AddWithValue("@y", y);
            command.Parameters.AddWithValue("@z", z);
            command.Parameters.AddWithValue("@text", text);

            command.ExecuteNonQuery();
        }

    }

    /// <summary>
    /// It inserts a ping event into the database.
    /// </summary>
    /// <param name="playerId">The player's ID.</param>
    /// <param name="x">The x coordinate of the player</param>
    /// <param name="y">The y coordinate of the player.</param>
    /// <param name="z">The z coordinate of the player.</param>
    /// <param name="pingX">The X coordinate of the ping</param>
    /// <param name="pingY">The Y coordinate of the ping.</param>
    /// <param name="pingZ">The Z coordinate of the ping.</param>
    internal void InsertPing(int playerId, double x, double y, double z, double pingX, double pingY, double pingZ)
    {

        using (var command = new SQLiteCommand(connection))
        {
            command.CommandType = CommandType.Text;
            command.CommandText = @"
                    INSERT INTO pings (player_id, x, y, z, ping_x, ping_y, ping_z)
                    VALUES (@playerId, @x, @y, @z, @pingX, @pingY, @pingZ);
                ";

            command.Parameters.AddWithValue("@playerId", playerId);
            command.Parameters.AddWithValue("@x", x);
            command.Parameters.AddWithValue("@y", y);
            command.Parameters.AddWithValue("@z", z);
            command.Parameters.AddWithValue("@pingX", pingX);
            command.Parameters.AddWithValue("@pingY", pingY);
            command.Parameters.AddWithValue("@pingZ", pingZ);

            command.ExecuteNonQuery();
        }

    }

    /// <summary>
    /// It returns a list of CountResult objects for the given category.
    /// </summary>
    /// <param name="StatCategory">The category of stats you want to get.</param>
    public List<CountResult> GetStatCounts(StatCategory category)
    {
        string tableName;
        try
        {
            tableName = TableNameFromCategory(category);
        }
        catch (ArgumentException ex)
        {
            // Handle the exception, e.g., log it, display a message to the user, or rethrow it
            Plugin.StaticLogger.LogWarning($"GetStatCounts failure: {ex.Message}");
            return new List<CountResult>();
        }

        var results = new List<CountResult>();


        using (var command = new SQLiteCommand(connection))
        {
            command.CommandType = CommandType.Text;
            command.CommandText = $@"
                    SELECT p.name AS playerName, COUNT(t.id) AS totalCount
                    FROM players p
                    LEFT JOIN {tableName} t ON p.id = t.player_id
                    GROUP BY p.id;
                ";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string playerName = reader["playerName"].ToString();
                    int totalCount = Convert.ToInt32(reader["totalCount"]);
                    results.Add(new CountResult(playerName, totalCount));
                }
            }
        }


        return results;
    }

    /// <summary>
    /// Record a new stat record for the given stat category and player.
    /// </summary>
    /// <param name="StatCategory">The category of the stat.</param>
    /// <param name="ZNetPeer">The peer that you want to record the stat for.</param>
    /// <param name="x">The x coordinate of the player's position.</param>
    /// <param name="y">The y-coordinate of the player's position.</param>
    /// <param name="z">The Z coordinate of the player.</param>
    /// <param name="pingX">The X coordinate of the ping location.</param>
    /// <param name="pingY">The Y coordinate of the ping.</param>
    /// <param name="pingZ">The Z coordinate of the ping.</param>
    /// <param name="shoutText">The text that the player shouted.</param>
    public void Record(StatCategory category, ZNetPeer peer, double pingX = 0.0, double pingY = 0.0, double pingZ = 0.0, string shoutText = null)
    {
        int playerId = PlayerIdRefFromPeer(peer);
        Record(category, playerId, peer.m_refPos.x, peer.m_refPos.y, peer.m_refPos.z, pingX, pingY, pingZ, shoutText);
    }

    /// <summary>
    /// Record a new stat record for the given stat category and player.
    /// </summary>
    /// <param name="StatCategory">The category of the stat.</param>
    /// <param name="playerName">The name of the player.</param>
    /// <param name="playerHostname">The hostname of the player.</param>
    /// <param name="x">The x coordinate of the player.</param>
    /// <param name="y">The category of the stat.</param>
    /// <param name="z">The Z coordinate of the player.</param>
    /// <param name="pingX">The X coordinate of the player's ping.</param>
    /// <param name="pingY">The Y coordinate of the player's ping.</param>
    /// <param name="pingZ">The Z coordinate of the player's ping.</param>
    /// <param name="shoutText">The text that the player shouted.</param>
    public void Record(StatCategory category, string playerName, string playerHostname, double x, double y, double z, double pingX = 0.0, double pingY = 0.0, double pingZ = 0.0, string shoutText = null)
    {
        int playerId = PlayerIdRefFromNameAndHostName(playerName, playerHostname);
        Record(category, playerId, x, y, z, pingX, pingY, pingZ, shoutText);
    }

    /// <summary>
    /// Record a new stat record for the given stat category and player.
    /// </summary>
    /// <param name="StatCategory">The category of the stat.</param>
    /// <param name="playerId">The player's ID.</param>
    /// <param name="x">The x coordinate of the player.</param>
    /// <param name="y">The y coordinate of the player.</param>
    /// <param name="z">The z-coordinate of the player.</param>
    /// <param name="pingX">The x coordinate of the ping.</param>
    /// <param name="pingY">The Y coordinate of the ping.</param>
    /// <param name="pingZ">The Z coordinate of the ping.</param>
    /// <param name="shoutText">The text that the player shouts.</param>
    public void Record(StatCategory category, int playerId, double x, double y, double z, double pingX = 0.0, double pingY = 0.0, double pingZ = 0.0, string shoutText = null)
    {
        switch (category)
        {
            case StatCategory.Join:
                InsertJoin(playerId, x, y, z);
                break;
            case StatCategory.Leave:
                InsertLeave(playerId, x, y, z);
                break;
            case StatCategory.Death:
                InsertDeath(playerId, x, y, z);
                break;
            case StatCategory.Ping:
                InsertPing(playerId, x, y, z, pingX, pingY, pingZ);
                break;
            case StatCategory.Shout:
                if (shoutText == null)
                {
                    Plugin.StaticLogger.LogWarning("The Shout category requires the 'shoutText' parameter.");
                    return;
                }
                InsertShout(playerId, x, y, z, shoutText);
                break;
            default:
                Plugin.StaticLogger.LogWarning($"Invalid StatCategory: {category}");
                return;
        }
    }


    /// <summary>
    /// It returns the total time a player has been online.
    /// </summary>
    /// <param name="playerId">The player's ID.</param>
    internal TimeSpan GetTotalTimeOnline(int playerId)
    {
        TimeSpan totalTime = TimeSpan.Zero;


        using (var command = new SQLiteCommand(connection))
        {
            command.CommandType = CommandType.Text;
            command.CommandText = @"
                    SELECT j.joined_at AS joinTime, l.left_at AS leaveTime
                    FROM joins j
                    INNER JOIN leaves l ON j.player_id = l.player_id AND l.left_at > j.joined_at
                    WHERE j.player_id = @playerId
                    ORDER BY j.joined_at, l.left_at;
                ";

            command.Parameters.AddWithValue("@playerId", playerId);

            using (var reader = command.ExecuteReader())
            {
                DateTime? lastJoin = null;
                DateTime? lastLeave = null;

                while (reader.Read())
                {
                    DateTime joinTime = Convert.ToDateTime(reader["joinTime"]);
                    DateTime leaveTime = Convert.ToDateTime(reader["leaveTime"]);

                    if (lastJoin == null || joinTime > lastLeave)
                    {
                        lastJoin = joinTime;
                    }

                    if (lastLeave == null || leaveTime > lastLeave)
                    {
                        lastLeave = leaveTime;
                        totalTime += lastLeave.Value - lastJoin.Value;
                        lastJoin = null;
                    }
                }
            }
        }

        return totalTime;
    }

    /// <summary>
    /// Inserts a player into the database if the player does not already exist.
    /// </summary>
    /// <param name="ZNetPeer">The peer for the player that you want to insert into the database.</param>
    public void InsertPlayerIfNotExists(ZNetPeer peer)
    {
        // Guard against null peer
        if (peer == null) { return; }
        // Guard against null socket
        if (peer.m_socket == null) { return; }

        string playerHostName = peer.m_socket.GetHostName();
        string playerName = peer.m_playerName;

        InsertPlayerIfNotExists(playerName, playerHostName);
    }

    /// <summary>
    /// Inserts a player into the database if the player does not already exist.
    /// </summary>
    /// <param name="name">The name of the player.</param>
    /// <param name="hostname">The hostname of the server.</param>
    public void InsertPlayerIfNotExists(string name, string hostname)
    {

        using (var command = new SQLiteCommand(connection))
        {
            command.CommandType = CommandType.Text;
            command.CommandText = @"
                INSERT INTO players (name, hostname)
                SELECT @name, @hostname
                WHERE NOT EXISTS (
                    SELECT 1 FROM players WHERE name = @name AND hostname = @hostname
                );
            ";

            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@hostname", hostname);

            command.ExecuteNonQuery();
        }

    }

    /// <summary>
    /// Checks if this is the first event in the given category for the player.
    /// </summary>
    /// <param name="category">The category of the stat you want to check.</param>
    /// <param name="peer">The peer connection of the player</param>
    internal bool IsFirstForPlayer(StatCategory category, ZNetPeer peer)
    {
        int playerId = PlayerIdRefFromPeer(peer);
        return IsFirstForPlayer(category, playerId);
    }


    /// <summary>
    /// Checks if this is the first event in the given category for the player.
    /// </summary>
    /// <param name="category">The category of the stat.</param>
    /// <param name="playerName">The name of the player.</param>
    /// <param name="playerHostName">The hostname of the player.</param>
    internal bool IsFirstForPlayer(StatCategory category, string playerName, string playerHostName)
    {
        int playerId = PlayerIdRefFromNameAndHostName(playerName, playerHostName);
        return IsFirstForPlayer(category, playerId);
    }

    /// <summary>
    /// Checks if this is the first event in the given category for the player.
    /// </summary>
    /// <param name="category">The category of the stat you want to check.</param>
    /// <param name="playerId">The player's ID.</param>
    internal bool IsFirstForPlayer(StatCategory category, int playerId)
    {
        string tableName;
        try
        {
            tableName = TableNameFromCategory(category);
        }
        catch (ArgumentException ex)
        {
            // Handle the exception, e.g., log it, display a message to the user, or rethrow it
            Plugin.StaticLogger.LogWarning($"IsFirstForPlayer failure: {ex.Message}");
            return false;
        }


        using (var command = new SQLiteCommand(connection))
        {
            command.CommandType = CommandType.Text;
            command.CommandText = $@"
                SELECT COUNT(*)
                FROM {tableName}
                WHERE player_id = @playerId;
            ";

            command.Parameters.AddWithValue("@playerId", playerId);

            int count = Convert.ToInt32(command.ExecuteScalar());
            return count == 0;
        }
    }


    /// <summary>
    /// It returns the name of the table that the category is stored in.
    /// </summary>
    /// <param name="StatCategory">The category of the stat you want to get the table name for.</param>
    public string TableNameFromCategory(StatCategory category)
    {
        switch (category)
        {
            case StatCategory.Join:
                return "joins";
            case StatCategory.Leave:
                return "leaves";
            case StatCategory.Death:
                return "deaths";
            case StatCategory.Ping:
                return "pings";
            case StatCategory.Shout:
                return "shouts";
            default:
                throw new ArgumentException($"Invalid StatCategory: {category}");
        }
    }
}
