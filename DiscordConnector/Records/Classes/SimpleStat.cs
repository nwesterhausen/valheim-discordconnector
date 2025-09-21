using System;
using LiteDB;

namespace DiscordConnector.Records;

public class SimpleStat
{
	public SimpleStat(string name, string playerId, float x, float y, float z)
		: this(ObjectId.NewObjectId(), name, DateTime.Now, playerId, new Position(x, y, z))
	{
	}

	[BsonCtor]
	public SimpleStat(ObjectId id, ObjectId statId, string name, DateTime date, string playerId, BsonDocument pos)
		: this(statId, name, date, playerId, BsonMapper.Global.Deserialize<Position>(pos))
	{
	}

	public SimpleStat(ObjectId statId, string name, DateTime date, string playerId, Position pos)
	{
		this.StatId = statId;
		this.Name = name;
		this.Date = date;
		this.PlayerId = playerId;
		this.Pos = pos;
	}

	public ObjectId StatId { get; }
	public string Name { get; }
	public DateTime Date { get; }
	public string PlayerId { get; }
	public Position Pos { get; }

	public override string ToString() =>
		$"{this.Date.ToShortDateString()} {this.Date.ToShortTimeString()}: {this.Name} ({this.PlayerId}) at {this.Pos}";
}
