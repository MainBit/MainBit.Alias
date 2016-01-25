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
using MainBit.Alias.Descriptors;

namespace MainBit.Alias.Routes
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

        public static bool IsNeedToConfigureUrlTemplates(HttpContextBase httpContext)
        {
            var allowInvalid = new[] {
                "Admin",
                "Users/Account/LogOff",
                "Users/Account/LogOn",
                "Users/Account/AccessDenied"
            };
            var allowInvalidStart = new[] {
                "MainBit.Alias/"
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

            IWorkContextScope wcs = null;
            try
            {
                var workContext = _workContextAccessor.GetContext();
                if (workContext == null)
                {
                    wcs = _workContextAccessor.CreateWorkContextScope(new HttpContextWrapper(httpContext.ApplicationInstance.Context));
                    workContext = wcs.WorkContext;
                }
                var urlService = workContext.Resolve<IUrlService>();

                if (!urlService.IsUrlTemplatesConfigured())
                    return null;

                var urlContext = urlService.GetCurrentContext();
                if (urlContext == null)
                {
                    if (IsNeedToConfigureUrlTemplates(httpContext)) {
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

                // check if this url is soterd url (or wrond base url) and need to be redireced to right display url
                // virtual path for display url can be the same (they differ in base url only)
                // virtual path for stored url cannot be the same (orchard doesn't allow this)
                if (urlContext.NeedRedirect)
                {
                    var data = new RouteData(this, new RedirectRouteHandler(urlContext.GetFullDisplayUrl()));
                    return data;
                }

                // check if this url doesn't need to find alias by current route
                // because it will be processed by default orchard alias route
                if (string.Equals(urlContext.StoredVirtualPath, urlContext.DisplayVirtualPath))
                {
                    return null;
                }

                // Attempt to lookup RouteValues in the alias map
                AliasInfo aliasInfo; //AliasInfo aliasInfo;
                // TODO: Might as well have the lookup in AliasHolder...
                if (_aliasMap.TryGetAlias(urlContext.StoredVirtualPath, out aliasInfo)) //if (_aliasMap.TryGetAlias(baseUrlContext.StoredVirtualPath, out aliasInfo))
                {
                    // Construct RouteData from the route values
                    var data = new RouteData(this, _routeHandler);
                    foreach (var routeValue in aliasInfo.RouteValues)  //foreach (var routeValue in aliasInfo.RouteValues)
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
            finally
            {
                if (wcs != null)
                    wcs.Dispose();
            }

            return null;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary routeValues) {

            var workContext = _workContextAccessor.GetContext();
            var urlService = workContext.Resolve<IUrlService>();
            var urlContext = urlService.GetCurrentContext();

            // need return not fount page
            if (urlContext == null) { return null; }

            // check if this url doesn't need to find alias by current route
            // because it will be processed by default orchard alias route
            if (string.IsNullOrWhiteSpace(urlContext.Descriptor.StoredPrefix)) { return null; }

            // Lookup best match for route values in the expanded tree
            var match = _aliasMap.Locate(routeValues);
            if (match != null)
            {
                // Build any "spare" route values onto the Alias (so we correctly support any additional query parameters)
                var sb = new StringBuilder();
                if (match.Item2.Equals(urlContext.Descriptor.StoredPrefix, StringComparison.InvariantCultureIgnoreCase)) {
                    
                }
                else if (match.Item2.StartsWith(urlContext.Descriptor.StoredPrefix + "/", StringComparison.InvariantCultureIgnoreCase)) {
                    sb.Append(match.Item2.Substring(urlContext.Descriptor.StoredPrefix.Length + 1));
                }
                else {
                    sb.Append(match.Item2);
                }

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