using System;

namespace ChatBotReminder
{
    public static class ServiceParameters
    {
        public static readonly TimeSpan _schedulerTimeSpan = TimeSpan.FromSeconds(1);
        public static readonly TimeSpan _taskHandlerTimeScope = TimeSpan.FromMinutes(15);
        public static readonly string _defaultReminderMessage = "Reminder";
        public static readonly TimeSpan _taskHandlerTimeAccuracy = TimeSpan.FromMilliseconds(100);
    }
}
