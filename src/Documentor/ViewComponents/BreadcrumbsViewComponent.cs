using Documentor.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Documentor.ViewComponents
{
    public class BreadcrumbsViewComponent : ViewComponent
    {
        private readonly INavigator _navigator;

        public BreadcrumbsViewComponent(INavigator navigator)
        {
            if (navigator == null)
                throw new ArgumentNullException(nameof(navigator));

            _navigator = navigator;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _navigator.GetNavAsync());
        }
    }
}
