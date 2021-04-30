namespace Disco
{
    public class Configuration
    {
        public string Prefix { get; set; }
        public string BotName { get; set; }
        public string BotImage { get; set; }
        public string BotUrl { get; set; }
        public string BotToken { get; set; }
    }

    public class ConfigBucket : DataStructure<Configuration, ConfigBucket>
    {

        public override bool ReadOnly => true;
        public override string Location => "Config.json";

        public static string prefix { get => Instance.Data.Prefix; }
        public static string botName { get => Instance.Data.BotName; }
        public static string botImage { get => Instance.Data.BotImage; }
        public static string botURL { get => Instance.Data.BotUrl; }
        public static string botToken { get => Instance.Data.BotToken; }
    }
}