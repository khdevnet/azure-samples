using System;

namespace DurableFunctionsSaga.OrderProcess.Models
{
    public class UpdateOrder
    {
        public Guid OrderId { get; set; }
        public string OrchestrationId { get; set; }
        public string Status { get; set; }
    }
}
