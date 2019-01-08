using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.GazpromBank.Core
{
    public static class GazpromBankPaymentCore
    {
        public static string PaymentAvailResponseFalse()
        {
            string result = "<payment-avail-response><result><code>2</code><desc>Unable to accept payment </desc></result></payment-avail-response>";
            return result;
        }

        public static string PaymentAvailResponseSuccess(Order order, GazpromBankPaymentSettings paymentSettings)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("<payment-avail-response>");
            result.AppendLine("<result>");
            result.AppendLine("<code>1</code>");
            result.AppendLine("<desc>OK</desc>");
            result.AppendLine("</result>");
            result.AppendFormat("<merchant-trx>{0}</merchant-trx>", order.Id);
            result.AppendLine("<purchase>");
            result.AppendFormat("<shortDesc>Заказ: {0}</shortDesc>", order.CustomOrderNumber);
            result.AppendFormat("<longDesc>Оплата по заказу: {0}</longDesc>", order.CustomOrderNumber);
            result.AppendLine("<account-amount>");
            result.AppendFormat("<id>{0}</id>", paymentSettings.AccountId);
            result.AppendFormat("<amount>{0}</amount>", (int)(order.OrderTotal * 100));
            result.AppendFormat("<fee>{0}</fee>", (int)(order.PaymentMethodAdditionalFeeExclTax));
            result.AppendLine("<currency>643</currency>");
            result.AppendLine("<exponent>2</exponent>");
            result.AppendLine("</account-amount>");
            result.AppendLine("</purchase>");
            result.AppendLine("</payment-avail-response>");

            return result.ToString();
        }

        public static string PaymentRegisterResponseSuccess()
        {
            var result = "<register-payment-response><result><code>1</code><desc>OK</desc></result></register-payment-response>";
            return result;
        }

        public static string PaymentRegisterResponseFail()
        {
            var result = "<register-payment-response><result><code>2</code><desc>Temporary unavailable</desc></result></register-payment-response>";
            return result;
        }
    }
}
