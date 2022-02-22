namespace Checkers.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Components;
    using Checkers.Data;
    using Checkers.Data.Models;
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
        /// On Player Queued.
        /// </summary>
        /// <param name="context"> The Command Context for this event. </param>
        /// /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public delegate Task<Player> OnPlayerQueued(SocketCommandContext context, Player player);

        /// <summary>
        /// On Player DeQueued.
        /// </summary>
        /// <param name="context"> The Command Context for this event. </param>
        /// /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public delegate Task<Player> OnPlayerDeQueued(SocketCommandContext context, Player player);

        /// <summary>
        /// On Match Found.
        /// </summary>
        /// <param name="context"> The Command Context for this event. </param>
        /// <returns> The Match generated on trigger. </returns>
        public delegate Task<Match> OnMatchFound(SocketCommandContext context);

        /// <summary>
        /// On Match End.
        /// </summary>
        /// <param name="context"> The Command Context for this event. </param>
        /// <param name="player"> The player that called this command. </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public delegate Task OnMatchEnd(SocketCommandContext context);

        /// <summary>
        /// On Match Found.
        /// </summary>
        /// <param name="context"> The Command Context for this event. </param>
        public event OnMatchFound MatchFound;

        /// <summary>
        /// On Match End.
        /// </summary>
        /// <param name="context"> The Command Context for this event. </param>
        public event OnMatchEnd MatchEnd;

        /// <summary>
        /// OnQueue.
        /// </summary>
        /// <param name="context"> The Command Context for this event. </param>
        public event OnPlayerQueued OnQueue;

        /// <summary>
        /// On DeQueue.
        /// </summary>
        /// <param name="context"> The Command Context for this event. </param>
        public event OnPlayerDeQueued OnDeQueue;

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
            if (this.Context.Channel.Id != CheckersConstants.GeneralText)
            {
                return;
            }

            var player = this.DataAccessLayer.HasPlayer(this.Context.User.Id);

            if (player != null)
            {
                if (player.IsActive && !player.IsQueued && !player.IsPlaying)
                {
                    // await this.ReplyAsync(DateTime.UtcNow.ToString());
                    player = await this.OnQueue.Invoke(this.Context, player);
                    await this.DataAccessLayer.UpdatePlayer(player);

                    // await this.ReplyAsync(DateTime.UtcNow.ToString());
                }
            }
        }

        [Command("UnQueue")]
        public async Task DeQueuePlayer()
        {
            if (this.Context.Channel.Id != CheckersConstants.GeneralText)
            {
                return;
            }

            var player = this.DataAccessLayer.HasPlayer(this.Context.User.Id);

            if (player != null)
            {
                if (player.IsActive && player.IsQueued && !player.IsPlaying)
                {
                    player = await this.OnDeQueue.Invoke(this.Context, player);
                    await this.DataAccessLayer.UpdatePlayer(player);
                }
            }
        }

        [Command("EndMatch")]
        public async Task EndMatch()
        {
            await this.MatchEnd.Invoke(this.Context);
        }
    }
}
