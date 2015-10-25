using MainBit.Alias.Descriptors;
using Orchard;
using Orchard.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias
{
    public interface IUrlSegmentProvider : IDependency
    {
        void Describe(DescribeUrlSegmentContext context);
        void MonitorChanged(AcquireContext<string> acquire);
    }
}