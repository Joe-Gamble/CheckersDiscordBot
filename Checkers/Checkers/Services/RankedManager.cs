// <copyright file="RankedManager.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Components;
    using Checkers.Data;
    using Checkers.Data.Models;
    using Discord.Addons.Hosting;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class RankedManager : CheckersService
    {
        private readonly IServiceProvider provider;
        private readonly CommandService service;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="RankedManager"/> class.
        /// </summary>
        /// <param name="provider"> The <see cref="IServiceProvider"/> that should be injected. </param>
        /// <param name="client"> The <see cref="DiscordSocketClient"/> that should be injected. </param>
        /// <param name="logger"> The <see cref="ILogger"/> that should be injected. </param>
        /// <param name="service"> The <see cref="CommandService"/> that should be injected. </param>
        /// <param name="configuration"> The <see cref="IConfiguration"/> that should be injected. </param>
        /// <param name="dataAccessLayer"> The <see cref="DataAccessLayer"/> that should be injected. </param>
        public RankedManager(IServiceProvider provider, DiscordSocketClient client, ILogger<DiscordClientService> logger, CommandService service, IConfiguration configuration, DataAccessLayer dataAccessLayer)
            : base(client, logger, configuration, dataAccessLayer)
        {
            this.provider = provider;
            this.service = service;
            this.configuration = configuration;
        }

        public async Task ProcessMatchResult(CheckersMatchResult result)
        {
            switch (result.Outcome)
            {
                case MatchOutcome.Cancelled:
                    {
                        // Do we have to do something here?
                        break;
                    }

                case MatchOutcome.Draw:
                    {
                        // Add a game played to all users?
                        break;
                    }

                case MatchOutcome.TeamA:
                    {
                        this.CalculateTeamAPoints(result.TeamA, result.SkillFavor, result.Multiplier, true);
                        this.CalculateTeamBPoints(result.TeamB, result.SkillFavor, result.Multiplier, false);
                        break;
                    }

                case MatchOutcome.TeamB:
                    {
                        this.CalculateTeamAPoints(result.TeamA, result.SkillFavor, result.Multiplier, false);
                        this.CalculateTeamBPoints(result.TeamB, result.SkillFavor, result.Multiplier, true);
                        break;
                    }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await this.service.AddModulesAsync(Assembly.GetEntryAssembly(), this.provider);
        }

        private void CalculateTeamAPoints(Team team, SkillFavors favors, int multiplier, bool win)
        {
            var average = team.AverageRating;

            foreach (Player player in team.Players)
            {
                // The bigger the player weighting the smaller the multiplier should be
                // Higher weightings should always be punished more.

                var playerWeighting = player.Rating / average;
                var ratingBase = CheckersConstants.StandardWin;

                if (favors != SkillFavors.Equal)
                {
                    if (win)
                    {
                        if (favors == SkillFavors.TeamA)
                        {
                            var teamBase = ratingBase - (ratingBase / multiplier);

                            var playerMultiplier = multiplier / playerWeighting;
                            var playerRating = teamBase - playerMultiplier;

                            RatingUtils.Gain(player, playerRating);
                        }
                        else if (favors == SkillFavors.TeamB)
                        {
                            var teamBase = ratingBase + (ratingBase / multiplier);

                            var playerMultiplier = multiplier / playerWeighting;
                            var playerRating = teamBase - playerMultiplier;

                            RatingUtils.Gain(player, playerRating);
                        }
                    }
                    else
                    {
                        if (favors == SkillFavors.TeamA)
                        {
                            var teamBase = ratingBase + (ratingBase / multiplier);

                            var playerMultiplier = multiplier / playerWeighting;
                            var playerRating = teamBase - playerMultiplier;

                            RatingUtils.Lose(player, playerRating);
                        }
                        else if (favors == SkillFavors.TeamB)
                        {
                            var teamBase = ratingBase - (ratingBase / multiplier);

                            var playerMultiplier = multiplier / playerWeighting;
                            var playerRating = teamBase - playerMultiplier;

                            RatingUtils.Lose(player, playerRating);
                        }
                    }
                }
            }
        }

        private void CalculateTeamBPoints(Team team, SkillFavors favors, int multiplier, bool win)
        {
            var average = team.AverageRating;

            foreach (Player player in team.Players)
            {
                // The bigger the player weighting the smaller the multiplier should be
                // Higher weightings should always be punished more.

                var playerWeighting = player.Rating / average;
                var ratingBase = CheckersConstants.StandardWin;

                if (favors != SkillFavors.Equal)
                {
                    if (win)
                    {
                        if (favors == SkillFavors.TeamB)
                        {
                            var teamBase = ratingBase - (ratingBase / multiplier);

                            var playerMultiplier = multiplier / playerWeighting;
                            var playerRating = teamBase - playerMultiplier;

                            RatingUtils.Gain(player, playerRating);
                        }
                        else if (favors == SkillFavors.TeamA)
                        {
                            var teamBase = ratingBase + (ratingBase / multiplier);

                            var playerMultiplier = multiplier / playerWeighting;
                            var playerRating = teamBase - playerMultiplier;

                            RatingUtils.Gain(player, playerRating);
                        }
                    }
                    else
                    {
                        if (favors == SkillFavors.TeamB)
                        {
                            var teamBase = ratingBase + (ratingBase / multiplier);

                            var playerMultiplier = multiplier / playerWeighting;
                            var playerRating = teamBase - playerMultiplier;

                            RatingUtils.Lose(player, playerRating);
                        }
                        else if (favors == SkillFavors.TeamA)
                        {
                            var teamBase = ratingBase - (ratingBase / multiplier);

                            var playerMultiplier = multiplier / playerWeighting;
                            var playerRating = teamBase - playerMultiplier;

                            RatingUtils.Lose(player, playerRating);
                        }
                    }
                }
            }
        }
    }
}
