// <copyright file="CheckersMessageFactory.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Common
{
    using Checkers.Components;
    using Checkers.Components.Voting;
    using Checkers.Data.Models;
    using Checkers.Services;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class CheckersMessageFactory
    {
        /// <summary>
        /// Make a match summary.
        /// </summary>
        /// <param name="channel"> The channel for this summary. </param>
        /// <param name="match"> The match to detail. </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task MakeMatchSummary(ISocketMessageChannel channel, Match match)
        {
            var teamARankEmote = RatingUtils.GetTierEmoteAt(RatingUtils.GetTierAt(match.TeamA.AverageRating));
            var teamBRankEmote = RatingUtils.GetTierEmoteAt(RatingUtils.GetTierAt(match.TeamB.AverageRating));

            var embed = new CheckersEmbedBuilder().WithTitle("Match Found!").WithTimestamp(DateTime.UtcNow)
                .AddField($"{teamARankEmote} Team A - {match.TeamA.AverageRating}", match.TeamA.GetPlayerNamesAndRanksString(), true)
                .AddField($"{teamBRankEmote} Team B - {match.TeamB.AverageRating}", match.TeamB.GetPlayerNamesAndRanksString(), true)
                .Build();

            await channel.SendMessageAsync(embed: embed);
        }

        /// <summary>
        /// Make a match results summary.
        /// </summary>
        /// <param name="channel"> The channel for this summary. </param>
        /// <param name="match"> The match to detail. </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task MakeMatchResultsSummary(ISocketMessageChannel channel, List<PlayerMatchData> playerData, EndMatchVote vote)
        {
            TimeSpan span = DateTime.UtcNow.Subtract(vote.Match.TimeStarted);

            var duration = string.Format("{0} minutes, {1} seconds", span.Minutes, span.Seconds);

            string teamAPlayers = string.Empty;
            string teamBPlayers = string.Empty;

            if (channel is IGuildChannel guildChannel)
            {
                var guild = guildChannel.Guild;

                teamAPlayers = await vote.Match.TeamA.GetPlayerNamesRanksAndPointDifferences(playerData, guild);
                teamBPlayers = await vote.Match.TeamB.GetPlayerNamesRanksAndPointDifferences(playerData, guild);
            }
            else
            {
                teamAPlayers = await vote.Match.TeamA.GetPlayerNamesRanksAndPointDifferences(playerData);
                teamBPlayers = await vote.Match.TeamB.GetPlayerNamesRanksAndPointDifferences(playerData);
            }

            var embed = new CheckersEmbedBuilder().WithTitle("Match Report").WithTimestamp(DateTime.UtcNow)
                .AddField("Outcome: ", vote.Proposal, false)
                .AddField("Duration: ", duration, false)
                .AddField("Team A", teamAPlayers, true)
                .AddField("Team B", teamBPlayers, true)
                .Build();

            await channel.SendMessageAsync(embed: embed);
        }

        public static async Task MakeMatchMapVote(ISocketMessageChannel channel, MapVoteManager mapManager)
        {
            var voteModule = new CheckersComponentBuilder(VoteType.MapPick, false).Build();

            var embed = new CheckersEmbedBuilder().WithTitle($"{mapManager.Title}:")
            .AddField($"Map 1:  {mapManager.Maps[0].TypeName} - {mapManager.Maps[0].TotalVotes} Votes", mapManager.Maps[0].Title, true)
            .AddField($"Map 2:  {mapManager.Maps[1].TypeName} - {mapManager.Maps[1].TotalVotes} Votes", mapManager.Maps[1].Title, false)
            .AddField($"Map 3:  {mapManager.Maps[2].TypeName} - {mapManager.Maps[2].TotalVotes} Votes", mapManager.Maps[2].Title, false).Build();

            await channel.SendMessageAsync(components: voteModule, embed: embed);
        }

        public static async Task MakeMatchCancelledMessage(ISocketMessageChannel channel, EndMatchVote vote, List<PlayerMatchData>? data = null)
        {
            var embed = new CheckersEmbedBuilder().WithTitle(vote.Title).WithTimestamp(DateTime.UtcNow)
                .AddField("Reason: ", vote.Proposal, false);

            if (data != null)
            {
                embed.AddField("Players:", vote.Match.GetPlayersReport(data), false);
            }

            await channel.SendMessageAsync(embed: embed.Build());
        }

        public static async Task<bool> MakeMatchVote(Player player, SocketCommandContext context, EndMatchVote vote)
        {
            var voteModule = new CheckersComponentBuilder(vote.Type, vote.HasVotes()).Build();
            var embed = new CheckersEmbedBuilder().WithTitle($"{vote.Title}:      {vote.TotalVotes} / {vote.RequiredVotes}").AddField("Created By", vote.CreatedByPlayer, true).AddField("Proposal:", vote.Proposal, true).Build();
            await context.Message.ReplyAsync(components: voteModule, embed: embed);

            if (vote.HasVotes())
            {
                return true;
            }

            return false;
        }

        public static async Task<bool> ModifyMapVote(MapVote vote, SocketMessageComponent component, Player player)
        {
            if (!vote.Manager.HasPlayer(player))
            {
                if (!vote.Manager.AddVote(player, (int)vote.VoteID))
                {
                    var embed = new CheckersEmbedBuilder().WithTitle($"{vote.Manager.Title}:")
                        .AddField($"Map 1:  {vote.Manager.Maps[0].TypeName} - {vote.Manager.Maps[0].TotalVotes} Votes", vote.Manager.Maps[0].Title, true)
                        .AddField($"Map 2:  {vote.Manager.Maps[1].TypeName} - {vote.Manager.Maps[1].TotalVotes} Votes", vote.Manager.Maps[1].Title, false)
                        .AddField($"Map 3:  {vote.Manager.Maps[2].TypeName} - {vote.Manager.Maps[2].TotalVotes} Votes", vote.Manager.Maps[2].Title, false).Build();

                    await component.UpdateAsync(x => x.Embed = embed);
                    return false;
                }
                else
                {
                    var embed = new CheckersEmbedBuilder().WithTitle($"{vote.Manager.Title}:")
                       .AddField($"Map 1:  {vote.Manager.Maps[0].TypeName} - {vote.Manager.Maps[0].TotalVotes} Votes", vote.Manager.Maps[0].Title, true)
                       .AddField($"Map 2:  {vote.Manager.Maps[1].TypeName} - {vote.Manager.Maps[1].TotalVotes} Votes", vote.Manager.Maps[1].Title, false)
                       .AddField($"Map 3:  {vote.Manager.Maps[2].TypeName} - {vote.Manager.Maps[2].TotalVotes} Votes", vote.Manager.Maps[2].Title, false)
                       .WithColor(CheckersConstants.CheckerGreen)
                       .Build();

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embed;
                        x.Components = new CheckersComponentBuilder(VoteType.MapPick, true).Build();
                    });

                    return true;
                }
            }
            else
            {
                await component.DeferAsync();
                return false;
            }
        }

        public static async Task ExpireMapVote(MapVoteManager mapVoteManager, SocketMessage message)
        {
            await message.DeleteAsync();
            await message.Channel.SendMessageAsync("Vote Passed!");
        }

        public static async Task<bool> ModifyVote(Vote vote, SocketMessageComponent component, Player player, bool votefor)
        {
            if (!vote.HasPlayer(player.Id))
            {
                if (votefor)
                {
                    if (!vote.AddForVote(player))
                    {
                        var embed = new CheckersEmbedBuilder().WithTitle($"{vote.Title}:       {vote.TotalVotes} / {vote.RequiredVotes}").AddField("Created By", vote.CreatedByPlayer, true).AddField("Proposal:", vote.Proposal, true).Build();
                        await component.UpdateAsync(x => x.Embed = embed);
                        return false;
                    }
                    else
                    {
                        var embed = new CheckersEmbedBuilder().WithTitle($"{vote.Title}:      {vote.TotalVotes} / {vote.RequiredVotes}").AddField("Created By", vote.CreatedByPlayer, true).AddField("Proposal:", vote.Proposal, true).WithColor(CheckersConstants.CheckerGreen).Build();
                        await component.UpdateAsync(x =>
                        {
                            x.Embed = embed;
                            x.Components = new CheckersComponentBuilder(VoteType.EndMatch, true).Build();
                        });

                        await component.Channel.SendMessageAsync("Vote Passed!");
                        return true;
                    }
                }
                else
                {
                    if (!vote.AddAgainstVote(player))
                    {
                        var embed = new CheckersEmbedBuilder().WithTitle($"{vote.Title}:       {vote.TotalVotes} / {vote.RequiredVotes}").AddField("Created By", vote.CreatedByPlayer, true).AddField("Proposal:", vote.Proposal, true).Build();
                        await component.UpdateAsync(x => x.Embed = embed);
                        return false;
                    }
                    else
                    {
                        var embed = new CheckersEmbedBuilder().WithTitle($"{vote.Title}:      {vote.TotalVotes} / {vote.RequiredVotes}").AddField("Created By", vote.CreatedByPlayer, true).AddField("Proposal:", vote.Proposal, true).WithColor(CheckersConstants.CheckerRed).Build();
                        await component.UpdateAsync(x =>
                        {
                            x.Embed = embed;
                            x.Components = new CheckersComponentBuilder(VoteType.EndMatch, true).Build();
                        });

                        await component.Channel.SendMessageAsync("Vote Failed!");
                        return true;
                    }
                }
            }
            else
            {
                await component.DeferAsync();
                return false;
            }
        }
    }
}
