using System;
using System.Reflection;
using Disco;

namespace SboxDiscordBot.Commands
{
    public class Help : Command
    {
        public override string[] Aliases => new[] {"help", "h"};
        public override string[] Syntax => new string[] { };
        public override string Description => "Get a list of bot commands";
        public override string Icon => "💁";
        public override int MinArgs => 0;
        public override int MaxArgs => 0;

        public override void Run(CommandArgs args)
        {
            var eb = Utils.BuildDefaultEmbed();
            eb.WithTitle("Help");

            foreach (var t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.Namespace == null)
                    continue;

                if (t.Namespace.Equals("SboxDiscordBot.Commands", StringComparison.CurrentCultureIgnoreCase))
                    if (!t.IsAbstract && t.BaseType == typeof(Command))
                    {
                        var cmd = (Command) Activator.CreateInstance(t);

                        var cmdSyntaxStr = "";
                        foreach (var s in cmd.Syntax) cmdSyntaxStr += $" [{s}]";

                        var cmdAliasesStr = "";
                        for (var i = 0; i < cmd.Aliases.Length; ++i)
                        {
                            if (i > 0 && i < cmd.Aliases.Length)
                                cmdAliasesStr += " / ";

                            cmdAliasesStr += cmd.Aliases[i];
                        }

                        eb.AddField("`" + ConfigBucket.prefix + cmdAliasesStr + cmdSyntaxStr + "`", cmd.Description);
                    }
            }

            args.Message.Channel.SendMessageAsync("", false, eb.Build());
        }
    }
}