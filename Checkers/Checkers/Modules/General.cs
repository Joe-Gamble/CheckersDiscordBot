// <copyright file="General.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Common;
    using Checkers.Data;
    using Checkers.Services;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using ProfanityFilter;

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
        public async Task RegisterPlayer(string customName = null)
        {
            if (this.Context.IsPrivate)
            {
                await this.ReplyAsync("Players must register through the offical Checkers Discord server.");
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

                if (role == null)
                {
                    await this.ReplyAsync($"Couldn't find the registration role.");
                    return;
                }
                else
                {
                    // We shouldn't have to account for this, but just in case? I have no idea how robust this is.
                    // TODO: Further testing with non-admin users.
                    if (customName != null)
                    {
                        name = customName;
                    }

                    var player = this.DataAccessLayer.HasPlayer(id);
                    var profanity = ProfanityHandler.Instance;

                    bool nameAllowed = !profanity.Filter().ContainsProfanity(name);

                    if (player == null)
                    {
                        if (nameAllowed)
                        {
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

        /// <summary>
        /// Delete a users account.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Command("DeleteAccount")]
        public async Task RemovePlayer()
        {
            if (this.Context.IsPrivate)
            {
                await this.ReplyAsync("Players must register through the offical Checkers Discord server.");
                return;
            }

            SocketGuildUser user = (SocketGuildUser)this.Context.User;

            ulong id = user.Id;

            var player = this.DataAccessLayer.HasPlayer(id);

            if (player != null)
            {
                await this.DataAccessLayer.RemovePlayer(this.Context.User.Id);
                await this.Context.Message.ReplyAsync($"Account deleted. Thanks for playing!");
            }

            // This should be retrieved from server database.
            var role = this.Context.Guild.GetRole(942533679027200051);

            if (role != null)
            {
                await user.RemoveRoleAsync(role);
                return;
            }
        }

        [Command("Group")]
        public async Task UpdatePlayer()
        {
            var role = this.Context.Guild.GetRole(942533679027200051);

            var role1 = await this.Context.Guild.CreateRoleAsync(name: "Role1", permissions: GuildPermissions.None, isMentionable: false);
            var role2 = await this.Context.Guild.CreateRoleAsync(name: "Role2", permissions: GuildPermissions.None, isMentionable: false);

            var basePermissions = new GuildPermissions(role.Permissions.RawValue);

            // THIS IS A VERY GOOD IDEA. CREATE CATEGORTY & CHANNELS PER MATCH.
            var category = await this.Context.Guild.CreateCategoryChannelAsync("Test Category");
            await category.AddPermissionOverwriteAsync(this.Context.Guild.EveryoneRole, OverwritePermissions.DenyAll(category));

            await category.AddPermissionOverwriteAsync(role1, OverwritePermissions.DenyAll(category).Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow, addReactions: PermValue.Allow, speak: PermValue.Allow, connect: PermValue.Allow));
            await category.AddPermissionOverwriteAsync(role2, OverwritePermissions.DenyAll(category).Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow, addReactions: PermValue.Allow, speak: PermValue.Allow, connect: PermValue.Allow));

            var channel = await this.Context.Guild.CreateTextChannelAsync("Test Channel", x => x.CategoryId = category.Id);

            var teamA_VC = await this.Context.Guild.CreateVoiceChannelAsync("Team A Voice", x => x.CategoryId = category.Id);
            await teamA_VC.AddPermissionOverwriteAsync(role2, OverwritePermissions.DenyAll(category));

            var teamB_VC = await this.Context.Guild.CreateVoiceChannelAsync("Team B Voice", x => x.CategoryId = category.Id);
            await teamB_VC.AddPermissionOverwriteAsync(role1, OverwritePermissions.DenyAll(category));
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
               .AddField($"Created At:", socketGuildUser.CreatedAt, true).Build();

                await this.ReplyAsync(embed: embed);
            }
        }

        //TEST FUNCTION CAN I UPDATE EMBEDS LIKE THIS?
        public async Task EditEmbed(ulong id)
        {
            var message = await this.Context.Channel.GetMessageAsync(id) as IUserMessage;



            var embed = new EmbedBuilder()
                .WithDescription("test edit")
                .Build();

            if (message != null)
            {
                await message.ModifyAsync(x => x.Embed = embed);
            }
        }
    }
}
