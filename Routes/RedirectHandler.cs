using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Routes
{
    // http://stackoverflow.com/questions/11518935/mvc-redirect-to-default-route
    public class RedirectHandler : IHttpHandler
    {
        private readonly string _redirectUrl;

        public RedirectHandler(string redirectUrl, bool isReusable)
        {
            _redirectUrl = redirectUrl;
            IsReusable = isReusable;
        }

        public bool IsReusable { get; private set; }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Status = "301 Moved Permanently";
            context.Response.StatusCode = 301;
            context.Response.AddHeader("Location", _redirectUrl);
        }
    }
}