using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.GazpromBank
{
    public partial class RouteProvider : IRouteProvider 
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapLocalizedRoute("Plugin.Payments.GazpromBank.Return",
               "Plugins/GazpromBankPaymentProcessing/Return",
               new { controller = "GazpromBankPaymentProcessing", action = "Return" },
               new[] { "Nop.Plugin.Payments.GazpromBank.Controllers" }
          );

            routeBuilder.MapLocalizedRoute("Plugin.Payments.GazpromBank.Fail",
                 "Plugins/GazpromBankPaymentProcessing/Fail",
                 new { controller = "GazpromBankPaymentProcessing", action = "Fail" },
                 new[] { "Nop.Plugin.Payments.GazpromBank.Controllers" }
            );

            routeBuilder.MapLocalizedRoute("Plugin.Payments.GazpromBank.CheckPaymentAvail",
                "Plugins/GazpromBankPaymentProcessing/CheckPaymentAvail",
                new { controller = "GazpromBankPaymentProcessing", action = "CheckPaymentAvail" },
                new[] { "Nop.Plugin.Payments.GazpromBank.Controllers" }
           );

            routeBuilder.MapLocalizedRoute("Plugin.Payments.GazpromBank.PaymentRegisterResponse",
                "Plugins/GazpromBankPaymentProcessing/PaymentRegisterResponse",
                new { controller = "GazpromBankPaymentProcessing", action = "PaymentRegisterResponse" },
                new[] { "Nop.Plugin.Payments.GazpromBank.Controllers" }
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
