namespace DiscordConnector.Database.Models;

public class Position(float _x, float _y, float _z)
{
  public float x { get; } = _x;
  public float y { get; } = _y;
  public float z { get; } = _z;

  public Position() : this(0, 0, 0)
  {
  }

  public override string ToString()
  {
    return $"({x},{y},{z})";
  }
}
