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
            else if (rating >= 4000)
            {
                return SkillTier.Warlord;
            }
            else if (rating >= 3500)
            {
                return SkillTier.Masters;
            }
            else if (rating >= 3000)
            {
                return SkillTier.Sapphire;
            }
            else if (rating >= 2500)
            {
                return SkillTier.Platnium;
            }
            else if (rating >= 2000)
            {
                return SkillTier.Gold;
            }
            else
            {
                return SkillTier.Silver;
            }
        }

        public static Emote? GetTierEmoteAt(SkillTier tier)
        {
            Emote result;
            string text = string.Empty;

            if (tier == SkillTier.Silver)
            {
                text = "<:silver:949739166667767818>";
            }
            else if (tier == SkillTier.Gold)
            {
                text = "<:gold:949739167116587078>";
            }
            else if (tier == SkillTier.Platnium)
            {
                text = "<:plat:949739167582146580>";
            }
            else if (tier == SkillTier.Sapphire)
            {
                text = "<:sapphire:949739167347253368>";
            }
            else if (tier == SkillTier.Masters)
            {
                text = "<:masters:949739168169336882>";
            }
            else if (tier == SkillTier.Warlord)
            {
                text = "<:warlord:949739167909302302>";
            }

            if (Emote.TryParse(text, out result))
            {
                return result;
            }

            return null;
        }

        public static ulong GetRoleOfTier (SkillTier tier)
        {
            if (tier == SkillTier.Silver)
            {
                return 942528550962098186;
            }
            else if (tier == SkillTier.Gold)
            {
                return 942528871096516609;
            }
            else if (tier == SkillTier.Platnium)
            {
                return 942529009638572052;
            }
            else if (tier == SkillTier.Sapphire)
            {
                return 942529349343641681;
            }
            else if (tier == SkillTier.Masters)
            {
                return 942530077130899486;
            }
            else if (tier == SkillTier.Warlord)
            {
                return 942530129731661894;
            }
            else
            {
                return 942532554915971082;
            }
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

            player.Rating = ClampRating(player.Rating);

            SkillTier tier = GetTierAt(player.Rating);

            bool rankUp = false;

            if (player.HighestRating < player.Rating)
            {
                player.HighestRating = player.Rating;
                rankUp = true;
            }

            if (player.CurrentTier != (int)tier)
            {
                if (rankUp)
                {
                    // New Rank!!!!!
                }

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

            player.Rating = ClampRating(player.Rating);

            if ((int)GetTierAt(player.Rating) != player.CurrentTier)
            {
                if (player.CurrentTier <= (int)SkillTier.Sapphire)
                {
                    player.GamesOutOfDivision++;

                    if (player.GamesOutOfDivision >= 5)
                    {
                        player.CurrentTier = (int)GetTierAt(player.Rating);
                        player.GamesOutOfDivision = 0;
                        return true;
                    }
                }
                else
                {
                    player.CurrentTier = (int)GetTierAt(player.Rating);
                    return true;
                }
            }

            return false;
        }

        private static int ClampRating(int rating)
        {
            rating = Math.Clamp(rating, 0, CheckersConstants.MaxRank);
            return rating;
        }
    }
}
