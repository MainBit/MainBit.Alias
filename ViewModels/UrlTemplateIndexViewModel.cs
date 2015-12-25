using MainBit.Alias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.ViewModels
{
    public class UrlTemplateIndexViewModel
    {
        public List<UrlTemplateRecord> Templates { get; set; }
        public List<UrlTemplateDescriptor> Descriptors { get; set; }
    }
}