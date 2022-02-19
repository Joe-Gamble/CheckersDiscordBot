namespace Checkers.Services
{
    using Checkers.Data;
    using Discord.Addons.Hosting;
    using Discord.WebSocket;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A custom implementation of <see cref="DiscordClientService"/> for Checkers.
    /// </summary>
    public abstract class CheckersService : DiscordClientService
    {
        public new readonly DiscordSocketClient Client;
        public readonly IConfiguration Config;
        public readonly DataAccessLayer DataAccessLayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckersService"/> class.
        /// </summary>
        /// <param name="client"> The <see cref="DiscordSocketClient"/> to be injected.</param>
        /// <param name="logger"> The <see cref="ILogger"/> to be injected. </param>
        /// <param name="configuration"> The <see cref="IConfiguration"/> to be injected. </param>
        /// <param name="dataAccessLayer"> The <see cref="DataAccessLayer"/> to be injected. </param>
        public CheckersService(DiscordSocketClient client, ILogger<DiscordClientService> logger, IConfiguration configuration, DataAccessLayer dataAccessLayer)
            : base(client, logger)
        {
            this.Client = client;
            this.DataAccessLayer = dataAccessLayer;
            this.Config = configuration;
            this.DataAccessLayer = dataAccessLayer;
        }
    }
}
