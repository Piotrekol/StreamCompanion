namespace OsuSongsFolderWatcher.Models
{
    public class Configuration
    {
        public int MaxCacheEntries { get; set; } = 1000;
        public int MinimumGenerationMilliseconds { get; set; } = 250;
    }
}
