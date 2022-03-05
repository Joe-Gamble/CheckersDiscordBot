using Checkers.Common;
using Checkers.Components.Voting;
using Checkers.Data;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services.Generic
{
    public class ComponentHandler : CheckersService
    {
        private readonly IServiceProvider provider;
        private readonly CommandService service;
        private readonly IConfiguration configuration;

        private MatchManager matchManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentHandler"/> class.
        /// </summary>
        /// <param name="provider"> The <see cref="IServiceProvider"/> that should be injected. </param>
        /// <param name="client"> The <see cref="DiscordSocketClient"/> that should be injected. </param>
        /// <param name="logger"> The <see cref="ILogger"/> that should be injected. </param>
        /// <param name="service"> The <see cref="CommandService"/> that should be injected. </param>
        /// <param name="configuration"> The <see cref="IConfiguration"/> that should be injected. </param>
        /// <param name="dataAccessLayer"> The <see cref="DataAccessLayer"/> that should be injected. </param>
        public ComponentHandler(IServiceProvider provider, DiscordSocketClient client, ILogger<DiscordClientService> logger, CommandService service, IConfiguration configuration, DataAccessLayer dataAccessLayer, MatchManager mm)
            : base(client, logger, configuration, dataAccessLayer)
        {
            this.provider = provider;
            this.service = service;
            this.configuration = configuration;
            this.matchManager = mm;
        }

        public async Task ButtonHandler(SocketMessageComponent component)
        {
            var player = this.DataAccessLayer.HasPlayer(component.User.Id);

            if (player != null)
            {
                var match = this.matchManager.GetMatchOfPLayer(player);

                if (match != null)
                {
                    Vote vote = await match.GetVote(component.Channel.Id);

                    switch (component.Data.CustomId)
                    {
                        case "match_voteyes":
                            {
                                var result = await CheckersMessageFactory.ModifyVote(vote, component, player, true);
                                if (result)
                                {
                                    var guild = (component.Channel as IGuildChannel)?.Guild;

                                    if (guild != null)
                                    {
                                        await match.RemoveVote((SocketGuild)guild, component.Channel.Id, vote);
                                        await match.Channels.ChangeTextPerms((SocketGuild)guild, component.Channel.Id, true);
                                    }

                                    EndMatchVote? matchvote = vote as EndMatchVote;

                                    if (matchvote != null)
                                    {
                                        await this.matchManager.ProcessMatch(matchvote.MatchOutcome, component.Channel);
                                    }
                                }

                                break;
                            }

                        case "match_voteno":
                            {
                                var result = await CheckersMessageFactory.ModifyVote(vote, component, player, false);
                                if (result)
                                {
                                    var guild = (component.Channel as IGuildChannel)?.Guild;

                                    if (guild != null)
                                    {
                                        await match.RemoveVote((SocketGuild)guild, component.Channel.Id, vote);
                                        await match.Channels.ChangeTextPerms((SocketGuild)guild, component.Channel.Id, true);
                                    }
                                }

                                break;
                            }

                        case "match_accept":
                            {
                                break;
                            }

                        case "match_decline":
                            {
                                break;
                            }

                        default:
                            break;
                    }
                }
            }
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           this.Client.ButtonExecuted += this.ButtonHandler;
           await this.service.AddModulesAsync(Assembly.GetEntryAssembly(), this.provider);
        }
    }
}
