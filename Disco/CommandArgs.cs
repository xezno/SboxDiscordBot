using Discord.WebSocket;

namespace Disco
{
    public class CommandArgs
    {
        public CommandArgs(string[] args, SocketMessage message, DiscordShardedClient discordClient)
        {
            Args = args;
            Message = message;
            DiscordClient = discordClient;
        }

        public string[] Args { get; }
        public SocketMessage Message { get; }
        public DiscordShardedClient DiscordClient { get; }
    }
}