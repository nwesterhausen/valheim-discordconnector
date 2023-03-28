namespace DiscordConnector.SQLite;

/// <summary>
/// Valid categories to record stats in the database for.
/// </summary>
public enum StatCategory
{
    Join,
    Leave,
    Death,
    Ping,
    Shout,
}

/// <summary>
/// Class used to contain information destined for leaderboards and leaderboard creation.
/// </summary>
public class CountResult
{
    /// <summary>
    /// The player's name.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Total count of the events
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Constructor to create a CountResult (which is designed readonly)
    /// </summary>
    /// <param name="name">The player's name</param>
    /// <param name="count">Count of events</param>
    public CountResult(string name, int count)
    {
        Name = name;
        Count = count;
    }
}