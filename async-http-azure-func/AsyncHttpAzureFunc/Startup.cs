using AsyncHttpAzureFunc.LongTaskProcessor;
using ChatSample.Hubs;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;

namespace ChatSample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSignalR();
            services.AddHttpClient();
            services.AddMassTransit(x =>
            {
                 x.AddSaga<MessageProcessSaga>().InMemoryRepository();

                x.AddConsumer<MessagesConsumer>(typeof(SubmitOrderConsumerDefinition));

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.TransportConcurrencyLimit = 100;
                    cfg.ConfigureEndpoints(context);
                });
            });
            services.AddMassTransitHostedService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseFileServer();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }
}
