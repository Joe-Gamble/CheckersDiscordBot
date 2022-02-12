using Checkers.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Data.Context
{
    public class CheckersDbContext : DbContext
    {
        public CheckersDbContext(DbContextOptions options)
            : base(options)
        {}

        public DbSet<Guild> Guilds { get; set; }
        public DbSet<Player> Players { get; set; }
    }
}
