using System;
using System.Text;

namespace AzureStorageQueueLongRunnigTasks.Services
{
    public static class Base64Service
    {
        public static string DecodeBase64ToString(string embedCode)
        {
            byte[] decodedBytes = Convert.FromBase64String(embedCode);
            return Encoding.UTF8.GetString(decodedBytes);
        }
    }
}
