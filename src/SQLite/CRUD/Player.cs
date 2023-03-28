using System.Collections.Generic;
using SQLite;

namespace DiscordConnector.SQLite.Repositories;
public class PlayerRepository
{
    private SQLiteConnection _connection;

    public PlayerRepository(SQLiteConnection connection)
    {
        _connection = connection;
        _connection.CreateTable<Player>();
    }

    // Create
    public int Insert(string playerName, string playerHostname)
    {
        Models.Player player = new Models.Player()
        {
            Name = playerName,
            Hostname = playerHostname,
        };

        return _connection.Insert(player);
    }
    public int Insert(Models.Player player)
    {
        return _connection.Insert(player);
    }

    // Read
    public Models.Player GetById(int id)
    {
        return _connection.Table<Models.Player>().FirstOrDefault(p => p.Id == id);
    }

    public List<Models.Player> GetAll()
    {
        return _connection.Table<Models.Player>().ToList();
    }

    // Update
    public int Update(Models.Player player)
    {
        return _connection.Update(player);
    }

    // Delete
    public int Delete(Models.Player player)
    {
        return _connection.Delete(player);
    }

    // Custom query

    public int GetIdByNameAndHostname(string name, string hostname)
    {
        var player = _connection.Table<Models.Player>().FirstOrDefault(p => p.Name == name && p.Hostname == hostname);
        if (player != null)
        {
            return player.Id;
        }
        else
        {
            return Insert(new Models.Player { Name = name, Hostname = hostname });
        }
    }
}
