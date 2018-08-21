using Documentor.Config;
using Documentor.Constants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.Services.Impl
{
    public class CacheManager : ICacheManager
    {
        private readonly CacheSettings _cacheSettings;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CacheManager(IOptionsSnapshot<IOConfig> configOptions,
            IHostingEnvironment hostingEnvironment)
        {
            if (configOptions == null)
                throw new ArgumentNullException(nameof(configOptions));
            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));

            _cacheSettings = configOptions.Value.Cache;
            _hostingEnvironment = hostingEnvironment;
        }

        public DirectoryInfo GetCacheDirectory()
        {
            if (String.IsNullOrWhiteSpace(_cacheSettings.Path))
                throw new InvalidOperationException("Cache directory undefined");

            DirectoryInfo cacheDirectory = new DirectoryInfo(Path.Combine(_hostingEnvironment.ContentRootPath, _cacheSettings.Path));
            if (!cacheDirectory.Exists)
                cacheDirectory.Create();

            return cacheDirectory;
        }

        public async Task SaveToCacheAsync(string cachename, string cache)
        {
            if (String.IsNullOrWhiteSpace(cachename))
                throw new ArgumentNullException(nameof(cachename));

            DirectoryInfo cacheDirectory = GetCacheDirectory();
            await File.WriteAllTextAsync(Path.Combine(cacheDirectory.FullName, GetCacheFilename(cachename)), cache);
        }

        public async Task<string> LoadFromCacheAsync(string cachename)
        {
            if (String.IsNullOrWhiteSpace(cachename))
                throw new ArgumentNullException(nameof(cachename));

            FileInfo cacheFile = GetCacheFile(cachename);
            if (cacheFile == null || !cacheFile.Exists)
                return null;

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
            if (String.IsNullOrWhiteSpace(pattern))
                ClearCache();

            DirectoryInfo cacheDirectory = GetCacheDirectory();
            cacheDirectory.GetFiles(GetCacheFilename(pattern), SearchOption.TopDirectoryOnly)
                    .ToList()
                    .ForEach(f => f.Delete());
        }

        private FileInfo GetCacheFile(string cachename)
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
