using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias
{
    public interface IBaseUrlSegmentProvider
    {
        string Name { get; }
        List<string> GetValues();
        string GetDefaultValue();
    }
}