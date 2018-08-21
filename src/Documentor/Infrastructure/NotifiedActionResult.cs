using Documentor.Extensions;
using Documentor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Documentor.Infrastructure
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
            _notificationMessage = notificationMessage?.Trim(); ;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var factory = context.HttpContext.RequestServices.GetService<ITempDataDictionaryFactory>();
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
