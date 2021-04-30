using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace SboxDiscordBot
{
    public class BotInstance
    {
        private DiscordSocketClient client;

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            client.Log += message =>
            {
                Console.WriteLine(message);
                return Task.CompletedTask;
            };

            string token = File.ReadAllText("DiscordToken.txt");

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            
            await Task.Delay(-1);
        }
    }
}