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
using System.Threading.Tasks;

namespace AsyncHttpAzureFunc.LongTaskProcessor
{
    public class MessagesConsumer : IConsumer<SubmitMessage>
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<MessagesConsumer> logger;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly IPublishEndpoint publishEndpoint;

        public MessagesConsumer(
            IHttpClientFactory clientFactory,
            ILogger<MessagesConsumer> logger,
            IHubContext<ChatHub> hubContext,
            IPublishEndpoint publishEndpoint)
        {
            this.clientFactory = clientFactory;
            this.logger = logger;
            this.hubContext = hubContext;
            this.publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<SubmitMessage> context)
        {
           
            await publishEndpoint.Publish(new AcceptedMessage(context.Message.CorrelationId, DateTime.Now));

            var httpClinet = clientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get,
          "http://localhost:7071/api/Function1");
            request.Headers.Add("Accept", "application/vnd.github.v3+json");
            request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            var response = await httpClinet.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                logger.LogInformation(result);
                await hubContext.Clients.All.SendAsync("broadcastMessage", "MessagesConsumer", result);
            }

            await publishEndpoint.Publish(new ProcessedMessage(context.Message.CorrelationId, DateTime.Now));
        }
    }

    class SubmitOrderConsumerDefinition :
        ConsumerDefinition<MessagesConsumer>
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
            IConsumerConfigurator<MessagesConsumer> consumerConfigurator)
        {
            // configure message retry with millisecond intervals
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));

            // use the outbox to prevent duplicate events from being published
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
