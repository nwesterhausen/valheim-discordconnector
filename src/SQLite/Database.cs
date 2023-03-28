using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DiscordConnector.SQLite.Repositories;
using SQLite;

namespace DiscordConnector.SQLite;
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
    private const string DB_NAME = "discordconnector-db.sqlite";
    /// <summary>
    /// Connection string used to connect to the database.
    /// </summary>
    private static string DbConnectionString;
    /// <summary>
    /// Reference for the SQLite database connection.
    /// </summary>
    private SQLiteConnection connection;

    private PlayerRepository playerRepository;
    private JoinRepository joinRepository;
    private LeaveRepository leaveRepository;
    private DeathRepository deathRepository;
    private PingRepository pingRepository;
    private ShoutRepository shoutRepository;


    /// <summary>
    /// Set's up the database using the compiled string `"${PluginInfo.PLUGIN_ID}-records.db"`, which in this
    /// case is probably `games.nwest.valheim.discordconnector-records.db`. This method needs to know where to
    /// store the database, since that is something that is only known at runtime.
    /// </summary>
    public Database()
    {
        if (!Plugin.StaticConfig.SQLiteEnabled)
        {
            Plugin.StaticLogger.LogInfo("Enable the SQLite database to migrate and enable better record keeping.");
            return;
        }

        // Setup the connection string
        string dbPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, PluginInfo.PLUGIN_ID, DB_NAME);
        DbConnectionString = $"Data Source={dbPath}";

        // Open database connection
        connection = new SQLiteConnection(DbConnectionString);

        playerRepository = new PlayerRepository(connection);
        joinRepository = new JoinRepository(connection);
        leaveRepository = new LeaveRepository(connection);
        deathRepository = new DeathRepository(connection);
        pingRepository = new PingRepository(connection);
        shoutRepository = new ShoutRepository(connection);
    }

    /// <summary>
    /// Setup the database by validating the migrations. Closes the database if SQLite is disabled.
    /// </summary>
    public void Awake()
    {
        if (!Plugin.StaticConfig.SQLiteEnabled)
        {
            Plugin.StaticLogger.LogInfo("Enable the SQLite database to migrate and enable better record keeping.");
            return;
        }

        // Migrate data from LiteDB
        var liteDbMigrator = new LiteDbMigrator(connection);
        liteDbMigrator.Migrate();
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
    /// It returns a list of CountResult objects for the given category.
    /// </summary>
    /// <param name="StatCategory">The category of stats you want to get.</param>
    public List<CountResult> GetStatCounts(StatCategory category)
    {
        switch (category)
        {
            case StatCategory.Join:
                return joinRepository.GetCountResults();
            case StatCategory.Leave:
                return leaveRepository.GetCountResults();
            case StatCategory.Death:
                return deathRepository.GetCountResults();
            case StatCategory.Ping:
                return pingRepository.GetCountResults();
            case StatCategory.Shout:
                return shoutRepository.GetCountResults();
            default:
                Plugin.StaticLogger.LogWarning($"Invalid StatCategory: {category}");
                return new List<CountResult>();
        }
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
        if (peer == null || peer.m_socket == null)
        {
            Plugin.StaticLogger.LogDebug("Unable to insert new {category} record for null peer");
            return;
        }

        string playerName = peer.m_playerName;
        string playerHostName = peer.m_socket.GetHostName();

        Record(category, playerName, playerHostName, peer.m_refPos.x, peer.m_refPos.y, peer.m_refPos.z, pingX, pingY, pingZ, shoutText);
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
        int playerId = playerRepository.GetIdByNameAndHostname(playerName, playerHostname);

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
                joinRepository.Insert(playerId, x, y, z);
                break;
            case StatCategory.Leave:
                leaveRepository.Insert(playerId, x, y, z);
                break;
            case StatCategory.Death:
                deathRepository.Insert(playerId, x, y, z);
                break;
            case StatCategory.Ping:
                pingRepository.Insert(playerId, x, y, z, pingX, pingY, pingZ);
                break;
            case StatCategory.Shout:
                shoutRepository.Insert(playerId, x, y, z, shoutText);
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

        var joinTimes = joinRepository.GetByPlayerId(playerId).OrderBy(j => j.JoinedAt).ToList();
        var leaveTimes = leaveRepository.GetByPlayerId(playerId).OrderBy(l => l.LeftAt).ToList();

        DateTime? lastJoin = null;
        DateTime? lastLeave = null;

        foreach (var joinTime in joinTimes)
        {
            if (lastJoin == null || joinTime.JoinedAt > lastLeave)
            {
                lastJoin = joinTime.JoinedAt;
            }

            var matchingLeave = leaveTimes.FirstOrDefault(l => l.LeftAt > joinTime.JoinedAt);

            if (matchingLeave != null)
            {
                lastLeave = matchingLeave.LeftAt;
                totalTime += lastLeave.Value - lastJoin.Value;
                lastJoin = null;

                leaveTimes.Remove(matchingLeave);
            }
        }

        return totalTime;
    }


    /// <summary>
    /// Inserts a player into the database if the player does not already exist.
    /// </summary>
    /// <param name="ZNetPeer">The peer for the player that you want to insert into the database.</param>
    /// <returns>The ID of the player, creating a new record if they don't exist, or 0 if insertion failed</returns>
    public int InsertPlayerIfNotExists(ZNetPeer peer)
    {
        // Guard against null peer
        if (peer == null) { return 0; }
        // Guard against null socket
        if (peer.m_socket == null) { return 0; }

        string playerHostName = peer.m_socket.GetHostName();
        string playerName = peer.m_playerName;

        return InsertPlayerIfNotExists(playerName, playerHostName);
    }

    /// <summary>
    /// Inserts a player into the database if the player does not already exist.
    /// </summary>
    /// <param name="name">The name of the player.</param>
    /// <param name="hostname">The hostname of the server.</param>
    /// <returns>The ID of the player, creating a new record if they don't exist, or 0 if insertion failed</returns>
    public int InsertPlayerIfNotExists(string name, string hostname)
    {
        // Guard against null parameters
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(hostname))
        {
            Plugin.StaticLogger.LogWarning($"InsertPlayerIfNotExists: name or hostname is null or empty (name: {name}, hostname: {hostname})");
            return 0;
        }

        return playerRepository.Insert(name, hostname);
    }


    /// <summary>
    /// Checks if this is the first event in the given category for the player.
    /// </summary>
    /// <param name="category">The category of the stat you want to check.</param>
    /// <param name="peer">The peer connection of the player</param>
    internal bool IsFirstForPlayer(StatCategory category, ZNetPeer peer)
    {
        if (peer == null || peer.m_socket == null)
        {
            return false;
        }

        string playerName = peer.m_playerName;
        string playerHostname = peer.m_socket.GetHostName();

        return IsFirstForPlayer(category, playerName, playerHostname);
    }


    /// <summary>
    /// Checks if this is the first event in the given category for the player.
    /// </summary>
    /// <param name="category">The category of the stat.</param>
    /// <param name="playerName">The name of the player.</param>
    /// <param name="playerHostName">The hostname of the player.</param>
    internal bool IsFirstForPlayer(StatCategory category, string playerName, string playerHostName)
    {
        int playerId = playerRepository.GetIdByNameAndHostname(playerName, playerHostName);

        return IsFirstForPlayer(category, playerId);
    }

    /// <summary>
    /// Checks if this is the first event in the given category for the player.
    /// </summary>
    /// <param name="category">The category of the stat you want to check.</param>
    /// <param name="playerId">The player's ID.</param>
    internal bool IsFirstForPlayer(StatCategory category, int playerId)
    {
        Plugin.StaticLogger.LogDebug($"Checking if total {category} for player:{playerId} is 0");
        switch (category)
        {
            case StatCategory.Join:
                return joinRepository.GetByPlayerId(playerId).Count == 0;
            case StatCategory.Leave:
                return leaveRepository.GetByPlayerId(playerId).Count == 0;
            case StatCategory.Death:
                return deathRepository.GetByPlayerId(playerId).Count == 0;
            case StatCategory.Ping:
                return pingRepository.GetByPlayerId(playerId).Count == 0;
            case StatCategory.Shout:
                return shoutRepository.GetByPlayerId(playerId).Count == 0;
            default:
                Plugin.StaticLogger.LogWarning($"Invalid StatCategory: {category}");
                return false;
        }
    }
}
