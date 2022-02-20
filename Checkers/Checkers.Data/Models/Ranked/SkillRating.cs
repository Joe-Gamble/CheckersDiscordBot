// <copyright file="SkillRating.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Data.Models.Ranked
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Wrapper Class for everything associated with a Player's SkillRating.
    /// </summary>
    public class SkillRating
    {
        private int gamesOutOfDivision = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillRating"/> class.
        /// </summary>
        public SkillRating()
        {
            this.CurrentRating = 0;
            this.CurrentTier = SkillTier.Undefined;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillRating"/> class from pre-existing data.
        /// </summary>
        /// <param name="rating"> The rating to initialise at. </param>
        public SkillRating(int rating)
        {
            this.CurrentRating = rating;
            this.CurrentTier = GetTierAt(rating);
        }

        /// <summary>
        /// Gets or Sets the current rating.
        /// </summary>
        public int CurrentRating { get; set; }

        /// <summary>
        /// Gets or Sets the current Tier.
        /// </summary>
        public SkillTier CurrentTier { get; set; }

        /// <summary>
        /// Returns a SkillTier based on the passed rating.
        /// </summary>
        /// <param name="rating"> The skill rating total. </param>
        /// <returns> The SkilTier of the Player's current rank. </returns>
        public static SkillTier GetTierAt(int rating)
        {
            if (rating < 0)
            {
                return SkillTier.Undefined;
            }
            else if (rating <= 1500)
            {
                return SkillTier.Silver;
            }
            else if (rating <= 2000)
            {
                return SkillTier.Gold;
            }
            else if (rating <= 2500)
            {
                return SkillTier.Platnium;
            }
            else if (rating <= 3000)
            {
                return SkillTier.Sapphire;
            }
            else if (rating <= 3500)
            {
                return SkillTier.Masters;
            }
            else if (rating > 4000)
            {
                return SkillTier.Warlord;
            }

            return SkillTier.Undefined;
        }

        /// <summary>
        /// Add Skill Rating to the Current Rating. Auto adjusts rating if players surpass their current Tier's threshold.
        /// </summary>
        /// <param name="total"> The total to be added. </param>
        public void Gain(int total)
        {
            this.CurrentRating += total;

            SkillTier tier = GetTierAt(this.CurrentRating);

            if (this.CurrentTier != tier)
            {
                // promoted
                this.CurrentTier = GetTierAt(this.CurrentRating);
            }
        }

        /// <summary>
        /// Subract Skill Rating from the Current Rating. Auto adjusts rating if players drop below their current tier.
        /// </summary>
        /// <param name="total"> The amount to subtract. </param>
        public void Lose(int total)
        {
            this.CurrentRating -= total;

            if (GetTierAt(this.CurrentRating) != this.CurrentTier)
            {
                this.gamesOutOfDivision++;

                if (this.gamesOutOfDivision >= 5)
                {
                    this.CurrentTier = GetTierAt(this.CurrentRating);
                    this.gamesOutOfDivision = 0;
                }
            }
        }
    }
}
