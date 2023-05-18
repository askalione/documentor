namespace Documentor.Settings
{
    public class AppSettings
    {
        public string? DisplayName { get; set; }
        public bool ShowSequenceNumbers { get; set; }
        public DownloadSettings Download { get; set; } = default!;
        public ExternalLinksSettings ExternalLinks { get; set; } = default!;
        public bool UseHttps { get; set;  }
    }
}
