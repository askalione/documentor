using Documentor.Config;
using Documentor.Constants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
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
                throw new InvalidOperationException("Pages directory not found");

            return pagesDirectory;
        }

        public async Task<string> LoadPage(string path)
        {
            return await File.ReadAllTextAsync(GetPageFilepath(path));
        }

        public async Task SavePage(string path, string content)
        {
            await File.WriteAllTextAsync(GetPageFilepath(path), content ?? "");
        }

        public async Task<string> LoadMetadataAsync(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            FileInfo matadataFile = GetMetadataFile(path);
            return matadataFile != null ? await File.ReadAllTextAsync(matadataFile.FullName) : null;
        }

        public FileInfo GetMetadataFile(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            FileInfo matadataFile = new FileInfo(GetMetadataFilepath(path));
            return matadataFile.Exists ? matadataFile : null;
        }

        private string GetMetadataFilepath(string path)
        {
            return Path.IsPathRooted(path) ?
                Path.Combine(path, Metadata.Filename) :
                Path.Combine(GetPagesDirectory().FullName, path, Metadata.Filename);
        }

        private string GetPageFilepath(string path)
        {
            return Path.IsPathRooted(path) ?
                path :
                Path.Combine(GetPagesDirectory().FullName, path);
        }
    }
}
