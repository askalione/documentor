using Documentor.Framework.Constants;

namespace Documentor.Models.Pages
{
    public sealed class Location
    {
        private List<Folder> _folders;
        public IEnumerable<Folder> Folders => _folders.ToList();
        public bool Undefined => _folders.Count == 0;

        public Location(List<Folder> folders)
        {
            if (folders == null)
                throw new ArgumentNullException(nameof(folders));

            _folders = folders;
        }

        public Folder? GetBaseFolder()
        {
            return Folders.FirstOrDefault();
        }

        public Folder? GetDestinationFolder()
        {
            return Folders.LastOrDefault();
        }

        public string GetVirtualPath()
        {
            return GetPath(x => x.VirtualName);
        }

        public string GetDirectoryPath()
        {
            return GetPath(x => x.DirectoryName);
        }

        private string GetPath(Func<Folder, string> select)
        {
            return string.Join(Separator.Path, _folders.Select(select));
        }

        public override string ToString()
        {
            return GetDirectoryPath();
        }

        public static Location Empty => new Location(new List<Folder>());
    }
}
