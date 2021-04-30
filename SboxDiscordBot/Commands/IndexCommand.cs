using System;
using Disco;

namespace SboxDiscordBot.Commands
{
    public class IndexCommand : Command
    {
        public override string Icon => "📜";
        public override string[] Aliases => new[] {"index", "ind", "list"};
        public override string Description => "Get a list of gamemodes and maps";
        public override string[] Syntax => new string[] { };
        public override int MinArgs => 0;
        public override int MaxArgs => 0;

        public override void Run(CommandArgs commandArgs)
        {
            SboxApi.Instance.GetIndex().Then(index =>
                {
                    var eb = Utils.BuildDefaultEmbed();
                    eb.WithTitle("Index");
                    eb.WithDescription("Available Categories");

                    foreach (var category in index) eb.AddField(category.Title, category.Description);

                    commandArgs.Message.Channel.SendMessageAsync(embed: eb.Build());
                }
            ).Catch(exception =>
            {
                Logging.Log(exception.ToString(), Logging.Severity.High);
                Utils.SendError(commandArgs.Message.Channel, exception.Message);
            });
        }
    }
}