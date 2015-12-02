using MainBit.Alias.Helpers;
using MainBit.Alias.Models;
using MainBit.Alias.Services;
using MainBit.Alias.ViewModels;
using Orchard;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MainBit.Alias.Controllers
{
    [Admin]
    public class EnumUrlSegmentAdminController : Controller, IUpdateModel
    {
        private readonly IEnumUrlSegmentRepository _enumUrlSegmentService;
        private readonly IOrchardServices _orchardServices;

        public EnumUrlSegmentAdminController(
            IEnumUrlSegmentRepository enumUrlSegmentService,
            IOrchardServices orchardServices)
        {
            _enumUrlSegmentService = enumUrlSegmentService;
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index()
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage enum url segments")))
                return new HttpUnauthorizedResult();

            var viewModel = new EnumUrlSegmentIndexViewModel()
            {
                Templates = _enumUrlSegmentService.GetList()
            };

            return View(viewModel);
        }

        public ActionResult Add()
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage enum url segments")))
                return new HttpUnauthorizedResult();

            var viewModel = new EnumUrlSegmentRecord();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(EnumUrlSegmentRecord viewModel)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage enum url segments")))
                return new HttpUnauthorizedResult();

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            _enumUrlSegmentService.Create(viewModel);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage enum url segments")))
                return new HttpUnauthorizedResult();

            var viewModel = _enumUrlSegmentService.Get(id);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(EnumUrlSegmentRecord viewModel)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage enum url segments")))
                return new HttpUnauthorizedResult();

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var model = _enumUrlSegmentService.Get(viewModel.Id);
            if (TryUpdateModel(model))
            {
                _enumUrlSegmentService.Update(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(viewModel);
            }
        }

        public ActionResult Delete(int id)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage enum url segments")))
                return new HttpUnauthorizedResult();

            var viewModel = _enumUrlSegmentService.Get(id);
            _enumUrlSegmentService.Delete(viewModel);
            return RedirectToAction("Index");
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.Text);
        }
    }
}