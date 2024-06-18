
namespace DiscordConnector.Records;
public class Position
{
    public float x { get; }
    public float y { get; }
    public float z { get; }

    public Position()
    {
        x = 0;
        y = 0;
        z = 0;
    }
    public Position(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public override string ToString()
    {
        return $"({x},{y},{z})";
    }
}
