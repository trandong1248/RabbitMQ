using _2.RabbitMq.Consumer.Api.Infrastructure.Services;
using _3.RabbitMq.Service.Bus.CommandBus;
using _3.RabbitMq.Service.Bus.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace _2.RabbitMq.Consumer.Api.Infrastructure.Handlers
{
    public class EmailIntegrationHandler : IIntegrationEventHandler<EmailIntegrationEvent>
    {
        private readonly ILogger<EmailIntegrationHandler> _logger;
        private readonly IEmailService _emailService;

        public EmailIntegrationHandler(ILogger<EmailIntegrationHandler> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public async Task Handle(EmailIntegrationEvent @event)
        {
            _logger.LogInformation("Start EmailIntegrationHandler");
            try
            {
                await _emailService.SendEmailAsync(@event.Subject, @event.Body, @event.To, @event.Cc, @event.Bcc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmailIntegrationHandler error: ");
            }
            _logger.LogInformation("End EmailIntegrationHandler");
        }
    }
}