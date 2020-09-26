using _1.RabbitMq.Producer.Api.Configuration;
using _1.RabbitMq.Producer.Api.Ioc;
using _3.RabbitMq.Service.Bus.Abstractions;
using _3.RabbitMq.Service.Bus.CommandBus;
using _3.RabbitMq.Service.Bus.Events.Core;
using _3.RabbitMq.Service.Bus.RabbitMQ;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Linq;

namespace _1.RabbitMq.Producer.Api
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

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.AddHttpContextAccessor();
            services.AddHttpClient();

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
            if (env.IsDevelopment() || env.IsStaging())
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
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RABBITMQ PRODUCER REST API V1");
                    });
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseMvc();
        }

        private static void RegisterEventBus(IServiceCollection services, IConfiguration configuration)
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

            services.AddSingleton<IEventBusSubscriptionsManager, EventBusSubscriptionsManager>();
        }
    }
}