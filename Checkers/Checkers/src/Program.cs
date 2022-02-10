using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using System.Text.Json;

namespace Checkers
{
    class Program
    { 
        public static void Main(string [] args) =>
                new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client;

        public async Task MainAsync()
        {
            Player player = new Player();
    
            string json = JsonSerializer.Serialize(player.data);
            Console.WriteLine(json);

            /*
            var config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.All
            };
            
            client = new DiscordSocketClient(config);


            client.MessageReceived += CommandHandler;
            client.Log += Log;


            var token = File.ReadAllText("token.txt");

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
*/
            await Task.Delay(-1);
            
        }

        private Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        private Task CommandHandler (SocketMessage message)
        {
            Console.WriteLine("test");

            string command = "";
            int lengthOfCommand = -1;

            //Filter the messages
            if(!message.Content.StartsWith('#'))
                return Task.CompletedTask;

            if (message.Author.IsBot)
                return Task.CompletedTask;

            if (message.Content.Contains(' '))
                lengthOfCommand = message.Content.IndexOf(' ');
            else
                lengthOfCommand = message.Content.Length;

            command = message.Content.Substring(1, lengthOfCommand - 1);


            //Commands begin here
            if(command == "Hello")
            {
                message.Channel.SendMessageAsync($@"Hello {message.Author.Mention}");
            }

            return Task.CompletedTask;
        }
    }

}
