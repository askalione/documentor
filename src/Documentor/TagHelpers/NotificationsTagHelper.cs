using Documentor.Framework.Notifications;
using Documentor.Framework.TempData;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Documentor.TagHelpers
{
    [HtmlTargetElement("notifications")]
    public class NotificationsTagHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            List<Notification> notifications = ViewContext.TempData.GetNotifications();
            if (notifications.Count == 0)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "script";
            output.Attributes.Add("type", "text/javascript");
            notifications.ForEach(n => output.Content.AppendHtml($"notifications.add('{n.Type.ToString().ToLower()}', '{n.Message}');"));
        }
    }
}
