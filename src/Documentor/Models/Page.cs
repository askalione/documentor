using System;

namespace Documentor.Models
{
    public sealed class Page
    {
        public PagePath Path { get; }
        public PageMetadata Metadata { get; }
        public PageContent Content { get; }

        public Page(PagePath path, 
            PageMetadata metadata, 
            PageContent content)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            Path = path;
            Metadata = metadata;
            Content = content;
        }
    }
}
