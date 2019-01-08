using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Plugins;
using Nop.Services.Payments;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Directory;
using Nop.Services.Orders;
using Nop.Services.Localization;
using Nop.Services.Common;
using Nop.Services.Tax;
using Nop.Services.Configuration;
using Nop.Services.Catalog;
using Nop.Core;
using Nop.Services.Directory;
using Nop.Plugin.Payments.YandexMoney.Controllers;

namespace Nop.Plugin.Payments.YandexMoney
{
    /// <summary>
    /// PayPalStandard payment processor
    /// </summary>
    public class YandexMoneyPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ISettingService _settingService;
        private readonly ITaxService _taxService;
        private readonly IProductService _productService;
        private readonly IWebHelper _webHelper;
        private readonly YandexMoneyPaymentSettings _paymentSettings;

        #endregion

        #region Ctor

        public YandexMoneyPaymentProcessor(CurrencySettings currencySettings,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ISettingService settingService,
            ITaxService taxService,
            IProductService productService,
            IWebHelper webHelper,
            YandexMoneyPaymentSettings paymentSettings)
        {
            this._currencySettings = currencySettings;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._currencyService = currencyService;
            this._genericAttributeService = genericAttributeService;
            this._httpContextAccessor = httpContextAccessor;
            this._localizationService = localizationService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._settingService = settingService;
            this._taxService = taxService;
            this._productService = productService;
            this._webHelper = webHelper;
            this._paymentSettings = paymentSettings;
        }

        #endregion
                 
        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult();
        }

        public string BuildTargets(Order order)
        {
            var result = new StringBuilder();
            result.AppendFormat(_localizationService.GetResource("Plugins.Payments.YandexMoney.PaymentForm.Targets"), order.CustomOrderNumber, order.CreatedOnUtc.ToShortDateString());
            return result.ToString();
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var queryParameters = new Dictionary<string, string>();
            AddOrderTotalParameters(queryParameters, postProcessPaymentRequest);

            var url = QueryHelpers.AddQueryString("https://money.yandex.ru/transfer", queryParameters);
            _httpContextAccessor.HttpContext.Response.Redirect(url);
        }

        private void AddOrderTotalParameters(IDictionary<string, string> parameters, PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var roundedOrderTotal = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);
            var orderInfo = BuildTargets(postProcessPaymentRequest.Order);
            parameters.Add("receiver", _paymentSettings.WalletNumber);
            parameters.Add("successURL", _webHelper.GetStoreLocation() + "checkout/completed/");
            parameters.Add("quickpay-back-url", _webHelper.GetStoreLocation() + "checkout/completed/");
            parameters.Add("shop-host", _webHelper.GetStoreLocation());
            parameters.Add("label", postProcessPaymentRequest.Order.Id.ToString());
            parameters.Add("targets", orderInfo);
            parameters.Add("comment", "");
            parameters.Add("origin", "form");
            parameters.Add("selectedPaymentType", "AC");
            parameters.Add("destination", orderInfo);
            parameters.Add("form-comment", orderInfo);
            parameters.Add("short-dest", "");
            parameters.Add("transferDirection", "c2p");
            parameters.Add("sum", roundedOrderTotal.ToString());
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return decimal.Zero;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            return new CapturePaymentResult { Errors = new[] { "Capture method not supported" } };
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            return new RefundPaymentResult { Errors = new[] { "Refund method not supported" } };
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            return new VoidPaymentResult { Errors = new[] { "Void method not supported" } };
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } };
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } };
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return false;

            return true;
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            return new List<string>();
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/YandexMoney/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return "YandexMoney";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {

            var settings = new YandexMoneyPaymentSettings()
            {
                DescriptionText = "<p>Yandex money payments - from card to yandex money wallet.</p><br><p>Warning! Check wallet payment limit!</p>"
            };
            _settingService.SaveSetting(settings);
            
            //locales
            this._localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexMoney.DescriptionText", "Description");
            this._localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexMoney.DescriptionText.Hint", "Insert wallet description.");
            this._localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexMoney.Fields.WalletNumber", "Wallet");
            this._localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexMoney.Fields.WalletNumber.Hint", "Insert wallet number.");
            this._localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexMoney.Fields.SecretKey", "Secret key");
            this._localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexMoney.Fields.SecretKey.Hint", "Insert secret key.");
            this._localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexMoney.Fields.ApiKey", "Api key");
            this._localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexMoney.Fields.ApiKey.Hint", "Insert api key.");
            this._localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexMoney.PaymentForm.Targets", "Payment for order with number {0} от {1}");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<YandexMoneyPaymentSettings>();

            //locales
            this._localizationService.DeletePluginLocaleResource("Plugins.Payments.YandexMoney.DescriptionText");
            this._localizationService.DeletePluginLocaleResource("Plugins.Payments.YandexMoney.DescriptionText.Hint");
            this._localizationService.DeletePluginLocaleResource("Plugins.Payments.YandexMoney.Fields.WalletNumber");
            this._localizationService.DeletePluginLocaleResource("Plugins.Payments.YandexMoney.Fields.WalletNumber.Hint");
            this._localizationService.DeletePluginLocaleResource("Plugins.Payments.YandexMoney.Fields.SecretKey");
            this._localizationService.DeletePluginLocaleResource("Plugins.Payments.YandexMoney.Fields.SecretKey.Hint");
            this._localizationService.DeletePluginLocaleResource("Plugins.Payments.YandexMoney.Fields.ApiKey");
            this._localizationService.DeletePluginLocaleResource("Plugins.Payments.YandexMoney.Fields.ApiKey.Hint");
            this._localizationService.DeletePluginLocaleResource("Plugins.Payments.YandexMoney.PaymentForm.Targets");

            base.Uninstall();
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "YandexMoney";
        }

        public Type GetControllerType()
        {
            return typeof(YandexMoneyController);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get { return RecurringPaymentType.NotSupported; }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get { return PaymentMethodType.Redirection; }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public string PaymentMethodDescription
        {
            get { return String.Empty; }
        }

        #endregion
    }
}