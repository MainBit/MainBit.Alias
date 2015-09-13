using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Helpers
{
    public static class BaseUrlExtensions
    {
        public static string GetBaseUrl(this HttpRequestBase request)
        {
            return (request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath).TrimEnd('/');
        }
    }
}