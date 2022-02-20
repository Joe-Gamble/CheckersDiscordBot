// <copyright file="Queue.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Data.Models.Ranked
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Data.Models.Game;

    /// <summary>
    /// Checkers matchmaking Queue class.
    /// </summary>
    public class Queue
    {
        /// <summary>
        /// List of Players currently in Queue.
        /// </summary>
        private readonly List<Player> players = new ();
        private int gameSize = 12;

        /// <summary>
        /// Add a player to the Queue.
        /// </summary>
        /// <param name="player"> The Player to be added. </param>
        /// <returns> True if there's enough players for a game. </returns>
        public bool AddToQueue(Player player)
        {
            this.players.Add(player);

            if (this.players.Count >= this.gameSize)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Add a player to the Queue.
        /// </summary>
        /// <param name="player"> The Player to be removed. </param>
        public void RemoveFromQueue(Player player)
        {
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
