// <copyright file="Match.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Checkers Match class for initialising new matches.
    /// </summary>
    public class Match
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Match"/> class.
        /// </summary>
        /// <param name="teamA"> The first team to be injected. </param>
        /// <param name="teamB"> The second team to be injected. </param>
        /// <param name="catID"> The CategoryID of the Match. </param>
        /// <param name="textID"> The Text Channel ID of the Match. </param>
        public Match(Team teamA, Team teamB, ulong catID, ulong textID)
        {
            this.TeamA = teamA;
            this.TeamB = teamB;
            this.CategoryID = catID;
            this.TectChannelID = textID;
            this.TimeStarted = DateTime.UtcNow;
        }

        // TODO: Get Player Ids into this class to be passed into Team constructors.

        /// <summary>
        /// Gets or Sets team A in the match.
        /// </summary>
        public Team TeamA { get; set; }

        /// <summary>
        /// Gets or Sets team B in the match.
        /// </summary>
        public Team TeamB { get; set; }

        /// <summary>
        /// Gets the CategoryID for this match.
        /// </summary>
        public ulong CategoryID { get; }

        /// <summary>
        /// Gets the Text CHannel ID for this match.
        /// </summary>
        public ulong TectChannelID { get; }

        /// <summary>
        /// Gets the start time of the Match.
        /// </summary>
        public DateTimeOffset TimeStarted { get; }
    }
}