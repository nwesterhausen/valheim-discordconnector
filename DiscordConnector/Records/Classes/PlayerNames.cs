using System;
using LiteDB;

namespace DiscordConnector.Records;

public class PlayerToName
{
	// Parameterless constructor for serialization
	public PlayerToName() => this._id = ObjectId.NewObjectId();

	// Domain constructor for normal code usage
	public PlayerToName(string characterName, string playerId)
	{
		this._id = ObjectId.NewObjectId();
		this.CharacterName = characterName;
		this.PlayerId = playerId;
		this.InsertedDate = DateTime.Now;
	}

	public ObjectId _id { get; set; }
	public string CharacterName { get; set; }
	public string PlayerId { get; set; }
	public DateTime InsertedDate { get; set; }

	public override string ToString() => $"{this.CharacterName} ({this.PlayerId})";
}
