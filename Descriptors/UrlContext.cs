using MainBit.Alias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Descriptors
{
    public class UrlContext
    {
        public UrlTemplateDescriptor Descriptor { get; set; }
        public string DisplayVirtualPath { get; set; }
        public string StoredVirtualPath { get; set; }
        public bool NeedRedirect { get; set; }

        public string GetFullDisplayUrl() {
            if(string.IsNullOrEmpty(DisplayVirtualPath)) {
                return Descriptor.BaseUrl;
            }
            else {
                return Descriptor.BaseUrl + "/" + DisplayVirtualPath;
            }
        }
    }

    //public static class UrlContextExtensions
    //{
    //    public static string GetFullDisplayUrl(this UrlContext urlContext)
    //    {
    //        if (string.IsNullOrEmpty(urlContext.DisplayVirtualPath))
    //        {
    //            return urlContext.Descriptor.BaseUrl;
    //        }
    //        else
    //        {
    //            return urlContext.Descriptor.BaseUrl + "/" + urlContext.DisplayVirtualPath;
    //        }
    //    }
    //}
}