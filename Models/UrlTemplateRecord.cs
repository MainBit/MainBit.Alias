using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Models
{
    public class UrlTemplateRecord
    {
        public virtual int Id { get; set; }
        public virtual int Position { get; set; }
        public virtual string BaseUrl { get; set; }
        public virtual string StoredPrefix { get; set; }
        public virtual string Constraints { get; set; }
        public virtual bool IncludeDefaultValues { get; set; }
    }
}