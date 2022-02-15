// <copyright file="Player.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Player
    {
        private PlayerData data;

        public Player()
        {
            this.data = new PlayerData
            {
                Roles = new Dictionary<RoleType, Role>(),
            };
        }

        /// <summary>
        /// Utility role to find best role of a player.
        /// </summary>
        /// <returns> The players best role. </returns>
        public Role GetBestRole()
        {
            var bestRole = from r in this.data.Roles.Values
                           orderby r.Rating ascending
                           select r;
            return (Role)bestRole;
        }
    }
}
