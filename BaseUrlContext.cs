﻿using MainBit.Alias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias
{
    public class BaseUrlContext
    {
        public BaseUrlDescriptor Descriptor { get; set; }
        public string DisplayVirtualPath { get; set; }
        public string StoredVirtualPath { get; set; }
    }
}