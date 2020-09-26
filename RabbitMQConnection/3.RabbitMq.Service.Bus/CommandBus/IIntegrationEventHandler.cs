using _3.RabbitMq.Service.Bus.Events.Core;
using System.Threading.Tasks;

namespace _3.RabbitMq.Service.Bus.CommandBus
{
    public interface IIntegrationEventHandler<in T> where T : IntegrationEvent
    {
        Task Handle(T @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}