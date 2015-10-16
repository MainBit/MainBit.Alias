using MainBit.Alias.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias
{
    public class UrlSegmentDescriptor
    {
        public string Name { get; set; }
        public List<UrlSegmentValueDescriptor> Values { get; set; }
        public string DefaultValue { get; set; }
        public string DefaultStoredValue { get; set; }
    }

    public static class UrlSegmentDescriptorExtensions {
        public static UrlSegmentValueDescriptor GetDefaultValue(this UrlSegmentDescriptor urlSegmentDescriptor)
        {
            return new UrlSegmentValueDescriptor()
            {
                Value = urlSegmentDescriptor.DefaultValue,
                StoredValue = urlSegmentDescriptor.DefaultStoredValue
            };
        }
    }
}