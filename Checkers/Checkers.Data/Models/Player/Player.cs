// <copyright file="Player.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Player class for Checkers.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="id"> The uID of the Player. </param>
        /// <param name="username"> The Username of the Player. </param>
        public Player(ulong id, string username)
        {
            this.Id = id;
            this.Username = username;
            this.IsActive = true;
            this.IsQueued = false;
            this.IsPlaying = false;

            this.Rating = 0;
            this.CurrentTier = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="player"> The Player to copy data from. </param>
        public Player(Player player)
        {
            this.Id = player.Id;
            this.Username = player.Username;
            this.IsActive = player.IsActive;
            this.IsQueued = player.IsQueued;
            this.IsPlaying = player.IsPlaying;

            this.Rating = player.Rating;
            this.CurrentTier = player.CurrentTier;
            this.InPlacements = player.InPlacements;
        }

        /// <summary>
        /// Gets or sets the ID for the player.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// Gets or sets the Username for the player.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Player's account is currently active in the system.
        /// </summary>
        public bool IsActive { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the Player's account is currently queued in the system.
        /// </summary>
        public bool IsQueued { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the Player's account is currently playing a match in the system.
        /// </summary>
        public bool IsPlaying { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the Player's account is currently in placements.
        /// </summary>
        public bool InPlacements { get; set; } = false;

        /// <summary>
        /// Gets or Sets the Players current rating.
        /// </summary>
        public int Rating { get; set; }


        /// <summary>
        /// Gets or Sets the Players highest rating.
        /// </summary>
        public int HighestRating { get; set; }

        /// <summary>
        /// Gets or Sets the Players Winrate.
        /// </summary>
        public int WinRate { get; set; }

        /// <summary>
        /// Gets or Sets the games the Player has played.
        /// </summary>
        public int GamesPlayed { get; set; }

        /// <summary>
        /// Gets or Sets the games won.
        /// </summary>
        public int GamesWon { get; set; }

        /// <summary>
        /// Gets or Sets the games lost.
        /// </summary>
        public int GamesLost { get; set; }

        /// <summary>
        /// Gets or Sets the current Tier.
        /// </summary>
        public int CurrentTier { get; set; }

        /// <summary>
        /// Gets or Sets the players current games out of their division.
        /// </summary>
        public int GamesOutOfDivision { get; set; }

        /// <summary>
        /// Get the current skill rating.
        /// </summary>
        /// <returns> The Players skill rating. </returns>
        public int GetCurrentRanting()
        {
            return this.Rating;
        }

        /// <summary>
        /// Get the current skill rating.
        /// </summary>
        /// <returns> The Players Stats. </returns>
        public int GetStats()
        {
            return this.Rating;
        }

        /// <summary>
        /// Add a game to the players stat.
        /// </summary>
        /// <param name="win"> If a win state of the game. Null = Draw. </param>
        public void AddGamePlayed(bool? win = null)
        {
            this.GamesPlayed++;
            if (win != null)
            {
                if (win == true)
                {
                    this.GamesWon++;
                }
                else
                {
                    this.GamesLost++;
                }

                this.UpdateWinRate();
            }
        }

        private void UpdateWinRate()
        {
            var totalApplicableGames = this.GamesWon + this.GamesLost;
            double percentage = (double)this.GamesWon / totalApplicableGames * 100;

            this.WinRate = (int)percentage;
        }
    }
}
