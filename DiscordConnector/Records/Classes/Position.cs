using LiteDB;

namespace DiscordConnector.Records;

public class Position
{
    public Position()
    {
        x = 0;
        y = 0;
        z = 0;
    }

    /// <summary>
    ///     LiteDB fills read-only properties through this constructor, matching parameter names to field names.
    ///     Without [BsonCtor] it used the empty constructor, so every stored position read back as (0,0,0).
    /// </summary>
    [BsonCtor]
    public Position(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public float x { get; }
    public float y { get; }
    public float z { get; }

    public override string ToString()
    {
        return $"({x},{y},{z})";
    }
}
