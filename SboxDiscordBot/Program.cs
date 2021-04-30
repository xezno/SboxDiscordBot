using Disco;

namespace SboxDiscordBot
{
    internal class Program
    {
        private static DiscoApplication discoApplication;

        public static void Main(string[] args)
        {
            discoApplication = new DiscoApplication();
            discoApplication.Run().Wait();
            Logging.Log("Bot Quit");
        }
    }
}