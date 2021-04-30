namespace SboxDiscordBot
{
    public partial class SboxApi
    {
        public static SboxApi Instance { get; private set; } = new SboxApi();
        
        public struct Package
        {
            public string Identifier { get; set; }
            public string Title { get; set; }
        }

        public struct Organization
        {
            public string Identifier { get; set; }
            public string Title { get; set; }
        }
    }
}