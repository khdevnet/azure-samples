using System;

namespace DurableFunctionsSaga
{
    public class Helper
    {
        public static string GetHostUrl()
        {
            return $"http://{Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME")}";
        }
    }
}
