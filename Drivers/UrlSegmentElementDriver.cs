using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Forms.Services;
using Orchard.Layouts.Elements;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Drivers;
using Orchard.UI.Navigation;
using Orchard.Layouts.Helpers;
using DescribeContext = Orchard.Forms.Services.DescribeContext;
using MainBit.Alias.Descriptors;
using MainBit.Alias.Elements;
using MainBit.Alias.Services;
using MainBit.Utility.Services;
using MainBit.Alias.ViewModels;

namespace Orchard.Layouts.Drivers {
    public class UrlSegmentElementDriver : FormsElementDriver<UrlSegment> {
        
        private readonly IUrlTemplateManager _urlTemplateManager;
        private readonly ICurrentContentAccessor _currentContentAccessor;
        private readonly IUrlService _urlService;

        public UrlSegmentElementDriver(
            IUrlTemplateManager urlTemplateManager,
            IFormManager formManager,
            ICurrentContentAccessor currentContentAccessor,
            IUrlService urlService)
            : base(formManager) {

            _urlTemplateManager = urlTemplateManager;
            _currentContentAccessor = currentContentAccessor;
            _urlService = urlService;
        }

        protected override IEnumerable<string> FormNames {
            get {
                yield return "UrlSegmentForm";
            }
        }

        protected override void OnDisplaying(UrlSegment element, ElementDisplayingContext context)
        {
            context.ElementShape.Metadata.Alternates.Add(String.Format("Elements_{0}_SegmentName__{1}", element.GetType().Name, element.SegmentName.ToLower()));

            var segment = _urlTemplateManager.DescribeUrlSegments().FirstOrDefault(s => s.Name == element.SegmentName);
            var currentContent = _currentContentAccessor.CurrentContentItem; // context.Content - display in
            var currentUrlContext = _urlService.GetCurrentContext();

            var viewModel = new UrlSegmentViewModel() {
                Descriptor = segment
            };

            foreach(var segmentValue in segment.Values) {
                if (segmentValue.Name == currentUrlContext.Descriptor.Segments[segment.Name].Name)
                {
                    viewModel.CurrentValue = new UrlSegmentValueDescriptorEntry {
                        Descriptor = segmentValue,
                        Url = currentUrlContext.GetFullDisplayUrl()
                    };
                    viewModel.Values.Add(viewModel.CurrentValue);
                }
                else
                {
                    var newUrlContext = _urlService.ChangeSegmentValues(
                        currentUrlContext,
                        new Dictionary<string, string> { { segment.Name, segmentValue.Value } },
                        currentContent);
                    viewModel.Values.Add(new UrlSegmentValueDescriptorEntry()
                    {
                        Descriptor = segmentValue,
                        Url = newUrlContext.GetFullDisplayUrl()
                    });
                }
            }

            context.ElementShape.ViewModel = viewModel;
        }

        protected override void DescribeForm(DescribeContext context) {
            context.Form("UrlSegmentForm", factory =>
            {
                var shape = (dynamic)factory;
                var form = shape.Fieldset(
                    Id: "UrlSegmentForm",
                    _SegmentName: shape.SelectList(
                        Id: "SegmentName",
                        Name: "SegmentName",
                        Title: T("For segment"),
                        Description: T("The segment to display."))
                    );

                // Populate the list of segments.
                var segments = _urlTemplateManager.DescribeUrlSegments();
                foreach (var segment in segments.OrderBy(s => s.Name)) {
                    form._SegmentName.Add(new SelectListItem { Text = segment.DisplayName, Value = segment.Name });
                }

                return form;
            });
        }

        protected override void OnExporting(UrlSegment element, ExportElementContext context)
        {
            context.ExportableData["segmentName"] = element.SegmentName;
        }

        protected override void OnImporting(UrlSegment element, ImportElementContext context)
        {
            element.SegmentName = context.ExportableData.Get("SegmentName"); // XmlHelper.Parse<string>(context.ExportableData.Get("SegmentName"))
        }
    }
}