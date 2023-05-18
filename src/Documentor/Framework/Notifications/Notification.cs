namespace Documentor.Framework.Notifications
{
    public record Notification
    {
        public NotificationType Type { get; set; }
        public string Message { get; set; } = default!;
    }
}
