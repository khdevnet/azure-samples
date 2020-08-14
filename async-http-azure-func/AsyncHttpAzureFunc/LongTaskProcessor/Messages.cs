using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncHttpAzureFunc.LongTaskProcessor
{

    public class MessageSubmit : IMessage
    {
        public MessageSubmit(Guid messageId, DateTime timestamp)
        {
            MessageId = messageId;
            Timestamp = timestamp;
        }
        public Guid MessageId { get; }
        public DateTime Timestamp { get; }

    }

    public class MessageProcessed : IMessage
    {
        public MessageProcessed(Guid messageId, DateTime timestamp)
        {
            Timestamp = timestamp;
            MessageId = messageId;
        }

        public DateTime Timestamp { get; }
        public Guid MessageId { get; }
    }

    public class MessageError : IMessage
    {
        public MessageError(Guid messageId, DateTime timestamp)
        {
            MessageId = messageId;
            Timestamp = timestamp;
        }

        public Guid MessageId { get; }
        public DateTime Timestamp { get; }
    }

    public interface IMessage
    {
        Guid MessageId { get; }
        DateTime Timestamp { get; }
    }
}
