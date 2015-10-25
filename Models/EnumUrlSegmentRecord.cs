using Orchard.Data.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Models
{
    public class EnumUrlSegmentRecord
    {
        public EnumUrlSegmentRecord()
        {
            SegmentValues = new List<EnumUrlSegmentValueRecord>();
        }

        public virtual int Id { get; set; }
        public virtual int Position { get; set; }
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }


        [CascadeAllDeleteOrphan, Aggregate]
        public virtual IList<EnumUrlSegmentValueRecord> SegmentValues { get; set; }
    }
}