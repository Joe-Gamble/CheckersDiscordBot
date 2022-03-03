using Checkers.Common;
using Checkers.Components.Voting;
using Checkers.Data;
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
            // We can now check for our custom id
            switch (component.Data.CustomId)
            {
                // Since we set our buttons custom id as 'custom-id', we can check for it like this:
                case "match_voteyes":
                    {
                        var player = this.DataAccessLayer.HasPlayer(component.User.Id);

                        if (player != null)
                        {
                            var match = this.matchManager.GetMatchOfPLayer(player);

                            if (match != null)
                            {
                                Vote vote = await match.GetVote(component.Channel.Id);
                                if (!vote.AddVote(player))
                                {
                                    var embed = new CheckersEmbedBuilder().WithTitle($"Match Vote:      {vote.TotalVotes} / {vote.RequiredVotes}").AddField("Created By", vote.CreatedByPlayer, true).AddField("Proposal:", "Team A Win", true).Build();
                                    await component.UpdateAsync(x => x.Embed = embed);
                                }
                                else
                                {
                                    var embed = new CheckersEmbedBuilder().WithTitle($"Match Vote:      {vote.TotalVotes} / {vote.RequiredVotes}").AddField("Created By", vote.CreatedByPlayer, true).AddField("Proposal:", "Team A Win", true).Build();
                                    await component.UpdateAsync(x =>
                                        {
                                            x.Embed = embed;
                                            x.Components = new CheckersComponentBuilder(VoteType.EndMatch, true).Build(); 
                                        });

                                    await component.Channel.SendMessageAsync("Vote Passed!");
                                }

                                await component.DeferAsync();
                            }
                        }

                        break;
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
