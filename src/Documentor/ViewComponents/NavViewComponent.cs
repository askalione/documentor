using Documentor.Services.Navigation;

namespace Documentor.ViewComponents
{
    public class NavViewComponent : ViewComponent
    {
        private readonly INavigator _navigator;

        public NavViewComponent(INavigator navigator)
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
