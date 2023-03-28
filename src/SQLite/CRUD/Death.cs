using System;
using System.Collections.Generic;
using System.Linq;
using DiscordConnector.SQLite.Models;
using SQLite;

namespace DiscordConnector.SQLite.Repositories;
public class DeathRepository
{
    private SQLiteConnection _connection;

    public DeathRepository(SQLiteConnection connection)
    {
        _connection = connection;
        _connection.CreateTable<Death>();
    }

    public int Insert(int playerId, double x, double y, double z, DateTime? diedAt = null)
    {
        var death = new Death
        {
            PlayerId = playerId,
            X = x,
            Y = y,
            Z = z,
            DiedAt = diedAt ?? DateTime.UtcNow
        };

        return Insert(death);
    }

    public int Insert(Death death)
    {
        return _connection.Insert(death);
    }

    public int Update(Death death)
    {
        return _connection.Update(death);
    }

    public int Delete(Death death)
    {
        return _connection.Delete(death);
    }

    public List<Death> GetAll()
    {
        return _connection.Table<Death>().ToList();
    }

    public Death GetById(int id)
    {
        return _connection.Table<Death>().Where(d => d.Id == id).FirstOrDefault();
    }

    public List<Death> GetByPlayerId(int playerId)
    {
        return _connection.Table<Death>().Where(d => d.PlayerId == playerId).ToList();
    }

    public List<CountResult> GetCountResults()
    {
        var query = _connection.Table<Death>()
                    .Join(_connection.Table<Models.Player>(), d => d.PlayerId, p => p.Id, (d, p) => new { Death = d, Player = p })
                    .GroupBy(dp => new { dp.Player.Id, dp.Player.Name })
                    .OrderByDescending(g => g.Count())
                    .Select(g => new CountResult(g.Key.Name, g.Count()));

        return query.ToList();
    }
}
