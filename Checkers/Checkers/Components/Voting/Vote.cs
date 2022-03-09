// <copyright file="Vote.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

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
        public Vote(ulong id, VoteType type, Match match, Player? created_by_player = null)
        {
            this.Match = match;
            this.VoteID = id;
            this.VoterIDs = new List<ulong>();
            this.Title = string.Empty;
            this.Proposal = string.Empty;
            this.Type = type;

            if(created_by_player != null)
            {
                this.CreatedByPlayer = created_by_player.Username;
            }

            switch (type)
            {
                case VoteType.EndMatch:
                    {
                        this.MaxVotes = match.GetPlayers().Count;
                        this.RequiredVotes = (int)Math.Ceiling(this.MaxVotes * 0.66);
                        this.Title = "Match Vote";
                        break;
                    }

                case VoteType.Forfeit:
                    {
                        this.MaxVotes = match.GetPlayers().Count / 2;
                        this.RequiredVotes = (int)Math.Ceiling(this.MaxVotes * 0.5);
                        this.Title = "Match Forfeit";
                        break;
                    }

                case VoteType.Disconnect:
                    {
                        this.MaxVotes = match.GetPlayers().Count;
                        this.RequiredVotes = (int)Math.Ceiling(this.MaxVotes * 0.66);
                        this.Title = "Match Cancelled";
                        break;
                    }

                case VoteType.MapPick:
                    {
                        this.MaxVotes = match.GetPlayers().Count;
                        this.RequiredVotes = (int)(Math.Ceiling((decimal)this.MaxVotes / 3) + 1);
                        break;
                    }

                default:
                    {
                        this.RequiredVotes = 0;
                        this.Title = string.Empty;
                        break;
                    }
            }

            if (created_by_player != null)
            {
                this.AddForVote(created_by_player);
            }
        }

        public ulong VoteID { get; }

        public Match Match { get; }

        public string CreatedByPlayer { get; set; }

        public string Title { get; set; }

        public VoteType Type { get; set; }

        public string Proposal { get; set; }

        public int RequiredVotes { get; set; }

        public int TotalVotes { get; set; } = 0;

        public int MaxVotes { get; set; }

        public List<ulong> VoterIDs { get; }

        public virtual bool AddForVote(Player player)
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

        public virtual bool AddAgainstVote(Player player)
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

        public bool HasVotes()
        {
            return this.TotalVotes >= this.RequiredVotes;
        }

        public bool HasPlayer(ulong id)
        {
            return this.VoterIDs.Contains(id);
        }
    }
}
