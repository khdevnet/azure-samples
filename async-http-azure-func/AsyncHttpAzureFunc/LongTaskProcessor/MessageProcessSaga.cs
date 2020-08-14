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
        private readonly CancelationTokenStore cancelationTokenStore;

        public MessageProcessSaga(
            IHttpClientFactory clientFactory,
            ILogger<MessageProcessSaga> logger,
            IHubContext<ChatHub> hubContext,
            IPublishEndpoint publishEndpoint,
            CancelationTokenStore cancelationTokenStore
            )
        {
            this.clientFactory = clientFactory;
            this.logger = logger;
            this.hubContext = hubContext;
            this.publishEndpoint = publishEndpoint;
            this.cancelationTokenStore = cancelationTokenStore;
        }

        public async Task Consume(ConsumeContext<MessageSubmit> context)
        {
            using (var tokenScope = cancelationTokenStore.CreateScope(context.Message.MessageId))
            {
                var httpClient = clientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:7071/api/Function1");

                var response = await httpClient.SendAsync(request, tokenScope.Token);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    logger.LogInformation(result);
                    await hubContext.Clients.All.SendAsync("broadcastMessage", "MessagesConsumer", result);
                    await publishEndpoint.Publish(new MessageProcessed(context.Message.MessageId, DateTime.Now), tokenScope.Token);
                }
                else
                {
                    await publishEndpoint.Publish(new MessageError(context.Message.MessageId, DateTime.Now), tokenScope.Token);
                }
            }
        }

        public void Dispose()
        {
        }
    }

    class SubmitOrderConsumerDefinition :
        ConsumerDefinition<MessageProcessSaga>
    {
        private readonly ILogger<SubmitOrderConsumerDefinition> logger;

        public SubmitOrderConsumerDefinition(ILogger<SubmitOrderConsumerDefinition> logger)
        {
            // override the default endpoint name
            EndpointName = "order-service";

            // limit the number of messages consumed concurrently
            // this applies to the consumer only, not the endpoint
            ConcurrentMessageLimit = 8;
            this.logger = logger;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<MessageProcessSaga> consumerConfigurator)
        {
            // configure message retry with millisecond intervals
            endpointConfigurator.UseMessageRetry(r => r.None());
            //endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));

            // use the outbox to prevent duplicate events from being published
            endpointConfigurator.UseInMemoryOutbox();
            endpointConfigurator.DiscardSkippedMessages();
            endpointConfigurator.DiscardFaultedMessages();

            endpointConfigurator.ConfigureError(x => x.UseExecute(ex =>
            {
                logger.LogError(ex.Exception.ToString());
            }));
        }
    }
}
