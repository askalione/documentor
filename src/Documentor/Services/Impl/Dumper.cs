using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentor.Services.Impl
{
    public class Dumper : IDumper
    {
        private readonly IPageManager _pageManager;

        public Dumper(IPageManager pageManager)
        {
            if (pageManager == null)
                throw new ArgumentNullException(nameof(pageManager));

            _pageManager = pageManager;
        }

        public byte[] Export()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var zip = new ZipFile())
                {
                    // TODO: Dont work
                    zip.AddDirectory(_pageManager.GetPagesDirectory().FullName);
                    zip.Save(memoryStream);
                }

                return memoryStream.ToArray();
            }
        }
    }
}
