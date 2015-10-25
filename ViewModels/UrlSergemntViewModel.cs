using MainBit.Alias.Descriptors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MainBit.Alias.ViewModels
{
    public class UrlSegmentViewModel
    {
        public UrlSegmentViewModel()
        {
            Values = new List<UrlSegmentValueDescriptorEntry>();
        }
        public UrlSegmentDescriptor Descriptor { get; set; }
        public UrlSegmentValueDescriptorEntry CurrentValue { get; set; }
        public List<UrlSegmentValueDescriptorEntry> Values { get; set; }
    }

    public class UrlSegmentValueDescriptorEntry
    {
        public UrlSegmentValueDescriptor Descriptor { get; set; }
        public string Url { get; set; }
    }
}