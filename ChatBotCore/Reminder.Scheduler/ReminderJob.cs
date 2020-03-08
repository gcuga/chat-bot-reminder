using Reminder.Parameters;
using Reminder.Storage.Core;
using Reminder.Storage.Entity;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Reminder.Scheduler
{
    class ReminderJob
    {
        private SortedSet<ReminderItem> ReminderItems { get; set; }
        private ISelectableForUpdateToSortedSet<ReminderItem> Storage { get; }

        public ReminderJob(ISelectableForUpdateToSortedSet<ReminderItem> storage)
        {
            Storage = storage;
        }

        public void Run()
        {
            Func<ReminderItem, bool> predicate = (item) => 
                (!item.IsProcessing && item.IsActive &&
                (item.NextReminderDate == null || 
                item.NextReminderDate <= (DateTimeOffset.Now + ServiceParameters._taskHandlerTimeScope)));
            
            ReminderItems = Storage.SelectForUpdateSkipLocked(predicate, out string threadName);

            foreach (var item in ReminderItems)
            {
                DateTimeOffset? currentNextReminderDate = item.NextReminderDate;
                if (currentNextReminderDate != null)
                {
                    DateTimeOffset currentNextReminderDateNotNull = currentNextReminderDate.Value;
                    do
                    {
                        DateTimeOffset processingDateWithAccuracy = 
                            DateTimeOffset.Now - ServiceParameters._taskHandlerTimeAccuracy;
                        if (processingDateWithAccuracy < currentNextReminderDateNotNull)
                        {
                            Thread.Sleep(currentNextReminderDateNotNull - processingDateWithAccuracy);
                            continue;
                        }
                        break;
                    } while (true);

                    bool isSended = SendMessage(item);
                    if (isSended)
                    {
                        item.NextReminderDate = ReminderItem.CalculateNextReminderDate(item.IsActive,
                            item.FrequencyType, item.DateBegin, item.DateGoal, currentNextReminderDateNotNull);
                        if (item.NextReminderDate == null)
                        {
                            item.IsActive = false;
                        }
                    }
                }
                else
                {
                    item.IsActive = false;
                }

                item.ThreadGuid = null;
                item.IsProcessing = false;
                ((ICruStorage<ReminderItem>)Storage).Update(item);
            }
        }

        private bool SendMessage(ReminderItem item)
        {
            // вызов отправщика сообщений
            Console.WriteLine($"Send remainder {item.Id}\t {item.GetCategoryName()}\t {item.Message}" +
                $" \t{item.FrequencyType} \tcurrent NextReminderDate = {item.NextReminderDate}\t date goal {item.DateGoal}");
            Console.WriteLine();
            return true;
        }
    }
}
