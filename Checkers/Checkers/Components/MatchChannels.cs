// <copyright file="MatchChannels.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Components
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Discord;
    using Discord.WebSocket;

    /// <summary>
    /// A struct containing all relevant ID's for a match.
    /// </summary>
    public struct MatchChannels
    {
        private MatchChannels(ulong cat, ulong mText, ulong tA_VC, ulong tB_VC)
        {
            this.MatchCategoryID = cat;
            this.MatchText = mText;
            this.BVc = tB_VC;
            this.AVc = tA_VC;
        }

        /// <summary>
        /// Gets the ID of the matches category.
        /// </summary>
        public ulong MatchCategoryID { get; }

        /// <summary>
        /// Gets the ID of the matches text channel.
        /// </summary>
        public ulong MatchText { get; }

        /// <summary>
        /// Gets the ID of Team A's Voice CHannel.
        /// </summary>
        public ulong AVc { get; }

        /// <summary>
        /// Gets the ID of Team B's Voice Channel.
        /// </summary>
        public ulong BVc { get; }

        /// <summary>
        /// Construct a new Match Channel Object.
        /// </summary>
        /// <param name="guild"> The Checkers Guild. </param>
        /// <returns> A match channel object containing the Id's of the newly created channels.</returns>
        public static async Task<MatchChannels> BuildMatchChannel(SocketGuild guild)
        {
            var role = guild.GetRole(942533679027200051);

            var role1 = await guild.CreateRoleAsync(name: "Role1", permissions: GuildPermissions.None, isMentionable: false);
            var role2 = await guild.CreateRoleAsync(name: "Role2", permissions: GuildPermissions.None, isMentionable: false);

            var basePermissions = new GuildPermissions(role.Permissions.RawValue);

            // THIS IS A VERY GOOD IDEA. CREATE CATEGORTY & CHANNELS PER MATCH.
            var category = await guild.CreateCategoryChannelAsync("Test Category");
            await category.AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.DenyAll(category));

            await category.AddPermissionOverwriteAsync(role1, OverwritePermissions.DenyAll(category).Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow, addReactions: PermValue.Allow, speak: PermValue.Allow, connect: PermValue.Allow));
            await category.AddPermissionOverwriteAsync(role2, OverwritePermissions.DenyAll(category).Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow, addReactions: PermValue.Allow, speak: PermValue.Allow, connect: PermValue.Allow));

            var channel = await guild.CreateTextChannelAsync("Test Channel", x => x.CategoryId = category.Id);

            var teamA_VC = await guild.CreateVoiceChannelAsync("Team A Voice", x => x.CategoryId = category.Id);
            await teamA_VC.AddPermissionOverwriteAsync(role2, OverwritePermissions.DenyAll(category));

            var teamB_VC = await guild.CreateVoiceChannelAsync("Team B Voice", x => x.CategoryId = category.Id);
            await teamB_VC.AddPermissionOverwriteAsync(role1, OverwritePermissions.DenyAll(category));

            return new MatchChannels(category.Id, channel.Id, teamA_VC.Id, teamB_VC.Id);
        }
    }
}
