using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Nop.Plugin.Payments.YandexMoney;
using Nop.Plugin.Payments.YandexMoney.Models;
using Nop.Core.Domain.Orders;

namespace Grand.Plugin.Payments.YandexMoney.Controllers
{
    public class YandexMoneyPaymentProcessingController : BasePluginController
    {

        private readonly YandexMoneyPaymentSettings _pluginSettings;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;

        public YandexMoneyPaymentProcessingController(YandexMoneyPaymentSettings pluginSettings,
                                                    IOrderProcessingService orderProcessingService,
                                                    IOrderService orderService,
                                                    ILocalizationService localizationService,
                                                    IWebHelper webHelper)
        {
            this._orderProcessingService = orderProcessingService;
            this._pluginSettings = pluginSettings;
            this._orderService = orderService;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
        }

        [HttpPost]
        public IActionResult ChangePaymentStatus(YandexPaymentModel model)
        {
            try
            {
                var order = _orderService.GetOrderById(model.label);
                if (order != null && ValidateSha(model))
                {
                    if (order.OrderTotal == Decimal.Parse(model.withdraw_amount))
                    {
                        if (_orderProcessingService.CanMarkOrderAsPaid(order))
                        {
                            _orderProcessingService.MarkOrderAsPaid(order);
                            UpdateOrderFromTransaction(model, order);
                            return Json(new { status = true, message = "200Ok" });
                        }
                    }
                }

                return Json(new { status = false, message = "Invalid format" });
            }
            catch (Exception ex) {

                return Json(new { status = false, message = ex.Message.ToString()});
            }       
        }

        private bool ValidateSha(YandexPaymentModel model)
        {
            var shaString = model.notification_type + "&" + model.operation_id + "&" + model.amount + "&" + model.currency + "&" + model.datetime + "&";
            shaString += model.sender + "&" + model.codepro + "&" + _pluginSettings.SecretKey + "&" + model.label;

            var sha = string.Join("", (new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(shaString))).Select(x => x.ToString("X2")).ToArray());
            return model.sha1_hash.ToLower() == sha.ToLower();
        }

        private void UpdateOrderFromTransaction(YandexPaymentModel model, Order order)
        {
            order.CardType = "YandexMoney";
            order.CardNumber = model.sender;
            order.CaptureTransactionResult = model.operation_id;

            _orderService.UpdateOrder(order);
        }
    }
}
