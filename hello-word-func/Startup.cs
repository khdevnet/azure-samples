using HelloWordFunc.Services;
using log4net;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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
            builder.Services.AddSingleton<ILoggerProvider>((s) =>
            {
                var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var path = Path.Combine(binDirectory, "..", "log4net.config");
                var options = new Log4NetProviderOptions(path);
                return new Log4NetProvider(options);
            });
        }
    }
}
