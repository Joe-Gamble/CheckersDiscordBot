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
    /// Util CLass for handling skill rating data.
    /// </summary>
    public class RatingUtils
    {
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
        /// <param name="player"> The Player to add points to. </param>
        /// <param name="total"> The total to be added. </param>
        public static void Gain(Player player, int total)
        {
            player.Rating += total;

            SkillTier tier = GetTierAt(player.Rating);

            if (player.CurrentTier != tier)
            {
                // promoted
                player.CurrentTier = GetTierAt(player.Rating);
            }
        }

        /// <summary>
        /// Subract Skill Rating from the Current Rating. Auto adjusts rating if players drop below their current tier.
        /// </summary>
        /// <param name="player"> The Player to add points to. </param>
        /// <param name="total"> The amount to subtract. </param>
        public static void Lose(Player player, int total)
        {
            player.Rating -= total;

            if (GetTierAt(player.Rating) != player.CurrentTier)
            {
                player.GamesOutOfDivision++;

                if (player.GamesOutOfDivision >= 5)
                {
                    player.CurrentTier = GetTierAt(player.Rating);
                    player.GamesOutOfDivision = 0;
                }
            }
        }
    }
}
