using System;

using LiteDB;

namespace DiscordConnector.Records;

public class PlayerToName
{
    public PlayerToName(string characterName, string playerHostName)
        : this(ObjectId.NewObjectId(), characterName, playerHostName, DateTime.Now)
    {
    }

    [BsonCtor]
    public PlayerToName(ObjectId id, string characterName, string playerId, DateTime insertedDate)
    {
        _id = id;
        CharacterName = characterName;
        PlayerId = playerId;
        InsertedDate = insertedDate;
    }

    public ObjectId _id { get; }
    public string CharacterName { get; }
    public string PlayerId { get; }
    public DateTime InsertedDate { get; }

    public override string ToString()
    {
        return $"{CharacterName} ({PlayerId})";
    }
}
