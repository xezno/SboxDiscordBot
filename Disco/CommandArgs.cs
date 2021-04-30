using Discord.WebSocket;

namespace Disco
{
    public class CommandArgs
    {
        public CommandArgs(string[] args, SocketMessage message, DiscordShardedClient discordClient)
        {
            this.Args = args;
            this.Message = message;
            this.DiscordClient = discordClient;
        }

        public string[] Args { get; }
        public SocketMessage Message { get; }
        public DiscordShardedClient DiscordClient { get; }
    }
}
