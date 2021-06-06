using System;

namespace DurableFunctionsSaga.OrderProcess
{
    public class OrderEntity
    {
        public const string Name = "Orders";
        public const string Key = "OrderState";

        public string PartitionKey { get; } = Key;
        public string RowKey { get; set; }
        public Guid OrderId { get; set; }
        public string Status { get; set; }
    }
}
