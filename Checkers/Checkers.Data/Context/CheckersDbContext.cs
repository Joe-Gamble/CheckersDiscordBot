// <copyright file="CheckersDbContext.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Data.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Checkers.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class CheckersDbContext : DbContext
    {
        public CheckersDbContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets collection of Guilds registered with the bot.
        /// </summary>
        public DbSet<Guild> Guilds { get; set; }

        /// <summary>
        /// Gets or sets collection of Players registered with the bot.
        /// </summary>
        public DbSet<Player> Players { get; set; }
    }
}
