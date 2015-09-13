using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Services
{
    public interface IBaseUrlManager : IDependency {
        BaseUrlContext GetBaseUrlContext(string baseUrl);
        string GetDisplayVirtualPath(BaseUrlContext baseUrlContext, string virtualPath);

    }

    public class BaseUrlManager : IBaseUrlManager
    {
        private readonly IEnumerable<IBaseUrlSegmentProvider> _baseUrlSegmentProviders;
        private readonly IBaseUrlTemplateService _baseUrlTemplateService;

        public BaseUrlManager(IEnumerable<IBaseUrlSegmentProvider> baseUrlSegmentProviders,
            IBaseUrlTemplateService baseUrlTemplateService)
        {
            _baseUrlSegmentProviders = baseUrlSegmentProviders;
            _baseUrlTemplateService = baseUrlTemplateService;
        }

        public BaseUrlContext GetBaseUrlContext(string baseUrl)
        {
            var baseUrlDescriptors = GetBaseUrlDescriptors();
            var baseUrlDescriptor = baseUrlDescriptors.FirstOrDefault(d => d.BaseUrl.Equals(baseUrl, StringComparison.InvariantCultureIgnoreCase));
            if(baseUrlDescriptor == null) { return null; }

            var baseUrlContext = new BaseUrlContext()
            {
                Descriptor = baseUrlDescriptor
            };

            return baseUrlContext;
        }

        private List<BaseUrlDescriptor> GetBaseUrlDescriptors()
        {
            var descriptors = new List<BaseUrlDescriptor>();
            var segmentsDescriptors = GetBaseUrlSegmentsDescriptors();
            var baseUrlTemplates = _baseUrlTemplateService.GetList();
            
            foreach (var baseUrlTemplate in baseUrlTemplates)
            {
                var definedSegments = segmentsDescriptors.Where(d => 
                        baseUrlTemplate.BaseUrlTemplate.Contains(string.Format("{0}", d.Name))
                        && d.Values.Count > 0);
                var defaultSegments = segmentsDescriptors.Where(d => 
                        !baseUrlTemplate.BaseUrlTemplate.Contains(string.Format("{0}", d.Name))
                        && !string.IsNullOrWhiteSpace(d.DefaultValue));

                var baseUrlDescriptor = new BaseUrlDescriptor()
                {
                    Template = baseUrlTemplate,
                    BaseUrl = baseUrlTemplate.BaseUrlTemplate
                };
                foreach (var segment in defaultSegments)
                {
                    baseUrlDescriptor.BaseUrl.Replace(string.Format("{0}", segment.Name), segment.DefaultValue);
                    baseUrlDescriptor.Segments.Add(segment.Name, segment.DefaultValue);
                }
                var baseUrlDescriptors = new List<BaseUrlDescriptor>() { baseUrlDescriptor };
                GenerateSegmentsCombinations(baseUrlDescriptors, definedSegments);

                descriptors.AddRange(baseUrlDescriptors);
            }
            return descriptors;
        }

        private List<BaseUrlSegmentDescriptor> GetBaseUrlSegmentsDescriptors()
        {
            return _baseUrlSegmentProviders.Select(p => new BaseUrlSegmentDescriptor() {
                Name = p.Name,
                DefaultValue = p.GetDefaultValue(),
                Values = p.GetValues()
            }).ToList();
        }

        private void GenerateSegmentsCombinations(
            List<BaseUrlDescriptor> baseUrlDescriptors,
            IEnumerable<BaseUrlSegmentDescriptor> segmentDescriptors)
        {
            var segment = segmentDescriptors.FirstOrDefault();
            if (segment == null) { return; }

            var newBaseUrlDescriptors = new List<BaseUrlDescriptor>();
            foreach (var segmentValue in segment.Values)
            {
                var newValueBaseUrlDescriptors = baseUrlDescriptors.Select(d => (BaseUrlDescriptor)d.Clone());
                foreach (var newValueBaseUrlDescriptor in newValueBaseUrlDescriptors)
                {
                    newValueBaseUrlDescriptor.BaseUrl.Replace(string.Format("{0}", segment.Name), segmentValue);
                    newValueBaseUrlDescriptor.Segments.Add(segment.Name, segmentValue);
                }
                newBaseUrlDescriptors.AddRange(newValueBaseUrlDescriptors);
            }
            GenerateSegmentsCombinations(baseUrlDescriptors, segmentDescriptors.Skip(1));
        }

        public string GetDisplayVirtualPath(BaseUrlContext baseUrlContext, string virtualPath)
        {
            // work only for template like this {lang}/{virtualPath}
            foreach (var segment in baseUrlContext.Descriptor.Segments)
            {
                if (baseUrlContext.StoredVirtualPath.StartsWith(string.Format("{0}", segment.Key) + "/", StringComparison.InvariantCultureIgnoreCase) &&
                    virtualPath.StartsWith(segment.Value, StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    return virtualPath.Substring(segment.Value.Length + 1);
                }
            }

            return virtualPath;
        }
    }

}