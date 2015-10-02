using MainBit.Alias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias
{
    public class UrlContext
    {
        public UrlTemplateDescriptor Descriptor { get; set; }
        public string DisplayVirtualPath { get; set; }
        public string StoredVirtualPath { get; set; }
        public bool NeedRedirect { get; set; }
    }

    public static class UrlContextExtensions
    {
        public static string GetFullDisplayUrl(this UrlContext urlContext)
        {
            if(string.IsNullOrEmpty(urlContext.DisplayVirtualPath)) {
                return urlContext.Descriptor.BaseUrl;
            }
            else {
                return urlContext.Descriptor.BaseUrl + "/" + urlContext.DisplayVirtualPath;
            }
        }
    }
}