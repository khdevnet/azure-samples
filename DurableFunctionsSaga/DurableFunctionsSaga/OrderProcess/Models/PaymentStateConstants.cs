using System;
using System.Collections.Generic;
using System.Text;

namespace DurableFunctionsSaga.OrderProcess.Models
{
    public class PaymentStateConstants
    {
        public const string WebhookAction = "payment-webhook";
        public const string PaymentResultEventName = "PaymentResult";
    }
}
