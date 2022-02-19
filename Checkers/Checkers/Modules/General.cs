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

        /// <summary>
        /// Gets or Sets the prefix of the current Guild or the DM channel.
        /// </summary>
        /// <param name="prefix"> If null retrieves the current prefix. If not, attempts to assign it to the database. </param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Command("Prefix")]
        public async Task GetPrefix(string prefix = null)
        {
            if (this.Context.IsPrivate)
            {
                await this.ReplyAsync("All DM Commands use ! as their prefix.");
                return;
            }

            if (prefix == null)
            {
                var currentPrefix = this.DataAccessLayer.GetPrefix(this.Context.Guild.Id);
                await this.ReplyAsync($"The prefix of this guild is {currentPrefix}.");
                return;
            }

            await this.DataAccessLayer.SetPrefix(this.Context.Guild.Id, prefix);
            await this.ReplyAsync($"The prefix has been set to {prefix}.");

        }

        /// <summary>
        /// Register function for new Players.
        /// TODO: Move Register function into another sub instead of General?
        /// </summary>
        /// <param name="args"> Optional arguments that users can pass while registering. </param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Command("Register")]
        public async Task RegisterPlayer(RegisterArguments args = null)
        {
            if (this.Context.IsPrivate)
            {
                await this.ReplyAsync("Must be a member of the Checkers Discord Server to Register an account.");
                return;
            }

            // TODO:
            // Check needed to see if an account already exists for that user. DONE.
            // Check for if the user has been registered but doesnt have the role? DONE.
            if (this.Context.Channel == this.Context.Guild.DefaultChannel)
            {
                SocketGuildUser user = (SocketGuildUser)this.Context.User;

                ulong id = user.Id;
                string name = user.Username;

                // This should be retrieved from server database.
                var role = this.Context.Guild.GetRole(942533679027200051);

                if (args != null)
                {
                    if (args.Name != null && args.Name != "name:")
                    {
                        name = args.Name;
                    }
                }

                if (role == null)
                {
                    await this.ReplyAsync($"Couldn't find the registration role.");
                    return;
                }
                else
                {
                    // We shouldn't have to account for this, but just in case? I have no idea how robust this is.
                    // TODO: Further testing with non-admin users.
                    var player = this.DataAccessLayer.HasPlayer(id);
                    var profanity = ProfanityHandler.Instance;

                    bool nameAllowed = !profanity.Filter().ContainsProfanity(name);

                    if (player != null)
                    {
                        player.Registered = true;

                        if (nameAllowed)
                        {
                            if (player.Username != name)
                            {
                                if (args != null && args.Name != "name:")
                                {
                                    // This seemns like an awful way to do it. Problem is we need feedback instantly with new name. How?
                                    await this.DataAccessLayer.UpdatePlayerName(player, name);
                                    await user.SendMessageAsync($"Account already exists. Your name has been updated to {player.Username}!");

                                    // Database login function.
                                }
                            }

                            await this.Context.Message.ReplyAsync($"Welcome back, {player.Username}!");
                        }
                        else
                        {
                            await this.ReplyAsync($"The chosen name is inappropiate.");
                            return;
                        }
                    }
                    else
                    {
                        if (nameAllowed)
                        {
                            // TODO: Check if user already has the registered role.

                            // TODO: Add a new player entry to the database.
                            await this.DataAccessLayer.RegisterPlayer(name, id);
                            await this.Context.Message.ReplyAsync($"Account registered! Welcome to Checkers!");
                        }
                        else
                        {
                            await this.ReplyAsync($"The chosen name is inappropiate.");
                        }
                    }

                    await user.AddRoleAsync(role);
                }
            }
        }

        [Command("Update")]
        public async Task UpdatePlayer()
        {
            if (this.Context.IsPrivate)
            {
                // This is a dm.
                await this.ReplyAsync("Test");
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
        public async Task GetDetails(SocketGuildUser? socketGuildUser = null)
        {
            if (socketGuildUser == null)
            {
                socketGuildUser = this.Context.User as SocketGuildUser;
            }

            if (socketGuildUser != null)
            {
                var embed = new CheckersEmbedBuilder().WithTitle($"{socketGuildUser.Username}#{socketGuildUser.Discriminator}")
               .AddField("ID", socketGuildUser.Id, true).AddField($"Name: ", socketGuildUser.Username, true)
               .AddField($"Created At:", socketGuildUser.CreatedAt, true).WithImageUrl("https://ibb.co/6RG5YKC").Build();

                await this.ReplyAsync(embed: embed);
            }
        }
    }
}
