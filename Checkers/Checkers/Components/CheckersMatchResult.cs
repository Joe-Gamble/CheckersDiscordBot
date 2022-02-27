// <copyright file="CheckersMatchResult.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Data.Models;
    using Checkers.Services;

    public class CheckersMatchResult
    {
        public CheckersMatchResult(Match match, MatchOutcome outcome)
        {
            this.Outcome = outcome;
            this.TeamA = match.TeamA;
            this.TeamB = match.TeamB;
            this.SkillFavor = this.CalculateFavor();
            this.Multiplier = this.CalculateMultiplier();
        }

        private SkillFavors CalculateFavor()
        {
            if (this.TeamA.AverageRating == this.TeamB.AverageRating)
            {
                return SkillFavors.Equal;
            }
            else if (this.TeamA.AverageRating > this.TeamB.AverageRating)
            {
                return SkillFavors.TeamA;
            }
            else
            {
                return SkillFavors.TeamB;
            }
        }

        private int CalculateMultiplier()
        {
            int skillDifference = Math.Abs(this.TeamA.AverageRating - this.TeamB.AverageRating);
            return skillDifference / CheckersConstants.StandardWin;
        }

        /// <summary>
        /// Gets or Sets the multiplier for elo dissparities.
        /// </summary>
        public int Multiplier { get; set; }

        /// <summary>
        /// Gets the Outcome of the match.
        /// </summary>
        public MatchOutcome Outcome { get; }

        /// <summary>
        /// Gets TeamA from this match.
        /// </summary>
        public Team TeamA { get; }

        /// <summary>
        /// Gets TeamB from this match.
        /// </summary>
        public Team TeamB { get; }

        /// <summary>
        /// Gets or Sets the Skill Favor from this match.
        /// </summary>
        public SkillFavors SkillFavor { get; set; }
    }
}
