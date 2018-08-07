namespace Documentor.Config
{
    public class AppConfig
    {
        public string DisplayName { get; set; }
        public bool ShowSequenceNumbers { get; set; }
        public DownloadSettings Download { get; set; }
        public ExternalLinksSettings ExternalLinks { get; set; }
    }
}
