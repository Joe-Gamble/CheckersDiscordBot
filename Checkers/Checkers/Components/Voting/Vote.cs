using Checkers.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Components.Voting
{
    public abstract class Vote
    {
        public Vote(Player created_by_player, ulong id, VoteType type, Match match)
        {
            this.Match = match;
            this.VoteID = id;
            this.CreatedByPlayer = created_by_player.Username;
            this.VoterIDs = new List<ulong>();
            this.MaxVotes = match.GetPlayers().Count;
            this.Title = string.Empty;
            this.Proposal = string.Empty;

            switch (type)
            {
                case VoteType.EndMatch:
                    {
                        this.RequiredVotes = (int)Math.Ceiling(this.MaxVotes * 0.66);
                        this.AddForVote(created_by_player);
                        this.Title = "Match Vote";
                        break;
                    }

                case VoteType.Forfeit:
                    {
                        this.RequiredVotes = (int)Math.Ceiling(this.MaxVotes * 0.66);
                        this.AddForVote(created_by_player);
                        this.Title = "Forfeit";
                        break;
                    }

                default:
                    {
                        this.RequiredVotes = 0;
                        this.Title = string.Empty;
                        break;
                    }
            }
        }

        public ulong VoteID { get; }

        public Match Match { get; }

        public string CreatedByPlayer { get; set; }

        public string Title { get; set; }

        public string Proposal { get; set; }

        public int RequiredVotes { get; set; }

        public int TotalVotes { get; set; } = 0;

        public int MaxVotes { get; set; }

        public List<ulong> VoterIDs { get; }

        public bool AddForVote(Player player)
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

        public bool AddAgainstVote(Player player)
        {
            if (this.Match.HasPlayer(player))
            {
                if (!this.VoterIDs.Contains(player.Id))
                {
                    this.VoterIDs.Add(player.Id);

                    // If needed votes > votes left
                    if ((this.RequiredVotes - this.TotalVotes) >= this.MaxVotes - this.VoterIDs.Count)
                    {
                        // Vote cannot pass.
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HasPlayer(ulong id)
        {
            return this.VoterIDs.Contains(id);
        }
    }
}
