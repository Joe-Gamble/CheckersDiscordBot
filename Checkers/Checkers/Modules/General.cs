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
    using Checkers.Common;
    using Checkers.Services;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using ProfanityFilter;
    using System.Net.Http;
    using Checkers.Data;

    /// <summary>
    /// General module containing basic commands.
    /// </summary>
    public class General : CheckersModuleBase
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="General"/> class.
        /// </summary>
        /// <param name="httpClientFactory"> The <see cref="IHttpClientFactory"/> to be used. </param>
        /// <param name=dataAccessLayer"> The <see cref="DataAccessLayer"/> to be used. </param>
        public General(IHttpClientFactory httpClientFactory, DataAccessLayer dataAccessLayer)
            : base(dataAccessLayer)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [Command("Prefix")]
        public async Task GetPrefix(string prefix = null)
        {
            if (prefix == null)
            {
                var currentPrefix = this.DataAccessLayer.GetPrefix(this.Context.Guild.Id);
                await this.ReplyAsync($"The prefix of this guild is {currentPrefix}.");
                return;
            }

            await DataAccessLayer.SetPrefix(this.Context.Guild.Id, prefix);
            await this.ReplyAsync($"The prefix has been set to {prefix}.");

        }

        /// <summary>
        /// Register function for new Players.
        /// </summary>
        /// <param name="args"> Optional arguments that users can pass while registering. </param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Command("Register")]
        public async Task RegisterPlayer(RegisterArguments args = null)
        {
            // TODO:
            // Check needed to see if an account already exists for that user.
            // Check for if the user has been registered but doesnt have the role?
            if (this.Context.Channel == this.Context.Guild.DefaultChannel)
            {
                string name = this.Context.User.Username;
                if (args != null)
                {
                    name = args.Name;
                }

                // This should be the identifier as it is guarenteed to be unique per discord user.
                ulong id = this.Context.User.Id;

                // This should be retrieved from server database.
                var role = this.Context.Guild.GetRole(942533679027200051);

                var profanity = ProfanityHandler.Instance;

                if (!profanity.Filter().IsProfanity(name))
                {
                    if (role != null)
                    {
                        // TODO: Check if user already has the registered role.
                        if (!((SocketGuildUser)this.Context.User).Roles.Contains(role))
                        {
                            // TODO: Add a new player entry to the database.
                             await ((SocketGuildUser)this.Context.User).AddRoleAsync(role);
                             await this.Context.Message.ReplyAsync($"Account registered! Welcome to Checkers!");
                             return;
                        }

                        await this.ReplyAsync($"Temp Response: User already has that role.");
                        return;
                    }

                    await this.ReplyAsync($"Couldn't find the registration role.");
                    return;
                }

                await this.ReplyAsync($"The chosen name is inappropiate.");
                return;
            }
        }

        /// <summary>
        /// Test event function to see how it works.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Command("Event")]
        public async Task TestEvent()
        {
            // TODO: Auto desc and names for seasons.
            var guildEvent = await this.Context.Guild.CreateEventAsync("Ranked", DateTimeOffset.UtcNow.AddDays(1),
                GuildScheduledEventType.Voice, GuildScheduledEventPrivacyLevel.Private, description: "This is a test desc", endTime: DateTimeOffset.UtcNow.AddDays(8), channelId: 942528248561156136);
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

            var embed = new CheckersEmbedBuilder().WithTitle($"{socketGuildUser.Username}#{socketGuildUser.Discriminator}")
                .AddField("ID", socketGuildUser.Id, true).AddField($"Name: ", socketGuildUser.Username, true)
                .AddField($"Created At:", socketGuildUser.CreatedAt, true).WithImageUrl("https://ibb.co/6RG5YKC").Build();

            await this.ReplyAsync(embed: embed);
        }
    }
}
