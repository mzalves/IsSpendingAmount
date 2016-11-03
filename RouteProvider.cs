using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.DiscountRules.IsSpendingAmount
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.DiscountRules.IsSpendingAmount.Configure",
                 "Plugins/DiscountRulesIsSpendingAmount/Configure",
                 new { controller = "DiscountRulesIsSpendingAmount", action = "Configure" },
                 new[] { "Nop.Plugin.DiscountRules.IsSpendingAmount.Controllers" }
            );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
