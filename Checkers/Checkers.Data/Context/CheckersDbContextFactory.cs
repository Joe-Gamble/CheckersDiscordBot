// <copyright file="CheckersDbContextFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Data.Context
{
    public class CheckersDbContextFactory : IDesignTimeDbContextFactory<CheckersDbContext>
    {
        public CheckersDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", false, true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder()
                .UseMySql(configuration.GetConnectionString("Default"), 
                new MySqlServerVersion(new Version(8, 0, 28)), 
                options => options.EnableRetryOnFailure());

            return new CheckersDbContext(optionsBuilder.Options);
        }
    }
}
