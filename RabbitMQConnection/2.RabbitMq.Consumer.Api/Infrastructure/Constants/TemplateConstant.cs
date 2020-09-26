namespace _2.RabbitMq.Consumer.Api.Infrastructure.Constants
{
    public static class TemplateConstant
    {
        public static class Email
        {
            public static class Subject
            {
                public const string SampleSubject = "Templates/Emails/Subject/SampleSubjectTemplate.html";
            }

            public static class Body
            {
                public const string SampleBody = "Templates/Emails/Body/SampleBodyTemplate.html";
            }

            public static class Notification
            {
                public const string SampleNotification = "Templates/Notifications/SampleNotificationTemplate.html";
            }
        }
    }
}