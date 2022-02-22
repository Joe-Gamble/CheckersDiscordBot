namespace Checkers.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Data.Models;
    using Checkers.Data.Models.Game;
    using Discord.WebSocket;

    /// <summary>
    /// Checkers matchmaking Queue class.
    /// </summary>
    public class CheckersQueue
    {
        /// <summary>
        /// List of Players currently in Queue.
        /// </summary>
        private readonly List<Player> players = new ();
        private readonly int gameSize = 12;

        /// <summary>
        /// Add a player to the Queue.
        /// </summary>
        /// <param name="player"> The Player to be added. </param>
        /// <returns> True if there's enough players for a game. </returns>
        public bool AddToQueue(Player player)
        {
            player.IsQueued = true;
            this.players.Add(player);

            if (this.HasEnoughPlayers())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if theres enough Players in Queue.
        /// </summary>
        /// <returns> True if there are enough Players in Queue for a game. </returns>
        public bool HasEnoughPlayers()
        {
            return this.players.Count >= this.gameSize;
        }

        /// <summary>
        /// Add a player to the Queue.
        /// </summary>
        /// <param name="player"> The Player to be removed. </param>
        public void RemoveFromQueue(Player player)
        {
            player.IsQueued = false;
            this.players.Remove(player);
        }

        /// <summary>
        /// Pop the Queue.
        /// </summary>
        /// <returns> A list containing the closest 12 players to the average currently in the Queue. </returns>
        public List<Player> Pop()
        {
            return this.GetPlayersClosestToAverage();
        }

        private List<Player> GetPlayersClosestToAverage()
        {
            List<Player> closestPlayers = new List<Player>();

            var avRating = this.players.Average(player => player.GetCurrentRanting());

            var result = this.players.OrderBy(i => Math.Abs(i.GetCurrentRanting() - avRating))
             .ThenBy(i => i.GetCurrentRanting() < avRating)
             .ToArray();

            closestPlayers.AddRange(this.players.GetRange(0, 12));

            return closestPlayers;
        }
    }
}
