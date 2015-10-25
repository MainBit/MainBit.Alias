using MainBit.Alias.Models;
using MainBit.Alias.Services;
using MainBit.Alias.ViewModels;
using Orchard;
using Orchard.Localization;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MainBit.Alias.Helpers;
using Orchard.Themes;
using Orchard.UI.Admin;

namespace MainBit.Alias.Controllers
{
    [Themed]
    public class UrlTemplateController : Controller
    {
        private readonly IUrlTemplateRepository _baseUrlTemplateService;
        private readonly IOrchardServices _orchardServices;

        public UrlTemplateController(
            IUrlTemplateRepository baseUrlTemplateService,
            IOrchardServices orchardServices)
        {
            _baseUrlTemplateService = baseUrlTemplateService;
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        // base url invalid
        public ActionResult Invalid()
        {
            Response.StatusCode = 404;

            var viewModel = new UrlTemplateIndexViewModel()
            {
                Templates = _baseUrlTemplateService.GetList()
            };

            return View(viewModel);
        }
    }
}