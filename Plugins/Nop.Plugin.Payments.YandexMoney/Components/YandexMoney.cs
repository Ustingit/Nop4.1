using Nop.Plugin.Payments.YandexMoney.Models;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Plugin.Payments.YandexMoney.Components
{
    [ViewComponent(Name = "YandexMoney")]
    public class YandexMoneyViewComponent : ViewComponent
    {
        private readonly YandexMoneyPaymentSettings _pluginSettings;
        public YandexMoneyViewComponent(YandexMoneyPaymentSettings pluginSettings)
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
