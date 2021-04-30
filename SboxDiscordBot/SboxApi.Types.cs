using System;
using Newtonsoft.Json;

namespace SboxDiscordBot
{
    public partial class SboxApi
    {
        public enum PackageType
        {
            Map = 1,
            Gamemode = 2
        }

        public class FindResult
        {
            [JsonProperty("type")] public long Type { get; set; }

            [JsonProperty("assets")] public Package[] Assets { get; set; }
        }

        public class Category
        {
            [JsonProperty("title")] public string Title { get; set; }

            [JsonProperty("description")] public string Description { get; set; }

            [JsonProperty("packages")] public Package[] Packages { get; set; }
        }

        public class Asset
        {
            [JsonProperty("asset")] public Package Package { get; set; }
        }

        public class Package
        {
            [JsonProperty("org")] public Org Org { get; set; }

            [JsonProperty("ident")] public string Ident { get; set; }

            [JsonProperty("title")] public string Title { get; set; }

            [JsonProperty("summary")] public string Summary { get; set; }

            [JsonProperty("thumb")] public Uri Thumb { get; set; }

            [JsonProperty("packageType")] public PackageType PackageType { get; set; }

            [JsonProperty("updated")] public long Updated { get; set; }

            [JsonProperty("description")] public string Description { get; set; }

            [JsonProperty("background")] public Uri Background { get; set; }

            [JsonProperty("downloadUrl")] public Uri DownloadUrl { get; set; }
        }

        public class Org
        {
            [JsonProperty("ident")] public string Ident { get; set; }

            [JsonProperty("title")] public string Title { get; set; }

            [JsonProperty("description")] public string Description { get; set; }

            [JsonProperty("thumb")] public Uri Thumb { get; set; }

            [JsonProperty("socialTwitter")] public string SocialTwitter { get; set; }

            [JsonProperty("socialWeb")] public string SocialWeb { get; set; }
            
            public string[] PackageIdents { get; set; } // Runtime
        }
    }
}