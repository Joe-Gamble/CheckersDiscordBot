// <copyright file="CheckersModuleBase.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Data;
    using Discord;
    using Discord.Commands;
    using Discord.Rest;

    /// <summary>
    /// The custom implemenation of <see cref="ModuleBase{T}"/> for Checkers.
    /// </summary>
    public abstract class CheckersModuleBase : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// The <see cref="DataAccessLayer"/> of Checkers.
        /// </summary>
        public readonly DataAccessLayer DataAccessLayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckersModuleBase"/> class.
        /// </summary>
        /// <param name="dataAccessLayer"> The <see cref="DataAccessLayer"/> to inject. </param>
        public CheckersModuleBase(DataAccessLayer dataAccessLayer)
        {
            this.DataAccessLayer = dataAccessLayer;
        }

        /// <summary>
        /// Send an embed containing a title and descripion to a channel.
        /// </summary>
        /// <param name="title"> The title of the embed. </param>
        /// <param name="description"> The description of the embed. </param>
        /// <returns>A <see cref="RestUserMessage"/> containing the embed.</returns>
        public async Task<RestUserMessage> SendEmbedAsync(string title, string description)
        {
            var builder = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description);

            return await this.Context.Channel.SendMessageAsync(embed: builder.Build());
        }
    }
}
