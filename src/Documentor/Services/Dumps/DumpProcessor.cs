using Documentor.Services.Pages;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Documentor.Services.Dumps
{
    public class DumpProcessor : IDumpProcessor
    {
        private readonly IPageIOManager _pageIOManager;

        public DumpProcessor(IPageIOManager pageIOManager)
        {
            if (pageIOManager == null)
                throw new ArgumentNullException(nameof(pageIOManager));

            _pageIOManager = pageIOManager;
        }

        public byte[] ExportDump()
        {
            string path = _pageIOManager.GetPagesDirectory().FullName;
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
