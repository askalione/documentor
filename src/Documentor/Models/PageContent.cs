using System;

namespace Documentor.Models
{
    public sealed class PageContent
    {
        public string Markdown { get; }
        public string Html { get; }

        public PageContent(string markdown, 
            string html)
        {
            if (markdown == null)
                throw new ArgumentNullException(nameof(markdown));
            if (html == null)
                throw new ArgumentNullException(nameof(html));

            Markdown = markdown;
            Html = html;
        }
    }
}
