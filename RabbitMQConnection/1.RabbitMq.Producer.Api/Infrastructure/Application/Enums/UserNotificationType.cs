using System;

namespace _1.RabbitMq.Producer.Api.Infrastructure.Application.Enums
{
    [Flags]
    public enum UserNotificationType
    {
        Notify = 1,
        Email = 2,
    }
}