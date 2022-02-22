namespace Checkers.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Components;
    using Checkers.Data.Models;
    using Checkers.Modules;
    using Discord.Commands;

    /// <summary>
    /// The Match Manager for Checkers.
    /// </summary>
    public class MatchManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchManager"/> class.
        /// <param name="maker"> The <see cref="MatchMaker"/> class to be injected. </param>
        /// </summary>
        public MatchManager(MatchMaker maker)
        {
            this.Queue = new CheckersQueue();
            this.Matches = new List<Match>();

            maker.MatchFound += this.OnMatchFound;
            maker.MatchEnd += this.OnMatchEnd;
            maker.OnQueue += this.QueuePlayer;
            maker.OnDeQueue += this.UnQueuePlayer;
        }

        /// <summary>
        /// Gets the Checkers Queue.
        /// </summary>
        public CheckersQueue Queue { get; }


        private List<Match> Matches { get; set; }

        /// <summary>
        /// Make a Checkers match.
        /// </summary>
        /// <param name="channels"> The <see cref="MatchChannels"/> struct. </param>
        /// <returns> A new Match. </returns>
        public Match MakeMatch(MatchChannels channels)
        {
            this.MakeTeams(out Team teamA, out Team teamB, channels);

            Match match = new Match(teamA, teamB, channels);
            this.Matches.Add(match);

            return match;
        }

        private Match? GetMatchOfPLayer(Player player)
        {
            foreach (Match match in this.Matches)
            {
                if (match.HasPlayer(player))
                {
                    return match;
                }
            }

            return null;
        }

        private Match? GetMatchFromChannel(ulong channelID)
        {
            foreach (Match match in this.Matches)
            {
                if (match.Channels.MatchText == channelID)
                {
                    return match;
                }
            }

            return null;
        }

        private async Task<Player> QueuePlayer(SocketCommandContext context, Player player)
        {
            Player new_player = new Player(player);

            if (!new_player.IsPlaying && new_player.IsActive)
            {
                if (!player.IsQueued)
                {
                    this.Queue.AddToQueue(new_player);
                    return new_player;
                }
            }

            // Log here that the player couldnt be queued.
            return player;
        }

        private async Task<Player> UnQueuePlayer(SocketCommandContext context, Player player)
        {
            if (!player.IsPlaying && player.IsActive)
            {
                if (player.IsQueued)
                {
                    this.Queue.RemoveFromQueue(player);
                }
            }
            return player;
        }

        private async Task<Match> OnMatchFound(SocketCommandContext context)
        {
            MatchChannels channels = await MatchChannels.BuildMatchChannel(context.Guild);
            return this.MakeMatch(channels);
        }

        private async Task OnMatchEnd(SocketCommandContext context)
        {
            var match = this.GetMatchFromChannel(context.Channel.Id);

            if (match == null)
            {
                return;
            }

            foreach (Player matchPlayer in match.GetPlayers())
            {
                // Make sure every player in the match is still a member of the server. (Rage quit lol)
                var user = context.Guild.GetUser(matchPlayer.Id);

                if (user != null)
                {
                    // Is the user currently connected to a voice channel in the server?
                    var channel = user.VoiceChannel;
                    if (channel != null)
                    {
                        // If so, move them tback to the lobby.
                        var queueVoice = context.Guild.GetChannel(CheckersConstants.QueueVoice);
                        await user.ModifyAsync(x => x.Channel = channel);
                    }

                    var team = match.GetTeamOfPlayer(matchPlayer);

                    // player is on a team
                    if (team != null)
                    {
                        // if team role exists in server
                        var role = context.Guild.GetRole(team.RoleID);

                        if (role != null)
                        {
                            // remove role from user
                            await user.RemoveRoleAsync(role);
                        }
                    }
                }
            }

            // FInally clean-up the match channels from the guild.
            await match.Channels.RemoveChannels(context.Guild);
            this.Matches.Remove(match);
        }

        private void MakeTeams(out Team teamA, out Team teamB, MatchChannels channels)
        {
            var players = this.Queue.Pop();

            List<Player> playersA = new List<Player>();
            List<Player> playersB = new List<Player>();

            for (int i = 0; i < players.Count; i++)
            {
                players[i].IsQueued = false;
                players[i].IsPlaying = true;

                if (i % 2 == 0)
                {
                    playersA.Add(players[i]);
                }
                else
                {
                    playersB.Add(players[i]);
                }
            }

            teamA = new Team(playersA, channels.AVc, channels.ARole);
            teamB = new Team(playersB, channels.BVc, channels.BRole);
        }
    }
}
