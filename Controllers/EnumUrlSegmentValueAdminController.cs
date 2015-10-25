using MainBit.Alias.Helpers;
using MainBit.Alias.Models;
using MainBit.Alias.Services;
using MainBit.Alias.ViewModels;
using Orchard;
using Orchard.Caching;
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
    public class EnumUrlSegmentValueAdminController : Controller
    {
        private readonly IEnumUrlSegmentRepository _enumUrlSegmentRepository;
        private readonly IEnumUrlSegmentValueRepository _enumUrlSegmentValueRepository;
        private readonly IOrchardServices _orchardServices;

        public EnumUrlSegmentValueAdminController(
            IEnumUrlSegmentRepository enumUrlSegmentRepository,
            IEnumUrlSegmentValueRepository enumUrlSegmentValueRepository,
            IOrchardServices orchardServices)
        {
            _enumUrlSegmentRepository = enumUrlSegmentRepository;
            _enumUrlSegmentValueRepository = enumUrlSegmentValueRepository;
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index(int segmentId)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage enum url segments")))
                return new HttpUnauthorizedResult();

            var segment = _enumUrlSegmentRepository.Get(segmentId);
            return View(segment);
        }

        public ActionResult Add(int segmentId)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage enum url segments")))
                return new HttpUnauthorizedResult();

            var viewModel = new EnumUrlSegmentValueRecord() {
                EnumUrlSegmentRecord_Id = segmentId
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(int segmentId, EnumUrlSegmentValueRecord viewModel)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage enum url segments")))
                return new HttpUnauthorizedResult();

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var segment = _enumUrlSegmentRepository.Get(segmentId);
            viewModel.EnumUrlSegmentRecord_Id = segment.Id;
            segment.SegmentValues.Add(viewModel);

            return RedirectToAction("Index", new { segmentId = segmentId });
        }

        public ActionResult Edit(int id)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage enum url segments")))
                return new HttpUnauthorizedResult();

            var viewModel = _enumUrlSegmentValueRepository.Get(id);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(EnumUrlSegmentValueRecord viewModel)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage enum url segments")))
                return new HttpUnauthorizedResult();

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            _enumUrlSegmentValueRepository.Update(viewModel);

            return RedirectToAction("Index", new { segmentId = viewModel.EnumUrlSegmentRecord_Id });
        }

        public ActionResult Delete(int id)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage enum url segments")))
                return new HttpUnauthorizedResult();

            var viewModel = _enumUrlSegmentValueRepository.Get(id);
            _enumUrlSegmentValueRepository.Delete(viewModel);
            return RedirectToAction("Index", new { segmentId = viewModel.EnumUrlSegmentRecord_Id });
        }
    }
}