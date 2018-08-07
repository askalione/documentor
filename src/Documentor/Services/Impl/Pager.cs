using Documentor.Config;
using Documentor.Constants;
using Documentor.Models;
using Documentor.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly IOConfig _config;

        public Pager(ILogger<Pager> logger,
            IMarkdownConverter markdownConverter,
            IOptions<IOConfig> appConfigOptions)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (markdownConverter == null)
                throw new ArgumentNullException(nameof(markdownConverter));
            if (appConfigOptions == null)
                throw new ArgumentNullException(nameof(appConfigOptions));

            _logger = logger;
            _markdownConverter = markdownConverter;
            _config = appConfigOptions.Value;
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
                string filePath = Directory.GetFiles(Path.Combine(_config.Pages.Path, location.GetDirectoryPath()), Markdown.Filename).FirstOrDefault();
                if (!String.IsNullOrEmpty(filePath))
                    return new PagePath(location, new FileInfo(filePath).Name);
            }

            return null;
        }

        private async Task<PageContent> GetPageContentAsync(PagePath pagePath)
        {
            DirectoryInfo cacheDirectory = new DirectoryInfo(_config.Cache.Path);

            if (cacheDirectory.Exists)
            {
                cacheDirectory.GetFiles()
                    .Where(f => f.CreationTime < DateTime.Now.AddSeconds(0 - _config.Cache.Expire))
                    .ToList()
                    .ForEach(f => f.Delete());
            }
            else
            {
                cacheDirectory.Create();
            }

            FileInfo pageFileInfo = new FileInfo(Path.Combine(_config.Pages.Path, pagePath.ToString()));

            string pageLocationHash = Hasher.GetMd5Hash(pagePath.Location.GetDirectoryPath());
            string htmlCacheFilename = pageLocationHash + "_" + pageFileInfo.LastWriteTime.ToString("yyyyMMddHHmmss") + Cache.PagePostfix + Cache.FileExtension;
            FileInfo htmlCacheFile = cacheDirectory.GetFiles(htmlCacheFilename, SearchOption.TopDirectoryOnly).FirstOrDefault();

            string markdown = await File.ReadAllTextAsync(pageFileInfo.FullName);
            string html;
            if (htmlCacheFile != null)
            {
                html = await File.ReadAllTextAsync(htmlCacheFile.FullName);
            }
            else
            {
                cacheDirectory.GetFiles(pageLocationHash + "*" + Cache.PagePostfix + Cache.FileExtension, SearchOption.TopDirectoryOnly)
                    .ToList()
                    .ForEach(f => f.Delete());

                html = _markdownConverter.ConvertToHtml(markdown);

                string htmlCacheFilePath = Path.Combine(_config.Cache.Path, htmlCacheFilename);
                await File.WriteAllTextAsync(htmlCacheFilePath, html);
            }

            return new PageContent(markdown, html);
        }

        private async Task<PageMetadata> GetPageMetadataAsync(PagePath pagePath)
        {
            string metadataPath = Path.Combine(_config.Pages.Path, pagePath.Location.GetDirectoryPath(), Metadata.Filename);
            PageMetadata metadata = new PageMetadata();

            if (File.Exists(metadataPath))
                metadata = JObject.Parse(await File.ReadAllTextAsync(metadataPath)).ToObject<PageMetadata>();

            if (String.IsNullOrWhiteSpace(metadata.Title))
                metadata.Title = pagePath.Location.GetDestinationFolder().VirtualName;

            return metadata;
        }

        private Location GetLocation(string virtualPath)
        {
            if (String.IsNullOrWhiteSpace(virtualPath))
                return Location.Empty();

            Stack<string> virtualNamesForScan = new Stack<string>(virtualPath.Split(Separator.Path).Reverse());
            List<Folder> folders = new List<Folder>();
            string scanPath = _config.Pages.Path;

            while (!String.IsNullOrWhiteSpace(scanPath) && virtualNamesForScan.Count > 0)
            {
                string directoryPath = Directory.GetDirectories(scanPath, "*." + virtualNamesForScan.Pop(), SearchOption.TopDirectoryOnly).FirstOrDefault();
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
            Regex regex = new Regex($"((0*){sequenceNumber}|{sequenceNumber}).*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string directoryPath = Directory.EnumerateDirectories(Path.Combine(_config.Pages.Path, location.GetDirectoryPath()))
                .FirstOrDefault(x => regex.IsMatch(x));

            if (!String.IsNullOrEmpty(directoryPath))
            {
                List<Folder> folders = location.Folders.ToList();
                folders.Add(new Folder(new DirectoryInfo(directoryPath).Name));
                return new Location(folders);
            }
            else
            {
                return Location.Empty();
            }
        }
    }
}
