using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace HelloWordFunc
{
    public class ConsoleLogger : ILogger
    {
        private readonly string _name;

        public ConsoleLogger(string name)
        {
            _name = name;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                            Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
                Console.WriteLine($"{logLevel} - {eventId.Id} " +
                                  $"- {_name} - {formatter(state, exception)}");
        }
    }

    public class Log4netLoggerProvider: ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ConsoleLogger> _loggers = new ConcurrentDictionary<string, ConsoleLogger>();

        public Log4netLoggerProvider()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new ConsoleLogger(name));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
