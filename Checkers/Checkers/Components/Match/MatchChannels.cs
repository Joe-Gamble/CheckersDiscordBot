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
    using Checkers.Data.Models;
    using Checkers.Services;
    using Discord;
    using Discord.WebSocket;

    /// <summary>
    /// A struct containing all relevant ID's for a match.
    /// </summary>
    public struct MatchChannels
    {
        private MatchChannels(ulong cat, ulong mText, ulong tA_VC, ulong tB_VC, ulong tA_TC, ulong tB_TC, ulong a_Role, ulong b_Role)
        {
            this.MatchCategoryID = cat;
            this.MatchText = mText;
            this.BVc = tB_VC;
            this.AVc = tA_VC;
            this.ATc = tA_TC;
            this.BTc = tB_TC;
            this.ARole = a_Role;
            this.BRole = b_Role;
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
        /// Gets the ID of Team A's Text CHannel.
        /// </summary>
        public ulong ATc { get; }

        /// <summary>
        /// Gets the ID of Team B's Text Channel.
        /// </summary>
        public ulong BTc { get; }

        /// <summary>
        /// Gets the ID of Team A's Role.
        /// </summary>
        public ulong ARole { get; }

        /// <summary>
        /// Gets the ID of Team B's Role.
        /// </summary>
        public ulong BRole { get; }

        /// <summary>
        /// Construct a new Match Channel Object.
        /// </summary>
        /// <param name="guild"> The Checkers Guild. </param>
        ///  /// <param name="match"> The Checkers Match. </param>
        /// <returns> A match channel object containing the Id's of the newly created channels.</returns>
        public static async Task<MatchChannels> BuildMatchChannel(SocketGuild guild, Match match)
        {
            var role = guild.GetRole(942533679027200051);

            var role1 = await guild.CreateRoleAsync(name: "Role1", permissions: GuildPermissions.None, isMentionable: false);
            var role2 = await guild.CreateRoleAsync(name: "Role2", permissions: GuildPermissions.None, isMentionable: false);

            var basePermissions = new GuildPermissions(role.Permissions.RawValue);

            var category = await guild.CreateCategoryChannelAsync("Test Category");
            await category.AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.DenyAll(category));

            await category.AddPermissionOverwriteAsync(role1, OverwritePermissions.DenyAll(category).Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow, addReactions: PermValue.Allow, speak: PermValue.Allow, connect: PermValue.Allow, useVoiceActivation: PermValue.Allow, readMessageHistory: PermValue.Allow));
            await category.AddPermissionOverwriteAsync(role2, OverwritePermissions.DenyAll(category).Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow, addReactions: PermValue.Allow, speak: PermValue.Allow, connect: PermValue.Allow, useVoiceActivation: PermValue.Allow, readMessageHistory: PermValue.Allow));


            var matchChannel = await guild.CreateTextChannelAsync("Match-Chat", x => x.CategoryId = category.Id);

            // Team A CHannels
            var teamA_TC = await guild.CreateTextChannelAsync("Team Chat", x => x.CategoryId = category.Id);
            await teamA_TC.AddPermissionOverwriteAsync(role2, OverwritePermissions.DenyAll(category));

            var teamA_VC = await guild.CreateVoiceChannelAsync("Team A Voice", x => x.CategoryId = category.Id);
            await teamA_VC.AddPermissionOverwriteAsync(role2, OverwritePermissions.DenyAll(category));

            // Team B CHannels
            var teamB_TC = await guild.CreateTextChannelAsync("Team Chat", x => x.CategoryId = category.Id);
            await teamB_TC.AddPermissionOverwriteAsync(role1, OverwritePermissions.DenyAll(category));

            var teamB_VC = await guild.CreateVoiceChannelAsync("Team B Voice", x => x.CategoryId = category.Id);
            await teamB_VC.AddPermissionOverwriteAsync(role1, OverwritePermissions.DenyAll(category));

            match.TeamA.VoiceID = teamA_VC.Id;
            match.TeamA.RoleID = role1.Id;
            match.TeamA.TextID = teamA_TC.Id;

            foreach (Player player in match.TeamA.Players)
            {
                var user = guild.GetUser(player.Id);

                if (user != null)
                {
                    await user.AddRoleAsync(role1);

                    // Is the user currently connected to a voice channel in the server?
                    var voice = user.VoiceChannel;
                    if (voice != null)
                    {
                        // If so, move them tback to the lobby.
                        var queueVoice = guild.GetVoiceChannel(CheckersConstants.QueueVoice);
                        if (queueVoice != null)
                        {
                            if (queueVoice == voice)
                            {
                                await user.ModifyAsync(x => x.Channel = teamA_VC);
                            }
                        }
                    }
                }
            }

            match.TeamB.VoiceID = teamB_VC.Id;
            match.TeamB.RoleID = role2.Id;
            match.TeamB.TextID = teamB_TC.Id;

            foreach (Player player in match.TeamB.Players)
            {
                var user = guild.GetUser(player.Id);

                if (user != null)
                {
                    await user.AddRoleAsync(role2);

                    // Is the user currently connected to a voice channel in the server?
                    var voice = user.VoiceChannel;
                    if (voice != null)
                    {
                        // If so, move them tback to the lobby.
                        var queueVoice = guild.GetVoiceChannel(CheckersConstants.QueueVoice);
                        if (queueVoice != null)
                        {
                            if (queueVoice == voice)
                            {
                                await user.ModifyAsync(x => x.Channel = teamB_VC);
                            }
                        }
                    }
                }
            }

            return new MatchChannels(category.Id, matchChannel.Id, teamA_VC.Id, teamB_VC.Id, teamA_TC.Id, teamB_TC.Id, role1.Id, role2.Id);
        }

        public async Task ChangeTextPerms(SocketGuild guild, ulong channelID, bool enabled)
        {
            PermValue permValue = PermValue.Allow;

            if (!enabled)
            {
                permValue = PermValue.Deny;
            }

            if (channelID == this.ATc)
            {
                var teamAChannel = guild.GetChannel(channelID);
                if (teamAChannel != null)
                {
                    var role = guild.GetRole(this.ARole);
                    if (role != null)
                    {
                        await teamAChannel.AddPermissionOverwriteAsync(role, OverwritePermissions.DenyAll(teamAChannel).Modify(viewChannel: PermValue.Allow, sendMessages: permValue, addReactions: PermValue.Allow, speak: PermValue.Allow, connect: PermValue.Allow, useVoiceActivation: PermValue.Allow, readMessageHistory: PermValue.Allow));
                    }
                }
            }
            else if (channelID == this.BTc)
            {
                var teamBChannel = guild.GetChannel(channelID);
                if (teamBChannel != null)
                {
                    var role = guild.GetRole(this.BRole);
                    if (role != null)
                    {
                        await teamBChannel.AddPermissionOverwriteAsync(role, OverwritePermissions.DenyAll(teamBChannel).Modify(viewChannel: PermValue.Allow, sendMessages: permValue, addReactions: PermValue.Allow, speak: PermValue.Allow, connect: PermValue.Allow, useVoiceActivation: PermValue.Allow, readMessageHistory: PermValue.Allow));
                    }
                }
            }
            else if (channelID == this.MatchText)
            {
                var teamMatchChannel = guild.GetChannel(this.MatchText);
                if (teamMatchChannel != null)
                {
                    var roleA = guild.GetRole(this.ARole);
                    var roleB = guild.GetRole(this.BRole);

                    if (roleA != null && roleB != null)
                    {
                        await teamMatchChannel.AddPermissionOverwriteAsync(roleA, OverwritePermissions.DenyAll(teamMatchChannel).Modify(viewChannel: PermValue.Allow, sendMessages: permValue, addReactions: PermValue.Allow, speak: PermValue.Allow, connect: PermValue.Allow, useVoiceActivation: PermValue.Allow, readMessageHistory: PermValue.Allow));
                        await teamMatchChannel.AddPermissionOverwriteAsync(roleB, OverwritePermissions.DenyAll(teamMatchChannel).Modify(viewChannel: PermValue.Allow, sendMessages: permValue, addReactions: PermValue.Allow, speak: PermValue.Allow, connect: PermValue.Allow, useVoiceActivation: PermValue.Allow, readMessageHistory: PermValue.Allow));
                    }
                }
            }
        }

        /// <summary>
        /// Deletes Match channels from Guild.
        /// </summary>
        /// <param name="guild"> The Checkers Guild. </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RemoveChannels(SocketGuild guild)
        {
            await guild.GetChannel(this.MatchCategoryID).DeleteAsync();
            await guild.GetChannel(this.MatchText).DeleteAsync();
            await guild.GetChannel(this.AVc).DeleteAsync();
            await guild.GetChannel(this.BVc).DeleteAsync();
            await guild.GetChannel(this.ATc).DeleteAsync();
            await guild.GetChannel(this.BTc).DeleteAsync();
            await guild.GetRole(this.ARole).DeleteAsync();
            await guild.GetRole(this.BRole).DeleteAsync();
        }
    }
}
