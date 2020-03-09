using Reminder.Storage.Core;
using Reminder.Storage.Entity;
using Reminder.Storage.InMemory;
using System;

namespace Reminder.Parameters
{
    public static class ServiceParameters
    {
        // Периодичность срабатывания таймера шедулера
        public static readonly TimeSpan _schedulerTimeSpan = TimeSpan.FromSeconds(10);

        // Параметр для отбора напоминаний в список на отправку, отбираются все активные
        // напоминания не находящиеся в процессе отправки у которых дата следующей отправки
        // не позже текущей даты плюс _taskHandlerTimeScope
        public static readonly TimeSpan _taskHandlerTimeScope = TimeSpan.FromSeconds(15);

        // текст сообщения по умолчанию
        public static readonly string _defaultReminderMessage = "Reminder";

        // точность сравнения даты отправки и текущей даты для функции ожидания отправки
        // обрабатывающей список на отправку напоминаний
        public static readonly TimeSpan _taskHandlerTimeAccuracy = TimeSpan.FromMilliseconds(100);

        // источник данных для выборки напоминаний
        public static readonly ISelectableForUpdateToSortedSet<ReminderItem> _storage = 
            ReminderItemsInMemory.Instance;

    }
}
