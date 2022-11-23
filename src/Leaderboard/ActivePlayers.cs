
namespace DiscordConnector.LeaderBoards
{
    /// <summary>
    /// <para>
    ///     A board that posts periodically the number of unique players and other stats.
    /// </para>
    /// <para>
    ///     Example:
    /// <code>
    ///     SERVER PLAYERS
    ///     Online now: 4
    ///     Today: 8
    ///     Week: 29
    ///     Month: 31
    ///     Total Unique: 42
    ///     Most at once: 17
    /// </code>
    /// </para>
    /// 
    /// </summary>
    internal class ActivePlayersBoard
    {
        /// <summary>
        /// Return the number of currently online players. This is grabbed from the GetAllCharacterZDOS method.
        /// </summary>
        /// <returns>Count of character ZDOS</returns>
        private static int CurrentOnlinePlayers()
        {
            return ZNet.instance.GetAllCharacterZDOS().Count;
        }
    }
}