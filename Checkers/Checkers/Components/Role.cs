// <copyright file="Role.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Role
    {
        public Role()
        {
            this.Type = RoleType.Undefined;
            this.Rating = new Rating();
        }

        public Role(RoleType newType, Rating newRating)
        {
            this.Type = newType;
            this.Rating = newRating;
        }

        public bool IsSelected { get; set; } = false;

        private bool IsPlayersBest {get; set;} = false;

        public string icon = ":joy:";
        public Rating Rating { get; set; }

        public RoleType Type { get; set; }

        public void AddPoints(int points)
        {
            this.Rating.Gain(points);
        }

        public void LosePoints(int points)
        {
            this.Rating.Lose(points);
        }

        
    }
}
