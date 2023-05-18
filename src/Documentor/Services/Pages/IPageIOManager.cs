using Documentor.Models.Pages;

namespace Documentor.Services.Pages
{
    public interface IPageIOManager
    {
        DirectoryInfo GetPagesDirectory();
        Task<string> LoadPage(string path);
        Task SavePage(string path, string content);
        Task<string?> LoadMetadataAsync(string path);
        Task SaveMetadataAsync(string path, PageMetadata metadata);
        FileInfo? GetMetadataFile(string path);
    }
}
