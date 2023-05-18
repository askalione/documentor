using Documentor.Settings;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Documentor.TagHelpers
{
    [HtmlTargetElement("external-links")]
    public class ExternalLinksTagHelper : TagHelper
    {
        private readonly ExternalLinksSettings _settings;
        private readonly UrlHelper _urlHelper;

        public ExternalLinksTagHelper(IOptionsSnapshot<AppSettings> settingsOptions,
            IActionContextAccessor actionContextAccessor)
        {
            Ensure.NotNull(settingsOptions, nameof(settingsOptions));
            Ensure.NotNull(actionContextAccessor, nameof(actionContextAccessor));

            _settings = settingsOptions.Value.ExternalLinks;
            _urlHelper = new UrlHelper(actionContextAccessor.ActionContext!);
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            List<ExternalLink> links = new List<ExternalLink>();
            if (string.IsNullOrWhiteSpace(_settings.Github) == false)
            {
                links.Add(new ExternalLink { Url = _settings.Github, Name = nameof(_settings.Github), UseImage = false });
            }
            if (string.IsNullOrWhiteSpace(_settings.Bitbucket) == false)
            {
                links.Add(new ExternalLink { Url = _settings.Bitbucket, Name = nameof(_settings.Bitbucket), UseImage = false });
            }
            if (string.IsNullOrWhiteSpace(_settings.Nuget) == false)
            {
                links.Add(new ExternalLink { Url = _settings.Nuget, Name = nameof(_settings.Nuget), UseImage = true });
            }
            if (string.IsNullOrWhiteSpace(_settings.Bower) == false)
            {
                links.Add(new ExternalLink { Url = _settings.Bower, Name = nameof(_settings.Bower), UseImage = true });
            }
            if (string.IsNullOrWhiteSpace(_settings.Npm) == false)
            {
                links.Add(new ExternalLink { Url = _settings.Npm, Name = nameof(_settings.Npm), UseImage = true });
            }
            if (string.IsNullOrWhiteSpace(_settings.Pypi) == false)
            {
                links.Add(new ExternalLink { Url = _settings.Pypi, Name = nameof(_settings.Pypi), UseImage = true });
            }

            links = links
                .Where(x => !string.IsNullOrWhiteSpace(x.Url))
                .ToList();

            if (links.Count == 0)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "share scroller__section");

            TagBuilder inner = new TagBuilder("div");
            inner.AddCssClass("share__inner");

            TagBuilder items = new TagBuilder("ul");
            items.AddCssClass("share__items");

            foreach (ExternalLink link in links)
            {
                TagBuilder item = new TagBuilder("li");
                item.AddCssClass("share__item");

                TagBuilder itemLink = new TagBuilder("a");
                itemLink.AddCssClass("share__link");
                itemLink.Attributes.Add("href", link.Url);
                itemLink.Attributes.Add("title", link.Name);

                TagBuilder itemIcon = new TagBuilder(link.UseImage ? "img" : "i");
                itemIcon.AddCssClass("share__icon share__icon--" + (link.UseImage ? "image" : "font"));
                if (link.UseImage)
                    itemIcon.Attributes.Add("src", _urlHelper.Content("~/assets/images/icon-" + link.Name!.ToLower() + ".svg"));
                else
                    itemIcon.AddCssClass("la la-" + link.Name!.ToLower());

                itemLink.InnerHtml.AppendHtml(itemIcon);
                item.InnerHtml.AppendHtml(itemLink);
                items.InnerHtml.AppendHtml(item);
            }

            inner.InnerHtml.AppendHtml(items);
            output.Content.AppendHtml(inner);
        }

        class ExternalLink
        {
            public string Url { get; set; } = default!;
            public string Name { get; set; } = default!;
            public bool UseImage { get; set; }
        }
    }
}
