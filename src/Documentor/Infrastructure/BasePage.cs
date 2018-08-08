using Microsoft.AspNetCore.Mvc.Razor;

namespace Documentor.Infrastructure
{
    public abstract class BasePage<TModel> : RazorPage<TModel>
    {
        public string Title
        {
            get { return ViewBag.PageTitle ?? ""; }
            set { ViewBag.PageTitle = value; }
        }

        public string Description
        {
            get { return ViewBag.PageDescription ?? ""; }
            set { ViewBag.PageDescription = value; }
        }

        public bool Fullwidth
        {
            get { return ViewBag.Fullwidth ?? false; }
            set { ViewBag.Fullwidth = value; }
        }
    }
}
