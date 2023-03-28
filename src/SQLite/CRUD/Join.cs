using System;
using System.Collections.Generic;
using System.Linq;
using DiscordConnector.SQLite.Models;
using SQLite;

namespace DiscordConnector.SQLite.Repositories;
public class JoinRepository
{
    private SQLiteConnection _connection;

    public JoinRepository(SQLiteConnection connection)
    {
        _connection = connection;
        _connection.CreateTable<Join>();
    }

    public int Insert(int playerId, double x, double y, double z, DateTime? joinedAt = null)
    {
        var join = new Join
        {
            PlayerId = playerId,
            X = x,
            Y = y,
            Z = z,
            JoinedAt = joinedAt ?? DateTime.UtcNow
        };

        return Insert(join);
    }

    public int Insert(Join join)
    {
        return _connection.Insert(join);
    }

    public int Update(Join join)
    {
        return _connection.Update(join);
    }

    public int Delete(Join join)
    {
        return _connection.Delete(join);
    }

    public List<Join> GetAll()
    {
        return _connection.Table<Join>().ToList();
    }

    public Join GetById(int id)
    {
        return _connection.Table<Join>().Where(j => j.Id == id).FirstOrDefault();
    }

    public List<Join> GetByPlayerId(int playerId)
    {
        return _connection.Table<Join>().Where(j => j.PlayerId == playerId).ToList();
    }

    public List<CountResult> GetCountResults()
    {
        var query = _connection.Table<Join>()
                    .Join(_connection.Table<Models.Player>(), j => j.PlayerId, p => p.Id, (j, p) => new { Join = j, Player = p })
                    .GroupBy(jp => new { jp.Player.Id, jp.Player.Name })
                    .OrderByDescending(g => g.Count())
                    .Select(g => new CountResult(g.Key.Name, g.Count()));

        return query.ToList();
    }


}
