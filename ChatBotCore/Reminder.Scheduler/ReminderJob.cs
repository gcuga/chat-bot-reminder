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


            Console.WriteLine();
            Console.WriteLine($"SortedSet in thread:  {threadName} \t Now: {DateTimeOffset.Now:HH:mm:ss fff}");
            foreach (var r in ReminderItems)
            {
                Console.WriteLine($"{r.Id}\t{r.NextReminderDate:HH:mm:ss}\t{r.ThreadGuid}\t{r.Message}");
            }
            Console.WriteLine();




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
                        item.NextReminderDate = ReminderItem.CalculateNextReminderDate(
                            item.IsActive,
                            item.FrequencyType, 
                            item.DateBegin, 
                            item.DateGoal, 
                            currentNextReminderDateNotNull);

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
            ReminderItem r = item;
            Console.WriteLine($"Send remainder {r.Id}\t{r.NextReminderDate:HH:mm:ss}\t{r.ThreadGuid}\t{r.Message}"+
                $"\tNow {DateTimeOffset.Now:HH:mm:ss}");
            return true;
        }
    }
}
