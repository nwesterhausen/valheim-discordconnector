using SQLite;

namespace DiscordConnector.SQLite.Models;
public class Player
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [NotNull]
    public string Name { get; set; }
    [NotNull]
    public string Hostname { get; set; }
}