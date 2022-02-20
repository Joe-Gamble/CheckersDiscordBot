// <copyright file="Match.cs" company="GambleDev">
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
    /// Checkers Match class for initialising new matches.
    /// </summary>
    public class MatchData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchData"/> class.
        /// </summary>
        /// <param name="teamA"> The first team to be injected. </param>
        /// <param name="teamB"> The second team to be injected. </param>
        /// <param name="duration"> The duration of this match. </param>
        public MatchData(TeamData teamA, TeamData teamB, int duration)
        {
            this.TeamA = teamA;
            this.TeamB = teamB;

            this.Duration = duration;
        }

        // TODO: Get Player Ids into this class to be passed into Team constructors.

        /// <summary>
        /// Gets or Sets team A in the match.
        /// </summary>
        public TeamData TeamA { get; set; }

        /// <summary>
        /// Gets or Sets team B in the match.
        /// </summary>
        public TeamData TeamB { get; set; }


        /// <summary>
        /// Gets the duration of this Match.
        /// </summary>
        public int Duration { get; }
    }
}
