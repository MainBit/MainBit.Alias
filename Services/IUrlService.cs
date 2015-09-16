using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using MainBit.Alias.Helpers;

namespace MainBit.Alias.Services
{
    public interface IUrlService : IDependency
    {
        UrlContext GetContext(HttpRequestBase request);
        UrlContext GetContext(string baseUrl, string virtualPath = null);
        UrlContext GetContext(Dictionary<string, string> segments, string virtualPath = null);

        UrlContext ChangeSegmentValues(UrlContext urlContext, object segments);
        UrlContext ChangeSegmentValues(UrlContext urlContext, RouteValueDictionary segments);

        string GetDisplayVirtualPath(UrlContext urlContext, string virtualPath);
        string GetStoredVirtualPath(UrlContext urlContext, string virtualPath);
    }
    public class UrlService : IUrlService
    {
        private readonly IUrlTemplateManager _urlTemplateManager;

        public UrlService(IUrlTemplateManager urlTemplateManager)
        {
            _urlTemplateManager = urlTemplateManager;
        }

        public UrlContext GetContext(HttpRequestBase request)
        {
            var baseUrl = request.GetBaseUrl();
            var virtualPath = request.AppRelativeCurrentExecutionFilePath.Substring(2) + request.PathInfo;

            return GetContext(baseUrl, virtualPath);
        }

        public UrlContext GetContext(string baseUrl, string virtualPath = null)
        {
            var templateDescriptors = _urlTemplateManager.DescribeUrlTemplates();
            var templateDescriptor = templateDescriptors.FirstOrDefault(d =>
                d.BaseUrl.Equals(baseUrl, StringComparison.InvariantCultureIgnoreCase));

            return BuildUrlContext(templateDescriptor, virtualPath);
        }

        public UrlContext GetContext(Dictionary<string, string> segments, string virtualPath = null)
        {
            var templateDescriptors = _urlTemplateManager.DescribeUrlTemplates();
            var templateDescriptor = templateDescriptors.FirstOrDefault(d =>
                d.Segments.All(s => segments.ContainsKey(s.Key) && segments[s.Key] == s.Value)
                && d.Segments.Count == segments.Count);

            return BuildUrlContext(templateDescriptor, virtualPath);
        }

        public string GetDisplayVirtualPath(UrlContext urlContext, string virtualPath)
        {
            if (urlContext == null
                || string.IsNullOrWhiteSpace(urlContext.Descriptor.Template.StoredVirtualPath)
                || virtualPath == null)
            {
                return virtualPath; 
            }

            // work only for template like this {lang}/{virtualPath}
            foreach (var segment in urlContext.Descriptor.Segments)
            {
                if (urlContext.Descriptor.Template.StoredVirtualPath.StartsWith(
                    string.Format("{{{0}}}/", segment.Key), StringComparison.InvariantCultureIgnoreCase) &&
                    virtualPath.StartsWith(segment.Value, StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    return virtualPath.Substring(segment.Value.Length).TrimStart('/');
                }
            }

            return virtualPath;
        }

        public string GetStoredVirtualPath(UrlContext urlContext, string virtualPath)
        {
            if (urlContext == null
                || string.IsNullOrWhiteSpace(urlContext.Descriptor.Template.StoredVirtualPath)
                || virtualPath == null)
            {
                return virtualPath;
            }

            // work only for template like this {lang}/{virtualPath}
            foreach (var segment in urlContext.Descriptor.Segments)
            {
                if (urlContext.Descriptor.Template.StoredVirtualPath.StartsWith(
                    string.Format("{{{0}}}/", segment.Key), StringComparison.InvariantCultureIgnoreCase) &&
                    !virtualPath.StartsWith(segment.Value, StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    return segment.Value + (string.IsNullOrEmpty(virtualPath) ? "" : "/" + virtualPath);
                }
            }

            return virtualPath;
        }

        public UrlContext ChangeSegmentValues(UrlContext urlContext, object segments)
        {
            return ChangeSegmentValues(urlContext, new RouteValueDictionary(segments));
        }

        public UrlContext ChangeSegmentValues(UrlContext urlContext, RouteValueDictionary segments)
        {
            if (urlContext == null) { return null; }

            var segmentDescriptors = _urlTemplateManager.DescribeUrlSegments();
            var newSegments = new Dictionary<string, string>(urlContext.Descriptor.Segments);

            foreach (var changedSegment in segments)
            {
                var sementDescriptor = segmentDescriptors.FirstOrDefault(d => d.Name == changedSegment.Key);
                if (sementDescriptor == null)
                {
                    continue;
                }

                newSegments[changedSegment.Key] =
                    changedSegment.Value == null || changedSegment.Value.ToString() == string.Empty 
                    ? sementDescriptor.DefaultValue
                    : changedSegment.Value.ToString();
            }

            return GetContext(newSegments);
        }

        private UrlContext BuildUrlContext(UrlTemplateDescriptor descriptor, string virtualPath)
        {
            if (descriptor == null) { return null; }

            var urlContext = new UrlContext()
            {
                Descriptor = descriptor
            };
            urlContext.DisplayVirtualPath = GetDisplayVirtualPath(urlContext, virtualPath);
            urlContext.StoredVirtualPath = GetStoredVirtualPath(urlContext, virtualPath);

            return urlContext;
        } 
    }
}