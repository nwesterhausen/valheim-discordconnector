namespace DiscordConnector.Records;

public class Position
{
	public Position()
	{
		this.x = 0;
		this.y = 0;
		this.z = 0;
	}

	public Position(float _x, float _y, float _z)
	{
		this.x = _x;
		this.y = _y;
		this.z = _z;
	}

	public float x { get; }
	public float y { get; }
	public float z { get; }

	public override string ToString() => $"({this.x},{this.y},{this.z})";
}
