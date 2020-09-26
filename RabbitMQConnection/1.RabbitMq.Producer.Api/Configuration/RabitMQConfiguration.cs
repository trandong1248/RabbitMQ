namespace _1.RabbitMq.Producer.Api.Configuration
{
    public class RabitMQConfiguration
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int RetryCount { get; set; } = 5;
        public string QueueName { get; set; }
        public string HostName { get; set; }
        public int? Port { get; set; }
        public string VirtualHost { get; set; }
        public string Exchangetype { get; set; } = "topic";
        public string ExchangeName { get; set; } = "EventBusRabbitMQ";
    }
}