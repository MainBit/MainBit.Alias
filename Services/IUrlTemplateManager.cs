using MainBit.Alias.Descriptors;
using Orchard;
using Orchard.Caching;
using Orchard.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MainBit.Alias.Services
{
    public interface IUrlTemplateManager : IDependency {
        List<UrlSegmentDescriptor> DescribeUrlSegments();
        List<UrlTemplateDescriptor> DescribeUrlTemplates();
    }

    public class UrlTemplateManager : IUrlTemplateManager
    {
        private readonly IEnumerable<IUrlSegmentProvider> _urlSegmentProviders;
        private readonly IUrlTemplateRepository _urlTemplateService;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly IUrlTemplateHelper _urlTemplateHelper;
        private readonly ITokenizer _tokenizer;

        public UrlTemplateManager(IEnumerable<IUrlSegmentProvider> urlSegmentProviders,
            IUrlTemplateRepository urlTemplateService,
            ICacheManager cacheManager,
            ISignals signals,
            IUrlTemplateHelper urlTemplateHelper,
            ITokenizer tokenizer)
        {
            _urlSegmentProviders = urlSegmentProviders;
            _urlTemplateService = urlTemplateService;
            _cacheManager = cacheManager;
            _signals = signals;
            _urlTemplateHelper = urlTemplateHelper;
            _tokenizer = tokenizer;
        }

        public static readonly string SignalUrlTemplatesChanged = "MainBit.Alias.UrlTemplates.Changed";

        public List<UrlSegmentDescriptor> DescribeUrlSegments()
        {
            return _cacheManager.Get("MainBit.Alias.UrlSegments", acquire =>
            {
                MonitorChanged(acquire);

                var describeUrlSegmentsContext = new DescribeUrlSegmentContext();
                foreach (var urlSegmentProvider in _urlSegmentProviders)
                {
                    urlSegmentProvider.Describe(describeUrlSegmentsContext);
                }
                return describeUrlSegmentsContext.Describe();
            });
        }

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
                    var templateDescriptor = new UrlTemplateDescriptor()
                    {
                        BaseUrl = template.BaseUrl,
                        StoredPrefix = template.StoredPrefix ?? string.Empty,
                    };

                    // default values
                    var defaultSegmentDescriptors = segmentDescriptors.Where(d =>
                            !template.BaseUrl.Contains(string.Format("{{{0}}}", d.Name)) && d.DefaultValue != null);

                    foreach (var defaultSegmentDescriptor in defaultSegmentDescriptors)
                    {
                        templateDescriptor.Segments.Add(
                            defaultSegmentDescriptor.Name,
                            defaultSegmentDescriptor.DefaultValue);
                    }

                    // defined values
                    var definedSegmentDescriptors = segmentDescriptors.Where(d =>
                            template.BaseUrl.Contains(string.Format("{{{0}}}", d.Name))
                            && d.Values.Count > 0);

                    var templateDescriptors = GenerateSegmentsCombinations(
                        _urlTemplateHelper.ParseContraints(template.Constraints).ToDictionary(d => d.Key, d => new Regex(d.Value)),
                        new List<UrlTemplateDescriptor>() { templateDescriptor },
                        definedSegmentDescriptors,
                        template.IncludeDefaultValues);
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
                            segment.Value.StoredPrefix);
                    }
                }

                foreach(var templateDescriptor in allTemplateDescriptors)
                {
                    templateDescriptor.BaseUrl = _tokenizer.Replace(templateDescriptor.BaseUrl, null);
                }

                return allTemplateDescriptors;
            });
        }

        private List<UrlTemplateDescriptor> GenerateSegmentsCombinations(
            IDictionary<string, Regex> contraints,
            List<UrlTemplateDescriptor> urlTemplateDescriptors,
            IEnumerable<UrlSegmentDescriptor> segmentDescriptors,
            bool includeDefaultValues)
        {
            var segment = segmentDescriptors.FirstOrDefault();
            if (segment == null) { return urlTemplateDescriptors; }

            var newUrlTemplateDescriptors = new List<UrlTemplateDescriptor>();

            foreach (var segmentValue in segment.Values)
            {
                if(!includeDefaultValues && segmentValue == segment.DefaultValue)
                    continue;

                if (contraints.ContainsKey(segment.Name))
                    if (!contraints[segment.Name].IsMatch(segmentValue.Name))
                        continue;

                var clonedUrlTemplateDescriptors = urlTemplateDescriptors.Select(d => (UrlTemplateDescriptor)d.Clone()).ToList();
                foreach (var clonedUrlTemplateDescriptor in clonedUrlTemplateDescriptors)
                {
                    clonedUrlTemplateDescriptor.BaseUrl =
                        clonedUrlTemplateDescriptor.BaseUrl.Replace(string.Format("{{{0}}}", segment.Name), segmentValue.Value);
                    clonedUrlTemplateDescriptor.Segments.Add(segment.Name, segmentValue);
                }
                newUrlTemplateDescriptors.AddRange(clonedUrlTemplateDescriptors);
            }
            return GenerateSegmentsCombinations(contraints, newUrlTemplateDescriptors, segmentDescriptors.Skip(1), includeDefaultValues);
        }

        private void MonitorChanged(AcquireContext<string> acquire)
        {
            acquire.Monitor(_signals.When(SignalUrlTemplatesChanged));
            foreach (var urlSegmentProvider in _urlSegmentProviders)
            {
                urlSegmentProvider.MonitorChanged(acquire);
            }
        }
    }
}