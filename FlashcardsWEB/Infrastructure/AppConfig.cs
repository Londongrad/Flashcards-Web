namespace FlashcardsWEB.Infrastructure
{
    public class AppConfig
    {
        public TinyMCE TinyMCE { get; set; } = new();
        public string? Title { get; set; }
        public Database Database { get; set; } = new();
    }

    public class TinyMCE
    {
        public string? APIKey { get; set; }
    }

    public class Database
    {
        public string? ConnectionString { get; set; }
    }
}