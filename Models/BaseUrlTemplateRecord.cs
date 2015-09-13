using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Models
{
    public class BaseUrlTemplateRecord
    {
        public virtual int Id { get; set; }
        public virtual int Position { get; set; }
        public virtual string BaseUrlTemplate { get; set; }
        public virtual string StoredVirtualPathTemplate { get; set; }
    }
}