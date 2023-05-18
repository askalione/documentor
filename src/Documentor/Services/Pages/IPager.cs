using Documentor.Models.Pages;

namespace Documentor.Services.Pages
{
    public interface IPager
    {
        Task<Page?> GetPageAsync(string virtualPath);
        Task<Page> EditPageAsync(PagePath pagePath, string markdown);
        Task AddPageAsync(PageAddCommand command);
        Task<PageContext> GetPageContextAsync(string virtualPath);
        Task ModifyPageAsync(PageModifyCommand command);
        void DeletePage(string virtualPath);
        void MovePage(PageMoveCommand command);
    }
}
