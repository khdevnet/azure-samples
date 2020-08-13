using AzureStorageQueueLongRunnigTasks.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AzureStorageQueueLongRunnigTasks.Services.SignalR
{
    public class LongTaskRunningMessageSender
    {
        private readonly IHubContext<LongTaskRunningMessageHub, ILongTaskRunningMessageClient> hubContext;

        public LongTaskRunningMessageSender(IHubContext<LongTaskRunningMessageHub, ILongTaskRunningMessageClient> hubContext)
        {
            this.hubContext = hubContext;
        }

        public void SendAll(string message)
        {
            hubContext.Clients.All.ReceiveMessage(message);
        }
    }
}
