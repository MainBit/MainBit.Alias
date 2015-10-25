using MainBit.Alias.Models;
using Orchard;
using Orchard.Caching;
using Orchard.Data;
using Orchard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Alias.Services
{
    public interface IUrlTemplateHelper : IDependency
    {
        IDictionary<string, string> ParseContraints(string contraints);
    }

    public class UrlTemplateHelper : IUrlTemplateHelper
    {
        private readonly IJsonConverter _jsonConverter;

        public UrlTemplateHelper(IJsonConverter jsonConverter)
        {
            _jsonConverter = jsonConverter;
        }

        public IDictionary<string, string> ParseContraints(string contraints)
        {
            if (string.IsNullOrEmpty(contraints))
            {
                return new Dictionary<string, string>();
            }

            return _jsonConverter.Deserialize<Dictionary<string, string>>(contraints);
        }

    }
}