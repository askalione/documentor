namespace Documentor.Models.Errors
{
    public record Error
    {
        public int Code { get; set; }
        public string Message { get; set; } = default!;
    }
}
