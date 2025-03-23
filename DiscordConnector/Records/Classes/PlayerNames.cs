using System;

using LiteDB;

namespace DiscordConnector.Records;

public class PlayerToName
{
    // Parameterless constructor for serialization
    public PlayerToName() { }

    // Domain constructor for normal code usage
    public PlayerToName(string characterName, string playerId)
    {
        _id = ObjectId.NewObjectId();
        CharacterName = characterName;
        PlayerId = playerId;
        InsertedDate = DateTime.Now;
    }

    public ObjectId _id { get; set; }
    public string CharacterName { get; set; }
    public string PlayerId { get; set; }
    public DateTime InsertedDate { get; set; }

    public override string ToString()
    {
        return $"{CharacterName} ({PlayerId})";
    }
}
