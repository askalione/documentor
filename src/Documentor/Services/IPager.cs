using Documentor.Models;
using System.Threading.Tasks;

namespace Documentor.Services
{
    public interface IPager
    {
        Task<Page> GetPageAsync(string virtualPath);
    }
}
