using Documentor.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Documentor.Models
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
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentNullException(nameof(displayName));
            if (string.IsNullOrWhiteSpace(virtualPath))
                throw new ArgumentNullException(nameof(virtualPath));

            DisplayName = displayName.Trim();
            VirtualPath = virtualPath.Trim(Separator.Path, ' ');
            SequenceNumber = sequenceNumber;
        }

        public void AddChild(NavItem child)
        {
            _children.Add(child);
        }
    }
}
