using ChatSample.Hubs;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncHttpAzureFunc.LongTaskProcessor
{
    public class MessageProcessSaga : IConsumer<MessageSubmit>, IDisposable
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<MessageProcessSaga> logger;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly IPublishEndpoint publishEndpoint;

        public MessageProcessSaga(
            IHttpClientFactory clientFactory,
            ILogger<MessageProcessSaga> logger,
            IHubContext<ChatHub> hubContext,
            IPublishEndpoint publishEndpoint)
        {
            this.clientFactory = clientFactory;
            this.logger = logger;
            this.hubContext = hubContext;
            this.publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<MessageSubmit> context)
        {
            var httpClient = clientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get,
          "http://localhost:7071/api/Function1");

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                logger.LogInformation(result);
                await hubContext.Clients.All.SendAsync("broadcastMessage", "MessagesConsumer", result);
                await publishEndpoint.Publish(new MessageProcessed(context.Message.CorrelationId, DateTime.Now));
            }
            else
            {
                await publishEndpoint.Publish(new MessageError(context.Message.CorrelationId, DateTime.Now));
            }

        }

        public void Dispose()
        {
        }
    }

    class SubmitOrderConsumerDefinition :
        ConsumerDefinition<MessageProcessSaga>
    {
        public SubmitOrderConsumerDefinition()
        {
            // override the default endpoint name
            EndpointName = "order-service";

            // limit the number of messages consumed concurrently
            // this applies to the consumer only, not the endpoint
            ConcurrentMessageLimit = 8;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<MessageProcessSaga> consumerConfigurator)
        {
            // configure message retry with millisecond intervals
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));

            // use the outbox to prevent duplicate events from being published
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
