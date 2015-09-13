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
    [Admin]
    public class BaseUrlAdminController : Controller
    {
        private readonly IBaseUrlTemplateService _baseUrlTemplateService;
        private readonly IOrchardServices _orchardServices;

        public BaseUrlAdminController(
            IBaseUrlTemplateService baseUrlTemplateService,
            IOrchardServices orchardServices)
        {
            _baseUrlTemplateService = baseUrlTemplateService;
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index()
        {
            var viewModel = new IndexViewModel()
            {
                Templates = _baseUrlTemplateService.GetList()
            };

            return View(viewModel);
        }

        public ActionResult Add()
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage base urls")))
                return new HttpUnauthorizedResult();

            var viewModel = new BaseUrlTemplateRecord();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(BaseUrlTemplateRecord viewModel)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage base urls")))
                return new HttpUnauthorizedResult();

            viewModel.BaseUrlTemplate = viewModel.BaseUrlTemplate.TrimSafe();
            viewModel.StoredVirtualPathTemplate = viewModel.StoredVirtualPathTemplate.TrimSafe();

            if (String.IsNullOrWhiteSpace(viewModel.BaseUrlTemplate))
            {
                ModelState.AddModelError("BaseUrlTemplate", T("Base url template can't be empty").Text);
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            _baseUrlTemplateService.Create(viewModel);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage base urls")))
                return new HttpUnauthorizedResult();

            var viewModel = _baseUrlTemplateService.Get(id);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(BaseUrlTemplateRecord viewModel)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage base urls")))
                return new HttpUnauthorizedResult();

            viewModel.BaseUrlTemplate = viewModel.BaseUrlTemplate.TrimSafe();
            viewModel.StoredVirtualPathTemplate = viewModel.StoredVirtualPathTemplate.TrimSafe();

            if (String.IsNullOrWhiteSpace(viewModel.BaseUrlTemplate))
            {
                ModelState.AddModelError("BaseUrlTemplate", T("Base url template can't be empty").Text);
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            _baseUrlTemplateService.Update(viewModel);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage base urls")))
                return new HttpUnauthorizedResult();

            var viewModel = new BaseUrlTemplateRecord();
            return View(viewModel);
        }
    }
}