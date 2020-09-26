using RabbitMQ.Client;
using System;

namespace _3.RabbitMq.Service.Bus.RabbitMQ
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}