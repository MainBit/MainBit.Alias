using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace MainBit.Alias {
    public class AdminMenu : INavigationProvider {
        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder
                .Add(T("Settings"), menu => menu
                    .Add(T("URL templates"), "2", item => Describe(item, "Index", "UrlTemplateAdmin", false)
                        .Add(T("URL templates"), "2", subItem => Describe(subItem, "Index", "UrlTemplateAdmin", true))
                        .Add(T("Enumeration URL segments"), "3", subItem => Describe(subItem, "Index", "EnumUrlSegmentAdmin", true))
                    )
                );
        }

        static NavigationItemBuilder Describe(NavigationItemBuilder item, string actionName, string controllerName, bool localNav)
        {
            item = item.Action(actionName, controllerName, new { area = "MainBit.Alias" }).Permission(StandardPermissions.SiteOwner);
            if (localNav)
                item = item.LocalNav();
            return item;
        }
    }
}