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
        private readonly IEnumUrlSegmentService _enumUrlSegmentService;

        public EnumUrlSegmentProvider(IEnumUrlSegmentService enumUrlSegmentService)
        {
            _enumUrlSegmentService = enumUrlSegmentService;
        }

        public void Describe(Descriptors.DescribeUrlSegmentsContext context)
        {
            var enumUrlSegments = _enumUrlSegmentService.GetList();
            foreach (var enumUrlSegment in enumUrlSegments)
            {
                context.Element(
                    enumUrlSegment.Name,
                    enumUrlSegment.PossibleValues.Split(','),
                    enumUrlSegment.DefaultValue);
            }
        }

        public void MonitorChanged(AcquireContext<string> acquire)
        {
            // this uses the default signal - UrlTemplateManager.SignalUrlTemplatesChanged
        }
    }
}