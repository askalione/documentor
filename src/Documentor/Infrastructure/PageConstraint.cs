using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.Infrastructure
{
    public class PageConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, 
            IRouter route, 
            string routeKey, 
            RouteValueDictionary values, 
            RouteDirection routeDirection)
        {
            var test = !(string.Equals(values["controller"].ToString(), "Pages", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(values["action"].ToString(), "Page", StringComparison.OrdinalIgnoreCase));

            return test;
        }
    }
}
