
using LiteDB;

namespace DiscordConnector.Records;
public class PlayerToName
{
    public ObjectId _id { get; }
    public string CharacterName { get; }
    public string PlayerId { get; }
    public System.DateTime InsertedDate { get; }

    public PlayerToName(string characterName, string playerHostName)
        : this(ObjectId.NewObjectId(), characterName, playerHostName, System.DateTime.Now)
    {
    }

    [BsonCtor]
    public PlayerToName(ObjectId id, string characterName, string playerId, System.DateTime insertedDate)
    {
        _id = id;
        CharacterName = characterName;
        PlayerId = playerId;
        InsertedDate = insertedDate;
    }

    public override string ToString()
    {
        return $"{CharacterName} ({PlayerId})";
    }

}
