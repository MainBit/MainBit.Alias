using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Descriptors
{
    public class DescribeUrlSegmentContext
    {
        private readonly IDictionary<string, DescribeUrlSegmentFor> _describes = new Dictionary<string, DescribeUrlSegmentFor>();

        public List<UrlSegmentDescriptor> Describe()
        {
            return _describes.Select(d => new UrlSegmentDescriptor() {
                Name = d.Key,
                DisplayName = d.Value.DisplayName,
                Position = d.Value.Position,
                Values = d.Value.SegmentValues.OrderBy(v => v.Position).ToList(),
                DefaultValue = d.Value.DefaultSegmentValue,
            })
            .OrderBy(s => s.Position)
            .ToList();
        }

        public DescribeUrlSegmentFor For(string name, string displayName, int position)
        {
            DescribeUrlSegmentFor describeFor;
            if (!_describes.TryGetValue(name, out describeFor))
            {
                describeFor = new DescribeUrlSegmentFor(displayName, position);
                _describes[name] = describeFor;
            }
            return describeFor;
        }
    }
}