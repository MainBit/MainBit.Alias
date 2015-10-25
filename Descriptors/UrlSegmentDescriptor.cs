using MainBit.Alias.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Descriptors
{
    public class UrlSegmentDescriptor
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<UrlSegmentValueDescriptor> Values { get; set; }
        public UrlSegmentValueDescriptor DefaultValue { get; set; }
    }
}