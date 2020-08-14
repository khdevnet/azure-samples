using AsyncHttpAzureFunc.Data;
using Automatonymous;
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
    public class MessageProcessStateMachine : MassTransitStateMachine<MessageProcessSagaState>
    {
        public State Submitted { get; private set; }
        public State Processed { get; private set; }
        public State Error { get; private set; }

        public MessageProcessStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => MessageSubmit, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => MessageProcessed, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => MessageError, x => x.CorrelateById(context => context.Message.CorrelationId));

            Initially(
               When(MessageSubmit)
                   .TransitionTo(Submitted));

            During(Submitted,
               When(MessageProcessed)
               .TransitionTo(Processed));

            During(Submitted,
               When(MessageError)
               .TransitionTo(Error));

        }

        public Event<MessageSubmit> MessageSubmit { get; private set; }
        public Event<MessageProcessed> MessageProcessed { get; private set; }
        public Event<MessageError> MessageError { get; private set; }

    }

}
