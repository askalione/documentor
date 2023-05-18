namespace Documentor.Models.Pages
{
    public sealed class PageData
    {
        public string Markdown { get; }
        public string Html { get; }

        public PageData(string markdown, string html)
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
