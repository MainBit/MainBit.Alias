using MainBit.Alias.Descriptors;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Events
{
    public class ChangingUrlContext
    {
        public ChangingUrlContext()
        {
            ChangingSegments = new List<ChangingUrlSegmentContext>();
        }

        public UrlContext UrlContext { get; set; }
        public List<ChangingUrlSegmentContext> ChangingSegments { get; set; }
        public IContent Content { get; set; }
        public string NewDisplayVirtualPath { get; set; }
    }

    public class ChangingUrlSegmentContext
    {
        public UrlSegmentDescriptor UrlSegmentDescriptor { get; set; }
        public UrlSegmentValueDescriptor CurrentValue { get; set; }
        public UrlSegmentValueDescriptor NewValue { get; set; }
    }
}