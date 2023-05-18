using Documentor.Framework.TempData;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Documentor.Framework.Notifications
{
    public class NotifiedActionResult : IActionResult
    {
        private readonly IActionResult _innerResult;
        private readonly NotificationType _notificationType;
        private readonly string _notificationMessage;

        public NotifiedActionResult(IActionResult innerResult, NotificationType notificationType, string notificationMessage)
        {
            _innerResult = innerResult;
            _notificationType = notificationType;
            _notificationMessage = notificationMessage.Trim();
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var factory = context.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
            var tempData = factory.GetTempData(context.HttpContext);

            var notifications = tempData.GetNotifications();
            notifications.Add(new Notification
            {
                Type = _notificationType,
                Message = _notificationMessage
            });

            tempData.SetNotifications(notifications);

            await _innerResult.ExecuteResultAsync(context);
        }
    }
}
