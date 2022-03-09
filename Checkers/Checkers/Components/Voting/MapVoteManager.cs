// <copyright file="MapVoteManager.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Components.Voting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Common;
    using Checkers.Data.Models;
    using Checkers.Services;
    using Checkers.Services.Generic;
    using Discord.WebSocket;

    public class MapVoteManager
    {
        private readonly CheckersTimer timer;
        private MatchManager matchManager;

        public MapVoteManager(MatchManager mm, ulong id, Match match)
        {
            Random random = new Random();
            this.Maps = new List<MapVote>();

            for (int i = 0; i < 3; i++)
            {
                this.Maps.Add(new MapVote(this, i, match, this.GetNewMapEntry(random.Next(CheckersConstants.Maps.Count))));
            }

            // Create an AutoResetEvent to signal the timeout threshold in the
            // timer callback has been reached.

            this.timer = new CheckersTimer(OnVoteFinish, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));

            this.matchManager = mm;
        }

        public List<MapVote> Maps { get; set; }

        public string Title { get; } = "Map Vote";

        public SocketMessage Message { get; set; }

        public bool AddVote(Player player, int id)
        {
            if (this.Maps[id].AddForVote(player))
            {
                return true;
            }

            return false;
        }

        public bool HasPlayer(Player player)
        {
            foreach (MapVote vote in this.Maps)
            {
                if (vote.HasPlayer(player.Id))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task RemoveMapVotesFromMatch(Match match, SocketGuild guild)
        {
            foreach (MapVote mapVote in this.Maps)
            {
                match.ActiveVotes.Remove(mapVote);
            }
            this.timer.Dispose();
        }

        public void EndMapVote(MapVote vote)
        {
            this.timer.Dispose();
            if (this.Message.Channel is SocketGuildChannel guildChannel)
            {
                this.matchManager.SelectMap(guildChannel, vote);
            }
        }

        public TimeSpan GetTimeRemaining()
        {
            return this.timer.DueTime;
        }

        private KeyValuePair<string, MapType> GetNewMapEntry(int key)
        {
            return CheckersConstants.Maps.ElementAt(key);
        }

        private async void OnVoteFinish(object? state)
        {
            await CheckersMessageFactory.ExpireMapVote(this, this.Message);
            this.SelectBestMap();
            this.timer.Dispose();
        }

        private void SelectBestMap()
        {
            if (this.Message.Channel is SocketGuildChannel guildChannel)
            {
                MapVote? bestVote = null;
                int maxVotes = 0;

                List<MapVote> TiedVotes = new List<MapVote>();

                foreach (MapVote vote in this.Maps)
                {
                    if (vote.TotalVotes == maxVotes)
                    {
                        TiedVotes.Add(vote);
                    }
                    else if (vote.TotalVotes > maxVotes)
                    {
                        TiedVotes.Clear();
                        bestVote = vote;
                        maxVotes = vote.TotalVotes;
                        TiedVotes.Add(vote);
                    }
                }

                if (TiedVotes.Count > 1)
                {
                    Random random = new Random();
                    this.matchManager.SelectMap(guildChannel, TiedVotes[random.Next(TiedVotes.Count)]);
                }
                else
                {
                    if (bestVote != null)
                    {
                        this.matchManager.SelectMap(guildChannel, bestVote);
                    }
                }
            }
        }
    }
}
