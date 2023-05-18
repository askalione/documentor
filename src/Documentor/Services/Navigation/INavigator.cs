using Documentor.Framework.Navigation;

namespace Documentor.Services.Navigation
{
    public interface INavigator
    {
        Task<Nav> GetNavAsync();
    }
}
