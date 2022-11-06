
using LiteDB;

namespace DiscordConnector.Records
{
    public class PlayerToName
    {
        public ObjectId _id { get; }
        public string CharacterName { get; }
        public string PlayerId { get; }
        public System.DateTime InsertedDate { get; }

        public PlayerToName(string characterName, string playerHostName)
        {
            _id = ObjectId.NewObjectId();
            CharacterName = characterName;
            PlayerId = playerHostName;
            InsertedDate = System.DateTime.Now;
        }

        public override string ToString()
        {
            return $"{CharacterName} ({PlayerId})";
        }

    }
}