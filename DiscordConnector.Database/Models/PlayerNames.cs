
using LiteDB;

namespace DiscordConnector.Database.Models;

[method: BsonCtor]
public class PlayerToName(ObjectId id, string characterName, string playerId, System.DateTime insertedDate)
{
  public ObjectId _id { get; } = id;
  public string CharacterName { get; } = characterName;
  public string PlayerId { get; } = playerId;
  public System.DateTime InsertedDate { get; } = insertedDate;

  public PlayerToName(string characterName, string playerHostName)
    : this(ObjectId.NewObjectId(), characterName, playerHostName, System.DateTime.Now)
  {
  }

  public override string ToString()
  {
    return $"{CharacterName} ({PlayerId}) first seen on {InsertedDate.ToShortDateString()} {InsertedDate.ToShortTimeString()}";
  }

}
