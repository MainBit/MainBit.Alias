using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Descriptors
{
    public class UrlSegmentValueDescriptor : ICloneable
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public string Value { get; set; }
        public string StoredValue { get; set; }

        public UrlSegmentValueDescriptor TypedClone()
        {
            return Clone() as UrlSegmentValueDescriptor;
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public static class UrlSegmentValueDescriptorHelper
    {
        public static List<UrlSegmentValueDescriptor> CreateList(string[] values, string[] storedValues) {

            var urlSegmentValueDescriptors = new List<UrlSegmentValueDescriptor>();

            for (var i = 0; i < values.Length; i++)
            {
                urlSegmentValueDescriptors.Add(
                    new UrlSegmentValueDescriptor()
                    {
                        Value = values[i],
                        StoredValue = storedValues.Length > i ? storedValues[i] : values[i]
                    }
                );
            }

            return urlSegmentValueDescriptors;
        }
    }
}