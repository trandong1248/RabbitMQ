using _2.RabbitMq.Consumer.Api.Infrastructure.Configurations;
using _2.RabbitMq.Consumer.Api.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace _2.RabbitMq.Consumer.Api.Ioc
{
    public static class IocConfig
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmtpEmailConfiguation>(configuration.GetSection("SmtpEmailConfiguation"));
            services.AddTransient<IEmailService, EmailService>();
        }
    }
}