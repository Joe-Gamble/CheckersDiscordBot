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
        public Vote(ulong created_by_player, ulong id, VoteType type, Match match)
        {
            this.match = match;
            this.VoteID = id;
            this.CreatedByPlayer = created_by_player;

            switch (type)
            {
                case VoteType.EndMatch:
                {
                        this.RequiredVotes = 8;
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

        public Match match { get; }

        public ulong CreatedByPlayer { get; set; }

        public int RequiredVotes { get; set; }

        public int TotalVotes { get; set; } = 0;
    }
}
