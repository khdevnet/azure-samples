using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using AzureStorageQueueLongRunnigTasks.Services.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzureStorageQueueLongRunnigTasks.Services
{
    #region snippet1
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> _logger;
        private readonly LongTaskRunningMessageSender longTaskRunningMessageSender;
        private QueueClient _queueClient;
        private Timer _timer;

        public TimedHostedService(
            ILogger<TimedHostedService> logger,
            LongTaskRunningMessageSender longTaskRunningMessageSender)
        {
            _logger = logger;
            this.longTaskRunningMessageSender = longTaskRunningMessageSender;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            // Instantiate a QueueClient which will be used to create and manipulate the queue
            _queueClient = new QueueClient("UseDevelopmentStorage=true", "longtask-success");
            _queueClient.CreateIfNotExists();

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("=========================");

            QueueMessage[] retrievedMessage = _queueClient.ReceiveMessages(20);

            _logger.LogInformation("RetrievedMessage Count: " + retrievedMessage.Count());
            foreach (var message in retrievedMessage)
            {
                _logger.LogInformation($"Deleted message:");
                _logger.LogInformation(Base64Service.DecodeBase64ToString(message.MessageText));
                longTaskRunningMessageSender.SendAll(message.MessageText);
                _queueClient.DeleteMessage(message.MessageId, message.PopReceipt);
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);
            _queueClient?.DeleteIfExists();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
    #endregion
}