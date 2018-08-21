
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentor.Services.Impl
{
    public class DumpProcessor : IDumpProcessor
    {
        private readonly IPageManager _pageManager;

        public DumpProcessor(IPageManager pageManager)
        {
            if (pageManager == null)
                throw new ArgumentNullException(nameof(pageManager));

            _pageManager = pageManager;
        }

        public byte[] ExportDump()
        {
            string path = _pageManager.GetPagesDirectory().FullName;
            using (MemoryStream outputMemoryStream = new MemoryStream())
            {
                using (ZipOutputStream zipOutputStream = new ZipOutputStream(outputMemoryStream))
                {
                    zipOutputStream.SetLevel(5);
                    CompressFolder(path, zipOutputStream, path.Length + (path.EndsWith("\\") ? 0 : 1));
                    zipOutputStream.IsStreamOwner = false;
                    zipOutputStream.Close();
                    outputMemoryStream.Position = 0;
                    return outputMemoryStream.ToArray();
                }
            }
        }

        public void CompressFolder(string path, ZipOutputStream zipOutputStream, int folderOffset)
        {
            var files = Directory.GetFiles(path);
            var directories = Directory.GetDirectories(path);
            
            if (files.Count() == 0 && directories.Count() == 0)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                string cleanName = ZipEntry.CleanName(path.Substring(folderOffset)) + "/";
                ZipEntry zipEntry = new ZipEntry(cleanName);
                zipEntry.DateTime = directoryInfo.LastWriteTime;
                zipOutputStream.PutNextEntry(zipEntry);
                zipOutputStream.CloseEntry();
                return;
            }

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                string cleanName = ZipEntry.CleanName(file.Substring(folderOffset));
                ZipEntry zipEntry = new ZipEntry(cleanName);
                zipEntry.DateTime = fileInfo.LastWriteTime;
                zipEntry.Size = fileInfo.Length;
                zipOutputStream.PutNextEntry(zipEntry);
                byte[] numArray = new byte[4096];
                using (FileStream fileStream = File.OpenRead(file))
                {
                    StreamUtils.Copy(fileStream, zipOutputStream, numArray);
                }
                zipOutputStream.CloseEntry();
            }

            foreach (string directory in directories)
            {
                CompressFolder(directory, zipOutputStream, folderOffset);
            }
        }
    }
}
