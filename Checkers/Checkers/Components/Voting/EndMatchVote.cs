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
        public EndMatchVote(Player created_by_player, ulong id, VoteType type, Match match, MatchOutcome outcome, Player? target = null)
            : base(created_by_player, id, type, match)
        {
            this.MatchOutcome = outcome;


            switch (outcome)
            {
                case MatchOutcome.TeamA:
                    {
                        if (type == VoteType.Forfeit)
                        {
                            this.Proposal = "Team A Win **(Team B Forfeit)**";
                        }
                        else
                        {
                            this.Proposal = "Team A Win";
                        }
                        break;
                    }

                case MatchOutcome.TeamB:
                    {
                        if (type == VoteType.Forfeit)
                        {
                            this.Proposal = "Team B Win **(Team A Forfeit)**";
                        }
                        else
                        {
                            this.Proposal = "Team B Win";
                        }

                        break;
                    }

                case MatchOutcome.Draw:
                    {
                        this.Proposal = "Draw";
                        break;
                    }

                case MatchOutcome.Cancelled:
                    {
                        if (target != null)
                        {
                            this.TargetPlayer = target;
                            this.Proposal = $"Player Disconnect.";
                        }
                        else
                        {
                            this.Proposal = "Cancelled";
                        }

                        break;
                    }
            }
        }

        public MatchOutcome MatchOutcome { get; set; }

        public Player? TargetPlayer { get; set; }
    }
}
