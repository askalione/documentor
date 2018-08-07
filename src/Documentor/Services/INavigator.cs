using Documentor.Models;
using System.Threading.Tasks;

namespace Documentor.Services
{
    public interface INavigator
    {
        Task<Nav> GetNavAsync();
    }
}
