using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias
{
    public class UrlSegmentDescriptor
    {
        public string Name { get; set; }
        public List<string> Values { get; set; }
        public string DefaultValue { get; set; }
    }
}