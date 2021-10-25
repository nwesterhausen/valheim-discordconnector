using System;
using System.Collections.Generic;
using System.Timers;

namespace DiscordConnector
{
    internal class Leaderboard
    {
        private Leaderboards.Base overallHighest;
        private Leaderboards.Base overallLowest;
        private Leaderboards.Base topPlayers;
        private List<string> Boards = new List<string> {
            "ranking",
            "top",
            "bottom"
        };
        public Leaderboard()
        {
            overallHighest = new Leaderboards.OverallHighest();
            overallLowest = new Leaderboards.OverallLowest();
            topPlayers = new Leaderboards.TopPlayers();
        }

#if !NoBotSupport
        public Webhook.MessageResponse ExecuteCommand(Webhook.LeaderboardData command)
        {
            if (!Boards.Contains(command.type))
            {
                return new Webhook.MessageResponse
                {
                    message = $"invalid type {command.type}, valid options are {string.Join(",", Boards)}",
                    statusCode = 400
                };
            }
            // switch(command.type)
            // {
            //     case "ranking":
            //     case "top":
            //     case "bottom":
            // }
            return new Webhook.MessageResponse
            {
                message = $"not yet implemented",
                statusCode = 501
            };
        }
#endif

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

        /// <summary>
        /// Get a json string of the leaderboard
        /// </summary>
        // public abstract string GetLeaderboard();
    }

}
