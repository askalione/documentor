using Documentor.Config;
using Documentor.Constants;
using Documentor.Models;
using Documentor.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.Services.Impl
{
    public class PerRequestNavigator : INavigator
    {
        private readonly ILogger<Pager> _logger;
        private readonly IOConfig _config;

        private Nav _nav;

        public PerRequestNavigator(ILogger<Pager> logger,
            IOptions<IOConfig> appConfigOptions)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));            
            if (appConfigOptions == null)
                throw new ArgumentNullException(nameof(appConfigOptions));

            _logger = logger;
            _config = appConfigOptions.Value;
        }

        public async Task<Nav> GetNavAsync()
        {
            if (_nav != null)
                return _nav;

            string pagesPath = _config.Pages.Path;
            DirectoryInfo pagesDirectory = new DirectoryInfo(pagesPath);
            DirectoryInfo cacheDirectory = new DirectoryInfo(_config.Cache.Path);            
            string pagesPathHash = Hasher.GetMd5Hash(_config.Pages.Path);
            string navCacheFilename = pagesPathHash + "_" + pagesDirectory.LastWriteTime.ToString("yyyyMMddHHmmss") + Cache.NavPostfix + Cache.FileExtension;
            FileInfo navCacheFile = cacheDirectory.GetFiles(navCacheFilename, SearchOption.TopDirectoryOnly).FirstOrDefault();

            Nav nav = null;
            if (navCacheFile != null)
            {
                nav = JObject.Parse(await File.ReadAllTextAsync(navCacheFile.FullName)).ToObject<Nav>();
            }            
            if (nav == null)
            {
                cacheDirectory.GetFiles("*" + Cache.NavPostfix + Cache.FileExtension, SearchOption.TopDirectoryOnly)
                    .ToList()
                    .ForEach(f => f.Delete());

                nav = new Nav(await GetNavItemsAsync(_config.Pages.Path, new List<Folder>()));
                string navCacheFilePath = Path.Combine(_config.Cache.Path, navCacheFilename);
                await File.WriteAllTextAsync(navCacheFilePath, JObject.FromObject(nav).ToString(Formatting.None)); 
            }

            return _nav = nav;
        }

        public async Task<List<NavItem>> GetNavItemsAsync(string scanPath, List<Folder> parentFolders)
        {
            List<NavItem> navItems = new List<NavItem>();
            foreach (string directoryPath in Directory.GetDirectories(scanPath))
            {
                Folder folder = new Folder(new DirectoryInfo(directoryPath).Name);
                List<Folder> navItemparentFolders = parentFolders.ToList();
                navItemparentFolders.Add(folder);

                NavItem navItem = new NavItem(await GetNavItemDisplayNameAsync(String.Join(Separator.Path, navItemparentFolders.Select(x => x.DirectoryName)), folder.VirtualName), 
                    String.Join(Separator.Path, navItemparentFolders.Select(x => x.VirtualName)), 
                    folder.SequenceNumber);
                (await GetNavItemsAsync(directoryPath, navItemparentFolders))
                    .ForEach(x => navItem.AddChild(x));
                navItems.Add(navItem);
            }
            return navItems;
        }

        private async Task<string> GetNavItemDisplayNameAsync(string path, string virtualName)
        {
            string metadataPath = Path.Combine(_config.Pages.Path, path, Metadata.Filename);
            string metadataDisplayName = String.Empty;

            if (File.Exists(metadataPath))
                metadataDisplayName = JObject.Parse(await File.ReadAllTextAsync(metadataPath))?[nameof(PageMetadata.Title).ToLower()]?.ToString();

            return !String.IsNullOrWhiteSpace(metadataDisplayName) ? metadataDisplayName : virtualName;
        }
    }
}
