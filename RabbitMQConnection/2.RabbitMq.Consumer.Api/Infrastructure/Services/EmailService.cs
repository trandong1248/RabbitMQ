using _2.RabbitMq.Consumer.Api.Infrastructure.Configurations;
using _2.RabbitMq.Consumer.Api.Infrastructure.Helper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace _2.RabbitMq.Consumer.Api.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _client;
        private readonly ILogger<EmailService> _logger;
        private readonly SmtpEmailConfiguation _smtpEmailConfiguation;

        public EmailService(IOptions<SmtpEmailConfiguation> smtpEmailConfiguation, ILogger<EmailService> logger)
        {
            _logger = logger;
            _smtpEmailConfiguation = smtpEmailConfiguation.Value;
            _client = new SmtpClient
            {
                Host = _smtpEmailConfiguation.Host,
                Port = _smtpEmailConfiguation.Port,
                EnableSsl = _smtpEmailConfiguation.EnableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpEmailConfiguation.UserName, _smtpEmailConfiguation.Password)
            };
        }

        public async Task SendEmailAsync(string subject,
           string body,
           IEnumerable<string> toAddresses,
           IEnumerable<string> ccAddresses = null,
           IEnumerable<string> bccAddresses = null,
           bool isBodyHtml = true)
        {
            try
            {
                Policy.Handle<SmtpException>()
               .Or<SmtpFailedRecipientsException>()
               .Or<InvalidOperationException>()
               .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
               {
                   _logger.LogWarning(ex.ToString());
               })
               .Execute(() =>
               {
                   using (var message = new MailMessage())
                   {
                       message.From = new MailAddress(_smtpEmailConfiguation.From);
                       message.Subject = subject;
                       message.Body = body;
                       message.IsBodyHtml = isBodyHtml;

                       foreach (var toAddress in toAddresses.Where(email => !string.IsNullOrWhiteSpace(email) && email.IsMatchEmail()))
                       {
                           message.To.Add(toAddress);
                       }

                       if (ccAddresses?.Any() == true)
                       {
                           foreach (var ccAddress in ccAddresses.Where(email => !string.IsNullOrWhiteSpace(email) && email.IsMatchEmail()))
                           {
                               message.CC.Add(ccAddress);
                           }
                       }

                       if (bccAddresses?.Any() == true)
                       {
                           foreach (var bccAddress in bccAddresses.Where(email => !string.IsNullOrWhiteSpace(email) && email.IsMatchEmail()))
                           {
                               message.Bcc.Add(bccAddress);
                           }
                       }

                       if (message.To.Any() || message.CC.Any() || message.Bcc.Any())
                           _client.Send(message);
                   }
               });

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString());
            }

        }
    }
}