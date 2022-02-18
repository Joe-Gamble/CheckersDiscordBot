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
            this.Registered = true;
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
        /// Gets or sets a value indicating whether the Player is registered in the system.
        /// </summary>
        public bool Registered { get; set; } = false;
    }
}
