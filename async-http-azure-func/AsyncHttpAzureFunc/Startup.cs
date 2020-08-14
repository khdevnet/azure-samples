using AsyncHttpAzureFunc.Data;
using AsyncHttpAzureFunc.LongTaskProcessor;
using ChatSample.Hubs;
using GreenPipes;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Threading;

namespace ChatSample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = "Data Source=.;Initial Catalog=AsyncFuncState;Integrated Security=True";
            services.AddControllersWithViews();
            services.AddSignalR();
            services.AddHttpClient();
            services.AddMassTransit(x =>
            {
                 x.AddSagaStateMachine<MessageProcessStateMachine, MessageProcessSagaState>()
                .EntityFrameworkRepository(r =>
                 {
                     r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion

                     r.AddDbContext<DbContext, MessageStateDbContext>((provider, builder) =>
                     {
                         builder.UseSqlServer(connectionString, m =>
                         {
                             m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                             m.MigrationsHistoryTable($"__{nameof(MessageStateDbContext)}");
                         });
                     });
                 });

                x.AddConsumer<MessageProcessSaga>(typeof(SubmitOrderConsumerDefinition));

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseMessageRetry(r=>r.None());
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
