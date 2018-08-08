using Documentor.Constants;
using Documentor.Models;
using Documentor.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Documentor.Services.Impl
{
    public class Pager : IPager
    {
        private readonly ILogger<Pager> _logger;
        private readonly IMarkdownConverter _markdownConverter;
        private readonly ICacheManager _cacheManager;
        private readonly IPageManager _pageManager;

        public Pager(ILogger<Pager> logger,
            IMarkdownConverter markdownConverter,
            ICacheManager cacheManager,
            IPageManager pageManager)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (markdownConverter == null)
                throw new ArgumentNullException(nameof(markdownConverter));
            if (cacheManager == null)
                throw new ArgumentNullException(nameof(cacheManager));
            if (pageManager == null)
                throw new ArgumentNullException(nameof(pageManager));

            _logger = logger;
            _markdownConverter = markdownConverter;
            _cacheManager = cacheManager;
            _pageManager = pageManager;
        }

        public async Task<Page> GetPageAsync(string virtualPath)
        {
            PagePath pagePath = GetPagePath(virtualPath);
            if (pagePath == null)
                return null;

            PageContent pageContent = await GetPageContentAsync(pagePath);
            PageMetadata pageMetadata = await GetPageMetadataAsync(pagePath);

            return new Page(pagePath, pageMetadata, pageContent);
        }

        private PagePath GetPagePath(string virtualPath)
        {
            Location location = !String.IsNullOrWhiteSpace(virtualPath) ?
                GetLocation(virtualPath) :
                GetLocation("", 1);

            if (!location.Undefined)
            {
                string filePath = Directory.GetFiles(Path.Combine(_pageManager.GetPagesDirectory().FullName, location.GetDirectoryPath()), Markdown.Filename).FirstOrDefault();
                if (!String.IsNullOrEmpty(filePath))
                    return new PagePath(location, new FileInfo(filePath).Name);
            }

            return null;
        }

        private async Task<PageContent> GetPageContentAsync(PagePath pagePath)
        {
            DirectoryInfo pagesDirectory = _pageManager.GetPagesDirectory();
            DirectoryInfo cacheDirectory = _cacheManager.GetCacheDirectory();

            _cacheManager.ClearCache();

            FileInfo pageFileInfo = new FileInfo(Path.Combine(pagesDirectory.FullName, pagePath.ToString()));

            string pageLocationHash = Hasher.GetMd5Hash(pagePath.Location.GetDirectoryPath());
            string pageCachename = pageLocationHash + "_" + pageFileInfo.LastWriteTime.ToString("yyyyMMddHHmmss") + Cache.PagePostfix;
            string pageCache = await _cacheManager.LoadFromCacheAsync(pageCachename);

            string markdown = await File.ReadAllTextAsync(pageFileInfo.FullName);
            string html;
            if (!String.IsNullOrEmpty(pageCache))
            {
                html = pageCache;
            }
            else
            {
                _cacheManager.ClearCache(pageLocationHash + "*" + Cache.PagePostfix);
                html = _markdownConverter.ConvertToHtml(markdown);

                await _cacheManager.SaveToCacheAsync(pageCachename, html);
            }

            return new PageContent(markdown, html);
        }

        private async Task<PageMetadata> GetPageMetadataAsync(PagePath pagePath)
        {
            string metadata = await _pageManager.LoadMetadataAsync(pagePath.Location.GetDirectoryPath());
            PageMetadata pageMetadata = new PageMetadata();

            if (!String.IsNullOrWhiteSpace(metadata))
            {
                try
                {
                    pageMetadata = JObject.Parse(metadata).ToObject<PageMetadata>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Metadata invalid");
                }
            }

            if (String.IsNullOrWhiteSpace(pageMetadata.Title))
                pageMetadata.Title = pagePath.Location.GetDestinationFolder().VirtualName;

            return pageMetadata;
        }

        private Location GetLocation(string virtualPath)
        {
            if (String.IsNullOrWhiteSpace(virtualPath))
                return Location.Empty;

            Stack<string> virtualNamesForScan = new Stack<string>(virtualPath.Split(Separator.Path).Reverse());
            List<Folder> folders = new List<Folder>();
            string scanPath = _pageManager.GetPagesDirectory().FullName;

            while (!String.IsNullOrWhiteSpace(scanPath) && virtualNamesForScan.Count > 0)
            {
                Regex regex = new Regex($@"^(0*)([1-9]+)\.{virtualNamesForScan.Pop()}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                string directoryPath = Directory.GetDirectories(scanPath)
                    .FirstOrDefault(x => regex.IsMatch(new DirectoryInfo(x).Name));
                if (!String.IsNullOrEmpty(directoryPath))
                {
                    string directoryName = new DirectoryInfo(directoryPath).Name;
                    folders.Add(new Folder(directoryName));
                    scanPath = Path.Combine(scanPath, directoryName);
                }
                else
                {
                    scanPath = null;
                    folders.Clear();
                }
            }

            return new Location(folders);
        }

        private Location GetLocation(string virtualPath, int sequenceNumber)
        {
            Location location = GetLocation(virtualPath);
            Regex regex = new Regex($@"^(0*){sequenceNumber}\.(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            string directoryPath = Directory.EnumerateDirectories(Path.Combine(_pageManager.GetPagesDirectory().FullName, location.GetDirectoryPath()))
                .FirstOrDefault(x => regex.IsMatch(new DirectoryInfo(x).Name));

            if (!String.IsNullOrEmpty(directoryPath))
            {
                List<Folder> folders = location.Folders.ToList();
                folders.Add(new Folder(new DirectoryInfo(directoryPath).Name));
                return new Location(folders);
            }
            else
            {
                return Location.Empty;
            }
        }
    }
}
