using _1.RabbitMq.Producer.Api.Infrastructure.Application.Interfaces;
using _1.RabbitMq.Producer.Api.Infrastructure.Application.Provider;
using _2.RabbitMq.Consumer.Api.Infrastructure.Constants;
using _3.RabbitMq.Service.Bus.Events;
using _3.RabbitMq.Service.Bus.RabbitMQ;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _1.RabbitMq.Producer.Api.Infrastructure.Application.Implementation
{
    public class UserEmailService : IUserEmailService
    {
        private readonly TemplateViewProvider _templateViewProvider;
        private readonly IEventBusRabbitMQ _eventBusRabbitMQ;

        public UserEmailService(TemplateViewProvider templateViewProvider,
            IEventBusRabbitMQ eventBusRabbitMQ)
        {
            _templateViewProvider = templateViewProvider;
            _eventBusRabbitMQ = eventBusRabbitMQ;
        }

        public async Task SendEmailAsync()
        {

            _eventBusRabbitMQ.Publish(new EmailIntegrationEvent
            {
                To = new List<string> { "dongtv@gemvietnam.com" },
                Subject = await _templateViewProvider.GetTemplateBody(TemplateConstant.Email.Subject.SampleSubject, new { CustomerName = "TranDong", FrontendLink = "http://rabbitmq.com" }),
                Body = await _templateViewProvider.GetTemplateBody(TemplateConstant.Email.Body.SampleBody, new { CustomerName = "TranDong", FrontendLink = "http://rabbitmq.com" }),
            });

        }
    }
}