using MainBit.Alias.Services;
using Orchard;
using Orchard.Alias.Implementation.Holder;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.Logging;
using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MainBit.Alias.Routes
{
    public class UrlTemplateRoutesProvider : IRouteProvider
    {
        private readonly ShellBlueprint _blueprint;
        private readonly IAliasHolder _aliasHolder;
        private readonly IWorkContextAccessor _wca;
        private readonly IUrlTemplateManager _baseUrlManager;

        public UrlTemplateRoutesProvider(IWorkContextAccessor wca,
            IUrlTemplateManager baseUrlManager,
            ShellBlueprint blueprint,
            IAliasHolder aliasHolder)
        {
            _wca = wca;
            _baseUrlManager = baseUrlManager;
            _blueprint = blueprint;
            _aliasHolder = aliasHolder;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (RouteDescriptor routeDescriptor in GetRoutes()) {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            var distinctAreaNames = _blueprint.Controllers
                .Select(controllerBlueprint => controllerBlueprint.AreaName)
                .Distinct();

            return distinctAreaNames.Select(areaName =>
                new RouteDescriptor {
                    Priority = 100,
                    Route = new UrlTemplateRoutes(_wca, Logger, _aliasHolder, areaName, new MvcRouteHandler())
                }).ToList();
        }
    }
}