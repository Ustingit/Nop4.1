using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.GazpromBank.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.GazpromBank.Controllers
{
    public class GazpromBankController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly PaymentSettings _paymentSettings;
        private readonly GazpromBankPaymentSettings _pluginSettings;

        public GazpromBankController(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            IPaymentService paymentService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            ILogger logger,
            IWebHelper webHelper,
            PaymentSettings paymentSettings,
            GazpromBankPaymentSettings pluginSettings)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._localizationService = localizationService;
            this._storeContext = storeContext;
            this._logger = logger;
            this._webHelper = webHelper;
            this._paymentSettings = paymentSettings;
            this._pluginSettings = pluginSettings;
        }

        [AuthorizeAdmin]
        [Area("Admin")]
        public IActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var pluginSettings = _settingService.LoadSetting<GazpromBankPaymentSettings>(storeScope);

            var model = new ConfigurationModel();
            model.AccountId = pluginSettings.AccountId;
            model.AdditionalFee = pluginSettings.AdditionalFee;
            model.DescriptionText = pluginSettings.DescriptionText;
            model.EnableDebugMode = pluginSettings.EnableDebugMode;
            model.GatewayUrl = pluginSettings.GatewayUrl;
            model.MerchantId = pluginSettings.MerchantId;
            model.TestMode = pluginSettings.TestMode;
                        
            return View("~/Plugins/Payments.GazpromBank/Views/GazpromBank/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area("Admin")]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var plugintSettings = _settingService.LoadSetting<GazpromBankPaymentSettings>(storeScope);

            //save settings
            plugintSettings.AccountId = model.AccountId;
            plugintSettings.AdditionalFee = model.AdditionalFee;
            plugintSettings.DescriptionText = model.DescriptionText;
            plugintSettings.EnableDebugMode = model.EnableDebugMode;
            plugintSettings.GatewayUrl = model.GatewayUrl;
            plugintSettings.MerchantId = model.MerchantId;
            plugintSettings.TestMode = model.TestMode;

            _settingService.SaveSetting(plugintSettings);
           
            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
    }
}