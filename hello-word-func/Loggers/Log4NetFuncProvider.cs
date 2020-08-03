using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using HelloWordFunc.Loggers;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Scope;

namespace HelloWordFunc.Loggers
{
    public class Log4NetFuncProvider : ILoggerProvider
    {

        private readonly ConcurrentDictionary<string, Log4NetFuncLogger> loggers = new ConcurrentDictionary<string, Log4NetFuncLogger>();


        private bool disposedValue = false;


        private ILoggerRepository loggerRepository;

        public Log4NetFuncProvider(Assembly loggingAssembly, string configFilePath)
        {
            CreateLoggerRepository(loggingAssembly)
                .ConfigureLog4NetLibrary(configFilePath);
        }

        ~Log4NetFuncProvider()
        {
            Dispose(false);
        }

        public ILogger CreateLogger(string categoryName)
            => loggers.GetOrAdd(categoryName, CreateLoggerImplementation);


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    loggerRepository.Shutdown();
                    loggers.Clear();
                }

                disposedValue = true;
            }
        }

        private Log4NetFuncLogger CreateLoggerImplementation(string name)
        {

            return new Log4NetFuncLogger(name, loggerRepository.Name);
        }

        private Log4NetFuncProvider ConfigureLog4NetLibrary(string configFilePath)
        {
            XmlConfigurator.Configure(loggerRepository, new FileInfo(configFilePath));
            return this;
        }

        private Log4NetFuncProvider CreateLoggerRepository(Assembly assembly)
        {
            Type repositoryType = typeof(log4net.Repository.Hierarchy.Hierarchy);
            loggerRepository = LogManager.CreateRepository(assembly, repositoryType);
            return this;
        }
    }

}