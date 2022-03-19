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
    using Discord.Rest;
    using Discord.WebSocket;

    public class MapVoteManager
    {
        private readonly CheckersTimer voteTimer;

        private MatchManager matchManager;
        private Dictionary<string, MapType> mapCollection = new Dictionary<string, MapType>();

        public MapVoteManager(MatchManager mm, ulong id, Match match)
        {
            Random random = new Random();

            this.Maps = new List<MapVote>();
            this.mapCollection = CheckersConstants.Maps;

            for (int i = 0; i < 3; i++)
            {
                var mapkey = random.Next(CheckersConstants.Maps.Count);
                this.Maps.Add(new MapVote(this, i, match, this.GetNewMapEntry(mapkey)));
            }

            this.voteTimer = new CheckersTimer(OnVoteFinish, null, TimeSpan.FromSeconds(CheckersConstants.MapVoteDuration), TimeSpan.FromSeconds(CheckersConstants.MapVoteDuration));

            this.matchManager = mm;
        }

        public List<MapVote> Maps { get; set; }

        public string Title { get; } = "Map Vote";

        public DateTime TimeStarted { get; }

        public RestUserMessage Message { get; set; }

        public void AddVote(Player player, int id)
        {
            this.Maps[id].AddForVote(player);
        }

        public Player? HasPlayer(Player player)
        {
            foreach (MapVote vote in this.Maps)
            {
                if (vote.HasPlayer(player.Id))
                {
                    return player;
                }
            }

            return null;
        }

        public void RemovePlayerVote(Player player)
        {
            foreach (MapVote map in Maps)
            {
                if (map.HasPlayer(player.Id))
                {
                    map.RemoveVote(player);
                }
            }
        }

        public async Task RemoveMapVotesFromMatch(Match match, SocketGuild guild)
        {
            foreach (MapVote mapVote in this.Maps)
            {
                match.ActiveVotes.Remove(mapVote);
            }
            this.voteTimer.Dispose();
        }

        public async void EndMapVote(MapVote vote)
        {
            this.voteTimer.Dispose();

            if (this.Message.Channel is SocketGuildChannel guildChannel)
            {
                await this.matchManager.SelectMap(guildChannel, vote);
                await CheckersMessageFactory.ExpireMapVote(this, vote);
            }
        }

        public TimeSpan GetTimeRemaining()
        {
            return this.voteTimer.DueTime;
        }

        private KeyValuePair<string, MapType> GetNewMapEntry(int key)
        {
            var map = this.mapCollection.ElementAt(key);
            this.mapCollection.Remove(map.Key);

            return map;
        }

        private async void OnVoteFinish(object? state)
        {
            this.voteTimer.Dispose();

            if (this.Message.Channel is SocketGuildChannel guildChannel)
            {
                var map = await this.SelectBestMap();
                await this.matchManager.SelectMap(guildChannel, map);
                await CheckersMessageFactory.ExpireMapVote(this, map);
            }
        }

        private async Task<MapVote> SelectBestMap()
        {
            MapVote bestVote = this.Maps.First();
            int maxVotes = 0;

            List<MapVote> tiedVotes = new List<MapVote>();

            foreach (MapVote vote in this.Maps)
            {
                if (vote.TotalVotes == maxVotes)
                {
                    tiedVotes.Add(vote);
                }
                else if (vote.TotalVotes > maxVotes)
                {
                    tiedVotes.Clear();
                    bestVote = vote;
                    maxVotes = vote.TotalVotes;
                    tiedVotes.Add(vote);
                }
                else
                {
                    if (tiedVotes.Count == 0)
                    {
                        tiedVotes.Add(vote);
                    }
                }
            }

            if (tiedVotes.Count > 1)
            {
                Random random = new Random();
                var map = tiedVotes[random.Next(tiedVotes.Count)];

                return map;
            }
            else
            {
                return bestVote;
            }
        }
    }
}
