// <copyright file="MatchCommands.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Common;
    using Checkers.Components;
    using Checkers.Components.Voting;
    using Checkers.Data;
    using Checkers.Data.Models;
    using Checkers.Services;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;

    /// <summary>
    /// MatchMaker module for Checkers.
    /// </summary>
    public class MatchCommands : CheckersModuleBase
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly MatchManager matchManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchCommands"/> class.
        /// </summary>
        /// <param name="httpClientFactory"> The <see cref="IHttpClientFactory"/> to be used. </param>
        /// <param name=dataAccessLayer"> The <see cref="DataAccessLayer"/> to be used. </param>
        /// <param name="manager"> The <see cref="MatchManager"/> to be used. </param>
        public MatchCommands(IHttpClientFactory httpClientFactory, DataAccessLayer dataAccessLayer, MatchManager manager)
            : base(dataAccessLayer)
        {
            this.httpClientFactory = httpClientFactory;
            this.matchManager = manager;
        }

        [Command("Queue")]
        [Alias("Q")]
        public async Task QueuePlayer()
        {
            if (this.Context.Channel.Id != CheckersConstants.GeneralText)
            {
                return;
            }

            var player = this.DataAccessLayer.HasPlayer(this.Context.User.Id);


            if (player != null)
            {
                if (!player.IsActive)
                {
                    player.IsActive = true;
                }

                if (player.IsPlaying || player.IsQueued)
                {
                    await this.ReplyAsync("Cannot join queue while player is already queued or playing a match. If your match has ended, make sure to register your match in its repective channel.");
                    return;
                }

                var players = await this.matchManager.QueuePlayer(this.Context, player);

                if (players != null)
                {
                    foreach (Player matchPlayer in players)
                    {
                        await this.DataAccessLayer.UpdatePlayer(matchPlayer);
                    }
                }
            }
        }

        [Command("LeaveQueue")]
        [Alias("DQ", "UnQueue")]
        public async Task DeQueuePlayer()
        {
            if (this.Context.Channel.Id != CheckersConstants.GeneralText)
            {
                return;
            }

            var player = this.DataAccessLayer.HasPlayer(this.Context.User.Id);

            if (player != null)
            {
                if (player.IsPlaying || !player.IsQueued)
                {
                    await this.ReplyAsync("Cannot cancel a queue for a player that isn't in queue or is currently playing a match. Please register your current match to be able to queue again.");
                    return;
                }

                // UnQueue Player
                await this.matchManager.UnQueuePlayer(this.Context, player);

                player.IsQueued = false;
                await this.DataAccessLayer.UpdatePlayer(player);
            }
        }

        [Command("EndMatch")]
        [Alias("RegisterMatch")]
        public async Task EndMatch(string? arg = null)
        {
            if (arg != null)
            {
                var result = arg.ToLower();

                await this.matchManager.StartEndMatchVote(result, this.Context);
            }
            else
            {
                await this.ReplyAsync("Please specify the outcome of the match. For example, !EndMatch win");
            }
        }

        [Command("Forfeit")]
        public async Task ForfeitMatch()
        {
            await this.matchManager.StartMatchForfeitVote(this.Context);
        }

        [Command("RegisterDisconnect")]
        [Alias("DC", "Disconnect")]
        public async Task ForfeitMatch(SocketUser? user = null)
        {
            if (user != null)
            {
                await this.matchManager.StartDisconnectPlayerVote(this.Context, user.Id);
            }
        }

        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [Command("Destroy")]
        public async Task Destroy()
        {
            var players = await this.matchManager.CleanUpMatch(this.Context);

            if (players != null)
            {
                foreach (var player in players)
                {
                    player.IsQueued = false;
                    player.IsPlaying = false;
                    await this.DataAccessLayer.UpdatePlayer(player);
                }
            }
        }
    }
}
