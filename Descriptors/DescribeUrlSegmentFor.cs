using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Descriptors
{
    public class DescribeUrlSegmentFor
    {
        public DescribeUrlSegmentFor(string displayName, int position)
        {
            DisplayName = displayName;
            Position = position;
            SegmentValues = new List<UrlSegmentValueDescriptor>();
        }

        public string DisplayName { get; private set; }
        public int Position { get; set; }
        public List<UrlSegmentValueDescriptor> SegmentValues { get; private set; }
        public UrlSegmentValueDescriptor DefaultSegmentValue { get; private set; }

        public DescribeUrlSegmentFor Value(string name, string displayName, int position, string value, string storedValue, bool isDefault = false)
        {
            var segmentValue = new UrlSegmentValueDescriptor()
            {
                Name = name,
                DisplayName = displayName,
                Position = position,
                Value = value,
                StoredPrefix = storedValue
            };

            SegmentValues.Add(segmentValue);

            if (isDefault)
                DefaultSegmentValue = segmentValue;

            return this;
        }
    }
}