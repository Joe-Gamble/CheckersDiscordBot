// <copyright file="CheckersDbContext.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
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

        // These were changed
        public DbSet<Guild> Guilds { get; set; }
    }
}
