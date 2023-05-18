namespace Documentor.Framework.Notifications
{
    public static class NotificationActionResultExtensions
    {
        public static IActionResult Notify(this IActionResult actionResult, NotificationType type, string message)
        {
            return new NotifiedActionResult(actionResult, type, message);
        }
    }
}
