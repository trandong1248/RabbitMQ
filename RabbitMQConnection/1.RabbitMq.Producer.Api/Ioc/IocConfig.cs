using _1.RabbitMq.Producer.Api.Infrastructure.Application.Implementation;
using _1.RabbitMq.Producer.Api.Infrastructure.Application.Interfaces;
using _1.RabbitMq.Producer.Api.Infrastructure.Application.Provider;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace _1.RabbitMq.Producer.Api.Ioc
{
    public static class IocConfig
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            //To use TemplateViewProvider you have to add MVC in Startup.cs class
            services.AddTransient<TemplateViewProvider>();
            services.AddTransient<IUserEmailService, UserEmailService>();
        }
    }
}