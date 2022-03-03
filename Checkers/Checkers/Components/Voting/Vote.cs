using Checkers.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Components.Voting
{
    public struct Vote
    {
        public Vote(Player created_by_player, ulong id, VoteType type, Match match)
        {
            this.Match = match;
            this.VoteID = id;
            this.CreatedByPlayer = created_by_player.Username;
            this.VoterIDs = new List<ulong>();


            switch (type)
            {
                case VoteType.EndMatch:
                {
                        this.RequiredVotes = (int)Math.Ceiling(match.GetPlayers().Count * 0.66);
                        this.AddVote(created_by_player);
                        break;
                }

                default:
                {
                        this.RequiredVotes = 0;
                        break;
                }
            }
        }

        public ulong VoteID { get; }

        public Match Match { get; }

        public string CreatedByPlayer { get; set; }

        public int RequiredVotes { get; set; }

        public int TotalVotes { get; set; } = 0;

        public List<ulong> VoterIDs { get; }

        public bool AddVote(Player player)
        {
            if (this.Match.HasPlayer(player))
            {
                if (!this.VoterIDs.Contains(player.Id))
                {
                    this.TotalVotes++;
                    if (this.TotalVotes < this.RequiredVotes)
                    {
                        this.VoterIDs.Add(player.Id);
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
