using System;
using SQLite;

namespace DiscordConnector.SQLite.Models;
public class Ping
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Indexed]
    public int PlayerId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double PingX { get; set; }
    public double PingY { get; set; }
    public double PingZ { get; set; }
    public DateTime PingedAt { get; set; }
}
