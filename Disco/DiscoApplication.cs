using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using System.Threading;

namespace Disco
{
    public class DiscoApplication
    {
        private static List<Command> commands = new List<Command>();
        private bool isReady;

        public DiscordShardedClient Client { get; private set; }

        public async Task Run()
        {
            Console.WriteLine("Starting bot...");
            LoadCommands();
            LoadComponents();

            Client = new DiscordShardedClient();
            Client.Log += Log;
            Client.MessageReceived += MessageReceived;
            Client.ShardReady += Ready;
            Client.LoggedIn += LoggedIn;
            Client.ReactionAdded += ReactionAdded;

            await Client.LoginAsync(TokenType.Bot, ConfigBucket.botToken);
            await Client.StartAsync();
            Console.WriteLine("Finished initialization.");
            await Task.Delay(-1);
        }

        private void LoadComponents()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(DataStructureBase).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && t != typeof(DataStructure<,>))
                {
                    Utils.Log($"Loading component {t.Name}");
                    if (t.BaseType != null)
                    {
                        ((DataStructureBase)(t.BaseType // Class -> DataStructureBase -> Singleton<Class>
                            .GetMethod("Get")
                            ?.Invoke(null, null)))
                        ?.Load();
                    }
                    else
                    {
                        Utils.Log($"Couldn't load {t.Name} as base type was null");
                    }
                }
            }
        }

        private void LoadCommands()
        {
            Dictionary<string, List<Command>> aliases = new Dictionary<string, List<Command>>();
            foreach (var t in Assembly.GetEntryAssembly().GetTypes().Where(t => !t.IsAbstract && t.BaseType == typeof(Command)))
            {
                var instance = (Command)Activator.CreateInstance(t);
                commands.Add(instance);

                foreach (var alias in instance.Aliases)
                {
                    if (aliases.ContainsKey(alias))
                    {
                        Utils.Log($"Alias collision detected for '{alias}':");
                        aliases[alias].Add(instance);
                        foreach (var command in aliases[alias])
                        {
                            Utils.Log($"\t{command.GetType().Name}");
                        }
                    }
                    else
                    {
                        aliases.Add(alias, new List<Command>() { instance });
                    }
                }
            }

            Utils.Log($"{commands.Count} commands loaded");
        }

        private Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            ReactionHandler.Instance.TriggerReaction(reaction.User.Value, message.Id, reaction.Emote, this);
            return Task.CompletedTask;
        }

        public Task Ready(DiscordSocketClient socketClient)
        {
            Utils.Log("Ready");
            isReady = true;

            foreach (var guild in socketClient.Guilds)
            {
                Console.WriteLine(guild.Name);
                foreach (var channel in guild.Channels)
                {
                    Console.WriteLine($"\t{channel.Name}");
                }
            }
            return Task.CompletedTask;
        }
        public Task LoggedIn()
        {
            Utils.Log("Logged In!");
            return Task.CompletedTask;
        }

        public async Task MessageReceived(SocketMessage message)
        {
            if (!isReady)
                return;

            if (message.Author.IsBot)
                return;

            Console.WriteLine($"{message.Author.Username} in {message.Channel.Name}: {message.Content}");
            try
            {
                if (message.Author.Id == Client.CurrentUser.Id)
                    return;

                var splitContent = message.Content.Split(' ');
                bool commandRun = false;

                // Check if it's actually a command
                if (!splitContent[0].StartsWith(ConfigBucket.prefix, StringComparison.CurrentCultureIgnoreCase))
                    return;

                string userCommand = splitContent[0].Remove(0, ConfigBucket.prefix.Length);
                foreach (Command c in commands)
                {
                    commandRun = await RunCommand(message, userCommand, c);
                    if (commandRun)
                        break;
                }

                if (!commandRun)
                {
                    string closestCommand = null;
                    int closestCommandRating = 3;
                    foreach (Command c in commands)
                    {
                        foreach (string s in c.Aliases)
                        {
                            var levDist = Utils.LevenshteinDistance(s.ToLower(), userCommand.ToLower());
                            if (levDist <= 2 && levDist < closestCommandRating)
                            {
                                closestCommand = s;
                                closestCommandRating = levDist;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(closestCommand))
                        Utils.SendError(message.Channel, $"Command not found.");
                    else
                        Utils.SendError(message.Channel, $"Command not found; did you mean `{ConfigBucket.prefix}{closestCommand}`?");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public Task Log(LogMessage msg)
        {
            return Task.CompletedTask;
        }

        private async Task<bool> RunCommand(SocketMessage message, string userCommand, Command c)
        {
            bool commandRun = false;
            foreach (string s in c.Aliases)
            {
                if (s.Equals(userCommand, StringComparison.CurrentCultureIgnoreCase))
                {
                    string[] args;
                    if (message.Content.Contains(' '))
                        args = message.Content.Remove(0, message.Content.IndexOf(' ') + 1).Split(' ');
                    else
                        args = new string[] { };

                    if (args.Length < c.MinArgs)
                    {
                        Utils.SendError(message.Channel, "Not enough arguments.");
                    }
                    else if (args.Length > c.MaxArgs)
                    {
                        Utils.SendError(message.Channel, "Too many arguments.");
                    }
                    else
                    {
                        var thread = new Thread(() => c.Run(new CommandArgs(args, message, Client)));
                        thread.Start();
                    }

                    commandRun = true;
                    break;
                }
            }
            return commandRun;
        }
    }
}
