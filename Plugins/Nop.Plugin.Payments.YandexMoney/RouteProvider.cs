using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.YandexMoney
{
    public partial class RouteProvider : IRouteProvider 
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapLocalizedRoute("Plugin.Payments.YandexMoney.PaymentStatus",
                 "Payments/YandexMoney/PaymentStatus",
                 new { controller = "YandexMoneyPaymentProcessing", action = "ChangePaymentStatus" }
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
