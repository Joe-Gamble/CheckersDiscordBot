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
            this.CurrentTier = this.GetTierAt(rating);
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
        /// Add Skill Rating to the Current Rating. Auto adjusts rating if players surpass their current Tier's threshold.
        /// </summary>
        /// <param name="total"> The total to be added. </param>
        public void Gain(int total)
        {
            this.CurrentRating += total;

            SkillTier tier = this.GetTierAt(this.CurrentRating);

            if (this.CurrentTier != tier)
            {
                // promoted
                this.CurrentTier = this.GetTierAt(this.CurrentRating);
            }
        }

        /// <summary>
        /// Subract Skill Rating from the Current Rating. Auto adjusts rating if players drop below their current tier.
        /// </summary>
        /// <param name="total"> The amount to subtract. </param>
        public void Lose(int total)
        {
            this.CurrentRating -= total;

            if (this.GetTierAt(this.CurrentRating) != this.CurrentTier)
            {
                this.gamesOutOfDivision++;

                if (this.gamesOutOfDivision >= 5)
                {
                    this.CurrentTier = this.GetTierAt(this.CurrentRating);
                    this.gamesOutOfDivision = 0;
                }
            }
        }

        // this might need to be public or moved
        private SkillTier GetTierAt(int total)
        {
            if (total < 0)
            {
                return SkillTier.Undefined;
            }
            else if (total <= 1500)
            {
                return SkillTier.Silver;
            }
            else if (total <= 2000)
            {
                return SkillTier.Gold;
            }
            else if (total <= 2500)
            {
                return SkillTier.Platnium;
            }
            else if (total <= 3000)
            {
                return SkillTier.Sapphire;
            }
            else if (total <= 3500)
            {
                return SkillTier.Masters;
            }
            else if (total > 4000)
            {
                return SkillTier.Warlord;
            }

            return SkillTier.Undefined;
        }
    }
}
