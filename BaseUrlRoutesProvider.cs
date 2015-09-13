﻿using MainBit.Alias.Services;
using Orchard;
using Orchard.Alias.Implementation.Holder;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MainBit.Alias
{
    public class BaseUrlRoutesProvider : IRouteProvider
    {
        private readonly ShellBlueprint _blueprint;
        private readonly IAliasHolder _aliasHolder;
        private readonly IWorkContextAccessor _wca;
        private readonly IBaseUrlManager _baseUrlManager;

        public BaseUrlRoutesProvider(IWorkContextAccessor wca,
            IBaseUrlManager baseUrlManager,
            ShellBlueprint blueprint,
            IAliasHolder aliasHolder)
        {
            _wca = wca;
            _baseUrlManager = baseUrlManager;
            _blueprint = blueprint;
            _aliasHolder = aliasHolder;
        }

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
                    Route = new BaseUrlRoutes(_wca, _aliasHolder, areaName, new MvcRouteHandler())
                }).ToList();
        }
    }
}