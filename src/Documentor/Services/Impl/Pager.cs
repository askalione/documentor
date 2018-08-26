using Documentor.Constants;
using Documentor.Infrastructure;
using Documentor.Models;
using Documentor.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly IPageIOManager _pageIOManager;

        public Pager(ILogger<Pager> logger,
            IMarkdownConverter markdownConverter,
            ICacheManager cacheManager,
            IPageIOManager pageIOManager)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (markdownConverter == null)
                throw new ArgumentNullException(nameof(markdownConverter));
            if (cacheManager == null)
                throw new ArgumentNullException(nameof(cacheManager));
            if (pageIOManager == null)
                throw new ArgumentNullException(nameof(pageIOManager));

            _logger = logger;
            _markdownConverter = markdownConverter;
            _cacheManager = cacheManager;
            _pageIOManager = pageIOManager;
        }

        public async Task<Page> GetPageAsync(string virtualPath)
        {
            PagePath pagePath = GetPagePath(virtualPath);
            if (pagePath == null)
                return null;

            PageData pageData = await GetPageDataAsync(pagePath);
            PageMetadata pageMetadata = await GetPageMetadataAsync(pagePath);

            return new Page(new PageContext(pagePath, pageMetadata), pageData);
        }

        public async Task<Page> EditPageAsync(PagePath pagePath, string markdown)
        {
            if (pagePath == null)
                throw new ArgumentNullException(nameof(pagePath));

            DirectoryInfo pagesDirectory = _pageIOManager.GetPagesDirectory();
            FileInfo pageFileInfo = new FileInfo(Path.Combine(pagesDirectory.FullName, pagePath.ToString()));

            string pageLocationHash = Hasher.GetMd5Hash(pagePath.Location.GetDirectoryPath());
            string pageCachename = pageLocationHash + "_" + pageFileInfo.LastWriteTime.ToString("yyyyMMddHHmmss") + Cache.PagePostfix;
            _cacheManager.ClearCache(pageCachename);

            await _pageIOManager.SavePage(pageFileInfo.FullName, markdown);

            PageData pageData = await GetPageDataAsync(pagePath);
            PageMetadata pageMetadata = await GetPageMetadataAsync(pagePath);

            return new Page(new PageContext(pagePath, pageMetadata), pageData);
        }

        public async Task AddPageAsync(PageAddCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            string parentPath = _pageIOManager.GetPagesDirectory().FullName;
            if (!string.IsNullOrWhiteSpace(command.ParentVirtualPath))
            {
                Location parentLocation = GetLocation(command.ParentVirtualPath);
                if (parentLocation == null)
                    throw new AppException("Parent page not found");
                parentPath = Path.Combine(parentPath, parentLocation.GetDirectoryPath());
            }

            string[] siblingDirectoriesPath = Directory.GetDirectories(parentPath);
            if (siblingDirectoriesPath.Length > 0)
            {
                Regex regex = new Regex($@"^(0*)([1-9]+)\{Separator.Sequence}{command.VirtualName}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                string existsDirectoryPath = siblingDirectoriesPath
                    .FirstOrDefault(x => regex.IsMatch(new DirectoryInfo(x).Name));
                if (!string.IsNullOrWhiteSpace(existsDirectoryPath))
                    throw new AppException($"Virtual name {command.VirtualName} already exists");
            }

            int sequenceNumber = 0;
            string lastSiblingDirectoryPath = siblingDirectoriesPath.OrderBy(x => x).LastOrDefault();
            if (!string.IsNullOrWhiteSpace(lastSiblingDirectoryPath))
                Int32.TryParse(new DirectoryInfo(lastSiblingDirectoryPath).Name.Split(Separator.Sequence)[0], out sequenceNumber);
            sequenceNumber++;
            string virtualName = (sequenceNumber < 10 ? "0" : "") + sequenceNumber.ToString() + Separator.Sequence + command.VirtualName;
            DirectoryInfo pageDirectoryInfo = Directory.CreateDirectory(Path.Combine(parentPath, virtualName));
            await _pageIOManager.SavePage(Path.Combine(pageDirectoryInfo.FullName, Markdown.Filename), "");
            await _pageIOManager.SaveMetadataAsync(pageDirectoryInfo.FullName, new PageMetadata
            {
                Title = command.Title,
                Description = command.Description
            });
        }

        public async Task<PageContext> GetPageContextAsync(string virtualPath)
        {
            PagePath pagePath = GetPagePath(virtualPath);
            if (pagePath == null)
                throw new AppException("Page not found");
            PageMetadata pageMetadata = await GetPageMetadataAsync(pagePath);
            return new PageContext(pagePath, pageMetadata);
        }

        public async Task ModifyPageAsync(PageModifyCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            PagePath pagePath = GetPagePath(command.VirtualPath);
            if (pagePath == null)
                throw new AppException("Page not found");

            _cacheManager.ClearCache("*" + Cache.NavPostfix);

            string pageDirectoryPath = pagePath.Location.GetDirectoryPath();

            PageMetadata pageMetadata = await GetPageMetadataAsync(pagePath);
            pageMetadata.Title = command.Title;
            pageMetadata.Description = command.Description;
            await _pageIOManager.SaveMetadataAsync(pageDirectoryPath, pageMetadata);

            if (!command.VirtualName.Equals(pagePath.Location.GetDestinationFolder().VirtualName))
            {
                string pagesDirectoryPath = _pageIOManager.GetPagesDirectory().FullName;
                string oldPageDirectoryName = pagePath.Location.GetDestinationFolder().DirectoryName;
                string newPageDirectoryPath = pageDirectoryPath.Remove(pageDirectoryPath.LastIndexOf(Separator.Path) + 1) +
                    oldPageDirectoryName.Split(Separator.Sequence)[0] +
                    Separator.Sequence +
                    command.VirtualName;
                string fakePageDirectoryPath = newPageDirectoryPath + "_";
                Directory.Move(Path.Combine(pagesDirectoryPath, pageDirectoryPath), Path.Combine(pagesDirectoryPath, fakePageDirectoryPath));
                Directory.Move(Path.Combine(pagesDirectoryPath, fakePageDirectoryPath), Path.Combine(pagesDirectoryPath, newPageDirectoryPath));
            }
        }

        public void DeletePage(string virtualPath)
        {
            if (string.IsNullOrWhiteSpace(virtualPath))
                throw new ArgumentNullException(nameof(virtualPath));

            Location pageLocation = GetLocation(virtualPath);
            if (pageLocation == null)
                throw new AppException("Page not found");

            Directory.Delete(Path.Combine(_pageIOManager.GetPagesDirectory().FullName, pageLocation.GetDirectoryPath()), true);
            _cacheManager.ClearCache("*" + Cache.NavPostfix);
        }

        public void MovePage(PageMoveCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            command.NewParentVirtualPath = command.NewParentVirtualPath?.Trim(Separator.Path);

            string basePath = _pageIOManager.GetPagesDirectory().FullName;

            Location location = GetLocation(command.VirtualPath);
            if (location == null)
                throw new AppException("Page not found");

            _cacheManager.ClearCache("*" + Cache.NavPostfix);

            string directoryName = location.GetDestinationFolder().DirectoryName;
            string directoryPath = Path.GetFullPath(Path.Combine(basePath, location.GetDirectoryPath()));
            int oldSequenceNumber = int.Parse(directoryName.Split(Separator.Sequence)[0]);

            string newDirectoryName = (command.NewSequenceNumber < 10 ? "0" : "") + command.NewSequenceNumber.ToString() + Separator.Sequence + directoryName.Split(Separator.Sequence)[1];
            string newDirectoryPath = Path.GetFullPath(Path.Combine(basePath, newDirectoryName));

            if (command.NewParentVirtualPath != null)
            {
                if (!command.NewParentVirtualPath.Equals(""))
                {
                    Location parentLocation = GetLocation(command.NewParentVirtualPath);
                    if (parentLocation == null)
                        throw new AppException("Parent page not found");
                    newDirectoryPath = Path.GetFullPath(Path.Combine(basePath, parentLocation.GetDirectoryPath(), newDirectoryName));
                }
            }

            // Move directory
            if (!newDirectoryPath.Equals(directoryPath, StringComparison.OrdinalIgnoreCase))
                Directory.Move(directoryPath, newDirectoryPath);
            
            Regex regex = new Regex($@"^(0*)([1-9]+)\{Separator.Sequence}(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            // Change sequence number
            // If not move to another parent directory
            if (command.NewParentVirtualPath == null)
            {
                new DirectoryInfo(newDirectoryPath).Parent.GetDirectories()
                    .Where(x => regex.IsMatch(x.Name))
                    .Where(x => !x.FullName.Equals(newDirectoryPath, StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .ForEach(x =>
                    {
                        string[] sequenceDirectoryNameParts = x.Name.Split(Separator.Sequence);
                        if (int.TryParse(sequenceDirectoryNameParts[0], out int sequenceNumber) &&
                            ((command.NewSequenceNumber < oldSequenceNumber) && (sequenceNumber >= command.NewSequenceNumber && sequenceNumber < oldSequenceNumber)) ||
                            ((command.NewSequenceNumber > oldSequenceNumber) && (sequenceNumber <= command.NewSequenceNumber && sequenceNumber > oldSequenceNumber)))
                        {
                            sequenceNumber = sequenceNumber + (command.NewSequenceNumber < oldSequenceNumber ? 1 : -1);
                            string sequenceDirectoryName = (sequenceNumber < 10 ? "0" : "") + sequenceNumber.ToString() + Separator.Sequence + sequenceDirectoryNameParts[1];
                            Directory.Move(x.FullName, Path.Combine(x.Parent.FullName, sequenceDirectoryName));
                        }
                    });
            }
            // Move to another parent directory
            else
            {
                // New parent
                new DirectoryInfo(newDirectoryPath).Parent.GetDirectories()
                    .Where(x => regex.IsMatch(x.Name))
                    .Where(x => !x.FullName.Equals(newDirectoryPath, StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .ForEach(x =>
                    {
                        string[] sequenceDirectoryNameParts = x.Name.Split(Separator.Sequence);
                        if (int.TryParse(sequenceDirectoryNameParts[0], out int sequenceNumber) &&
                            sequenceNumber >= command.NewSequenceNumber)
                        {
                            sequenceNumber++;
                            string sequenceDirectoryName = (sequenceNumber < 10 ? "0" : "") + sequenceNumber.ToString() + Separator.Sequence + sequenceDirectoryNameParts[1];
                            string newPath = Path.Combine(x.Parent.FullName, sequenceDirectoryName);
                            directoryPath = directoryPath.Replace(x.FullName, newPath);
                            Directory.Move(x.FullName, newPath);
                        }
                    });

                // Old parent
                new DirectoryInfo(directoryPath).Parent.GetDirectories()
                    .Where(x => regex.IsMatch(x.Name))
                    .ToList()
                    .ForEach(x =>
                    {
                        string[] sequenceDirectoryNameParts = x.Name.Split(Separator.Sequence);
                        if (int.TryParse(sequenceDirectoryNameParts[0], out int sequenceNumber) &&
                            sequenceNumber > oldSequenceNumber)
                        {
                            sequenceNumber--;
                            string sequenceDirectoryName = (sequenceNumber < 10 ? "0" : "") + sequenceNumber.ToString() + Separator.Sequence + sequenceDirectoryNameParts[1];
                            Directory.Move(x.FullName, Path.Combine(x.Parent.FullName, sequenceDirectoryName));
                        }
                    });
            }
        }

        private PagePath GetPagePath(string virtualPath)
        {
            Location location = !string.IsNullOrWhiteSpace(virtualPath) ?
                GetLocation(virtualPath) :
                GetLocation("", 1);

            if (!location.Undefined)
            {
                string filePath = Directory.GetFiles(Path.Combine(_pageIOManager.GetPagesDirectory().FullName, location.GetDirectoryPath()), Markdown.Filename).FirstOrDefault();
                if (!string.IsNullOrEmpty(filePath))
                    return new PagePath(location, new FileInfo(filePath).Name);
            }

            return null;
        }

        private async Task<PageData> GetPageDataAsync(PagePath pagePath)
        {
            DirectoryInfo pagesDirectory = _pageIOManager.GetPagesDirectory();
            DirectoryInfo cacheDirectory = _cacheManager.GetCacheDirectory();

            _cacheManager.ClearCache();

            FileInfo pageFileInfo = new FileInfo(Path.Combine(pagesDirectory.FullName, pagePath.ToString()));

            string pageLocationHash = Hasher.GetMd5Hash(pagePath.Location.GetDirectoryPath());
            string pageCachename = pageLocationHash + "_" + pageFileInfo.LastWriteTime.ToString("yyyyMMddHHmmss") + Cache.PagePostfix;
            string pageCache = await _cacheManager.LoadFromCacheAsync(pageCachename);

            string markdown = await _pageIOManager.LoadPage(pageFileInfo.FullName);
            string html;
            if (!string.IsNullOrEmpty(pageCache))
            {
                html = pageCache;
            }
            else
            {
                _cacheManager.ClearCache(pageLocationHash + "*" + Cache.PagePostfix);
                html = _markdownConverter.ConvertToHtml(markdown);

                await _cacheManager.SaveToCacheAsync(pageCachename, html);
            }

            return new PageData(markdown, html);
        }

        private async Task<PageMetadata> GetPageMetadataAsync(PagePath pagePath)
        {
            string metadata = await _pageIOManager.LoadMetadataAsync(pagePath.Location.GetDirectoryPath());
            PageMetadata pageMetadata = new PageMetadata();

            if (!string.IsNullOrWhiteSpace(metadata))
            {
                try
                {
                    pageMetadata = JsonConvert.DeserializeObject<PageMetadata>(metadata);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Metadata invalid");
                }
            }

            if (string.IsNullOrWhiteSpace(pageMetadata.Title))
                pageMetadata.Title = pagePath.Location.GetDestinationFolder().VirtualName;

            return pageMetadata;
        }

        private Location GetLocation(string virtualPath)
        {
            if (string.IsNullOrWhiteSpace(virtualPath))
                return Location.Empty;

            Stack<string> virtualNamesForScan = new Stack<string>(virtualPath.Split(Separator.Path).Reverse());
            List<Folder> folders = new List<Folder>();
            string scanPath = _pageIOManager.GetPagesDirectory().FullName;

            while (!string.IsNullOrWhiteSpace(scanPath) && virtualNamesForScan.Count > 0)
            {
                Regex regex = new Regex($@"^(0*)([1-9]+)\{Separator.Sequence}{virtualNamesForScan.Pop()}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                string directoryPath = Directory.GetDirectories(scanPath)
                    .FirstOrDefault(x => regex.IsMatch(new DirectoryInfo(x).Name));
                if (!string.IsNullOrEmpty(directoryPath))
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
            Regex regex = new Regex($@"^(0*){sequenceNumber}\{Separator.Sequence}(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            string directoryPath = Directory.EnumerateDirectories(Path.Combine(_pageIOManager.GetPagesDirectory().FullName, location.GetDirectoryPath()))
                .FirstOrDefault(x => regex.IsMatch(new DirectoryInfo(x).Name));

            if (!string.IsNullOrEmpty(directoryPath))
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
