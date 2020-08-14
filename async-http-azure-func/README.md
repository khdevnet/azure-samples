## Asyn http operation processing
* azure functions http trigger
* [masstransit message bus](https://masstransit-project.com/)
* [asp.net core signalr](https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-3.1&tabs=visual-studio)

## Notes
*  use the UseMessageRetry to configure retry
*  use the UseInMemoryOutbox to prevent duplicate events from being published
*  use the DiscardSkippedMessages to prevent saving not processed messages
*  use the DiscardFaultedMessages to prevent saving not processed messages

```csharp

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
```
