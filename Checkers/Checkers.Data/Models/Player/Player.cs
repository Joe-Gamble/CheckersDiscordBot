// <copyright file="Player.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
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

            this.Stats = new PlayerStats();
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
        /// Gets or Sets the Players Stats.
        /// </summary>
        public PlayerStats Stats { get; set; }

        /// <summary>
        /// Get the current skill rating.
        /// </summary>
        /// <returns> The Players skill rating. </returns>
        public int GetCurrentRanting()
        {
            return this.Stats.Rating.CurrentRating;
        }
    }
}
