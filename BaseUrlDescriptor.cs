using MainBit.Alias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias
{
    public class BaseUrlDescriptor : ICloneable
    {
        public BaseUrlTemplateRecord Template { get; set; }
        public Dictionary<string, string> Segments { get; set; }
        public string BaseUrl { get; set; }

        
        public object Clone()
        {
            return CloneImpl();
        }
        protected virtual BaseUrlDescriptor CloneImpl()
        {
            var copy = (BaseUrlDescriptor)MemberwiseClone();
            copy.Segments = Segments.ToDictionary(entry => entry.Key, entry => entry.Value);
            return copy;
        }
    }
}