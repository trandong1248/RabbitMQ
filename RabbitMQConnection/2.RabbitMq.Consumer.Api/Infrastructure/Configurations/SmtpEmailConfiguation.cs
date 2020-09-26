namespace _2.RabbitMq.Consumer.Api.Infrastructure.Configurations
{
    public class SmtpEmailConfiguation
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string From { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}