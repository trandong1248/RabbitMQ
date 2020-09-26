using System;
using System.ComponentModel;

namespace _1.RabbitMq.Producer.Api.Infrastructure.Application.Enums
{
    [Flags]
    public enum UserNotificationStatus
    {
        [Description("Unread")]
        UnRead = 1,

        [Description("Readed")]
        Readed = 2,

        [Description("Removed")]
        Removed = 3
    }
}