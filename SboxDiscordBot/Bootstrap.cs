using System;

namespace SboxDiscordBot
{
    public class Bootstrap
    {
        private BotInstance botInstance;
        public void Run()
        {
            botInstance = new BotInstance();

            botInstance.MainAsync();
        }
    }
}