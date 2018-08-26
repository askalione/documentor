using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.Infrastructure
{
    public class HandleExceptionAttribute : ExceptionFilterAttribute
    {
        public string ViewName { get; set; }
            
        public override void OnException(ExceptionContext context)
        {
            if (IsAjaxRequest(context.HttpContext.Request))
                OnAjaxRequestException(context);
            else
                OnRequestException(context);
        }

        private void OnAjaxRequestException(ExceptionContext context)
        {
            JsonResponse result = new JsonResponse(false, context.Exception.GetType() == typeof(AppException) ? context.Exception.Message : "An error has occurred");
            context.Result = new JsonResult(result);
            context.ExceptionHandled = true;
        }

        private void OnRequestException(ExceptionContext context)
        {
            if (context.Exception.GetType() == typeof(AppException))
            {
                string viewName = !string.IsNullOrWhiteSpace(ViewName) ? ViewName : context.RouteData.Values["action"].ToString();
                var result = new ViewResult { ViewName = viewName };
                var modelMetadata = new EmptyModelMetadataProvider();
                result.ViewData = new ViewDataDictionary(modelMetadata, context.ModelState);
                result.ViewData.Add("HandleException", context.Exception);
                result.ViewData.ModelState.AddModelError("", context.Exception.Message);
                context.Result = result;
                context.ExceptionHandled = true;
            }
            else
            {
                base.OnException(context);
            }
        }

        private static bool IsAjaxRequest(HttpRequest httpRequest)
        {
            return httpRequest.Headers["x-requested-with"] == "XMLHttpRequest";
        }
    }
}
