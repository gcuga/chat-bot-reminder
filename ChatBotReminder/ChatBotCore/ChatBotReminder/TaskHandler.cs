using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBotReminder
{
    class TaskHandler
    {
        private SortedSet<ReminderItem> ReminderItems { get; set; }

        public TaskHandler(SortedSet<ReminderItem> reminderItems)
        {
            ReminderItems = reminderItems ?? throw new ArgumentNullException(nameof(reminderItems));
        }

        public void ProcessReminders()
        {
            foreach (var item in ReminderItems)
            {
                DateTimeOffset? currentNextReminderDate = item.NextReminderDate;
                if (currentNextReminderDate != null)
                {
                    DateTimeOffset currentNextReminderDateNotNull = currentNextReminderDate.Value;
                    do
                    {
                        DateTimeOffset processingDateWithAccuracy = DateTimeOffset.Now - ServiceParameters._taskHandlerTimeAccuracy;
                        if (currentNextReminderDateNotNull < processingDateWithAccuracy)
                        {
                            Console.WriteLine($"sleep {processingDateWithAccuracy - currentNextReminderDateNotNull}");
                            Console.WriteLine($"{processingDateWithAccuracy} {currentNextReminderDateNotNull}");
                            Thread.Sleep(processingDateWithAccuracy - currentNextReminderDateNotNull);
                            continue;
                        }
                        break;
                    } while (true);

                    bool isSended = SendMessage(item);
                    if (isSended)
                    {
                        // в дальнейшем нужны будут блокировки для присвоения значений полей в item
                        // возможно присвоение вообще через события в ReminderItem лучше делать
                        // чтобы собрать всю логику ReminderItem внутри

                        item.NextReminderDate = ReminderItem.CalculateNextReminderDate(item.IsActive,
                            item.FrequencyType, item.DateBegin, item.DateGoal, currentNextReminderDateNotNull);
                        if (item.NextReminderDate == null)
                        {
                            // если следующая дата отправки не получена, значит напоминание
                            // нужно перевести в неактивное состояние
                            item.IsActive = false;
                        }
                    }
                }
            }
        }

        private bool SendMessage(ReminderItem item)
        {
            // вызов отправщика сообщений
            Console.WriteLine($"Send remainder {item.Id} {item.GetCategoryName()} {item.Message}" +
                $" {item.FrequencyType} current NextReminderDate {item.NextReminderDate} date goal {item.DateGoal}");
            return true;
        }
    }
}
