using LiteDB;
namespace DiscordConnector.Database.Models;

public class SimpleStat
{
  public ObjectId StatId { get; }
  public string Name { get; }
  public System.DateTime Date { get; }
  public string PlayerId { get; }
  public Position Pos { get; }

  public SimpleStat(ObjectId statId, string name, System.DateTime date, string playerId, Position pos)
  {
    StatId = statId;
    Name = name;
    Date = date;
    PlayerId = playerId;
    Pos = pos;
  }

  public SimpleStat(string name, string playerId, Position pos)
    : this(ObjectId.NewObjectId(), name, System.DateTime.Now, playerId, pos)
  {
  }

  [BsonCtor]
  public SimpleStat(ObjectId statId, string name, System.DateTime date, string playerId, BsonDocument pos)
      : this(statId, name, date, playerId, BsonMapper.Global.Deserialize<Position>(pos))
  {
  }

  public override string ToString()
  {
    return $"{Date.ToShortDateString()} {Date.ToShortTimeString()}: {Name} ({PlayerId}) at {Pos}";
  }
}
