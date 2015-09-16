using MainBit.Alias.Services;
using Orchard;
using Orchard.Alias.Implementation.Holder;
using Orchard.Alias.Implementation.Map;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MainBit.Alias.Helpers;

namespace MainBit.Alias
{
    public class UrlTemplateRoutes : RouteBase, IRouteWithArea {

        private readonly AliasMap _aliasMap;
        private readonly IRouteHandler _routeHandler;
        private readonly IWorkContextAccessor _workContextAccessor;

        public UrlTemplateRoutes(
            IWorkContextAccessor workContextAccessor,
            IAliasHolder aliasHolder,
            string areaName,
            IRouteHandler routeHandler)
        {
            Area = areaName;
            _aliasMap = aliasHolder.GetMap(areaName);
            _routeHandler = routeHandler;
            _workContextAccessor = workContextAccessor;
        }

        public static readonly string BaseUrlInvalid = "MainBitBaseUrlInvalid";
        public static bool IsBaseUrlInvalid(HttpContextBase httpContext)
        {
            return httpContext.Items.Contains(BaseUrlInvalid);
        }
        public static void MarkInvalid(HttpContextBase httpContext)
        {
            // the value isn't important
            httpContext.Items[BaseUrlInvalid] = null;
        }

        public static bool IsNeedToConfigureBaseUrl(HttpContextBase httpContext)
        {
            var allowInvalid = new[] {
                "MainBit.Alias/UrlTemplateAdmin",
                "MainBit.Alias/UrlTemplateAdmin/Index",
                "MainBit.Alias/UrlTemplateAdmin/Add",
                "MainBit.Alias/UrlTemplateAdmin/Delete",
                "Users/Account/LogOff",
                "Users/Account/LogOn",
                "Users/Account/AccessDenied"
            };
            var allowInvalidStart = new[] {
                "MainBit.Alias/UrlTemplateAdmin/Edit"
            };

            var virtualPath = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + httpContext.Request.PathInfo;
            if (allowInvalid.Contains(virtualPath, StringComparer.InvariantCultureIgnoreCase)
                || allowInvalidStart.Any(p => virtualPath.StartsWith(p, StringComparison.InvariantCultureIgnoreCase))
                || httpContext.Request.QueryString.AllKeys.Contains("MainBit.Alias", StringComparer.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext) {
            // don't compute unnecessary virtual path if the map is empty
            if (!_aliasMap.Any()) {
                return null;
            }

            using (var wcs = _workContextAccessor.CreateWorkContextScope(new HttpContextWrapper(httpContext.ApplicationInstance.Context)))
            {
                var workContext = wcs.WorkContext;
                var urlService = workContext.Resolve<IUrlService>();
                var urlContext = urlService.GetContext(httpContext.Request);


                // marke base url ivalid and later check it in filter
                if (urlContext == null)
                {
                    if (IsNeedToConfigureBaseUrl(httpContext))
                    {
                        return null;
                    }
                    else
                    {
                        MarkInvalid(httpContext);
                        var data = new RouteData(this, _routeHandler);
                        data.Values.Add("controller", "UrlTemplate");
                        data.Values.Add("action", "Invalid");
                        data.Values["area"] = "MainBit.Alias";
                        data.DataTokens["area"] = "MainBit.Alias";
                        return data;
                    }
                }

                // not need process because it will process by default route provider
                if (string.IsNullOrWhiteSpace(urlContext.Descriptor.Template.StoredVirtualPath))
                { 
                    return null;
                }

                var virtualPath = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + httpContext.Request.PathInfo;
                if (!string.Equals(virtualPath, urlContext.StoredVirtualPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    // need redirect
                }
                
                // Attempt to lookup RouteValues in the alias map
                IDictionary<string, string> routeValues; //AliasInfo aliasInfo;
                // TODO: Might as well have the lookup in AliasHolder...
                if (_aliasMap.TryGetAlias(urlContext.StoredVirtualPath, out routeValues)) //if (_aliasMap.TryGetAlias(baseUrlContext.StoredVirtualPath, out aliasInfo))
                {
                    // Construct RouteData from the route values
                    var data = new RouteData(this, _routeHandler);
                    foreach (var routeValue in routeValues)  //foreach (var routeValue in aliasInfo.RouteValues)
                    {
                        var key = routeValue.Key;
                        if (key.EndsWith("-"))
                            data.Values.Add(key.Substring(0, key.Length - 1), routeValue.Value);
                        else
                            data.Values.Add(key, routeValue.Value);
                    }

                    data.Values["area"] = Area;
                    data.DataTokens["area"] = Area;

                    return data;
                }
            }

            return null;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary routeValues) {

            var workContext = _workContextAccessor.GetContext();
            var urlService = workContext.Resolve<IUrlService>();
            var baseUrl = requestContext.HttpContext.Request.GetBaseUrl();
            var urlContext = urlService.GetContext(baseUrl);

            // need return not fount page
            if (urlContext == null) { return null; }

            // not need process because it will process by default route provider
            if (string.IsNullOrWhiteSpace(urlContext.Descriptor.Template.StoredVirtualPath)) { return null; }

            // Lookup best match for route values in the expanded tree
            var match = _aliasMap.Locate(routeValues);
            if (match != null)
            {
                // Build any "spare" route values onto the Alias (so we correctly support any additional query parameters)
                var sb = new StringBuilder(urlService.GetDisplayVirtualPath(urlContext, match.Item2));
                var extra = 0;
                foreach (var routeValue in routeValues)
                {
                    // Ignore any we already have
                    if (match.Item1.ContainsKey(routeValue.Key))
                    {
                        continue;
                    }

                    // Add a query string fragment
                    sb.Append((extra++ == 0) ? '?' : '&');
                    sb.Append(Uri.EscapeDataString(routeValue.Key));
                    sb.Append('=');
                    sb.Append(Uri.EscapeDataString(Convert.ToString(routeValue.Value, CultureInfo.InvariantCulture)));
                }
                // Construct data
                var data = new VirtualPathData(this, sb.ToString());
                // Set the Area for this route
                data.DataTokens["area"] = Area;
                return data;
            }

            return null;
        }

        public string Area { get; private set; }
    }
}