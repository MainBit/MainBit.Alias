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
using Orchard.Environment.Configuration;
using Orchard.Mvc.Routes;
using MainBit.Alias.Events;
using Orchard.ContentManagement;

namespace MainBit.Alias.Services
{
    public interface IUrlService : IDependency
    {
        UrlContext GetCurrentContext();
        UrlContext GetContext(string baseUrl, string virtualPath);
        //UrlContext GetContext(Dictionary<string, string> segments, string virtualPath = "");
        UrlContext ChangeSegmentValues(UrlContext urlContext, object segments, IContent content = null);
        UrlContext ChangeSegmentValues(UrlContext urlContext, IDictionary<string, string> segments, IContent content = null);


        bool IsUrlTemplatesConfigured();
    }
    public class UrlService : IUrlService
    {
        private readonly IUrlContextEventHandler _urlContextEventHandler;
        private readonly IUrlTemplateManager _urlTemplateManager;
        private readonly IHttpContextAccessor _hta;
        private readonly ShellSettings _shellSettings;
        private readonly IWorkContextAccessor _wca;
        private readonly IUrlUtils _urlUtils;
        

        public UrlService(
            IUrlContextEventHandler urlContextEventHandler,
            IUrlTemplateManager urlTemplateManager,
            IHttpContextAccessor hta,
            ShellSettings shellSettings,
            IWorkContextAccessor wca,
            IUrlUtils urlUtils)
        {
            _urlContextEventHandler = urlContextEventHandler;
            _urlTemplateManager = urlTemplateManager;
            _hta = hta;
            _wca = wca;
            _shellSettings = shellSettings;
            _urlUtils = urlUtils;
            Logger = NullLogger.Instance;

        }

        public ILogger Logger { get; set; }

        private UrlContext _currentUrlContext = null;
        public UrlContext GetCurrentContext()
        {
            if (_currentUrlContext == null)
            {
                _currentUrlContext = GetContext(
                    _urlUtils.GetBaseUrl(),
                    _urlUtils.GetVirtualPath());
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

        public UrlContext GetContext(Dictionary<string, UrlSegmentValueDescriptor> segments, string virtualPath)
        {
            var allTemplateDescriptors = _urlTemplateManager.DescribeUrlTemplates();

            var templateDescriptors = allTemplateDescriptors.Where(d =>
                d.Segments.All(s => segments.ContainsKey(s.Key) && segments[s.Key].Value == s.Value.Value)
                && d.Segments.Count == segments.Count).ToList();

            return BuildUrlContext(templateDescriptors, virtualPath);
        }

        public UrlContext ChangeSegmentValues(UrlContext urlContext, object segments, IContent content = null)
        {
            return ChangeSegmentValues(urlContext, new RouteValueDictionary(segments));
        }

        public UrlContext ChangeSegmentValues(UrlContext urlContext, IDictionary<string, string> segments, IContent content = null)
        {
            if (urlContext == null) return null;

            var segmentDescriptors = _urlTemplateManager.DescribeUrlSegments();
            var newSegments = urlContext.Descriptor.Segments.ToDictionary(
                entry => entry.Key,
                entry => entry.Value.TypedClone());

            var changeUrlContext = new ChangingUrlContext() {
                UrlContext = urlContext,
                Content = content
            };

            foreach (var changedSegment in segments)
            {
                var segmentDescriptor = segmentDescriptors.FirstOrDefault(d => d.Name == changedSegment.Key);
                if (segmentDescriptor == null)
                {
                    continue;
                }

                var currentValue = newSegments[changedSegment.Key];
                var newValue = string.IsNullOrEmpty(changedSegment.Value)
                    ? segmentDescriptor.DefaultValue
                    : segmentDescriptor.Values.First(sd => sd.Value == changedSegment.Value);

                newSegments[changedSegment.Key] = newValue;

                changeUrlContext.ChangingSegments.Add(new ChangingUrlSegmentContext()
                {
                    UrlSegmentDescriptor = segmentDescriptor,
                    CurrentValue = currentValue,
                    NewValue = newValue
                });
            }

            _urlContextEventHandler.Changing(changeUrlContext);

            return GetContext(newSegments, changeUrlContext.NewDisplayVirtualPath);
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