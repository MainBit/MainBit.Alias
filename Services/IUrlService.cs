using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using MainBit.Alias.Helpers;
using Orchard.Logging;
using Orchard.Mvc;
using MainBit.Alias.Descriptors;

namespace MainBit.Alias.Services
{
    public interface IUrlService : IDependency
    {
        UrlContext CurrentUrlContext();
        //UrlContext GetContext(HttpRequestBase httpRequest);
        UrlContext GetContext(string baseUrl, string virtualPath = "");
        UrlContext GetContext(Dictionary<string, string> segments, string virtualPath = "");

        UrlContext ChangeSegmentValues(UrlContext urlContext, object segments);
        UrlContext ChangeSegmentValues(UrlContext urlContext, IDictionary<string, string> segments);

        //string GetDisplayVirtualPath(UrlContext urlContext, string virtualPath);
        //string GetStoredVirtualPath(UrlContext urlContext, string virtualPath);

        bool IsUrlTemplatesConfigured();
    }
    public class UrlService : IUrlService
    {
        private readonly IUrlTemplateManager _urlTemplateManager;
        private readonly IHttpContextAccessor _hta;

        public UrlService(IUrlTemplateManager urlTemplateManager,
            IHttpContextAccessor hta)
        {
            _urlTemplateManager = urlTemplateManager;
            _hta = hta;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        private UrlContext _currentUrlContext = null;
        public UrlContext CurrentUrlContext()
        {
            if (_currentUrlContext == null)
            {
                var httpContext = _hta.Current();

                var baseUrl = httpContext.Request.GetBaseUrl();
                var virtualPath = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2);
                // when command is running from command line than HttpContextBase is HttpContextPlaceholder (that doesn't implement PathInfo)
                try
                {
                    virtualPath += httpContext.Request.PathInfo;
                }
                catch (System.NotImplementedException e)
                {

                }
                

                _currentUrlContext = GetContext(baseUrl, virtualPath);
            }
            return _currentUrlContext;
        }

        public UrlContext GetContext(string baseUrl, string virtualPath)
        {
            var allTemplateDescriptors = _urlTemplateManager.DescribeUrlTemplates();

            var templateDescriptors = allTemplateDescriptors.Where(d =>
                d.BaseUrl.Equals(baseUrl, StringComparison.InvariantCultureIgnoreCase)).ToList();

            return BuildUrlContext(templateDescriptors, virtualPath);
        }

        public UrlContext GetContext(Dictionary<string, string> segments, string virtualPath)
        {
            var allTemplateDescriptors = _urlTemplateManager.DescribeUrlTemplates();

            var templateDescriptors = allTemplateDescriptors.Where(d =>
                d.Segments.All(s => segments.ContainsKey(s.Key) && segments[s.Key] == s.Value.Value)
                && d.Segments.Count == segments.Count).ToList();

            return BuildUrlContext(templateDescriptors, virtualPath);
        }

        public UrlContext GetContext(Dictionary<string, UrlSegmentValueDescriptor> segments, string virtualPath)
        {
            var allTemplateDescriptors = _urlTemplateManager.DescribeUrlTemplates();

            var templateDescriptors = allTemplateDescriptors.Where(d =>
                d.Segments.All(s => segments.ContainsKey(s.Key) && segments[s.Key].Value == s.Value.Value)
                && d.Segments.Count == segments.Count).ToList();

            return BuildUrlContext(templateDescriptors, virtualPath);
        }

        //public string GetDisplayVirtualPath(UrlContext urlContext, string virtualPath)
        //{
        //    if (urlContext == null
        //        || string.IsNullOrWhiteSpace(urlContext.Descriptor.Template.StoredVirtualPath)
        //        || virtualPath == null)
        //    {
        //        return virtualPath; 
        //    }

        //    // work only for template like this {lang}/{virtualPath}
        //    foreach (var segment in urlContext.Descriptor.Segments)
        //    {
        //        if (urlContext.Descriptor.Template.StoredVirtualPath.StartsWith(
        //            string.Format("{{{0}}}/", segment.Key), StringComparison.InvariantCultureIgnoreCase) &&
        //            virtualPath.StartsWith(segment.Value, StringComparison.InvariantCultureIgnoreCase)
        //            )
        //        {
        //            return virtualPath.Substring(segment.Value.Length).TrimStart('/');
        //        }
        //    }

        //    return virtualPath;
        //}

        //public string GetStoredVirtualPath(UrlContext urlContext, string virtualPath)
        //{
        //    if (urlContext == null
        //        || string.IsNullOrWhiteSpace(urlContext.Descriptor.Template.StoredVirtualPath)
        //        || virtualPath == null)
        //    {
        //        return virtualPath;
        //    }

        //    // work only for template like this {lang}/{virtualPath}
        //    foreach (var segment in urlContext.Descriptor.Segments)
        //    {
        //        if (urlContext.Descriptor.Template.StoredVirtualPath.StartsWith(
        //            string.Format("{{{0}}}/", segment.Key), StringComparison.InvariantCultureIgnoreCase) &&
        //            !virtualPath.StartsWith(segment.Value, StringComparison.InvariantCultureIgnoreCase)
        //            )
        //        {
        //            return segment.Value + (string.IsNullOrEmpty(virtualPath) ? "" : "/" + virtualPath);
        //        }
        //    }

        //    return virtualPath;
        //}

        public UrlContext ChangeSegmentValues(UrlContext urlContext, object segments)
        {
            return ChangeSegmentValues(urlContext, new RouteValueDictionary(segments));
        }

        public UrlContext ChangeSegmentValues(UrlContext urlContext, IDictionary<string, string> segments)
        {
            if (urlContext == null) { return null; }

            var segmentDescriptors = _urlTemplateManager.DescribeUrlSegments();
            var newSegments = urlContext.Descriptor.Segments.ToDictionary(
                entry => entry.Key,
                entry => entry.Value.TypedClone());

            foreach (var changedSegment in segments)
            {
                var sementDescriptor = segmentDescriptors.FirstOrDefault(d => d.Name == changedSegment.Key);
                if (sementDescriptor == null)
                {
                    continue;
                }

                newSegments[changedSegment.Key] =
                    changedSegment.Value == null || changedSegment.Value.ToString() == string.Empty
                    ? sementDescriptor.GetDefaultValue()
                    : sementDescriptor.Values.First(sd => sd.Value == changedSegment.Value);
            }

            return GetContext(newSegments, "");
        }

        public bool IsUrlTemplatesConfigured()
        {
            return _urlTemplateManager.DescribeUrlTemplates().Any();
        }

        private UrlContext BuildUrlContext(List<UrlTemplateDescriptor> descriptors, string virtualPath)
        {
            if (descriptors.Count == 0) { return null; }

            if (descriptors.Count > 1)
            {
                Logger.Debug(string.Format("Duplicate base url template for {0}", descriptors[0].BaseUrl));
                // return null
            }

            var desciptor = descriptors[0];
            var virtualPathSlashEnd = virtualPath + "/";
            if (!string.IsNullOrEmpty(desciptor.StoredPrefix)
                && virtualPathSlashEnd.StartsWith(desciptor.StoredPrefix + "/", StringComparison.InvariantCultureIgnoreCase))
            {
                return new UrlContext()
                {
                    NeedRedirect = true,
                    Descriptor = desciptor,
                    StoredVirtualPath = virtualPath,
                    DisplayVirtualPath = virtualPath.Substring(desciptor.StoredPrefix.Length).TrimStart('/')
                };
            }
            else
            {
                var allDescriptors = _urlTemplateManager.DescribeUrlTemplates();
                var otherTemplateDescriptor = allDescriptors
                    .FirstOrDefault(d => !string.IsNullOrEmpty(d.StoredPrefix)
                        && virtualPathSlashEnd.StartsWith(d.StoredPrefix + "/", StringComparison.InvariantCultureIgnoreCase)
                        && d != desciptor);

                if (otherTemplateDescriptor != null)
                {
                    return new UrlContext()
                    {
                        NeedRedirect = true,
                        Descriptor = otherTemplateDescriptor,
                        StoredVirtualPath = virtualPath,
                        DisplayVirtualPath = virtualPath.Substring(desciptor.StoredPrefix.Length).TrimStart('/')
                    };
                }
            }

            return new UrlContext()
            {
                NeedRedirect = false,
                Descriptor = desciptor,
                StoredVirtualPath = (desciptor.StoredPrefix + "/" + virtualPath).Trim('/'),
                DisplayVirtualPath = virtualPath
            };
        } 
    }
}