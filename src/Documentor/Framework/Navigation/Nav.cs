namespace Documentor.Framework.Navigation
{
    public class Nav
    {
        private List<NavItem> _items = new List<NavItem>();
        public IEnumerable<NavItem> Items => _items.OrderBy(x => x.SequenceNumber);

        private Nav() { }

        public Nav(List<NavItem> items) : this()
        {
            Ensure.NotNull(items, nameof(items));

            _items = items;
        }

        public static Nav Empty => new Nav();
    }
}
