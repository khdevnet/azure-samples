using System;
using HelloWordFunc.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
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
        public void Run([QueueTrigger("hwf-items", Connection = "")]string myQueueItem)
        {
            string upName = stringUppercaseService.Apply(myQueueItem);
            logger.LogInformation($"Hello word: {upName}");
        }
    }
}
