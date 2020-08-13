using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageQueueLongRunnigTasks.Hubs
{
    public interface ILongTaskRunningMessageClient
    {
        Task ReceiveMessage(string message);
    }
}
