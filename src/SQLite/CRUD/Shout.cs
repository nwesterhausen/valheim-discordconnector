using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DiscordConnector.SQLite.Models;
using SQLite;

namespace DiscordConnector.SQLite.Repositories;
public class ShoutRepository
{
    private SQLiteConnection _connection;

    public ShoutRepository(SQLiteConnection connection)
    {
        _connection = connection;
        _connection.CreateTable<Shout>();
    }

    public int Insert(int playerId, string text)
    {
        return Insert(playerId, 0.0, 0.0, 0.0, text);
    }

    public int Insert(int playerId, double x, double y, double z, string text)
    {
        return Insert(playerId, 0.0, 0.0, 0.0, text, null);
    }

    public int Insert(int playerId, double x, double y, double z, string text, DateTime? shoutedAt)
    {
        var shout = new Shout
        {
            PlayerId = playerId,
            X = x,
            Y = y,
            Z = z,
            ShoutedAt = shoutedAt ?? DateTime.UtcNow,
            Text = text ?? "",
        };


        return Insert(shout);
    }

    public int Insert(Shout shout)
    {
        return _connection.Insert(shout);
    }

    public int Update(Shout shout)
    {
        return _connection.Update(shout);
    }

    public int Delete(Shout shout)
    {
        return _connection.Delete(shout);
    }

    public List<Shout> GetAll()
    {
        return _connection.Table<Shout>().ToList();
    }

    public Shout GetById(int id)
    {
        return _connection.Table<Shout>().Where(s => s.Id == id).FirstOrDefault();
    }

    public List<Shout> GetByPlayerId(int playerId)
    {
        return _connection.Table<Shout>().Where(s => s.PlayerId == playerId).ToList();
    }


    public List<CountResult> GetCountResults()
    {
        var query = _connection.Table<Shout>()
                    .Join(_connection.Table<Models.Player>(), s => s.PlayerId, p => p.Id, (s, p) => new { Shout = s, Player = p })
                    .GroupBy(sp => new { sp.Player.Id, sp.Player.Name })
                    .OrderByDescending(g => g.Count())
                    .Select(g => new CountResult(g.Key.Name, g.Count()));

        return query.ToList();
    }
}