using System;
using System.Collections.Generic;
using System.Linq;

namespace Documentor.Models
{
    public class Nav
    {
        private List<NavItem> _items = new List<NavItem>();
        public IEnumerable<NavItem> Items => _items.OrderBy(x => x.SequenceNumber);

        public Nav(List<NavItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _items = items;
        }
    }
}
