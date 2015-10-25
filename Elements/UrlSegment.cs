using Orchard.Environment.Extensions;
using Orchard.Layouts.Framework.Elements;
using Orchard.Layouts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Elements
{
    public class UrlSegment : Element {

        public override string Category {
            get { return "MainBit"; }
        }

        public string SegmentName
        {
            get { return ElementDataHelper.Retrieve(this, x => x.SegmentName); }
            set { this.Store(x => x.SegmentName, value); }
        }
    }
}