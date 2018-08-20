using Documentor.Infrastructure;
using Documentor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.Extensions
{
    public static class ActionResultExtensions
    {
        public static IActionResult Notify(this IActionResult actionResult, NotificationType type, string message)
        {
            return new NotifiedActionResult(actionResult, type, message);
        }
    }
}
