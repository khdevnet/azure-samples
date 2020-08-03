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
using ILogger = Microsoft.Extensions.Logging.ILogger;

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
          //  RegisterLog4Net(context, logger);
            string upName = stringUppercaseService.Apply(myQueueItem);
            logger.LogInformation($"Hello word: {upName}");
        }

        private static void RegisterLog4Net(ExecutionContext context, ILogger log)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            var configFileName = "log4net.config";

            XmlConfigurator.Configure(logRepository, new FileInfo(Path.Combine(context.FunctionAppDirectory, configFileName)));
           // AdoNetAppenderHelper.SetConnectionString(logRepository, "Data Source=.;Initial Catalog=Logs;Integrated Security=True");
            BasicConfigurator.Configure(logRepository, new AzureFunctionLoggerAppender(log));
            var appenders = logRepository.GetAppenders();

            var adonet = appenders.FirstOrDefault(c => c is AdoNetAppender);
            var logger = LogManager.GetLogger(typeof(Function1));
            logger.Info("hellooooooooooooooooooooooooooooooooooooo");
        }
    }

    internal class AzureFunctionLoggerAppender : AppenderSkeleton
    {
        private readonly ILogger logger;

        public AzureFunctionLoggerAppender(ILogger logger)
        {
            this.logger = logger;
        }
        protected override void Append(LoggingEvent loggingEvent)
        {
            switch (loggingEvent.Level.Name)
            {
                case "DEBUG":
                    logger.LogDebug($"{loggingEvent.LoggerName} - {loggingEvent.RenderedMessage}");
                    break;
                case "INFO":
                    logger.LogInformation($"{loggingEvent.LoggerName} - {loggingEvent.RenderedMessage}");
                    break;
                case "WARN":
                    logger.LogWarning($"{loggingEvent.LoggerName} - {loggingEvent.RenderedMessage}");
                    break;
                case "ERROR":
                    logger.LogError($"{loggingEvent.LoggerName} - {loggingEvent.RenderedMessage}");
                    break;
                case "FATAL":
                    logger.LogCritical($"{loggingEvent.LoggerName} - {loggingEvent.RenderedMessage}");
                    break;
                default:
                    logger.LogTrace($"{loggingEvent.LoggerName} - {loggingEvent.RenderedMessage}");
                    break;
            }
        }
    }
}
