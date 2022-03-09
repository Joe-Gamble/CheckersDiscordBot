// <copyright file="RankedCommandsTest.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

using Checkers.Data;
using Checkers.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Modules
{
    public class RankedCommands : CheckersModuleBase
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly RankedManager rankManager;
        private readonly MatchManager matchManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchCommands"/> class.
        /// </summary>
        /// <param name="httpClientFactory"> The <see cref="IHttpClientFactory"/> to be used. </param>
        /// <param name=dataAccessLayer"> The <see cref="DataAccessLayer"/> to be used. </param>
        /// <param name="manager"> The <see cref="MatchManager"/> to be used. </param>
        public RankedCommands(IHttpClientFactory httpClientFactory, DataAccessLayer dataAccessLayer, RankedManager manager, MatchManager mManager)
            : base(dataAccessLayer)
        {
            this.httpClientFactory = httpClientFactory;
            this.rankManager = manager;
            this.matchManager = mManager;
        }

        [Command("Test")]
        public async Task TestCommand()
        {
            var player = this.DataAccessLayer.HasPlayer(this.Context.User.Id);
        }
    }
}
