using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncHttpAzureFunc.LongTaskProcessor
{
    public interface IProcessedMessage
    {
        DateTime ProcessedDate { get; }
        public Guid CorrelationId { get; }
    }
    public class ProcessedMessage : IProcessedMessage
    {
        public ProcessedMessage(Guid correlationId, DateTime processedData)
        {
            ProcessedData = processedData;
            CorrelationId = correlationId;
        }

        public DateTime ProcessedDate { get; }
        public Guid CorrelationId { get; }
        public DateTime ProcessedData { get; }
    }
}
