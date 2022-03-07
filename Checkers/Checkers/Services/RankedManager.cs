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

        public async Task<List<PlayerMatchData>> ProcessMatchResult(CheckersMatchResult result, Player? player = null)
        {
            List<PlayerMatchData> playerResultList = new List<PlayerMatchData>();

            switch (result.Outcome)
            {
                case MatchOutcome.Cancelled:
                    {
                        if (player != null)
                        {
                            RatingUtils.Lose(player, 50);

                            // This might need to be a list of players eventually.
                            playerResultList.Add(new PlayerMatchData(player, false, 50));
                        }

                        // Do we have to do something here?
                        break;
                    }

                case MatchOutcome.Draw:
                    {
                        this.ProcessDraw(result.GetAllPlayers(), out playerResultList);
                        return playerResultList;
                    }

                case MatchOutcome.TeamA:
                    {
                        this.CalculateTeamAPoints(playerResultList, result.TeamA, result.SkillFavor, result.Multiplier, true);
                        this.CalculateTeamBPoints(playerResultList, result.TeamB, result.SkillFavor, result.Multiplier, false);
                        break;
                    }

                case MatchOutcome.TeamB:
                    {
                        this.CalculateTeamAPoints(playerResultList, result.TeamA, result.SkillFavor, result.Multiplier, false);
                        this.CalculateTeamBPoints(playerResultList, result.TeamB, result.SkillFavor, result.Multiplier, true);
                        break;
                    }
            }

            return playerResultList;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await this.service.AddModulesAsync(Assembly.GetEntryAssembly(), this.provider);
        }

        private void ProcessDraw(List<Player> players, out List<PlayerMatchData> data)
        {
            data = new List<PlayerMatchData>();

            foreach (Player player in players)
            {
                data.Add(new PlayerMatchData(player, null, 0));
                player.AddGamePlayed();
            }
        }

        private void CalculateTeamAPoints(List<PlayerMatchData> playerResultList, Team team, SkillFavors favors, double multiplier, bool win)
        {
            var average = team.AverageRating;
            var ratingBase = CheckersConstants.StandardWin;

            foreach (Player player in team.Players)
            {
                double playerWeighting = (double)(player.Rating / (double)average);
                double playerRating = 0;
                bool promoted = false;

                if (favors != SkillFavors.Equal)
                {
                    if (win)
                    {
                        if (favors == SkillFavors.TeamA)
                        {
                            playerRating = ratingBase - (ratingBase * multiplier);
                            playerRating /= playerWeighting;
                        }
                        else if (favors == SkillFavors.TeamB)
                        {
                            playerRating = ratingBase + (ratingBase * multiplier);
                            playerRating /= playerWeighting; 
                        }

                        if (RatingUtils.Gain(player, (int)playerRating))
                        {
                            promoted = true;
                        }
                    }
                    else
                    {
                        if (favors == SkillFavors.TeamA)
                        {
                            playerRating = ratingBase + (ratingBase * multiplier);
                            playerRating *= playerWeighting;
                        }
                        else if (favors == SkillFavors.TeamB)
                        {
                            playerRating = ratingBase - (ratingBase * multiplier);
                            playerRating *= playerWeighting;
                        }

                        if (RatingUtils.Lose(player, (int)playerRating))
                        {
                            // Demoted
                        }
                    }
                }
                else
                {
                    if (win)
                    {
                        playerRating = CheckersConstants.StandardWin;

                        if (RatingUtils.Gain(player, (int)playerRating))
                        {
                            promoted = true;
                        }
                    }
                    else
                    {
                        playerRating = CheckersConstants.StandardWin;

                        if (RatingUtils.Lose(player, (int)playerRating))
                        {
                            // Demoted
                        }
                    }
                }

                PlayerMatchData data = new PlayerMatchData(player, win, (int)playerRating, promoted);

                playerResultList.Add(data);
                player.AddGamePlayed(win);
            }
        }

        private void CalculateTeamBPoints(List<PlayerMatchData> playerResultList, Team team, SkillFavors favors, double multiplier, bool win)
        {
            var average = team.AverageRating;
            var ratingBase = CheckersConstants.StandardWin;

            foreach (Player player in team.Players)
            {
                double playerWeighting = (double)(player.Rating / (double)average);
                double playerRating = 0;
                bool promoted = false;

                if (favors != SkillFavors.Equal)
                {
                    if (win)
                    {
                        if (favors == SkillFavors.TeamB)
                        {
                            playerRating = ratingBase - (ratingBase * multiplier);
                            playerRating /= playerWeighting;
                        }
                        else if (favors == SkillFavors.TeamA)
                        {
                            playerRating = ratingBase + (ratingBase * multiplier);
                            playerRating /= playerWeighting;
                        }

                        if (RatingUtils.Gain(player, (int)playerRating))
                        {
                            promoted = true;
                        }
                    }
                    else
                    {
                        if (favors == SkillFavors.TeamB)
                        {
                            playerRating = ratingBase + (ratingBase * multiplier);
                            playerRating *= playerWeighting;
                        }
                        else if (favors == SkillFavors.TeamA)
                        {
                            playerRating = ratingBase - (ratingBase * multiplier);
                            playerRating *= playerWeighting;
                        }

                        if (RatingUtils.Lose(player, (int)playerRating))
                        {
                            // Demoted
                        }
                    }
                }
                else
                {
                    if (win)
                    {
                        playerRating = CheckersConstants.StandardWin;

                        if (RatingUtils.Gain(player, (int)playerRating))
                        {
                            promoted = true;
                        }
                    }
                    else
                    {
                        playerRating = CheckersConstants.StandardWin;

                        if (RatingUtils.Lose(player, (int)playerRating))
                        {
                            // Demoted
                        }
                    }
                }

                PlayerMatchData data = new PlayerMatchData(player, win, (int)playerRating, promoted);

                playerResultList.Add(data);
                player.AddGamePlayed(win);
            }
        }
    }
}
