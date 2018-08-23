using Documentor.Models;
using System.IO;
using System.Threading.Tasks;

namespace Documentor.Services
{
    public interface IPageIOManager
    {
        DirectoryInfo GetPagesDirectory();
        Task<string> LoadPage(string path);
        Task SavePage(string path, string content);
        Task<string> LoadMetadataAsync(string path);        
        Task SaveMetadataAsync(string path, PageMetadata metadata);
        FileInfo GetMetadataFile(string path);
    }
}
