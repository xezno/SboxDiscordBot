using System;
using Disco;

namespace SboxDiscordBot
{
    class Program
    {
        public static DiscoApplication discoApplication;
        public static void Main(string[] args)
        {
            discoApplication = new DiscoApplication();
            discoApplication.Run().Wait();
            Console.WriteLine("Bot quit");
        }
    }
}