using Checkers.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Components.Voting
{
    public class EndMatchVote : Vote
    {
        public EndMatchVote(Player created_by_player, ulong id, VoteType type, Match match, MatchOutcome outcome)
            : base(created_by_player, id, type, match)
        {
            this.MatchOutcome = outcome;

            switch (outcome)
            {
                case MatchOutcome.TeamA:
                    {
                        this.Proposal = "Team A Win";
                        break;
                    }

                case MatchOutcome.TeamB:
                    {
                        this.Proposal = "Team B Win";
                        break;
                    }

                case MatchOutcome.Draw:
                    {
                        this.Proposal = "Draw";
                        break;
                    }
            }
        }

        public MatchOutcome MatchOutcome { get; set; }
    }
}
