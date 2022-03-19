// <copyright file="MatchManager.cs" company="GambleDev">
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
            this.Client.UserVoiceStateUpdated += this.OnVoiceUpdate;

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
        public async Task<List<Player>?> QueuePlayer(SocketCommandContext context, Player player)
        {
            player.IsQueued = true;

            // Is the user currently connected to a voice channel in the server?
            if (context.User is SocketGuildUser guildUser)
            {
                if (guildUser.VoiceChannel == context.Guild.GetChannel(CheckersConstants.QueueVoice))
                {
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
                else
                {
                    await context.Message.ReplyAsync($"Can't join Queue if not present in {context.Guild.GetChannel(CheckersConstants.QueueVoice).Name}.");
                    return null;
                }
            }
            else
            {
                await context.Message.ReplyAsync($"Can't join Queue through DM's. Queue through the Official Checkers Server.");
                return null;
            }
        }

        /// <summary>
        /// Queue a player.
        /// </summary>
        /// <param name="guild"> The Checkers Guild. </param>
        /// <param name="player"> the player being added to the Queue. </param>
        /// <returns> A list containing all relevant players. If the Queue pops, the list will contain all relevant players. </returns>
        public async Task<List<Player>?> QueuePlayer(SocketGuild guild, Player player)
        {
            player.IsQueued = true;

            // At some point this functin should return if theres 12 sufficiently close players in the Q.
            if (this.Queue.AddToQueue(player))
            {
                var match = await this.InitialiseMatch(guild);
                return match.GetPlayers();
            }

            var players = new List<Player>();
            players.Add(player);
            return players;
        }

        public async Task UnQueuePlayer(SocketCommandContext context, Player player)
        {
            if (player.IsQueued)
            {
                this.Queue.RemoveFromQueue(player);
                await context.Message.AddReactionAsync(Emoji.Parse(":white_check_mark:"));
            }
        }

        public async Task UnQueuePlayer(SocketUser user, Player player)
        {
            if (player.IsQueued)
            {
                this.Queue.RemoveFromQueue(player);
                player.IsQueued = false;

                await this.DataAccessLayer.UpdatePlayer(player);
                await user.SendMessageAsync("You've been removed from the Queue.");
            }
        }

        public async Task StartMapVote(Match match, ISocketMessageChannel channel)
        {
            MapVoteManager mapVoteManager = new MapVoteManager(this, channel.Id, match);

            if (channel is SocketGuildChannel guildChannel)
            {
                var guild = guildChannel.Guild;

                foreach (MapVote vote in mapVoteManager.Maps)
                {
                    if (!await match.MakeVote(guild, channel.Id, vote))
                    {
                        await mapVoteManager.RemoveMapVotesFromMatch(match, guild);
                        break;
                    }
                }

                await CheckersMessageFactory.MakeMatchMapVote(channel, mapVoteManager);
            }
        }

        public async Task StartDisconnectPlayerVote(SocketCommandContext context, ulong playerId)
        {
            var player = this.DataAccessLayer.HasPlayer(context.User.Id);

            if (player != null)
            {
                var targetPlayer = this.DataAccessLayer.HasPlayer(playerId);

                if (targetPlayer != null)
                {
                    var match = this.GetMatchFromMatchChannel(context.Channel.Id);

                    if (match != null)
                    {
                        if (match.HasPlayer(targetPlayer) && match.HasPlayer(player))
                        {
                            EndMatchVote forfeitVote = new EndMatchVote(player, context.Channel.Id, VoteType.Disconnect, match, MatchOutcome.Cancelled, targetPlayer);

                            if (await match.MakeVote(context.Guild, context.Channel.Id, forfeitVote))
                            {
                                if (await CheckersMessageFactory.MakeMatchVote(context, forfeitVote))
                                {
                                    // Vote instantly has enough to be fulfilled.
                                    await this.ProcessMatch(forfeitVote, context.Channel);
                                }
                            }
                        }
                        else
                        {
                            await context.Message.ReplyAsync("Both the Player and the Player mentioned must belong to the match.");
                        }
                    }
                    else
                    {
                        await context.Message.ReplyAsync("Invalid Channel for forfeiting a Match. If you're trying to forfeit a Match, please do so from your team text channel.");
                    }
                }
                else
                {
                    await context.Message.ReplyAsync("Player does not exist in the Database.");
                }
            }
        }

        public async Task StartMatchForfeitVote(SocketCommandContext context)
        {
            var player = this.DataAccessLayer.HasPlayer(context.User.Id);

            if (player != null)
            {
                var match = this.GetMatchFromTeamChannel(context.Channel.Id);

                if (match != null)
                {
                    Team? team = match.GetTeamOfPlayer(player);

                    if (team != null)
                    {
                        MatchOutcome outcome;
                        if (team.IsTeamA)
                        {
                            outcome = MatchOutcome.TeamB;
                        }
                        else
                        {
                            outcome = MatchOutcome.TeamA;
                        }

                        EndMatchVote forfeitVote = new EndMatchVote(player, context.Channel.Id, VoteType.Forfeit, match, outcome);

                        if (await match.MakeVote(context.Guild, context.Channel.Id, forfeitVote))
                        {
                            if (await CheckersMessageFactory.MakeMatchVote(context, forfeitVote))
                            {
                                // Vote instantly has enough to be fulfilled.
                                await this.ProcessMatch(forfeitVote, context.Channel);
                            }
                        }
                    }
                }
                else
                {
                    await context.Message.ReplyAsync("Invalid Channel for forfeiting a Match. If you're trying to forfeit a Match, please do so from your team text channel.");
                }
            }
        }

        /// <summary>
        /// Start a match vote.
        /// </summary>
        /// <param name="state"> The state of which the player is voting to end the match. </param>
        /// <param name="context"> The context of the command. </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task StartEndMatchVote(string state, SocketCommandContext context)
        {
            var player = this.DataAccessLayer.HasPlayer(context.User.Id);
            if (player != null)
            {
                var match = this.GetMatchFromMatchChannel(context.Channel.Id);

                if (match != null)
                {
                    if (match.HasStarted)
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

                                if (matchVote != null)
                                {
                                    if (await match.MakeVote(context.Guild, context.Channel.Id, matchVote))
                                    {
                                        await CheckersMessageFactory.MakeMatchVote(context, (EndMatchVote)matchVote);
                                    }
                                }
                            }
                        }
                        else
                        {
                            await context.Message.ReplyAsync("Invalid argument for ending a match. EndMatch commands must state whether the player registering the result has won, lost or drawn their match.");
                        }
                    }
                    else
                    {
                        await context.Message.ReplyAsync("The match must start before you can vote for the outcome.");
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
            var match = matchVote.Match;

            if (channel is IGuildChannel guildChannel)
            {
                // 2nd argument is null unless a player is being targeted.
                CheckersMatchResult result = new CheckersMatchResult(match, matchVote.MatchOutcome);

                var list = await this.rankManager.ProcessMatchResult(guildChannel.Guild, result, matchVote.TargetPlayer);
                await this.CleanUpMatch(match, channel);

                var general = await guildChannel.Guild.GetChannelAsync(CheckersConstants.GeneralText);

                if (general != null)
                {
                    if (matchVote.MatchOutcome != MatchOutcome.Cancelled)
                    {
                        await CheckersMessageFactory.MakeMatchResultsSummary((ISocketMessageChannel)general, list, matchVote);
                    }
                    else
                    {
                        await CheckersMessageFactory.MakeMatchCancelledMessage((ISocketMessageChannel)general, matchVote, list);
                    }
                }

                var guild = guildChannel.Guild;

                foreach (var data in list)
                {
                    var player = data.Player;

                    player.IsQueued = false;
                    player.IsPlaying = false;

                    await this.DataAccessLayer.UpdatePlayer(player);

                    if (data.Promoted != null)
                    {
                        var user = await guild.GetUserAsync(player.Id);
                        if (user != null)
                        {
                            await user.AddRoleAsync(guild.GetRole(RatingUtils.GetRoleOfTier((SkillTier)player.CurrentTier)));

                            ulong oldRoleID;

                            if (data.Promoted == true)
                            {
                                oldRoleID = RatingUtils.GetRoleOfTier((SkillTier)player.CurrentTier - 1);
                            }
                            else
                            {
                                oldRoleID = RatingUtils.GetRoleOfTier((SkillTier)player.CurrentTier + 1);
                            }

                            await user.RemoveRoleAsync(oldRoleID);
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
                        var lobbyVoice = context.Guild.GetVoiceChannel(CheckersConstants.LobbyVoice);
                        await user.ModifyAsync(x => x.Channel = lobbyVoice);
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


        private async Task CleanUpMatch(Match match, ISocketMessageChannel channel)
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
                            var lobbyVoice = guildChannel.Guild.GetVoiceChannel(CheckersConstants.LobbyVoice);
                            await user.ModifyAsync(x => x.Channel = lobbyVoice);
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
        }

        public async Task SelectMap(SocketGuildChannel channel, MapVote vote)
        {
            vote.Match.SetMap(vote.Maptype, vote.Title);
            vote.Match.Start();

            await vote.Match.Channels.ChangeTextPerms(channel.Guild, channel.Id, true);

            if (channel is ISocketMessageChannel messageChannel)
            {
                await messageChannel.SendMessageAsync($"The chosen map is {vote.Title}");
            }
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

        private async Task OnVoiceUpdate(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            var player = this.DataAccessLayer.HasPlayer(user.Id);

            if (user is SocketGuildUser guildUser)
            {
                var queueVoice = guildUser.Guild.GetVoiceChannel(CheckersConstants.QueueVoice);

                if (queueVoice != null)
                {
                    if (player != null)
                    {
                        if (oldState.VoiceChannel == queueVoice && newState.VoiceChannel != queueVoice)
                        {
                            if (player.IsQueued && !player.IsPlaying)
                            {
                                await this.UnQueuePlayer(user, player);
                            }
                        }
                        else if (newState.VoiceChannel == queueVoice)
                        {
                            if (!player.IsQueued && !player.IsPlaying)
                            {
                                var players = await this.QueuePlayer(guildUser.Guild, player);

                                if (players != null)
                                {
                                    foreach (Player matchPlayer in players)
                                    {
                                        await this.DataAccessLayer.UpdatePlayer(matchPlayer);
                                    }
                                }
                            }
                            else
                            {
                                await user.SendMessageAsync("Cannot join queue while player is already queued or playing a match. If your match has ended, make sure to register your match in its repective channel.");
                                return;
                            }
                        }
                    }
                }
            }
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

                    // Make Map Vote
                    await this.StartMapVote(match, socketchannel);
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
