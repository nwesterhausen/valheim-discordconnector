
using LiteDB;

namespace DiscordConnector.Records
{
    public class CountResult
    {
        public string Name{get;}
        public int Count{get;}
    
    [BsonCtor]
    public CountResult(string name, int count)
    {
        
        Name = name;
        Count = count;
    }
    }
}
