using MainBit.Alias.Descriptors;
using MainBit.Alias.Services;
using Orchard.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Providers
{
    public class EnumUrlSegmentProvider : IUrlSegmentProvider
    {
        private readonly IEnumUrlSegmentRepository _enumUrlSegmentService;

        public EnumUrlSegmentProvider(IEnumUrlSegmentRepository enumUrlSegmentService)
        {
            _enumUrlSegmentService = enumUrlSegmentService;
        }

        public void Describe(Descriptors.DescribeUrlSegmentContext context)
        {
            
            foreach(var enumUrlSegment in _enumUrlSegmentService.GetList().Where(s => s.SegmentValues.Any())) {

                var describeFor = context.For(enumUrlSegment.Name, enumUrlSegment.DisplayName, enumUrlSegment.Position);
                foreach (var segmentValue in enumUrlSegment.SegmentValues)
                {
                    describeFor.Value(
                        segmentValue.Name,
                        segmentValue.DisplayName,
                        segmentValue.Position,
                        segmentValue.UrlSegment,
                        segmentValue.StoredPrefix,
                        segmentValue.IsDefault
                    );
                }
            }
        }

        public void MonitorChanged(AcquireContext<string> acquire)
        {
            // this uses the default signal - UrlTemplateManager.SignalUrlTemplatesChanged
        }
    }
}