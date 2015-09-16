using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Helpers
{
    public static class HttpRequestExtensions
    {
        public static string GetBaseUrl(this HttpRequestBase request)
        {
            return (request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath).TrimEnd('/');
        }
        public static string GetBaseUrl(this HttpRequest request)
        {
            return (request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath).TrimEnd('/');
        }
    }
}