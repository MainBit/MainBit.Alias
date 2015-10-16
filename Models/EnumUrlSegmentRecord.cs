using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Models
{
    public class EnumUrlSegmentRecord
    {
        public virtual int Id { get; set; }
        public virtual int Position { get; set; }
        public virtual string Name { get; set; }
        public virtual string PossibleValues { get; set; } // The Values name doesn't work for nhibernate
        public virtual string PossibleStoredValues { get; set; }
        public virtual string DefaultValue { get; set; }
        public virtual string DefaultStoredValue { get; set; }
    }
}