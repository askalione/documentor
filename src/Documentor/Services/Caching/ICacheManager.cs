namespace Documentor.Services.Caching
{
    public interface ICacheManager
    {
        DirectoryInfo GetCacheDirectory();
        Task SaveToCacheAsync(string cachename, string cache);
        Task<string?> LoadFromCacheAsync(string cachename);
        void ClearCache();
        void ClearCache(string pattern);
    }
}
