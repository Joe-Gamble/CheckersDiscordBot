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
        public readonly DiscordSocketClient Client;
        public readonly ILogger Logger;
        public readonly IConfiguration Config;
        public readonly DataAccessLayer DataAccessLayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckersService"/> class.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="dataAccessLayer"></param>
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
