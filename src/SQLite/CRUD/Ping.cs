using System;
using System.Collections.Generic;
using System.Linq;
using DiscordConnector.SQLite.Models;
using SQLite;

namespace DiscordConnector.SQLite.Repositories;
public class PingRepository
{
    private SQLiteConnection _connection;

    public PingRepository(SQLiteConnection connection)
    {
        _connection = connection;
        _connection.CreateTable<Ping>();
    }

    public int Insert(int playerId, double x, double y, double z, double pingX, double pingY, double pingZ)
    {
        return Insert(playerId, x, y, z, pingX, pingY, pingZ, null);
    }

    public int Insert(int playerId, double x, double y, double z, double pingX, double pingY, double pingZ, DateTime? pingedAt)
    {
        var ping = new Ping
        {
            PlayerId = playerId,
            X = x,
            Y = y,
            Z = z,
            PingX = pingX,
            PingY = pingY,
            PingZ = pingZ,
            PingedAt = pingedAt ?? DateTime.UtcNow
        };

        return Insert(ping);
    }



    public int Insert(Ping ping)
    {
        return _connection.Insert(ping);
    }

    public int Update(Ping ping)
    {
        return _connection.Update(ping);
    }

    public int Delete(Ping ping)
    {
        return _connection.Delete(ping);
    }

    public List<Ping> GetAll()
    {
        return _connection.Table<Ping>().ToList();
    }

    public Ping GetById(int id)
    {
        return _connection.Table<Ping>().Where(p => p.Id == id).FirstOrDefault();
    }

    public List<Ping> GetByPlayerId(int playerId)
    {
        return _connection.Table<Ping>().Where(p => p.PlayerId == playerId).ToList();
    }

    public List<CountResult> GetCountResults()
    {
        var query = _connection.Table<Ping>()
                    .Join(_connection.Table<Models.Player>(), n => n.PlayerId, p => p.Id, (n, p) => new { Ping = n, Player = p })
                    .GroupBy(np => new { np.Player.Id, np.Player.Name })
                    .OrderByDescending(g => g.Count())
                    .Select(g => new CountResult(g.Key.Name, g.Count()));

        return query.ToList();
    }
}
