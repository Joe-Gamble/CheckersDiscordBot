// <copyright file="PlayerData.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Player data container.
    /// </summary>
    public struct PlayerData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerData"/> struct.
        /// </summary>
        public PlayerData()
        {
            this.Roles = new Dictionary<RoleType, Role>();
        }

        public int Id { get; set; } = -1;

        public bool IsQueued { get; set; } = false;

        // This should be three seperate
        public Dictionary<RoleType, Role> Roles { get; set; }

        
    }
}
