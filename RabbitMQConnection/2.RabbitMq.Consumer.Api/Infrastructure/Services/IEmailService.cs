using System.Collections.Generic;
using System.Threading.Tasks;

namespace _2.RabbitMq.Consumer.Api.Infrastructure.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string subject,
            string body,
            IEnumerable<string> toAddresses,
            IEnumerable<string> ccAddresses = null,
            IEnumerable<string> bccAddresses = null,
            bool isBodyHtml = true);
    }
}