namespace Documentor.Framework.Routes
{
    public class PageConstraint : IRouteConstraint
    {
        public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            bool result = (string.Equals(values["controller"]!.ToString(), "Pages", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(values["action"]!.ToString(), "Page", StringComparison.OrdinalIgnoreCase)) == false;

            return result;
        }
    }
}
