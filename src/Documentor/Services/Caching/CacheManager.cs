using Documentor.Settings;
using Microsoft.Extensions.Options;

namespace Documentor.Services.Caching
{
    public class CacheManager : ICacheManager
    {
        private readonly CacheSettings _cacheSettings;
        private readonly IHostEnvironment _hostEnvironment;

        public CacheManager(IOptionsSnapshot<IOSettings> settingsOptions,
            IHostEnvironment hostEnvironment)
        {
            Ensure.NotNull(settingsOptions, nameof(settingsOptions));
            Ensure.NotNull(hostEnvironment, nameof(hostEnvironment));

            _cacheSettings = settingsOptions.Value.Cache;
            _hostEnvironment = hostEnvironment;
        }

        public DirectoryInfo GetCacheDirectory()
        {
            if (string.IsNullOrWhiteSpace(_cacheSettings.Path))
                throw new InvalidOperationException("Cache directory undefined");

            DirectoryInfo cacheDirectory = new DirectoryInfo(Path.Combine(_hostEnvironment.ContentRootPath, _cacheSettings.Path));
            if (!cacheDirectory.Exists)
                cacheDirectory.Create();

            return cacheDirectory;
        }

        public async Task SaveToCacheAsync(string cachename, string cache)
        {
            if (string.IsNullOrWhiteSpace(cachename))
                throw new ArgumentNullException(nameof(cachename));

            DirectoryInfo cacheDirectory = GetCacheDirectory();
            await File.WriteAllTextAsync(Path.Combine(cacheDirectory.FullName, GetCacheFilename(cachename)), cache);
        }

        public async Task<string?> LoadFromCacheAsync(string cachename)
        {
            Ensure.NotEmpty(cachename, nameof(cachename));

            FileInfo? cacheFile = GetCacheFile(cachename);
            if (cacheFile == null || cacheFile.Exists == false)
            {
                return null;
            }

            return await File.ReadAllTextAsync(cacheFile.FullName);
        }

        public void ClearCache()
        {
            DirectoryInfo cacheDirectory = GetCacheDirectory();
            cacheDirectory.GetFiles()
                   .Where(f => f.CreationTime < DateTime.Now.AddSeconds(0 - _cacheSettings.Expire))
                   .ToList()
                   .ForEach(f => f.Delete());
        }

        public void ClearCache(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                ClearCache();

            DirectoryInfo cacheDirectory = GetCacheDirectory();
            cacheDirectory.GetFiles(GetCacheFilename(pattern), SearchOption.TopDirectoryOnly)
                    .ToList()
                    .ForEach(f => f.Delete());
        }

        private FileInfo? GetCacheFile(string cachename)
        {
            return GetCacheDirectory()
                .GetFiles(GetCacheFilename(cachename), SearchOption.TopDirectoryOnly)
                .FirstOrDefault();
        }

        private static string GetCacheFilename(string cachename)
        {
            return Path.GetFileNameWithoutExtension(cachename) + Cache.FileExtension;
        }
    }
}
