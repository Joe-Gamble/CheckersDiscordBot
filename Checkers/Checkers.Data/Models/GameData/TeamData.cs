// <copyright file="Team.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Data.Models.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Team class defining a standard Checkers team.
    /// </summary>
    public class TeamData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeamData"/> class.
        /// </summary>
        /// <param name="players"> The players for this Team. </param>
        /// <param name="averageRating"> The average rating from this team. </param>
        public TeamData(List<ulong> players, int averageRating)
        {
            this.Players = players;

            this.AverageRating = averageRating;
        }

        /// <summary>
        /// Gets the ID's of users that played on this team.
        /// </summary>
        public List<ulong> Players { get; }


        /// <summary>
        /// Gets the Average rating of this Team.
        /// </summary>
        public int AverageRating { get; }
    }
}
