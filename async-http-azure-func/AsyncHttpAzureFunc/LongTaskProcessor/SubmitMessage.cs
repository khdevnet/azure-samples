using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncHttpAzureFunc.LongTaskProcessor
{
    public class SubmitMessage : ISubmitMessage
    {
        public SubmitMessage(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        public Guid CorrelationId { get; }
    }

    public interface ISubmitMessage :
      CorrelatedBy<Guid>
    {
    }
}

