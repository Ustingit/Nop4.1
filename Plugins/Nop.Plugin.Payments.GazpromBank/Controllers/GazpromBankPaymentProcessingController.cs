using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Nop.Plugin.Payments.GazpromBank;
using Nop.Plugin.Payments.GazpromBank.Models;
using Nop.Core.Domain.Orders;
using Nop.Services.Payments;
using Nop.Core.Domain.Logging;
using Nop.Services.Logging;
using Nop.Plugin.Payments.GazpromBank.Core;
using Nop.Core.Domain.Payments;

namespace Grand.Plugin.Payments.GazpromBank.Controllers
{
    public class GazpromBankPaymentProcessingController : BasePluginController
    {

        private readonly GazpromBankPaymentSettings _pluginSettings;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        private readonly ILogger _logger;

        public GazpromBankPaymentProcessingController(GazpromBankPaymentSettings pluginSettings,
                                                    IPaymentService paymentService,
                                                    IOrderProcessingService orderProcessingService,
                                                    IOrderService orderService,
                                                    ILocalizationService localizationService,
                                                    IWorkContext workContext,
                                                    IWebHelper webHelper,
                                                    ILogger logger)
        {
            this._orderProcessingService = orderProcessingService;
            this._paymentService = paymentService;
            this._pluginSettings = pluginSettings;
            this._orderService = orderService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._webHelper = webHelper;
            this._logger = logger;
        }
        public ActionResult Fail()
        {
            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.GazpromBank") as GazpromBankPaymentProcessor;

            if (processor == null
                || !processor.PluginDescriptor.Installed)
                throw new NopException("GazpromBank module cannot be loaded");

            if (_pluginSettings.EnableDebugMode)
            {
                var shortString = "Газмпромбанк режим отладки: Ошибка оплаты возврат";
                var fullString = new StringBuilder();
                fullString.AppendFormat("Header : {0};", Request.Headers.ToString());
                fullString.AppendFormat("URL : {0};", Request.Host.ToString());
                fullString.AppendFormat("Body : {0},", Request.QueryString.ToString());
                _logger.InsertLog(LogLevel.Debug, shortString, fullString.ToString());
            }

            var order = _orderService.GetOrderByCustomOrderNumber(_webHelper.QueryString<string>("o.order_id"));
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new UnauthorizedResult();

            return RedirectToRoute("OrderDetails", new { orderId = order.Id });
        }

        public ActionResult Return()
        {
            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.GazpromBank") as GazpromBankPaymentProcessor;

            if (processor == null
                || !processor.PluginDescriptor.Installed)
                throw new NopException("GazpromBank module cannot be loaded");

            if (_pluginSettings.EnableDebugMode)
            {
                var shortString = "GazpromBank: Debug Information";
                var fullString = new StringBuilder();
                fullString.AppendFormat("Header : {0};", Request.Headers.ToString());
                fullString.AppendFormat("URL : {0};", Request.Host.ToString());
                fullString.AppendFormat("Body : {0},", Request.QueryString.ToString());
                _logger.InsertLog(LogLevel.Debug, shortString, fullString.ToString());
            }

            var order = _orderService.GetOrderByCustomOrderNumber(_webHelper.QueryString<string>("o.order_id"));
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new UnauthorizedResult();

            return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
        }

        [HttpGet]
        public ActionResult PaymentRegisterResponse()
        {
            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.GazpromBank") as GazpromBankPaymentProcessor;

            if (processor == null
               || !processor.PluginDescriptor.Installed)
                throw new NopException("GazpromBank module cannot be loaded");

            Response.ContentType = "application/xml; encoding='utf-8'";

            if (_pluginSettings.EnableDebugMode)
            {
                var shortString = "GazpromBank module cannot be loaded";
                var fullString = new StringBuilder();
                fullString.AppendFormat("Header : {0};", Request.Headers.ToString());
                fullString.AppendFormat("URL : {0};", Request.Host.ToString());
                fullString.AppendFormat("Body : {0},", Request.QueryString.ToString());
                _logger.InsertLog(LogLevel.Debug, shortString, fullString.ToString());
            }

            var order = _orderService.GetOrderByCustomOrderNumber(_webHelper.QueryString<string>("o.order_id"));
            var merch_id = _webHelper.QueryString<string>("merch_id").ToLower();
            var trx_id = _webHelper.QueryString<string>("trx_id");
            var resultCode = _webHelper.QueryString<int>("result_code");

            var responseBody = "";

            if (order == null || order.Deleted || merch_id != _pluginSettings.MerchantId.ToLower()
                || order.PaymentStatus == PaymentStatus.Paid)
            {
                responseBody = GazpromBankPaymentCore.PaymentAvailResponseFalse();
                Response.Body.Write(Encoding.UTF8.GetBytes(responseBody), 0, responseBody.Length);
                return new UnauthorizedResult();

            }
            else
            {
                var orderAmount = _webHelper.QueryString<decimal>("amount") / 100;
                var cardholder = _webHelper.QueryString<string>("p.cardholder");
                var cardnum = _webHelper.QueryString<string>("p.maskedPan");
                var accountid = _webHelper.QueryString<string>("account_id");
                var paymentDateTime = DateTime.Now;

                if (resultCode == 1)
                {
                    if (_orderProcessingService.CanMarkOrderAsPaid(order))
                    {
                        responseBody = GazpromBankPaymentCore.PaymentRegisterResponseSuccess();
                        Response.Body.Write(Encoding.UTF8.GetBytes(responseBody), 0, responseBody.Length);
                        _orderProcessingService.MarkOrderAsPaid(order);
                        return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                    }
                }

                responseBody = GazpromBankPaymentCore.PaymentRegisterResponseFail();
                Response.Body.Write(Encoding.UTF8.GetBytes(responseBody), 0, responseBody.Length);

                return Content("");
            }
        }


        [HttpGet]
        public ActionResult CheckPaymentAvail()
        {
            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.GazpromBank") as GazpromBankPaymentProcessor;

            if (processor == null
               || !processor.PluginDescriptor.Installed)
                throw new NopException("Gazprombank module cannot be loaded");

            Response.ContentType = "application/xml; encoding='utf-8'";

            if (_pluginSettings.EnableDebugMode)
            {
                var shortString = "GazpromBank: Debug Information";
                var fullString = new StringBuilder();
                fullString.AppendFormat("Header : {0};", Request.Headers.ToString());
                fullString.AppendFormat("URL : {0};", Request.Host.ToString());
                fullString.AppendFormat("Body : {0},", Request.QueryString.ToString());
                _logger.InsertLog(LogLevel.Debug, shortString, fullString.ToString());
            }

            var order = _orderService.GetOrderByCustomOrderNumber(_webHelper.QueryString<string>("o.order_id"));
            var merch_id = _webHelper.QueryString<string>("merch_id").ToLower();
            var trx_id = _webHelper.QueryString<string>("trx_id");
            var responseBody = "";
            if (order == null || order.Deleted
                              || merch_id != _pluginSettings.MerchantId.ToLower())
            {
                responseBody = GazpromBankPaymentCore.PaymentAvailResponseFalse();
            }
            else {
                responseBody = GazpromBankPaymentCore.PaymentAvailResponseSuccess(order, _pluginSettings);
            }

            Response.Body.Write(Encoding.UTF8.GetBytes(responseBody), 0, responseBody.Length);

            return Content("");
        }


    }
}
