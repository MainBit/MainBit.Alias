using Orchard;
using Orchard.Environment.Configuration;
using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Services
{
    public interface IUrlUtils : IDependency
    {
        string GetBaseUrl();
        string GetVirtualPath();
    }

    public class UrlUtils : IUrlUtils
    {
        private readonly IWorkContextAccessor _wca;
        private readonly ShellSettings _shellSettings;
        private readonly UrlPrefix _urlPrefix;

        public UrlUtils(IWorkContextAccessor wca,
            ShellSettings shellSettings)
        {
            _wca = wca;
            _shellSettings = shellSettings;

            if (!string.IsNullOrEmpty(_shellSettings.RequestUrlPrefix))
                _urlPrefix = new UrlPrefix(_shellSettings.RequestUrlPrefix);
        }

        public string GetBaseUrl()
        {
            var workContext = _wca.GetContext();
            var baseUrl = (workContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)
                + workContext.HttpContext.Request.ApplicationPath).TrimEnd('/');

            return string.IsNullOrWhiteSpace(_shellSettings.RequestUrlPrefix)
                ? baseUrl
                : baseUrl.EndsWith("/" + _shellSettings.RequestUrlPrefix) // for speial orchard httpContext
                    ? baseUrl
                    : baseUrl + "/" + _shellSettings.RequestUrlPrefix;
        }

        public string GetVirtualPath()
        {
            var workContext = _wca.GetContext();
            var virtualPath = workContext.HttpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2);

            // when command is running from command line than HttpContextBase is HttpContextPlaceholder (that doesn't implement PathInfo)
            try {
                virtualPath += workContext.HttpContext.Request.PathInfo;
            }
            catch (System.NotImplementedException e) {

            }

            return _urlPrefix == null
                ? virtualPath
                : _urlPrefix.RemoveLeadingSegments(virtualPath);
        }
    }
}