using Orchard.Mvc.Filters;
using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MainBit.Alias
{
    public class BaseRouteFilter : FilterProvider, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (BaseUrlRoutes.IsBaseUrlInvalid(filterContext.HttpContext) && !AdminFilter.IsApplied(filterContext.RequestContext))
            {
                // http://stackoverflow.com/questions/507536/return-view-from-actionfilter
            }
        }
    }
}