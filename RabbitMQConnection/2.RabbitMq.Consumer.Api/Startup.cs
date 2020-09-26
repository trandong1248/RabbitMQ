using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2.RabbitMq.Consumer.Api.Infrastructure.Configurations;
using _2.RabbitMq.Consumer.Api.Infrastructure.Handlers;
using _2.RabbitMq.Consumer.Api.Ioc;
using _3.RabbitMq.Service.Bus.Abstractions;
using _3.RabbitMq.Service.Bus.Events;
using _3.RabbitMq.Service.Bus.Events.Core;
using _3.RabbitMq.Service.Bus.RabbitMQ;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

namespace _2.RabbitMq.Consumer.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            #region ========== Swagger and JWT Token ==========

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger RabbitMq", Version = "v1" });
                c.EnableAnnotations();
            });

            #endregion ========== Swagger and JWT Token ==========

            // DI
            IocConfig.Register(services, Configuration);
            RegisterEventBus(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((document, request) =>
                    {
                        var paths = document.Paths.ToDictionary(item => item.Key.ToLowerInvariant(), item => item.Value);
                        document.Paths.Clear();
                        foreach (var pathItem in paths)
                        {
                            document.Paths.Add(pathItem.Key, pathItem.Value);
                        }
                        c.SerializeAsV2 = true;
                    });

                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RABBITMQ CONSUMER REST API V1");
                    });
                });
            }

            ConfigureEventBus(app);

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void RegisterEventBus(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabitMQConfiguration>(configuration.GetSection("RabitMQConfiguration"));

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var rabitMQConfiguration = new RabitMQConfiguration();
                configuration.GetSection("RabitMQConfiguration").Bind(rabitMQConfiguration);

                var logger = sp.GetRequiredService<ILogger<RabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = rabitMQConfiguration.HostName,
                };

                if (!string.IsNullOrEmpty(rabitMQConfiguration.UserName))
                    factory.UserName = rabitMQConfiguration.UserName;

                if (!string.IsNullOrEmpty(rabitMQConfiguration.Password))
                    factory.Password = rabitMQConfiguration.Password;

                if (!string.IsNullOrEmpty(rabitMQConfiguration.VirtualHost))
                    factory.VirtualHost = rabitMQConfiguration.VirtualHost;

                if (rabitMQConfiguration.Port.HasValue)
                    factory.Port = rabitMQConfiguration.Port.Value;

                return new RabbitMQPersistentConnection(factory, logger, rabitMQConfiguration.RetryCount);
            });

            services.AddSingleton<IEventBusRabbitMQ, EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var rabitMQConfiguration = new RabitMQConfiguration();
                configuration.GetSection("RabitMQConfiguration").Bind(rabitMQConfiguration);

                return new EventBusRabbitMQ(rabbitMQPersistentConnection,
                    logger, iLifetimeScope, eventBusSubcriptionsManager,
                    queueName: rabitMQConfiguration.QueueName,
                    exchangetype: rabitMQConfiguration.Exchangetype,
                    exchangeName: rabitMQConfiguration.ExchangeName,
                    retryCount: rabitMQConfiguration.RetryCount);
            });

            services.AddTransient<EmailIntegrationHandler>();

            //services.AddTransient<ReportNotificationIntegrationHandler>();
            //services.AddTransient<NotifyReportStatusIsDeliveredIntegrationHandler>();
            //services.AddTransient<NotifyReportStatusIsExpiredIntegrationHandler>();
            //services.AddTransient<NotifyReportStatusIsPendingIntegrationHandler>();
            //services.AddTransient<NotifyReportStatusIsRevokedIntegrationHandler>();
            //services.AddTransient<ExtractDataIntegrationHandler>();
            //services.AddTransient<RemoveExtractDataIntegrationHandler>();
            //services.AddTransient<NotifyFormStatusIsApprovedIntegrationHandler>();
            //services.AddTransient<NotifyFormStatusIsCompletedIntegrationHandler>();
            //services.AddTransient<NotifyFormStatusIsDeliveredIntegrationHandler>();
            //services.AddTransient<NotifyFormStatusIsExpiredIntegrationHandler>();
            //services.AddTransient<NotifyFormStatusIsPendingIntegrationHandler>();
            //services.AddTransient<NotifyFormStatusIsRejectedIntegrationHandler>();
            //services.AddTransient<NotifyFormStatusIsRevokedIntegrationHandler>();
            //services.AddTransient<NotifyFormStatusIsSentIntegrationHandler>();

            // EventBusSubscriptionsManager
            services.AddSingleton<IEventBusSubscriptionsManager, EventBusSubscriptionsManager>();
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBusRabbitMQ>();

            eventBus.Subscribe<EmailIntegrationEvent, EmailIntegrationHandler>();
            //eventBus.Subscribe<ReportNotificationIntegrationEvent, ReportNotificationIntegrationHandler>();
            //eventBus.Subscribe<NotifyReportStatusIsDeliveredIntegrationEvent, NotifyReportStatusIsDeliveredIntegrationHandler>();
            //eventBus.Subscribe<NotifyReportStatusIsExpiredIntegrationEvent, NotifyReportStatusIsExpiredIntegrationHandler>();
            //eventBus.Subscribe<NotifyReportStatusIsPendingIntegrationEvent, NotifyReportStatusIsPendingIntegrationHandler>();
            //eventBus.Subscribe<NotifyReportStatusIsRevokedIntegrationEvent, NotifyReportStatusIsRevokedIntegrationHandler>();
            //eventBus.Subscribe<ExtractDataIntegrationEvent, ExtractDataIntegrationHandler>();
            //eventBus.Subscribe<RemoveExtractDataIntegrationEvent, RemoveExtractDataIntegrationHandler>();

            //eventBus.Subscribe<NotifyFormStatusIsApprovedIntegrationEvent, NotifyFormStatusIsApprovedIntegrationHandler>();
            //eventBus.Subscribe<NotifyFormStatusIsCompletedIntegrationEvent, NotifyFormStatusIsCompletedIntegrationHandler>();
            //eventBus.Subscribe<NotifyFormStatusIsDeliveredIntegrationEvent, NotifyFormStatusIsDeliveredIntegrationHandler>();
            //eventBus.Subscribe<NotifyFormStatusIsExpiredIntegrationEvent, NotifyFormStatusIsExpiredIntegrationHandler>();
            //eventBus.Subscribe<NotifyFormStatusIsPendingIntegrationEvent, NotifyFormStatusIsPendingIntegrationHandler>();
            //eventBus.Subscribe<NotifyFormStatusIsRejectedIntegrationEvent, NotifyFormStatusIsRejectedIntegrationHandler>();
            //eventBus.Subscribe<NotifyFormStatusIsRevokedIntegrationEvent, NotifyFormStatusIsRevokedIntegrationHandler>();
            //eventBus.Subscribe<NotifyFormStatusIsSentIntegrationEvent, NotifyFormStatusIsSentIntegrationHandler>();
        }
    }
}
