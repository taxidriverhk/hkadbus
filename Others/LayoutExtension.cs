using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace HKAdBus.Others
{
    public static class LayoutExtension
    {
        public static MvcHtmlString NavigationLink(this HtmlHelper html,
            string linkText, string actionName, string controllerName)
        {
            string contextAction = (string)html.ViewContext.RouteData.Values["action"];
            string contextController = (string)html.ViewContext.RouteData.Values["controller"];

            bool isCurrent =
                string.Equals(contextAction, actionName, StringComparison.CurrentCultureIgnoreCase) &&
                string.Equals(contextController, controllerName, StringComparison.CurrentCultureIgnoreCase);

            return LinkExtensions.ActionLink(html,
                linkText, actionName, controllerName, routeValues: null,
                htmlAttributes: isCurrent ? new { @class = "currentPage" } : null);
        }
    }
}