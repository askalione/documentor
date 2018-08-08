using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
