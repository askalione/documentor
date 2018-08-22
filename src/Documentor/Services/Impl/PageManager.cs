using Documentor.Config;
using Documentor.Constants;
using Documentor.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Documentor.Services.Impl
{
    public class PageManager : IPageManager
    {
        private readonly PagesSettings _pagesSettings;
        private readonly IHostingEnvironment _hostingEnvironment;

        public PageManager(IOptionsSnapshot<IOConfig> configOptions,
            IHostingEnvironment hostingEnvironment)
        {
            if (configOptions == null)
                throw new ArgumentNullException(nameof(configOptions));
            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));

            _pagesSettings = configOptions.Value.Pages;
            _hostingEnvironment = hostingEnvironment;
        }

        public DirectoryInfo GetPagesDirectory()
        {
            if (String.IsNullOrWhiteSpace(_pagesSettings.Path))
                throw new InvalidOperationException("Pages directory undefined");

            DirectoryInfo pagesDirectory = new DirectoryInfo(Path.Combine(_hostingEnvironment.ContentRootPath, _pagesSettings.Path));
            if (!pagesDirectory.Exists)
                pagesDirectory.Create();

            return pagesDirectory;
        }

        public async Task<string> LoadPage(string path)
        {
            return await File.ReadAllTextAsync(GetPageFilePath(path));
        }

        public async Task SavePage(string path, string content)
        {
            await File.WriteAllTextAsync(GetPageFilePath(path), content ?? "");
        }

        public async Task<string> LoadMetadataAsync(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            FileInfo matadataFile = GetMetadataFile(path);
            return matadataFile != null ? await File.ReadAllTextAsync(matadataFile.FullName) : null;
        }

        public async Task SaveMetadataAsync(string path, PageMetadata metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            string metadataPath = GetMetadataFilePath(path);
            await File.WriteAllTextAsync(metadataPath, JsonConvert.SerializeObject(metadata));
        }

        public FileInfo GetMetadataFile(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            FileInfo matadataFile = new FileInfo(GetMetadataFilePath(path));
            return matadataFile.Exists ? matadataFile : null;
        }

        private string GetMetadataFilePath(string path)
        {
            return Path.IsPathRooted(path) ?
                Path.Combine(path, Metadata.Filename) :
                Path.Combine(GetPagesDirectory().FullName, path, Metadata.Filename);
        }

        private string GetPageFilePath(string path)
        {
            return Path.IsPathRooted(path) ?
                path :
                Path.Combine(GetPagesDirectory().FullName, path);
        }
    }
}
