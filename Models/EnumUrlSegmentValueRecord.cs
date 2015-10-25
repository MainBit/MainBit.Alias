using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Models
{
    public class EnumUrlSegmentValueRecord
    {
        public virtual int Id { get; set; }
        public virtual int EnumUrlSegmentRecord_Id { get; set; }
        public virtual int Position { get; set; }
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string UrlSegment { get; set; }
        public virtual string StoredPrefix { get; set; }
        public virtual bool IsDefault { get; set; }
    }
}