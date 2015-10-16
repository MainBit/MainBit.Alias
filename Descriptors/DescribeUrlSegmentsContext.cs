using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Descriptors
{
    public class DescribeUrlSegmentsContext
    {
        private readonly List<UrlSegmentDescriptor> _segments = new List<UrlSegmentDescriptor>();

        public List<UrlSegmentDescriptor> Describe()
        {
            return _segments;
        }

        public DescribeUrlSegmentsContext Element(string name, IEnumerable<UrlSegmentValueDescriptor> values, string defaultValue, string defaultStoredValue)
        {
            _segments.Add(new UrlSegmentDescriptor() {
                Name = name,
                Values = values.ToList(),
                DefaultValue = defaultValue,
                DefaultStoredValue = defaultStoredValue
            });
            return this;
        }
    }
}