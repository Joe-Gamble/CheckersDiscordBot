namespace Checkers.Components.Voting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Data;
    using Checkers.Services;
    using Discord.Addons.Hosting;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class VoteManager : CheckersService
    {
        private readonly IServiceProvider provider;
        private readonly CommandService service;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoteManager"/> class.
        /// </summary>
        /// <param name="provider"> The <see cref="IServiceProvider"/> that should be injected. </param>
        /// <param name="client"> The <see cref="DiscordSocketClient"/> that should be injected. </param>
        /// <param name="logger"> The <see cref="ILogger"/> that should be injected. </param>
        /// <param name="service"> The <see cref="CommandService"/> that should be injected. </param>
        /// <param name="configuration"> The <see cref="IConfiguration"/> that should be injected. </param>
        /// <param name="dataAccessLayer"> The <see cref="DataAccessLayer"/> that should be injected. </param>
        public VoteManager(IServiceProvider provider, DiscordSocketClient client, ILogger<DiscordClientService> logger, CommandService service, IConfiguration configuration, DataAccessLayer dataAccessLayer)
            : base(client, logger, configuration, dataAccessLayer)
        {
            this.provider = provider;
            this.service = service;
            this.configuration = configuration;
        }

        /// <summary>
        /// Executes the Service.
        /// </summary>
        /// <param name="stoppingToken"> Stopping token. </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
