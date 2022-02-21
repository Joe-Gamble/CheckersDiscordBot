namespace Checkers.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Components;
    using Checkers.Data;
    using Checkers.Services;
    using Discord.Commands;

    /// <summary>
    /// MatchMaker module for Checkers.
    /// </summary>
    public class MatchMaker : CheckersModuleBase
    {
        private readonly IHttpClientFactory httpClientFactory;
        private MatchManager matchManger;

        /// <summary>
        /// On Match Found.
        /// </summary>
        /// <param name="context"> The Command Context for this event. </param>
        /// <returns> The Match generated on trigger. </returns>
        public delegate Task<Match> OnMatchFound(SocketCommandContext context);

        /// <summary>
        /// On Match Found.
        /// </summary>
        /// <param name="context"> The Command Context for this event. </param>
        public event OnMatchFound MatchFound;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchMaker"/> class.
        /// </summary>
        /// <param name="httpClientFactory"> The <see cref="IHttpClientFactory"/> to be used. </param>
        /// <param name=dataAccessLayer"> The <see cref="DataAccessLayer"/> to be used. </param>
        public MatchMaker(IHttpClientFactory httpClientFactory, DataAccessLayer dataAccessLayer)
            : base(dataAccessLayer)
        {
            this.httpClientFactory = httpClientFactory;
            this.matchManger = new MatchManager(this);
        }

        [Command("Queue")]
        public async Task QueuePlayer()
        {
            // Rough implemantion.
            //await MatchFound(context).ConfigureAwait(false);
        }
    }
}
