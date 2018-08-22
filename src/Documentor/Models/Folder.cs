using Documentor.Constants;
using System;

namespace Documentor.Models
{
    public sealed class Folder
    {
        public string VirtualName { get; }
        public string DirectoryName { get; }
        public int SequenceNumber { get; } = 0;

        public Folder(string directoryName)
        {
            if (string.IsNullOrWhiteSpace(directoryName))
                throw new ArgumentNullException(nameof(directoryName));

            DirectoryName = directoryName.Trim();

            string[] nameParts = DirectoryName.Split(Separator.Sequence);
            VirtualName = nameParts[nameParts.Length - 1].Trim();

            if (nameParts.Length > 1 && Int32.TryParse(nameParts[0], out int sequenceNumber))
                SequenceNumber = sequenceNumber;
        }
    }
}
