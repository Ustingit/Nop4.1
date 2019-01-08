using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.GazpromBank
{
    public class GazpromBankPaymentSettings : ISettings
    {
        public string MerchantId { get; set; }
        public string AccountId { get; set; }
        public string GatewayUrl { get; set; }
        public bool TestMode { get; set; }
        public decimal AdditionalFee { get; set; }
        public string DescriptionText { get; set; }             
        public bool EnableDebugMode { get; set; }
    }
}
