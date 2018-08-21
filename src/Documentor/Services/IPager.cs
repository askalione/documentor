using Documentor.Models;
using System.Threading.Tasks;

namespace Documentor.Services
{
    public interface IPager
    {
        Task<Page> GetPageAsync(string virtualPath);
        Task<Page> EditPageAsync(PagePath pagePath, string markdown);
        Task AddPageAsync(PageAddCommand command);
        Task<PageContext> GetPageContextAsync(string virtualPath);
        Task ModifyPageAsync(PageModifyCommand command);
        void RemovePage(string virtualPath);
    }
}
