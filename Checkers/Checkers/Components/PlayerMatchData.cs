// <copyright file="PlayerMatchData.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

using Checkers.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Components
{
    public struct PlayerMatchData
    {
        public PlayerMatchData(Player player, bool? won, int pointDisplacement, bool? promoted = null)
        {
            this.Player = player;
            this.Won = won;
            this.PointDisplacement = pointDisplacement;

            if(promoted != null)
            {
                this.Promoted = promoted;
            }
            else
            {
                this.Promoted = false;
            }
        }

        public Player Player { get; }

        public bool? Won { get; }

        public int PointDisplacement { get; }

        public bool? Promoted { get; }
    }
}
