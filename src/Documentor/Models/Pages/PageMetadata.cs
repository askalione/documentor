namespace Documentor.Models.Pages
{
    public record PageMetadata
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
    }
}
