// <copyright file="Team.cs" company="GambleDev">
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
    using Discord.WebSocket;

    /// <summary>
    /// Team class defining a standard Checkers team.
    /// </summary>
    public class Team
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Team"/> class.
        /// </summary>
        /// <param name="players"> The players for this Team. </param>
        /// <param name="vcID"> The Voice Channel ID for Team VC. </param>
        public Team(List<Player> players, bool isA)
        {
            this.Players = players;

            this.AverageRating = this.GetAverageRating();
            this.IsTeamA = isA;
        }

        /// <summary>
        /// Gets the Players on the team.
        /// </summary>
        public List<Player> Players { get; }

        /// <summary>
        /// Gets or Sets the UId of this Teams voice channel.
        /// </summary>
        public ulong VoiceID { get; set; }

        /// <summary>
        /// Gets or Sets the UId of this Teams text channel.
        /// </summary>
        public ulong TextID { get; set; }

        /// <summary>
        /// Gets or Sets the UId of this Teams role.
        /// </summary>
        public ulong RoleID { get; set; }

        /// <summary>
        /// Gets the Average <see cref="RatingUtils"/> of this Team.
        /// </summary>
        public int AverageRating { get; }

        /// <summary>
        /// Gets a value indicating whether this Team is teamA.
        /// </summary>
        public bool IsTeamA { get; } = false;

        public string GetPlayerNamesAndRanksString()
        {
            string names = string.Empty;
            foreach (var player in this.Players)
            {
                names += $"{RatingUtils.GetTierEmoteAt((SkillTier)player.CurrentTier)} {player.Username}\n";
            }

            return names;
        }

        private int GetAverageRating()
        {
            int skillRating = 0;
            if (this.Players.Count != 0)
            {
                foreach (Player player in this.Players)
                {
                    skillRating += player.GetCurrentRanting();
                }

                int skillAverage = skillRating / this.Players.Count;

                return skillAverage;
            }

            return skillRating;
        }
    }
}