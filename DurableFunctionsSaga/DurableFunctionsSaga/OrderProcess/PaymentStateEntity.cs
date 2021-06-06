using System;

namespace DurableFunctionsSaga.OrderProcess
{
    public class PaymentStateEntity
    {
        public const string Name = "Payments";
        public const string Key = "PaymentState";

        public string PartitionKey { get; } = Key;
        public string RowKey { get; set; }
        public string OrchestrationId { get; set; }
        public Guid OrderId { get; set; }
    }
}
