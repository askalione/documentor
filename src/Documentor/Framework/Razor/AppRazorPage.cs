using Microsoft.AspNetCore.Mvc.Razor;

namespace Documentor.Framework.Razor
{
    public abstract class AppRazorPage<TModel> : RazorPage<TModel>
    {
        public virtual string Title
        {
            get { return ViewBag.PageTitle ?? ""; }
            set { ViewBag.PageTitle = value; }
        }

        public virtual string? Description
        {
            get { return ViewBag.PageDescription ?? ""; }
            set { ViewBag.PageDescription = value; }
        }

        public virtual bool FullWidth
        {
            get { return ViewBag.FullWidth ?? false; }
            set { ViewBag.FullWidth = value; }
        }

        public virtual bool Breadcrumbs
        {
            get { return ViewBag.Breadcrumbs ?? true; }
            set { ViewBag.Breadcrumbs = value; }
        }
    }
}
