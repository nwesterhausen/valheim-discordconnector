using System;
using SQLite;

namespace DiscordConnector.SQLite.Models;
public class Join
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Indexed]
    public int PlayerId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public DateTime JoinedAt { get; set; }
}