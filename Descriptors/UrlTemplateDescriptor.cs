using MainBit.Alias.Descriptors;
using MainBit.Alias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias
{
    public class UrlTemplateDescriptor : ICloneable
    {
        public UrlTemplateDescriptor()
        {
            Segments = new Dictionary<string, UrlSegmentValueDescriptor>();
        }

        public UrlTemplateRecord Template { get; set; }
        public Dictionary<string, UrlSegmentValueDescriptor> Segments { get; set; }
        public string BaseUrl { get; set; }
        public string StoredPrefix { get; set; }

        public UrlTemplateDescriptor TypedClone()
        {
            return Clone() as UrlTemplateDescriptor;
        }
        public object Clone()
        {
            return CloneImpl();
        }
        protected virtual UrlTemplateDescriptor CloneImpl()
        {
            var copy = (UrlTemplateDescriptor)MemberwiseClone();
            copy.Segments = Segments.ToDictionary(
                entry => entry.Key,
                entry => entry.Value.TypedClone()
            );
            return copy;
        }
    }
}