using System;

namespace DurableFunctionsSaga.OrderProcess.Models
{
    public class StartPayment
    {
        public string OrchestrationId { get; set; }
        public Guid OrderId { get; set; }
    }
}
