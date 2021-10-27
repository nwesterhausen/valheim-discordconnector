
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

    public static int CompareByCount(CountResult cr1, CountResult cr2)
    {
        return cr1.Count.CompareTo(cr2.Count);
    }

    public static int CompareByName(CountResult cr1, CountResult cr2)
    {
        return cr1.Name.CompareTo(cr2.Name);
    }
    }
}
