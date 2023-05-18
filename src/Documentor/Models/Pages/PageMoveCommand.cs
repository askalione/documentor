namespace Documentor.Models.Pages
{
    public class PageMoveCommand
    {
        public string VirtualPath { get; set; } = default!;
        public string NewParentVirtualPath { get; set; } = default!;
        public int NewSequenceNumber { get; set; } = default!;
    }
}
