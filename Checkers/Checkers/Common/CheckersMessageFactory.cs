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

            var embed = new CheckersEmbedBuilder().WithTitle("Match Report").WithTimestamp(DateTime.UtcNow)
                .AddField("Outcome: ", vote.Proposal, false)
                .AddField("Duration: ", duration, false)
                .AddField("Team A", vote.Match.TeamA.GetPlayerNamesRanksAndPointDifferences(playerData), true)
                .AddField("Team B", vote.Match.TeamB.GetPlayerNamesRanksAndPointDifferences(playerData), true)
                .Build();

            await channel.SendMessageAsync(embed: embed);
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
