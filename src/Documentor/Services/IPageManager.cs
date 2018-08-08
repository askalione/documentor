using System.IO;
using System.Threading.Tasks;

namespace Documentor.Services
{
    public interface IPageManager
    {
        DirectoryInfo GetPagesDirectory();
        Task<string> LoadMetadataAsync(string path);
        FileInfo GetMetadataFile(string path);
    }
}
