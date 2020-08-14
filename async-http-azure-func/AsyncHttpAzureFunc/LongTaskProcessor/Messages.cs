using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncHttpAzureFunc.LongTaskProcessor
{

    public class MessageSubmit : CorrelatedBy<Guid>, IMessage
    {
        public MessageSubmit(Guid messageId, DateTime timestamp)
        {
            CorrelationId = messageId;
            Timestamp = timestamp;
        }
        public Guid CorrelationId { get; }
        public DateTime Timestamp { get; }

    }

    public class MessageCancel : IMessage
    {
        public MessageCancel(Guid messageId, DateTime timestamp)
        {
            CorrelationId = messageId;
            Timestamp = timestamp;
        }
        public Guid CorrelationId { get; }
        public DateTime Timestamp { get; }
    }


    public class MessageProcessed : IMessage
    {
        public MessageProcessed(Guid messageId, DateTime timestamp)
        {
            Timestamp = timestamp;
            CorrelationId = messageId;
        }

        public DateTime Timestamp { get; }
        public Guid CorrelationId { get; }
    }

    public class MessageError : IMessage
    {
        public MessageError(Guid messageId, DateTime timestamp)
        {
            CorrelationId = messageId;
            Timestamp = timestamp;
        }

        public Guid CorrelationId { get; }
        public DateTime Timestamp { get; }
    }

    public interface IMessage
    {
        Guid CorrelationId { get; }
        DateTime Timestamp { get; }
    }
}
