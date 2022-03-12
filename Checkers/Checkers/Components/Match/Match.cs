// <copyright file="Match.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Components.Voting;
    using Checkers.Data.Models;
    using Discord.WebSocket;

    /// <summary>
    /// Checkers Match class for initialising new matches.
    /// </summary>
    public class Match
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Match"/> class.
        /// </summary>
        /// <param name="teamA"> The first team to be injected. </param>
        /// <param name="teamB"> The second team to be injected. </param>
        /// <param name="channels"> The MatchChannels for the match. </param>
        public Match(Team teamA, Team teamB)
        {
            this.TeamA = teamA;
            this.TeamB = teamB;

            this.ActiveVotes = new List<Vote>();

            this.TimeStarted = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets or Sets team A in the match.
        /// </summary>
        public Team TeamA { get; set; }

        /// <summary>
        /// Gets or Sets team B in the match.
        /// </summary>
        public Team TeamB { get; set; }

        /// <summary>
        /// Gets or Sets Channels associated with the match.
        /// </summary>
        public MatchChannels Channels { get; set; }

        /// <summary>
        /// Gets the start time of the Match.
        /// </summary>
        public DateTime TimeStarted { get; }

        /// <summary>
        /// Gets the list of Votes for this match.
        /// </summary>
        public List<Vote> ActiveVotes { get; }

        /// <summary>
        /// Gets the map name of the match.
        /// </summary>
        public string MapName { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether gets the Match has started.
        /// </summary>
        public bool HasStarted { get; set; }

        /// <summary>
        /// Gets thelist of Votes for this match.
        /// </summary>
        public MapType Maptype { get; set; }

        /// <summary>
        /// Return all players.
        /// </summary>
        /// <returns> A list of players. </returns>
        public List<Player> GetPlayers()
        {
            var players = new List<Player>();

            players.AddRange(this.TeamA.Players);
            players.AddRange(this.TeamB.Players);

            return players;
        }

        public void Start()
        {
            this.HasStarted = true;
        }

        /// <summary>
        /// Sets the map of the match.
        /// </summary>
        /// <param name="type"> The map type. </param>
        /// <param name="mapName"> The name of the map. </param>
        public void SetMap(MapType type, string mapName)
        {
            this.MapName = mapName;
            this.Maptype = type;
        }

        public string GetPlayersReport(List<PlayerMatchData> data)
        {
            string report = string.Empty;

            foreach (var playerData in data)
            {
                var player = playerData.Player;
                report += $"{RatingUtils.GetTierEmoteAt((SkillTier)player.CurrentTier)} {player.Username} - {playerData.PointDisplacement}\n";
            }

            return report;
        }

        public async Task<bool> MakeVote(SocketGuild guild, ulong channelID, Vote? vote)
        {
            if (vote != null)
            {
                if (!this.ActiveVotes.Contains(vote))
                {
                    this.ActiveVotes.Add(vote);
                    await this.Channels.ChangeTextPerms(guild, channelID, false);
                    return true;
                }
            }

            return false;
        }

        public async Task RemoveVote(SocketGuild guild, ulong channelID, Vote vote)
        {
            if (this.ActiveVotes.Contains(vote))
            {
                this.ActiveVotes.Remove(vote);
            }
        }

        public async Task<Vote> GetVote(ulong channelID)
        {
            return this.ActiveVotes.First(x => x.VoteID == channelID);
        }

        /// <summary>
        /// Checks if a match contains a player.
        /// </summary>
        /// <param name="player"> The player. </param>
        /// <returns>True if the player exists in the match.</returns>
        public bool HasPlayer(Player player)
        {
            if (this.TeamA.Players.Any(x => x.Id == player.Id) || this.TeamB.Players.Any(x => x.Id == player.Id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get the team of a player.
        /// </summary>
        /// <param name="player"> The player. </param>
        /// <returns> The team of the player. </returns>
        public Team? GetTeamOfPlayer(Player player)
        {
            if (this.TeamA.Players.Any(x => x.Id == player.Id))
            {
                return this.TeamA;
            }
            else if (this.TeamB.Players.Any(x => x.Id == player.Id))
            {
                return this.TeamB;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if the team is team A of this match
        /// </summary>
        /// <param name="team"> The team. </param>
        /// <returns> True if the team is TeamA. </returns>
        public bool IsTeamA(Team team)
        {
            if (team == this.TeamA)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}