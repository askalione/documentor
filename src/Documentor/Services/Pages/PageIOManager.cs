using Documentor.Framework.Constants;
using Documentor.Models.Pages;
using Documentor.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Documentor.Services.Pages
{
    public class PageIOManager : IPageIOManager
    {
        private readonly PagesSettings _pagesSettings;
        private readonly IHostEnvironment _hostEnvironment;

        public PageIOManager(IOptionsSnapshot<IOSettings> settingsOptions,
            IHostEnvironment hostingEnvironment)
        {
            if (settingsOptions == null)
                throw new ArgumentNullException(nameof(settingsOptions));
            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));

            _pagesSettings = settingsOptions.Value.Pages;
            _hostEnvironment = hostingEnvironment;
        }

        public DirectoryInfo GetPagesDirectory()
        {
            if (string.IsNullOrWhiteSpace(_pagesSettings.Path))
                throw new InvalidOperationException("Pages directory undefined");

            DirectoryInfo pagesDirectory = new DirectoryInfo(Path.Combine(_hostEnvironment.ContentRootPath, _pagesSettings.Path));
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

        public async Task<string?> LoadMetadataAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            FileInfo? matadataFile = GetMetadataFile(path);
            return matadataFile != null ? await File.ReadAllTextAsync(matadataFile.FullName) : null;
        }

        public async Task SaveMetadataAsync(string path, PageMetadata metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            string metadataPath = GetMetadataFilePath(path);
            await File.WriteAllTextAsync(metadataPath, JsonConvert.SerializeObject(metadata));
        }

        public FileInfo? GetMetadataFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
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
