using System;
using System.Linq;
using Disco;

namespace SboxDiscordBot.Commands
{
    public class OrgCommand : Command
    {
        public override string Icon => "📜";
        public override string[] Aliases => new[] {"org"};
        public override string Description => "Get info about an organisation";
        public override string[] Syntax => new[] {"ident"};
        public override int MinArgs => 1;
        public override int MaxArgs => 1;

        public override void Run(CommandArgs commandArgs)
        {
            SboxApi.Instance.GetOrg(commandArgs.Args[0]).Then(org =>
                {
                    var eb = Utils.BuildDefaultEmbed();

                    // bad bad!
                    var linkStr = "";
                    if (!string.IsNullOrEmpty(org.SocialTwitter))
                        linkStr += $"[Twitter]({org.SocialTwitter}) ";
                    if (!string.IsNullOrEmpty(org.SocialWeb))
                        linkStr += $"[Website]({org.SocialWeb}) ";
                    
                    // Set embed properties
                    eb.WithTitle(org.Title);
                    eb.WithUrl($"https://sbox.facepunch.com/dev/{org.Ident}");
                    eb.WithDescription(org.Description);

                    eb.AddField("Available Assets", $"```{string.Join('\n', org.PackageIdents)}```");

                    if (org.Thumb != null)
                        eb.WithThumbnailUrl(org.Thumb.ToString());

                    if (!string.IsNullOrEmpty(linkStr))
                        eb.AddField("Links", linkStr);
                    
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