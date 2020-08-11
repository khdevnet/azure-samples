using HelloWordFunc.Services;
using log4net;
using MicroKnights.Logging;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

[assembly: FunctionsStartup(typeof(HelloWordFunc.Startup))]

namespace HelloWordFunc
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IStringUppercaseService, StringUppercaseService>();

            builder.Services.AddLogging(logBuilder =>
            {
                var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var configFilePath = Path.Combine(binDirectory, "..", "log4net.config");
                logBuilder
                .AddFilter("HelloWordFunc", LogLevel.Debug)
                .AddLog4Net(configFilePath);
            });
        }
    }
}
