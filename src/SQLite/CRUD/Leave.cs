using System;
using System.Collections.Generic;
using System.Linq;
using DiscordConnector.SQLite.Models;
using SQLite;

namespace DiscordConnector.SQLite.Repositories;
public class LeaveRepository
{
    private SQLiteConnection _connection;

    public LeaveRepository(SQLiteConnection connection)
    {
        _connection = connection;
        _connection.CreateTable<Leave>();
    }

    public int Insert(int playerId, double x, double y, double z, DateTime? leftAt = null)
    {
        var leave = new Leave
        {
            PlayerId = playerId,
            X = x,
            Y = y,
            Z = z,
            LeftAt = leftAt ?? DateTime.UtcNow
        };

        return Insert(leave);
    }

    public int Insert(Leave leave)
    {
        return _connection.Insert(leave);
    }

    public int Update(Leave leave)
    {
        return _connection.Update(leave);
    }

    public int Delete(Leave leave)
    {
        return _connection.Delete(leave);
    }

    public List<Leave> GetAll()
    {
        return _connection.Table<Leave>().ToList();
    }

    public Leave GetById(int id)
    {
        return _connection.Table<Leave>().Where(l => l.Id == id).FirstOrDefault();
    }

    public List<Leave> GetByPlayerId(int playerId)
    {
        return _connection.Table<Leave>().Where(l => l.PlayerId == playerId).ToList();
    }

    public List<CountResult> GetCountResults()
    {
        var query = _connection.Table<Leave>()
                    .Join(_connection.Table<Models.Player>(), l => l.PlayerId, p => p.Id, (l, p) => new { Leave = l, Player = p })
                    .GroupBy(lp => new { lp.Player.Id, lp.Player.Name })
                    .OrderByDescending(g => g.Count())
                    .Select(g => new CountResult(g.Key.Name, g.Count()));

        return query.ToList();
    }
}
