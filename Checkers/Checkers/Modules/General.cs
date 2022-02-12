using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Modules
{
    public class General : ModuleBase<SocketCommandContext>
    {
        [Command("Ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong");
        }

        [Command("Details")]
        public async Task GetDetails(SocketGuildUser? socketGuildUser = null)
        {
            if(socketGuildUser == null)
            {
                socketGuildUser = Context.User as SocketGuildUser;
            }
            await ReplyAsync($"ID: {socketGuildUser.Id}");
        }
    }
}
