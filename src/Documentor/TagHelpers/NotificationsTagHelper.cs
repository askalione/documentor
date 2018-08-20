using Documentor.Extensions;
using Documentor.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.TagHelpers
{
    [HtmlTargetElement("notifications")]
    public class NotificationsTagHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

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

            //output.Content.AppendHtml("$(document).ready(function () {");
            notifications.ForEach(n => output.Content.AppendHtml($"notifications.add('{n.Type.ToString().ToLower()}', '{n.Message}')"));
            //output.Content.AppendHtml("});");
        }
    }
}
