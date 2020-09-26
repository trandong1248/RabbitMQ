using System.Threading.Tasks;

namespace _1.RabbitMq.Producer.Api.Infrastructure.Application.Interfaces
{
    public interface IUserEmailService
    {
        Task SendEmailAsync();
    }
}