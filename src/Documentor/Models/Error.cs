namespace Documentor.Models
{
    public class Error
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public bool Fullwidth { get; set; } = false;
    }
}
