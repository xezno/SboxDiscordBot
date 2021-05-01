using System;
using Disco;

namespace SboxDiscordBot.Commands
{
    public class AssetCommand : Command
    {
        public override string Icon => "📜";
        public override string[] Aliases => new[] {"package", "pkg", "asset", "ass"};
        public override string Description => "Get info about a particular asset";
        public override string[] Syntax => new[] {"identifier"};
        public override int MinArgs => 1;
        public override int MaxArgs => 1;

        public override void Run(CommandArgs commandArgs)
        {
            SboxApi.Instance.GetPackage(commandArgs.Args[0]).ContinueWith(task =>
                {
                    if (task.Exception != null)
                        Utils.SendError(commandArgs.Message.Channel, task.Exception.Message);
                    
                    var asset = task.Result;

                    var eb = Utils.BuildDefaultEmbed();
                    var package = asset.Package;

                    var orgTitle = package.Org.Title;
                    if (string.IsNullOrEmpty(orgTitle))
                        orgTitle = "Untitled Org";

                    var friendlyPackageType = package.PackageType.ToString().ToLower();

                    /*
                     * TODO: This is also being used in OrgCommand.cs so we should probably make this into its own
                     * util / helper function somewhere.
                     */
                    // bad bad bad!
                    var linkStr = "";
                    if (!string.IsNullOrEmpty(package.DownloadUrl?.ToString()))
                        linkStr += $"[Download]({package.DownloadUrl}) ";
                    if (!string.IsNullOrEmpty(package.Org.SocialTwitter))
                        linkStr += $"[Twitter]({package.Org.SocialTwitter}) ";
                    if (!string.IsNullOrEmpty(package.Org.SocialWeb))
                        linkStr += $"[Website]({package.Org.SocialWeb}) ";

                    // Set embed properties
                    eb.WithTitle(package.Title);
                    eb.WithUrl($"https://sbox.facepunch.com/dev/{package.Org.Ident}/{package.Ident}");
                    eb.WithDescription($"A {friendlyPackageType} by {orgTitle}");
                    eb.WithThumbnailUrl(package.Thumb?.ToString());

                    if (!string.IsNullOrEmpty(package.Description))
                        eb.AddField("Description", package.Description);

                    if (!string.IsNullOrEmpty(linkStr))
                        eb.AddField("Links", linkStr);
                    commandArgs.Message.Channel.SendMessageAsync(embed: eb.Build());
                }
            );
        }
    }
}