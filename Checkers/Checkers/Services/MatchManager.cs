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
        /// </summary>
        public MatchManager(MatchMaker maker)
        {
            this.Queue = new CheckersQueue();

            maker.MatchFound += this.OnMatchFound;
        }

        /// <summary>
        /// Gets the Checkers Queue.
        /// </summary>
        public CheckersQueue Queue { get; }

        /// <summary>
        /// Make a Checkers match.
        /// </summary>
        /// <param name="channels"> The <see cref="MatchChannels"/> struct. </param>
        /// <returns> A new Match. </returns>
        public Match MakeMatch(MatchChannels channels)
        {
            this.MakeTeams(channels.AVc, channels.BVc, out Team teamA, out Team teamB);

            Match match = new Match(teamA, teamB, channels.MatchCategoryID, channels.MatchText);

            return match;
        }

        private async Task<Match> OnMatchFound(SocketCommandContext context)
        {
            MatchChannels channels = await MatchChannels.BuildMatchChannel(context.Guild);
            return this.MakeMatch(channels);
        }

        private void MakeTeams(ulong aVC, ulong bVC, out Team teamA, out Team teamB)
        {
            var players = this.Queue.Pop();

            List<Player> playersA = new List<Player>();
            List<Player> playersB = new List<Player>();

            for (int i = 0; i < players.Count; i++)
            {
                if (i % 2 == 0)
                {
                    playersA.Add(players[i]);
                }
                else
                {
                    playersB.Add(players[i]);
                }
            }

            teamA = new Team(playersA);
            teamB = new Team(playersB);
        }
    }
}
