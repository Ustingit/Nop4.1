using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.YandexMoney.Models
{
    public class YandexPaymentModel
    {
        public string operation_id { get; set; }
        public string notification_type { get; set; }
        public string datetime { get; set; }
        public string sha1_hash { get;set;}
        public string currency { get; set; }
        public string amount { get; set; }
        public int label { get; set; }
        public string sender { get; set; }
        public string codepro { get; set; }
        public string withdraw_amount { get; set; }
    }
}
