using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging.Scope;

namespace HelloWordFunc.Loggers
{
    public class Log4NetFuncLogger : ILogger
    {
        private readonly ILog log;

        public Log4NetFuncLogger(string name, string logRepName)
        {
            log = LogManager.GetLogger(logRepName, name);
            Console.WriteLine(logRepName);
            Console.WriteLine(name);
        }

        public string Name
            => log.Logger.Name;

        public IDisposable BeginScope<TState>(TState state)
        {
            return new Log4NetScopeFactory(new Log4NetScopeRegistry()).BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return log.IsFatalEnabled;
                case LogLevel.Debug:
                case LogLevel.Trace:
                    return log.IsDebugEnabled;
                case LogLevel.Error:
                    return log.IsErrorEnabled;
                case LogLevel.Information:
                    return log.IsInfoEnabled;
                case LogLevel.Warning:
                    return log.IsWarnEnabled;
                case LogLevel.None:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {

            if (!IsEnabled(logLevel))
            {
                return;
            }

            EnsureValidFormatter(formatter);

            string message = formatter(state, exception);
            bool shouldLogSomething = !string.IsNullOrEmpty(message) || exception != null;
            if (shouldLogSomething)
            {
                switch (logLevel)
                {
                    case LogLevel.Critical:
                        log.Fatal(message, exception);
                        break;

                    case LogLevel.Debug:
                        log.Debug(message, exception);
                        break;

                    case LogLevel.Error:
                        log.Error(message, exception);
                        break;

                    case LogLevel.Information:
                        log.Info(message, exception);
                        break;

                    case LogLevel.Warning:
                        log.Warn(message, exception);
                        break;

                    case LogLevel.None:
                        // Just ignore the message. But this option shouldn't be reached.
                        break;

                    default:
                        log.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
                        log.Info(message, exception);
                        break;
                }
            }
        }

        private static void EnsureValidFormatter<TState>(Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
        }
    }
}
