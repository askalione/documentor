using Documentor.Framework.Constants;
using Newtonsoft.Json;

namespace Documentor.Framework.Navigation
{
    [JsonObject(MemberSerialization.OptIn)]
    public class NavItem
    {
        [JsonProperty]
        public string DisplayName { get; }
        [JsonProperty]
        public string VirtualPath { get; }
        [JsonProperty]
        public int SequenceNumber { get; }

        [JsonProperty("Children")]
        private List<NavItem> _children = new List<NavItem>();
        public IEnumerable<NavItem> Children => _children.OrderBy(x => x.SequenceNumber);

        public NavItem(string displayName,
            string virtualPath,
            int sequenceNumber)
        {
            Ensure.NotEmpty(displayName, nameof(displayName));
            Ensure.NotEmpty(virtualPath, nameof(virtualPath));

            DisplayName = displayName.Trim();
            VirtualPath = virtualPath.Trim(Separator.Path, ' ');
            SequenceNumber = sequenceNumber;
        }

        public void AddChild(NavItem child)
        {
            Ensure.NotNull(child, nameof(child));

            _children.Add(child);
        }
    }
}
