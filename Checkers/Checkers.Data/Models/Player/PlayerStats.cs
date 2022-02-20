// <copyright file="PlayerStats.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Data.Models.Ranked;

    /// <summary>
    /// Player stats.
    /// </summary>
    public class PlayerStats
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerStats"/> class.
        /// </summary>
        public PlayerStats()
        {
            this.Rating = new SkillRating();
        }

        /// <summary>
        /// Gets or Sets the Players SkillRating.
        /// </summary>
        public SkillRating Rating { get; set; }

        /// <summary>
        /// Gets or Sets the Players Winrate.
        /// </summary>
        public int WinRate { get; set; }
    }
}
