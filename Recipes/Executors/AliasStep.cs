using System;
using System.Collections.Generic;
using Orchard.Data;
using Orchard.Layouts.Models;
using Orchard.Logging;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;
using Orchard.Layouts.Services;
using Orchard.Layouts.Helpers;
using Orchard.Layouts.Framework.Drivers;
using System.Linq;
using Orchard.Layouts.Framework.Elements;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using MainBit.Alias.Models;
using MainBit.Alias.Services;

namespace MainBit.Alias.Recipes.Executors {
    public class AliasStep : RecipeExecutionStep {
        private readonly IEnumUrlSegmentRepository _segmentsRepository;
        private readonly IUrlTemplateRepository _templatesRepository;

        public AliasStep(
            IEnumUrlSegmentRepository segmentsRepository,
            IUrlTemplateRepository templatesRepository,
            RecipeExecutionLogger logger) : base(logger) {

            _segmentsRepository = segmentsRepository;
            _templatesRepository = templatesRepository;
        }

        public override string Name {
            get { return "MainBitAlias"; }
        }

        public override void Execute(RecipeExecutionContext context) {


            foreach (var xmlElement in context.RecipeStep.Step.Elements())
            {
                if(xmlElement.Name.LocalName == "Segments") {
                    foreach(var segmentElement in xmlElement.Elements()) {
                        var name = segmentElement.Attribute("Name").Value;
                        Logger.Information("Importing custom url segment '{0}'.", name);

                        try
                        {
                            var segment = GetOrCreateUrlSegment(name);
                            var valuesElement = segmentElement.Element("Values");
                            segment.DisplayName = segmentElement.Attribute("DisplayName").Value;
                            segment.Position = int.Parse(segmentElement.Attribute("Position").Value);

                            foreach (var valueElement in valuesElement.Elements())
                            {
                                var valueName = valueElement.Attribute("Name").Value;
                                var value = GetOrCreateUrlSegmentValue(segment, valueName);
                                value.DisplayName = valueElement.Attribute("DisplayName").Value;
                                value.Position = int.Parse(segmentElement.Attribute("Position").Value);
                                value.IsDefault = bool.Parse(valueElement.Attribute("IsDefault").Value);
                                value.StoredPrefix = valueElement.Attribute("StoredPrefix").Value;
                                value.UrlSegment = valueElement.Attribute("UrlSegment").Value;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "Error while importing custom url segment '{0}'.", name);
                            throw;
                        }
                    }
                }
                else if(xmlElement.Name.LocalName == "Templates") {
                    foreach (var templateElement in xmlElement.Elements())
                    {
                        var baseUrl = templateElement.Attribute("BaseUrl").Value;
                        Logger.Information("Importing url template '{0}'.", baseUrl);

                        try
                        {
                            var template = GetOrCreateUrlTemplate(baseUrl);
                            template.Position = int.Parse(templateElement.Attribute("Position").Value);
                            template.StoredPrefix = templateElement.Attribute("StoredPrefix").Value;
                            template.Constraints = templateElement.Attribute("Constraints").Value;
                            template.IncludeDefaultValues = bool.Parse(templateElement.Attribute("IncludeDefaultValues").Value);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "Error while importing url template '{0}'.", baseUrl);
                            throw;
                        }
                    }
                }
            }
        }

        private EnumUrlSegmentRecord GetOrCreateUrlSegment(string name) {
            var segment = _segmentsRepository.Get(name);

            if (segment == null) {
                segment = new EnumUrlSegmentRecord {
                    Name = name
                };
                _segmentsRepository.Create(segment);
            }

            return segment;
        }

        private EnumUrlSegmentValueRecord GetOrCreateUrlSegmentValue(EnumUrlSegmentRecord segment, string name)
        {
            var value = segment.SegmentValues.FirstOrDefault(v => v.Name == name);

            if (value == null)
            {
                value = new EnumUrlSegmentValueRecord
                {
                    Name = name
                };
                segment.SegmentValues.Add(value);
            }

            return value;
        }

        private UrlTemplateRecord GetOrCreateUrlTemplate(string baseUrl)
        {
            var template = _templatesRepository.Get(baseUrl);

            if (template == null)
            {
                template = new UrlTemplateRecord
                {
                    BaseUrl = baseUrl
                };
                _templatesRepository.Create(template);
            }

            return template;
        }
    }
}
