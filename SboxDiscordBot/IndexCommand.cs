using Disco;

namespace SboxDiscordBot
{
    public class IndexCommand : Command
    {
        public override string Icon => "📜";
        public override string[] Aliases => new[] { "index", "ind", "list" };
        public override string Description => "Fetch a list of cool stuff.";
        public override string[] Syntax => new string[] { };
        public override int MinArgs => 0;
        public override int MaxArgs => 0;
        public override void Run(CommandArgs commandArgs)
        {
            SboxApi.Instance.GetIndex().Then(index =>
                {
                    var eb = Disco.Utils.BuildDefaultEmbed();
                    eb.WithTitle("Index");
                    eb.WithDescription(index.Categories.ToString());

                    commandArgs.Message.Channel.SendMessageAsync(embed: eb.Build());
                }
            );
        }
    }
}