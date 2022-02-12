using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Checkers.Services
{
    public class CommandHandler : DiscordClientService
    {
        private readonly IServiceProvider provider;
        private readonly CommandService service;
        private readonly IConfiguration configuration;

        public CommandHandler(IServiceProvider _provider, DiscordSocketClient client, ILogger<DiscordClientService> logger, 
            CommandService _service, IConfiguration _configuration) 
            : base(client, logger)
        {
           this.provider = _provider;
           this.service = _service;
           this.configuration = _configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.Client.MessageReceived += OnMessageReceived;
            this.service.CommandExecuted += OnCommandExcecuted;
            await this.service.AddModulesAsync(Assembly.GetEntryAssembly(), this.provider);
        }

        private async Task OnCommandExcecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            if(result.IsSuccess)
            {
                return;
            }

            await commandContext.Channel.SendMessageAsync(result.ErrorReason); 
        }

        private async Task OnMessageReceived(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasStringPrefix(this.configuration["Prefix"], ref argPos) &&
                !message.HasMentionPrefix(this.Client.CurrentUser, ref argPos))
                return;

            var context = new SocketCommandContext(this.Client, message);
            await this.service.ExecuteAsync(context, argPos, this.provider);
        }
    }
}
