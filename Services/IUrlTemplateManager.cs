using MainBit.Alias.Descriptors;
using Orchard;
using Orchard.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Services
{
    public interface IUrlTemplateManager : IDependency {
        List<UrlTemplateDescriptor> DescribeUrlTemplates();
        List<UrlSegmentDescriptor> DescribeUrlSegments();
    }

    public class UrlTemplateManager : IUrlTemplateManager
    {
        private readonly IEnumerable<IUrlSegmentProvider> _urlSegmentProviders;
        private readonly IUrlTemplateService _urlTemplateService;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;

        public UrlTemplateManager(IEnumerable<IUrlSegmentProvider> urlSegmentProviders,
            IUrlTemplateService urlTemplateService,
            ICacheManager cacheManager,
            ISignals signals)
        {
            _urlSegmentProviders = urlSegmentProviders;
            _urlTemplateService = urlTemplateService;
            _cacheManager = cacheManager;
            _signals = signals;
        }

        public static readonly string SignalUrlTemplatesChanged = "MainBit.Alias.UrlTemplates.Changed";

        public List<UrlTemplateDescriptor> DescribeUrlTemplates()
        {
            return _cacheManager.Get("MainBit.Alias.UrlTemplates", acquire =>
            {
                MonitorChanged(acquire);

                var allTemplateDescriptors = new List<UrlTemplateDescriptor>();
                var segmentDescriptors = DescribeUrlSegments();
                var templates = _urlTemplateService.GetList();

                foreach (var template in templates)
                {
                    var definedSegmentDescriptors = segmentDescriptors.Where(d =>
                            template.BaseUrl.Contains(string.Format("{{{0}}}", d.Name))
                            && d.Values.Count > 0);
                    var defaultSegmentDescriptors = segmentDescriptors.Where(d =>
                            !template.BaseUrl.Contains(string.Format("{{{0}}}", d.Name)));

                    var templateDescriptor = new UrlTemplateDescriptor()
                    {
                        Template = template,
                        BaseUrl = template.BaseUrl,
                        StoredPrefix = template.StoredPrefix ?? string.Empty
                    };

                    foreach (var defaultSegmentDescriptor in defaultSegmentDescriptors)
                    {
                        templateDescriptor.BaseUrl.Replace(
                            string.Format("{0}", defaultSegmentDescriptor.Name),
                            defaultSegmentDescriptor.DefaultValue);
                        templateDescriptor.Segments.Add(
                            defaultSegmentDescriptor.Name,
                            new UrlSegmentValueDescriptor {
                                Value = defaultSegmentDescriptor.DefaultValue,
                                StoredValue = defaultSegmentDescriptor.DefaultStoredValue
                            });
                    }

                    var templateDescriptors = new List<UrlTemplateDescriptor>() { templateDescriptor };
                    templateDescriptors = GenerateSegmentsCombinations(templateDescriptors, definedSegmentDescriptors);
                    allTemplateDescriptors.AddRange(templateDescriptors);
                }

                foreach (var templateDescriptor in allTemplateDescriptors)
                {
                    if (templateDescriptor.StoredPrefix == null) {
                        continue;
                    }

                    foreach (var segment in templateDescriptor.Segments)
                    {
                        templateDescriptor.StoredPrefix = templateDescriptor.StoredPrefix.Replace(
                            string.Format("{{{0}}}", segment.Key),
                            segment.Value.StoredValue);
                    }
                }
                return allTemplateDescriptors;
            });
        }

        public List<UrlSegmentDescriptor> DescribeUrlSegments()
        {
            return _cacheManager.Get("MainBit.Alias.UrlSegments", acquire =>
            {
                MonitorChanged(acquire);

                var describeUrlSegmentsContext = new DescribeUrlSegmentsContext();
                foreach (var urlSegmentProvider in _urlSegmentProviders)
                {
                    urlSegmentProvider.Describe(describeUrlSegmentsContext);
                }
                return describeUrlSegmentsContext.Describe();
            });
        }

        private void MonitorChanged(AcquireContext<string> acquire)
        {
            acquire.Monitor(_signals.When(SignalUrlTemplatesChanged));
            foreach (var urlSegmentProvider in _urlSegmentProviders)
            {
                urlSegmentProvider.MonitorChanged(acquire);
            }
        }

        private List<UrlTemplateDescriptor> GenerateSegmentsCombinations(
            List<UrlTemplateDescriptor> urlTemplateDescriptors,
            IEnumerable<UrlSegmentDescriptor> segmentDescriptors)
        {
            var segment = segmentDescriptors.FirstOrDefault();
            if (segment == null) { return urlTemplateDescriptors; }

            var newUrlTemplateDescriptors = new List<UrlTemplateDescriptor>();
            foreach (var segmentValue in segment.Values)
            {
                var clonedUrlTemplateDescriptors = urlTemplateDescriptors.Select(d => (UrlTemplateDescriptor)d.Clone()).ToList();
                foreach (var clonedUrlTemplateDescriptor in clonedUrlTemplateDescriptors)
                {
                    clonedUrlTemplateDescriptor.BaseUrl =
                        clonedUrlTemplateDescriptor.BaseUrl.Replace(string.Format("{{{0}}}", segment.Name), segmentValue.Value);
                    clonedUrlTemplateDescriptor.Segments.Add(segment.Name, segmentValue);
                }
                newUrlTemplateDescriptors.AddRange(clonedUrlTemplateDescriptors);
            }
            return GenerateSegmentsCombinations(newUrlTemplateDescriptors, segmentDescriptors.Skip(1));
        }
    }
}