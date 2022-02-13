// <copyright file="General.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Discord.Commands;
    using Discord.WebSocket;

    /// <summary>
    /// General module containing basic commands.
    /// </summary>
    public class General : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Reply Pong test function.
        /// </summary>
        /// <param name="args"> Optional arguments that users can pass while registering. </param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Command("Register")]
        public async Task RegisterPlayer(RegisterArguments args = null)
        {
            if (args == null)
            {
                await this.ReplyAsync($"Registered User {this.Context.User.Username}.");
            }
            else
            {
                if (args.Name != null)
                {
                    await this.ReplyAsync($"Registered User {args.Name}.");
                }
            }
        }

        /// <summary>
        /// Get the details of a user.
        /// </summary>
        /// <param name="socketGuildUser"> An optinal Guild user to get information from. </param>
        /// <returns><see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [Command("Details")]
        public async Task GetDetails(SocketGuildUser socketGuildUser = null)
        {
            if (socketGuildUser == null)
            {
                socketGuildUser = this.Context.User as SocketGuildUser;
            }

            await this.ReplyAsync($"ID: {socketGuildUser.Id}");
        }
    }
}
