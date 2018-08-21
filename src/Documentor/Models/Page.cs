using System;

namespace Documentor.Models
{
    public sealed class Page
    {
        public PageContext Context { get; }
        public PageData Data { get; }

        public Page(PageContext context,
            PageData data)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Context = context;
            Data = data;
        }
    }
}
