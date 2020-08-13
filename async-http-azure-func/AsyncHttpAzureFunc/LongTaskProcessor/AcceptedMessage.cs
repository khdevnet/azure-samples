using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncHttpAzureFunc.LongTaskProcessor
{
    public class AcceptedMessage : IAcceptedMessage
    {
        public AcceptedMessage(Guid correlationId, DateTime timestamp)
        {
            CorrelationId = correlationId;
            Timestamp = timestamp;
        }
        public Guid CorrelationId { get; }
        public DateTime Timestamp { get; }
    }

    public interface IAcceptedMessage :
      CorrelatedBy<Guid>
    {
        DateTime Timestamp { get; }
    }
}

