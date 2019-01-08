using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.YandexMoney
{
    public class YandexMoneyPaymentSettings : ISettings
    {
        public string DescriptionText { get; set; }
        public string WalletNumber { get; set; }
        public string SecretKey { get; set; }
        public string ApiKey { get; set; }
    }
}
