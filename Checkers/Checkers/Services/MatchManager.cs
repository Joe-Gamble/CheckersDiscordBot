﻿namespace Checkers.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Common;
    using Checkers.Components;
    using Checkers.Components.Voting;
    using Checkers.Data;
    using Checkers.Data.Models;
    using Checkers.Services.Generic;
    using Discord;
    using Discord.Addons.Hosting;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Match Manager for Checkers.
    /// </summary>
    public class MatchManager : CheckersService
    {
        private readonly IServiceProvider provider;
        private readonly CommandService service;
        private readonly IConfiguration configuration;
        private readonly RankedManager rankManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchManager"/> class.
        /// </summary>
        /// <param name="provider"> The <see cref="IServiceProvider"/> that should be injected. </param>
        /// <param name="client"> The <see cref="DiscordSocketClient"/> that should be injected. </param>
        /// <param name="logger"> The <see cref="ILogger"/> that should be injected. </param>
        /// <param name="service"> The <see cref="CommandService"/> that should be injected. </param>
        /// <param name="configuration"> The <see cref="IConfiguration"/> that should be injected. </param>
        /// <param name="dataAccessLayer"> The <see cref="DataAccessLayer"/> that should be injected. </param>
        /// <param name="rm"> The <see cref="RankedManager"/> that should be injected. </param>
        public MatchManager(IServiceProvider provider, DiscordSocketClient client, ILogger<DiscordClientService> logger, CommandService service, IConfiguration configuration, DataAccessLayer dataAccessLayer, RankedManager rm)
            : base(client, logger, configuration, dataAccessLayer)
        {
            this.provider = provider;
            this.service = service;
            this.configuration = configuration;
            this.rankManager = rm;

            client.ChannelCreated += this.OnChannelCreated;

            this.Queue = new CheckersQueue();
            this.Matches = new List<Match>();
        }

        /// <summary>
        /// Gets the Checkers Queue.
        /// </summary>
        public CheckersQueue Queue { get; }

        private List<Match> Matches { get; set; }

        /// <summary>
        /// Queue a player.
        /// </summary>
        /// <param name="context"> The context of the message. </param>
        /// <param name="player"> the player being added to the Queue. </param>
        /// <returns> A list containing all relevant players. If the Queue pops, the list will contain all relevant players. </returns>
        public async Task<List<Player>> QueuePlayer(SocketCommandContext context, Player player)
        {
            player.IsQueued = true;

            await context.Message.AddReactionAsync(Emoji.Parse(":white_check_mark:"));

            // At some point this functin should return if theres 12 sufficiently close players in the Q.
            if (this.Queue.AddToQueue(player))
            {
                var match = await this.InitialiseMatch(context.Guild);
                return match.GetPlayers();
            }

            var players = new List<Player>();
            players.Add(player);
            return players;
        }

        public async Task UnQueuePlayer(SocketCommandContext context, Player player)
        {
            if (player.IsActive)
            {
                if (player.IsQueued)
                {
                    this.Queue.RemoveFromQueue(player);
                    await context.Message.AddReactionAsync(Emoji.Parse(":white_check_mark:"));
                }
            }
        }

        /// <summary>
        /// Start a match vote.
        /// </summary>
        /// <param name="state"> The state of which the player is voting to end the match. </param>
        /// <param name="context"> The context of the command. </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task StartMatchVote(string state, SocketCommandContext context)
        {
            var player = this.DataAccessLayer.HasPlayer(context.User.Id);
            if (player != null)
            {
                var match = this.GetMatchFromMatchChannel(context.Channel.Id);

                if (match != null)
                {
                    Vote? matchVote = null;

                    if (state == "win" || state == "lose" || state == "draw")
                    {
                        Team? team = match.GetTeamOfPlayer(player);
                        if (team != null)
                        {
                            switch (state)
                            {
                                case "win":
                                    {
                                        if (team.IsTeamA)
                                        {
                                            matchVote = new EndMatchVote(player, context.Channel.Id, VoteType.EndMatch, match, MatchOutcome.TeamA);
                                        }
                                        else
                                        {
                                            matchVote = new EndMatchVote(player, context.Channel.Id, VoteType.EndMatch, match, MatchOutcome.TeamB);
                                        }

                                        break;
                                    }

                                case "lose":
                                    {
                                        if (team.IsTeamA)
                                        {
                                            matchVote = new EndMatchVote(player, context.Channel.Id, VoteType.EndMatch, match, MatchOutcome.TeamB);
                                        }
                                        else
                                        {
                                            matchVote = new EndMatchVote(player, context.Channel.Id, VoteType.EndMatch, match, MatchOutcome.TeamA);
                                        }

                                        break;
                                    }

                                case "draw":
                                    {
                                        matchVote = new EndMatchVote(player, context.Channel.Id, VoteType.EndMatch, match, MatchOutcome.Draw);
                                        break;
                                    }
                            }
                        }

                        if (await match.MakeVote(context.Guild, context.Channel.Id, matchVote))
                        {
                            await CheckersMessageFactory.MakeMatchVote(context, match);
                        }
                    }
                    else
                    {
                        await context.Message.ReplyAsync("Invalid argument for ending a match. EndMatch commands must state whether the player registering the result has won, lost or drawn their match.");
                    }
                }
                else
                {
                    await context.Message.ReplyAsync("Invalid Channel for ending a Match. If you're trying to end a Match, please do so from the Match text channel.");
                }
            }
        }

        /// <summary>
        /// Process  finished match.
        /// </summary>
        /// <param name="matchVote"> The match vote. </param>
        /// <param name="channel"> The context of the command. </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ProcessMatch(EndMatchVote matchVote, ISocketMessageChannel channel)
        {
            var match = this.GetMatchFromMatchChannel(channel.Id);

            if (match != null)
            {
                if (channel is IGuildChannel guildChannel)
                {
                    CheckersMatchResult result = new CheckersMatchResult(match, matchVote.MatchOutcome);

                    var list = await this.rankManager.ProcessMatchResult(result);
                    var players = await this.CleanUpMatch(match, channel);

                    var general = await guildChannel.Guild.GetChannelAsync(CheckersConstants.GeneralText);

                    if (general != null)
                    {
                        await CheckersMessageFactory.MakeMatchResultsSummary((ISocketMessageChannel)general, list, matchVote);
                    }

                    if (players != null)
                    {
                        foreach (Player player in players)
                        {
                            player.IsQueued = false;
                            player.IsPlaying = false;
                            await this.DataAccessLayer.UpdatePlayer(player);
                        }
                    }
                }
            }
        }

        // This will be private.
        // Change to add match params.
        public async Task<List<Player>?> CleanUpMatch(SocketCommandContext context)
        {
            var match = this.GetMatchFromMatchChannel(context.Channel.Id);

            if (match == null)
            {
                await context.Message.ReplyAsync("Invalid Channel for ending a Match. If you're trying to end a Match, please do so from the Match text channel.");
                return null;
            }

            var players = new List<Player>(match.GetPlayers());

            foreach (Player matchPlayer in players)
            {
                // Make sure every player in the match is still a member of the server. (Rage quit lol)
                var user = context.Guild.GetUser(matchPlayer.Id);

                if (user != null)
                {
                    // Is the user currently connected to a voice channel in the server?
                    var channel = user.VoiceChannel;
                    if (channel != null)
                    {
                        // If so, move them tback to the lobby.
                        var queueVoice = context.Guild.GetChannel(CheckersConstants.QueueVoice) as SocketVoiceChannel;
                        await user.ModifyAsync(x => x.Channel = queueVoice);
                    }

                    var team = match.GetTeamOfPlayer(matchPlayer);

                    // player is on a team
                    if (team != null)
                    {
                        // if team role exists in server
                        var role = context.Guild.GetRole(team.RoleID);

                        if (role != null)
                        {
                            // remove role from user
                            await user.RemoveRoleAsync(role);
                        }
                    }
                }
            }

            // Finally clean-up the match channels from the guild.
            await match.Channels.RemoveChannels(context.Guild);
            this.Matches.Remove(match);

            return players;
        }

        // This will be private.
        // Change to add match params.
        private async Task<List<Player>?> CleanUpMatch(Match match, ISocketMessageChannel channel)
        {
            var guildChannel = channel as SocketGuildChannel;
            var players = new List<Player>(match.GetPlayers());

            if (guildChannel != null)
            {
                foreach (Player matchPlayer in players)
                {
                    // Make sure every player in the match is still a member of the server. (Rage quit lol)
                    var user = guildChannel.GetUser(matchPlayer.Id);

                    if (user != null)
                    {
                        // Is the user currently connected to a voice channel in the server?
                        var voiceChannel = user.VoiceChannel;
                        if (voiceChannel != null)
                        {
                            // If so, move them tback to the lobby.
                            var queueVoice = guildChannel.Guild.GetChannel(CheckersConstants.QueueVoice) as SocketVoiceChannel;
                            await user.ModifyAsync(x => x.Channel = queueVoice);
                        }

                        var team = match.GetTeamOfPlayer(matchPlayer);

                        // player is on a team
                        if (team != null)
                        {
                            // if team role exists in server
                            var role = guildChannel.Guild.GetRole(team.RoleID);

                            if (role != null)
                            {
                                // remove role from user
                                await user.RemoveRoleAsync(role);
                            }
                        }
                    }
                }
                // Finally clean-up the match channels from the guild.
                await match.Channels.RemoveChannels(guildChannel.Guild);
                this.Matches.Remove(match);
            }
            return players;
        }

        /// <summary>
        /// Get a match of a player.
        /// </summary>
        /// <param name="player"> The player to search for. </param>
        /// <returns> A match if a player is found. Null otherwise.</returns>
        public Match? GetMatchOfPLayer(Player player)
        {
            foreach (Match match in this.Matches)
            {
                if (match.HasPlayer(player))
                {
                    return match;
                }
            }

            return null;
        }

        /// <summary>
        /// Get a match from a match channel.
        /// </summary>
        /// <param name="channelID"> The ID of the match channel. </param>
        /// <returns> A Match if one is found, null otherwise. </returns>
        public Match? GetMatchFromMatchChannel(ulong channelID)
        {
            foreach (Match match in this.Matches)
            {
                if (match.Channels.MatchText == channelID)
                {
                    return match;
                }
            }

            return null;
        }

        /// <summary>
        /// Get a match from a team channel.
        /// </summary>
        /// <param name="channelID"> The ID of teh team channel. </param>
        /// <returns> A match if one is found, null otherwise. </returns>
        public Match? GetMatchFromTeamChannel(ulong channelID)
        {
            foreach (Match match in this.Matches)
            {
                if (match.Channels.ATc == channelID || match.Channels.BTc == channelID)
                {
                    return match;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await this.service.AddModulesAsync(Assembly.GetEntryAssembly(), this.provider);
        }

        private async Task OnChannelCreated(SocketChannel channel)
        {
            var match = this.Matches.Last();

            if (channel.Id == match.Channels.MatchText)
            {
                if (channel is ISocketMessageChannel socketchannel)
                {
                    // Match introduction here.
                    await CheckersMessageFactory.MakeMatchSummary(socketchannel, match);
                }
            }
        }

        /// <summary>
        /// Make a Checkers match.
        /// </summary>
        /// <returns> A new Match. </returns>
        private Match MakeMatch()
        {
            this.MakeTeams(out Team teamA, out Team teamB);

            Match match = new (teamA, teamB);

            foreach (Player player in match.GetPlayers())
            {
                player.IsQueued = false;
                player.IsPlaying = true;
            }

            return match;
        }

        private async Task<Match> InitialiseMatch(SocketGuild guild)
        {
            var match = this.MakeMatch();

            match.Channels = await MatchChannels.BuildMatchChannel(guild, match);
            this.Matches.Add(match);

            return match;
        }

        private void MakeTeams(out Team teamA, out Team teamB)
        {
            var players = this.Queue.Pop();
            players.OrderBy(player => player.Rating).ToList();

            List<Player> playersA = new List<Player>();
            List<Player> playersB = new List<Player>();

            var playersLeft = players.Count;

            bool takeFromTop = true;

            for (int i = 0; i < playersLeft; i++)
            {
                if (takeFromTop)
                {
                    players[i].IsQueued = false;
                    players[i].IsPlaying = true;

                    if ((i + 1) % 2 != 0)
                    {
                        playersA.Add(players[i]);
                    }
                    else
                    {
                        playersB.Add(players[i]);
                        takeFromTop = false;
                    }
                }
                else
                {
                    players[playersLeft - 1].IsQueued = false;
                    players[playersLeft - 1].IsPlaying = true;

                    if (playersLeft % 2 == 0)
                    {
                        playersA.Add(players[playersLeft - 1]);
                    }
                    else
                    {
                        playersB.Add(players[playersLeft - 1]);
                        takeFromTop = true;
                    }

                    playersLeft--;
                    i--;
                }
            }

            teamA = new Team(playersA, true);
            teamB = new Team(playersB, false);
        }
    }
}
