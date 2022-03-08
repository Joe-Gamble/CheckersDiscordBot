// <copyright file="Program.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Checkers.Data;
    using Checkers.Data.Context;
    using Checkers.Modules;
    using Checkers.Services;
    using Checkers.Services.Generic;
    using Discord;
    using Discord.Addons.Hosting;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Entry point for Checkers Client.
    /// </summary>
    internal class Program
    {

        private static async Task Main()
        {
            string dir = Directory.GetCurrentDirectory();
            var builder = new HostBuilder()
            .ConfigureAppConfiguration(x =>
            {
                var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", false, true)
                .Build();

                x.AddConfiguration(configuration);
            })
            .ConfigureLogging(x =>
            {
                x.AddConsole();
                x.SetMinimumLevel(LogLevel.Debug);
            })
            .ConfigureDiscordHost((context, config) =>
            {
                config.SocketConfig = new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.All,
                    LogLevel = LogSeverity.Debug,
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 200,
                };
                config.Token = context.Configuration["Token"];
            })
            .UseCommandService((context, config) =>
            {
                config.CaseSensitiveCommands = false;
                config.LogLevel = LogSeverity.Debug;
                config.DefaultRunMode = RunMode.Sync;
            })
            .ConfigureServices((context, services) =>
            {
                services
                .AddSingleton<RankedManager>()
                .AddHostedService<ComponentHandler>()
                .AddHostedService<CommandHandler>()
                .AddSingleton<MatchManager>()
                .AddHttpClient()
                .AddDbContextFactory<CheckersDbContext>(options =>
                    options.UseMySql(
                        connectionString:
                    context.Configuration.GetConnectionString("Default"),
                        new MySqlServerVersion(new Version(8, 0, 28)))).AddSingleton<DataAccessLayer>();
            })
            .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}
