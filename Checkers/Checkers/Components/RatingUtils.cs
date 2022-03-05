// <copyright file="RatingUtils.cs" company="GambleDev">
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
    using Discord;

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

        public static Emote? GetTierEmoteAt(SkillTier tier)
        {
            Emote result;
            string text = string.Empty;

            if (tier == SkillTier.Silver)
            {
                text = "<:SilverIcon:943255400495583303>";
            }
            else if (tier == SkillTier.Gold)
            {
                text = "<:SilverIcon:943255400495583303>";
            }
            else if (tier == SkillTier.Platnium)
            {
                text = "<:SilverIcon:943255400495583303>";
            }
            else if (tier == SkillTier.Sapphire)
            {
                text = "<:SilverIcon:943255400495583303>";
            }
            else if (tier == SkillTier.Masters)
            {
                text = "<:SilverIcon:943255400495583303>";
            }
            else if (tier == SkillTier.Warlord)
            {
                text = "<:SilverIcon:943255400495583303>";
            }

            if (Emote.TryParse(text, out result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Add Skill Rating to the Current Rating. Auto adjusts rating if players surpass their current Tier's threshold.
        /// </summary>
        /// <param name="player"> The Player to add points to. </param>
        /// <param name="total"> The total to be added. </param>
        /// <returns> True if promoted. </returns>
        public static bool Gain(Player player, int total)
        {
            player.Rating += total;

            ClampRating(player.Rating);

            SkillTier tier = GetTierAt(player.Rating);

            if (player.CurrentTier != (int)tier)
            {
                // promoted
                player.CurrentTier = (int)GetTierAt(player.Rating);
                player.GamesOutOfDivision = 0;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Subract Skill Rating from the Current Rating. Auto adjusts rating if players drop below their current tier.
        /// </summary>
        /// <param name="player"> The Player to add points to. </param>
        /// <param name="total"> The amount to subtract. </param>
        /// <returns> True if demoted. </returns>
        public static bool Lose(Player player, int total)
        {
            player.Rating -= total;

            ClampRating(player.Rating);

            if ((int)GetTierAt(player.Rating) != player.CurrentTier)
            {
                player.GamesOutOfDivision++;

                if (player.GamesOutOfDivision >= 5)
                {
                    player.CurrentTier = (int)GetTierAt(player.Rating);
                    player.GamesOutOfDivision = 0;
                    return true;
                }
            }

            return false;
        }

        private static void ClampRating(int rating)
        {
            Math.Clamp(rating, 0, CheckersConstants.MaxRank);
        }
    }
}
