using Nop.Plugin.Payments.GazpromBank.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.GazpromBank;

namespace Nop.Plugin.Payments.YandexMoney.Components
{
    [ViewComponent(Name = "GazpromBank")]
    public class GazpromBankViewComponent : ViewComponent
    {
        private readonly GazpromBankPaymentSettings _pluginSettings;
        public GazpromBankViewComponent(GazpromBankPaymentSettings pluginSettings)
        {      
            this._pluginSettings = pluginSettings;
        }
        public IViewComponentResult Invoke()
        {
            var model = new ConfigurationModel()
            {
                DescriptionText = _pluginSettings.DescriptionText
            };

            return View("/Plugins/Payments.YandexMoney/Views/PaymentInfo.cshtml", model);
        }
    }
}
