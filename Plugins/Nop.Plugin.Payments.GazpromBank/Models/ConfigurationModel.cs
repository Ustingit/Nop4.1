using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.GazpromBank.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payments.GazpromBank.DescriptionText")]
        public string DescriptionText { get; set; }
        [NopResourceDisplayName("Plugins.Payments.GazpromBank.MerchantId")]
        public string MerchantId { get; set; }
        [NopResourceDisplayName("Plugins.Payments.GazpromBank.AccountId")]
        public string AccountId { get; set; }
        [NopResourceDisplayName("Plugins.Payments.GazpromBank.GatewayUrl")]
        public string GatewayUrl { get; set; }
        [NopResourceDisplayName("Plugins.Payments.GazpromBank.TestMode")]
        public bool TestMode { get; set; }
        [NopResourceDisplayName("Plugins.Payments.GazpromBank.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        [NopResourceDisplayName("Plugins.Payments.GazpromBank.EnableDebugMode")]
        public bool EnableDebugMode { get; set; }
    }
}