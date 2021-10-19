using System.Timers;

namespace DiscordConnector
{
    internal class Leaderboard
    {
        private Leaderboards.Base overallHighest;
        private Leaderboards.Base overallLowest;
        private Leaderboards.Base topPlayers;

        public Leaderboard()
        {
            overallHighest = new Leaderboards.OverallHighest();
            overallLowest = new Leaderboards.OverallLowest();
            topPlayers = new Leaderboards.TopPlayers();
        }

        public Leaderboards.Base OverallHighest => overallHighest;
        public Leaderboards.Base OverallLowest => overallLowest;
        public Leaderboards.Base TopPlayers => topPlayers;
    }
}

namespace DiscordConnector.Leaderboards
{
    internal abstract class Base
    {
        /// <summary>
        /// An interface for sending the leaderboard as a timer event.
        /// </summary>
        public void SendLeaderboardOnTimer(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            this.SendLeaderboard();
        }

        /// <summary>
        /// Send the leaderboard to the DiscordAPI
        /// </summary>
        public abstract void SendLeaderboard();
    }

}
