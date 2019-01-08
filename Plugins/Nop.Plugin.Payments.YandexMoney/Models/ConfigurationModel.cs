using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.YandexMoney.Models
{
    public class ConfigurationModel : BaseNopModel
    {

        [NopResourceDisplayName("Plugins.Payments.YandexMoney.DescriptionText")]
        public string DescriptionText { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexMoney.Fields.WalletNumber")]
        public string WalletNumber { get; set; }
        
        [NopResourceDisplayName("Plugins.Payments.YandexMoney.Fields.ApiKey")]
        public string ApiKey { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexMoney.Fields.SecretKey")]
        public string SecretKey { get; set; }
    }
}