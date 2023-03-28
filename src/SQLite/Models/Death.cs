using System;
using SQLite;

namespace DiscordConnector.SQLite.Models;
public class Death
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Indexed]
    public int PlayerId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public DateTime DiedAt { get; set; }
}
