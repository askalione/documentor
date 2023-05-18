namespace Documentor.Models.Pages
{
    public sealed class PageContext
    {
        public PagePath Path { get; }
        public PageMetadata Metadata { get; }

        public PageContext(PagePath path, PageMetadata metadata)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            Path = path;
            Metadata = metadata;
        }
    }
}
