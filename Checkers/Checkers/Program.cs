using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Discord;
using Discord.WebSocket;
using Discord.Addons.Hosting;
using System.Text.Json;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Checkers.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Checkers
{
    class Program
    {
        static async Task Main()
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
                    LogLevel = LogSeverity.Debug,
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 200
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
                services.AddHostedService<CommandHandler>();
            })
            .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }

        //        private DiscordSocketClient client;

        //        public async Task MainAsync()
        //        {
        //            Player player = new Player();

        //            string json = JsonSerializer.Serialize(player.data);
        //            Console.WriteLine(json);

        //            /*
        //            var config = new DiscordSocketConfig()
        //            {
        //                GatewayIntents = GatewayIntents.All
        //            };

        //            client = new DiscordSocketClient(config);


        //            client.MessageReceived += CommandHandler;
        //            client.Log += Log;


        //            var token = File.ReadAllText("token.txt");

        //            await client.LoginAsync(TokenType.Bot, token);
        //            await client.StartAsync();
        //*/
        //            await Task.Delay(-1);

        //        }

        //        private Task Log(LogMessage message)
        //        {
        //            Console.WriteLine(message.ToString());
        //            return Task.CompletedTask;
        //        }

        //        private Task CommandHandler (SocketMessage message)
        //        {
        //            Console.WriteLine("test");

        //            string command = "";
        //            int lengthOfCommand = -1;

        //            //Filter the messages
        //            if(!message.Content.StartsWith('#'))
        //                return Task.CompletedTask;

        //            if (message.Author.IsBot)
        //                return Task.CompletedTask;

        //            if (message.Content.Contains(' '))
        //                lengthOfCommand = message.Content.IndexOf(' ');
        //            else
        //                lengthOfCommand = message.Content.Length;

        //            command = message.Content.Substring(1, lengthOfCommand - 1);


        //            //Commands begin here
        //            if(command == "Hello")
        //            {
        //                message.Channel.SendMessageAsync($@"Hello {message.Author.Mention}");
        //            }

        //            return Task.CompletedTask;
        //        }
        //    }
    }

}
