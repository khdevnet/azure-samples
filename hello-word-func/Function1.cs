using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HelloWordFunc.Services;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using MicroKnights.Logging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HelloWordFunc
{
    public class Function1
    {
        private readonly IStringUppercaseService stringUppercaseService;
        private readonly ILogger<Function1> logger;

        public Function1(IStringUppercaseService stringUppercaseService, ILogger<Function1> logger)
        {
            this.stringUppercaseService = stringUppercaseService;
            this.logger = logger;
        }

        [FunctionName("Function1")]
        public void Run([QueueTrigger("hwf-items", Connection = "")] string myQueueItem)
        {
            string upName = stringUppercaseService.Apply(myQueueItem);
            logger.LogInformation($"Hello word: {upName}");
        }
    }
}
