using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.Models
{
    public sealed class PageContext
    {
        public PagePath Path { get; }
        public PageMetadata Metadata { get; }

        public PageContext(PagePath path,
            PageMetadata metadata)
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
