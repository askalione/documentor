using Documentor.Framework.Constants;

namespace Documentor.Models.Pages
{
    public sealed class PagePath
    {
        public Location Location { get; }
        public string Filename { get; }

        public PagePath(Location location,
            string filename)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException(nameof(filename));

            Location = location;
            Filename = filename.Trim();
        }

        public override string ToString()
        {
            return Location.GetDirectoryPath() + Separator.Path + Filename;
        }
    }
}
