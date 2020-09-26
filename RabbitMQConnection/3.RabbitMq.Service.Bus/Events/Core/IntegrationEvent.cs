using System;

namespace _3.RabbitMq.Service.Bus.Events.Core
{
    public class IntegrationEvent
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public DateTime DateCreated { get; private set; } = DateTime.Now;
    }
}