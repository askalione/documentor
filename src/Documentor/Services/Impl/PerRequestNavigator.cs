using Documentor.Constants;
using Documentor.Models;
using Documentor.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Documentor.Services.Impl
{
    public class PerRequestNavigator : INavigator
    {
        private readonly ILogger<Pager> _logger;
        private readonly ICacheManager _cacheManager;
        private readonly IPageManager _pageManager;

        private Nav _navPerRequest;
        private readonly Regex _directoryScanRegex;

        public PerRequestNavigator(ILogger<Pager> logger,
            ICacheManager cacheManager,
            IPageManager pageManager)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (cacheManager == null)
                throw new ArgumentNullException(nameof(cacheManager));
            if (pageManager == null)
                throw new ArgumentNullException(nameof(pageManager));

            _logger = logger;
            _cacheManager = cacheManager;
            _pageManager = pageManager;

            _directoryScanRegex = new Regex($@"^(0*)([1-9]+)\{Separator.Sequence}(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public async Task<Nav> GetNavAsync()
        {
            if (_navPerRequest != null)
                return _navPerRequest;

            Nav nav = null;

            string navHash = ComputeNavHash();
            if (!String.IsNullOrWhiteSpace(navHash))
            {
                DirectoryInfo pagesDirectory = _pageManager.GetPagesDirectory();
                DirectoryInfo cacheDirectory = _cacheManager.GetCacheDirectory();

                _cacheManager.ClearCache();

                string navCachename = navHash + Cache.NavPostfix;
                string navCache = await _cacheManager.LoadFromCacheAsync(navCachename);

                if (!String.IsNullOrEmpty(navCache))
                {
                    try
                    {
                        nav = JObject.Parse(navCache).ToObject<Nav>();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Invalid nav cache");
                    }
                }
                if (nav == null)
                {
                    _cacheManager.ClearCache("*" + Cache.NavPostfix);

                    nav = new Nav(await GetNavItemsAsync(pagesDirectory, new List<Folder>()));
                    await _cacheManager.SaveToCacheAsync(navCachename, JObject.FromObject(nav).ToString(Formatting.None));
                }
            }

            return _navPerRequest = nav;
        }

        public async Task<List<NavItem>> GetNavItemsAsync(DirectoryInfo scanDirectory, List<Folder> parentFolders)
        {
            List<NavItem> navItems = new List<NavItem>();

            var test = scanDirectory
                .GetDirectories()
                .Where(x => _directoryScanRegex.IsMatch(x.Name));

            foreach (DirectoryInfo directory in scanDirectory
                .GetDirectories()
                .Where(x => _directoryScanRegex.IsMatch(x.Name)))
            {
                Folder folder = new Folder(directory.Name);
                List<Folder> navItemparentFolders = parentFolders.ToList();
                navItemparentFolders.Add(folder);

                FileInfo pageFile = directory.GetFiles(Markdown.Filename, SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (pageFile != null)
                {
                    NavItem navItem = new NavItem(await GetNavItemDisplayNameAsync(String.Join(Separator.Path, navItemparentFolders.Select(x => x.DirectoryName)), folder.VirtualName),
                        String.Join(Separator.Path, navItemparentFolders.Select(x => x.VirtualName)),
                        folder.SequenceNumber);
                    (await GetNavItemsAsync(directory, navItemparentFolders))
                        .ForEach(x => navItem.AddChild(x));
                    navItems.Add(navItem);
                }
            }
            return navItems;
        }

        private string ComputeNavHash()
        {
            StringBuilder sb = new StringBuilder();
            _pageManager.GetPagesDirectory()
                .GetDirectories("*", SearchOption.AllDirectories)
                .Where(x => _directoryScanRegex.IsMatch(x.Name))
                .OrderBy(x => x.FullName)
                .ToList()
                .ForEach(directory =>
                {
                    string hash = directory.LastWriteTime.ToString("yyyyMMddHHmmss");
                    FileInfo metadataFile = _pageManager.GetMetadataFile(directory.FullName);
                    if (metadataFile != null)
                        hash += metadataFile.LastWriteTime.ToString("yyyyMMddHHmmss");
                    sb.Append(hash);
                });

            return sb.Length > 0 ? Hasher.GetMd5Hash(sb.ToString()) : null;
        }

        private async Task<string> GetNavItemDisplayNameAsync(string path, string virtualName)
        {
            string metadataDisplayName = String.Empty;
            string metadata = await _pageManager.LoadMetadataAsync(path);

            if (!String.IsNullOrWhiteSpace(metadata))
            {
                try
                {
                    metadataDisplayName = JObject.Parse(metadata).GetValue(nameof(PageMetadata.Title))?.ToString();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Metadata invalid");
                }
            }

            return !String.IsNullOrWhiteSpace(metadataDisplayName) ? metadataDisplayName : virtualName;
        }
    }
}
