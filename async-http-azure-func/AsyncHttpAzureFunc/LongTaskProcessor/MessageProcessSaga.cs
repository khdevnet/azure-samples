using ChatSample.Hubs;
using MassTransit;
using MassTransit.Saga;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;

namespace AsyncHttpAzureFunc.LongTaskProcessor
{
    public class MessageProcessSaga :
        ISaga,
        InitiatedBy<SubmitMessage>,
        Orchestrates<AcceptedMessage>,
        Observes<ProcessedMessage, MessageProcessSaga>
    {
        public Guid CorrelationId { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? AcceptDate { get; set; }
        public DateTime? ProcessedDate { get; set; }

        public Expression<Func<MessageProcessSaga, ProcessedMessage, bool>> CorrelationExpression =>
        (saga, message) => saga.CorrelationId == message.CorrelationId;

        public async Task Consume(ConsumeContext<SubmitMessage> context)
        {
            SubmitDate = DateTime.Now;
        }

        public async Task Consume(ConsumeContext<AcceptedMessage> context)
        {
            AcceptDate = context.Message.Timestamp;
        }

        public async Task Consume(ConsumeContext<ProcessedMessage> context)
        {
            ProcessedDate = context.Message.ProcessedDate;
        }
    }

}
