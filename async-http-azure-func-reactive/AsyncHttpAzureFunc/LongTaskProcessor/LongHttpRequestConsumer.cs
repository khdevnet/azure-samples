using ChatSample.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncHttpAzureFunc.LongTaskProcessor
{
    public class LongHttpRequestConsumer : BackgroundService, IDisposable
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<LongTaskHandler> logger;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly ILongTaskHandler longTaskHandler;
        private readonly CancelationTokenStore cancelationTokenStore;

        public LongHttpRequestConsumer(
            IHttpClientFactory clientFactory,
            ILogger<LongTaskHandler> logger,
            IHubContext<ChatHub> hubContext,
            ILongTaskHandler longTaskHandler,
            CancelationTokenStore cancelationTokenStore
            )
        {
            this.clientFactory = clientFactory;
            this.logger = logger;
            this.hubContext = hubContext;
            this.longTaskHandler = longTaskHandler;
            this.cancelationTokenStore = cancelationTokenStore;
        }

        public async Task<string> SendHttpRequestAsync(IMessage message)
        {
            using (var tokenScope = cancelationTokenStore.CreateScope(message.MessageId))
            {
                var httpClient = clientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:7071/api/Function1");

                var response = await httpClient.SendAsync(request, tokenScope.Token);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();

                }
                return null;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            longTaskHandler.Subscribe(subscriberName: typeof(LongHttpRequestConsumer).Name,
                                    action: async (e) =>
                                    {
                                        if (e is IMessage)
                                        {
                                            var result = await SendHttpRequestAsync(e);
                                            logger.LogInformation(result);
                                            await hubContext.Clients.All.SendAsync("broadcastMessage", "MessagesConsumer", result);
                                        }
                                    });

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
