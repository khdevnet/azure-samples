using AsyncHttpAzureFunc.Data;
using Automatonymous;
using ChatSample.Hubs;
using GreenPipes;
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
        public State Canceled { get; private set; }

        public MessageProcessStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => MessageSubmit, x => x.CorrelateById(context => context.Message.MessageId));
            Event(() => MessageProcessed, x => x.CorrelateById(context => context.Message.MessageId));
            Event(() => MessageError, x => x.CorrelateById(context => context.Message.MessageId));

            Initially(
               When(MessageSubmit)
                //.Activity(x => x.OfType<PublishOrderSubmittedActivity>())
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

    public class PublishOrderSubmittedActivity :
    Activity<MessageProcessSagaState, MessageSubmit>
    {
        readonly ConsumeContext _context;

        public PublishOrderSubmittedActivity(ConsumeContext context)
        {
            _context = context;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("order-service");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<MessageProcessSagaState, MessageSubmit> context, Behavior<MessageProcessSagaState, MessageSubmit> next)
        {
            // do the activity thing
            await _context.Publish<MessageProcessed>(new { OrderId = context.Instance.CorrelationId }).ConfigureAwait(false);

            // call the next activity in the behavior
            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<MessageProcessSagaState, MessageSubmit, TException> context, Behavior<MessageProcessSagaState, MessageSubmit> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }

}
