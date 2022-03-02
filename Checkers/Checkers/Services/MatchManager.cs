namespace Checkers.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
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
        private readonly ComponentHandler componentHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        /// <param name="provider"> The <see cref="IServiceProvider"/> that should be injected. </param>
        /// <param name="client"> The <see cref="DiscordSocketClient"/> that should be injected. </param>
        /// <param name="logger"> The <see cref="ILogger"/> that should be injected. </param>
        /// <param name="service"> The <see cref="CommandService"/> that should be injected. </param>
        /// <param name="configuration"> The <see cref="IConfiguration"/> that should be injected. </param>
        /// <param name="dataAccessLayer"> The <see cref="DataAccessLayer"/> that should be injected. </param>
        public MatchManager(IServiceProvider provider, DiscordSocketClient client, ILogger<DiscordClientService> logger, CommandService service, IConfiguration configuration, DataAccessLayer dataAccessLayer, RankedManager rm)
            : base(client, logger, configuration, dataAccessLayer)
        {
            this.provider = provider;
            this.service = service;
            this.configuration = configuration;
            this.rankManager = rm;
            //this.componentHandler = ch;

            client.ChannelCreated += this.OnChannelCreated;

            this.Queue = new CheckersQueue();
            this.Matches = new List<Match>();
        }

        /// <summary>
        /// Gets the Checkers Queue.
        /// </summary>
        public CheckersQueue Queue { get; }

        private List<Match> Matches { get; set; }

        private async Task OnChannelCreated(SocketChannel channel)
        {
            var match = this.Matches.Last();

            if (channel.Id == match.Channels.MatchText)
            {
                if (channel is ISocketMessageChannel)
                {
                    ISocketMessageChannel? socketchannel = (ISocketMessageChannel)channel;

                    // Match introductyion here.
                    await socketchannel.SendMessageAsync("This is a test.");
                }
            }
        }

        public async Task<List<Player>> QueuePlayer(SocketCommandContext context, Player player)
        {
            player.IsQueued = true;

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

        public void UnQueuePlayer(SocketCommandContext context, Player player)
        {
            if (player.IsActive)
            {
                if (player.IsQueued)
                {
                    this.Queue.RemoveFromQueue(player);
                }
            }
        }

        public async Task StartMatchVote(string state, SocketCommandContext context)
        {
            var player = this.DataAccessLayer.HasPlayer(context.User.Id);
            if (player != null)
            {
                var match = this.GetMatchFromMatchChannel(context.Channel.Id);

                if (match != null)
                {
                    if (state == "win")
                    {
                        Team? team = match.GetTeamOfPlayer(player);
                        if (team != null)
                        {
                            if (team.IsTeamA)
                            {
                                await match.Channels.ChangeTextPerms(context.Guild, match.Channels.MatchText, false);
                                // Team A win vote.
                            }
                            else
                            {
                                // Team B win vote.
                            }
                        }
                    }
                    else if (state == "loss")
                    {
                        Team? team = match.GetTeamOfPlayer(player);
                        if (team != null)
                        {
                            if (team.IsTeamA)
                            {
                                // Team B win vote.
                            }
                            else
                            {
                                // Team A win vote.
                            }
                        }
                    }
                    else if (state == "draw")
                    {
                        // Draw vote.
                    }
                    else
                    {
                        await context.Message.ReplyAsync("Invalid argument for ending a match. EndMatch commands must state whether the player registering the result has won, lost or drawn their match.");
                    }
                }
            }
        }

        public async Task CancelMatch(Match match)
        {
            if (match != null)
            {
                CheckersMatchResult result = new CheckersMatchResult(match, MatchOutcome.Cancelled);
                await this.rankManager.ProcessMatchResult(result);
            }
        }

        public async Task ProcessMatch(MatchOutcome outcome, SocketCommandContext context)
        {
            var match = this.GetMatchFromMatchChannel(context.Channel.Id);

            if (match != null)
            {
                CheckersMatchResult result = new CheckersMatchResult(match, MatchOutcome.TeamA);
                await this.rankManager.ProcessMatchResult(result);
            }
            else
            {
                await context.Message.ReplyAsync("Invalid Match.");
            }
        }

        public async Task<List<Player>?> MatchEnd(SocketCommandContext context)
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

            // FInally clean-up the match channels from the guild.
            await match.Channels.RemoveChannels(context.Guild);

            this.Matches.Remove(match);

            return players;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await this.service.AddModulesAsync(Assembly.GetEntryAssembly(), this.provider);
        }

        private async Task<Match> InitialiseMatch(SocketGuild guild)
        {
            var match = this.MakeMatch();

            match.Channels = await MatchChannels.BuildMatchChannel(guild, match);
            this.Matches.Add(match);

            return match;
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
