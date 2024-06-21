namespace DiscordConnector.Database;

/// <summary>
/// Database-related configuration settings.
/// </summary>
public class Config
{
  public const string RetrieveBySteamID = "PlayerId: Treat each PlayerId as a separate player";
  public const string RetrieveByNameAndSteamID = "NameAndPlayerId: Treat each [PlayerId:CharacterName] combo as a separate player";
  public const string RetrieveByName = "Name: Treat each CharacterName as a separate player";
  /// <summary>
  /// Allowed methods for differentiating between players on the server
  /// </summary>
  public enum RetrievalDiscernmentMethods
  {
    [System.ComponentModel.Description(RetrieveBySteamID)]
    PlayerId,
    [System.ComponentModel.Description(RetrieveByName)]
    Name,
    [System.ComponentModel.Description(RetrieveByNameAndSteamID)]
    NameAndPlayerId,
  }
  /// <summary>
  /// Whether to enable debug logging for database methods. This doesn't log directly from LiteDB but instead
  /// adds debug logging for the methods that interact with the database. It can help pinpoint issues with
  /// the database.
  /// </summary>
  public bool DebugDatabaseMethods { get; set; } = false;
  /// <summary>
  /// How to store and retrieve player IDs from the database. This can be useful for servers that have
  /// multiple player IDs for the same player.
  /// </summary>
  public RetrievalDiscernmentMethods RecordRetrievalDiscernmentMethod { get; set; } = RetrievalDiscernmentMethods.NameAndPlayerId;
  /// <summary>
  /// Whether the collection of stats is enabled or not.
  /// </summary>
  public bool CollectStatsEnabled { get; set; } = true;
}
