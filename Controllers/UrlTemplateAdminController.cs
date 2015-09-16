using MainBit.Alias.Models;
using MainBit.Alias.Services;
using MainBit.Alias.ViewModels;
using Orchard;
using Orchard.Caching;
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
    [Admin]
    public class UrlTemplateAdminController : Controller
    {
        private readonly IUrlTemplateService _urlTemplateService;
        private readonly IOrchardServices _orchardServices;
        private readonly ISignals _signals;

        public UrlTemplateAdminController(
            IUrlTemplateService urlTemplateService,
            IOrchardServices orchardServices,
            ISignals signals)
        {
            _urlTemplateService = urlTemplateService;
            _orchardServices = orchardServices;
            _signals = signals;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index()
        {
            var viewModel = new UrlTemplateIndexViewModel()
            {
                Templates = _urlTemplateService.GetList()
            };

            return View(viewModel);
        }

        public ActionResult Add()
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage url templates")))
                return new HttpUnauthorizedResult();

            var viewModel = new UrlTemplateRecord();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(UrlTemplateRecord viewModel)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage url templates")))
                return new HttpUnauthorizedResult();

            viewModel.BaseUrl = viewModel.BaseUrl.TrimSafe();
            viewModel.StoredVirtualPath = viewModel.StoredVirtualPath.TrimSafe();

            if (String.IsNullOrWhiteSpace(viewModel.BaseUrl))
            {
                ModelState.AddModelError("BaseUrl", T("Base url can't be empty").Text);
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            _urlTemplateService.Create(viewModel);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage url templates")))
                return new HttpUnauthorizedResult();

            var viewModel = _urlTemplateService.Get(id);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(UrlTemplateRecord viewModel)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage url templates")))
                return new HttpUnauthorizedResult();

            viewModel.BaseUrl = viewModel.BaseUrl.TrimSafe();
            viewModel.StoredVirtualPath = viewModel.StoredVirtualPath.TrimSafe();

            if (String.IsNullOrWhiteSpace(viewModel.BaseUrl))
            {
                ModelState.AddModelError("BaseUrl", T("Base url can't be empty").Text);
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            _urlTemplateService.Update(viewModel);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage url templates")))
                return new HttpUnauthorizedResult();

            var viewModel = _urlTemplateService.Get(id);
            _urlTemplateService.Delete(viewModel);
            return RedirectToAction("Index");
        }

        public ActionResult ClearСache()
        {
            _signals.Trigger(UrlTemplateManager.SignalUrlTemplatesChanged);
            return RedirectToAction("Index");
        }
    }
}