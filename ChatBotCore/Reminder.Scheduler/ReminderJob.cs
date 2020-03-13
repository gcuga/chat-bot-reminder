using Reminder.Parameters;
using Reminder.Sender.Core;
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
        private IReminderSender Sender { get; }

        public ReminderJob(
            ISelectableForUpdateToSortedSet<ReminderItem> storage,
            IReminderSender sender)
        {
            Storage = storage;
            Sender = sender;
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
                    item.LastSendingError = null;
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

                    // отправка сообщения в фоновом потоке
                    Thread backgroundThread =
                        new Thread(new ThreadStart(() => { SendMessage(item); }));
                    backgroundThread.IsBackground = true;
                    backgroundThread.Start();
                }
                else
                {
                    item.IsActive = false;
                    item.ThreadGuid = null;
                    item.IsProcessing = false;
                    ((ICruStorage<ReminderItem>)Storage).Update(item);
                }
            }
        }

        private void SendMessage(ReminderItem item)
        {
            SendingResultTypes sendingResult = SendingResultTypes.Failed;
            AggregateException aggregateException = null;
            Exception exception = null;
            try
            {
                (sendingResult, aggregateException) = Sender.Send(item.GetChatId(), item.Message);
            }
            catch (Exception e)
            {
                exception = e;
            }

            if (exception == null && aggregateException == null && 
                (sendingResult == SendingResultTypes.Success || sendingResult == SendingResultTypes.WrongText))
            {
                // WrongText пока не отрабатывается никак
                item.NextReminderDate = ReminderItem.CalculateNextReminderDate(
                    item.IsActive,
                    item.FrequencyType,
                    item.DateBegin,
                    item.DateGoal,
                    item.NextReminderDate??DateTimeOffset.Now);

                if (item.NextReminderDate == null)
                {
                    item.IsActive = false;
                }
                item.ThreadGuid = null;
                item.IsProcessing = false;
            }
            else
            {
                item.ThreadGuid = null;
                item.IsProcessing = false;
                item.LastSendingError =
                    $"{nameof(sendingResult)}: {sendingResult}\t" +
                    $"{nameof(aggregateException)}: {aggregateException?.Message??"None"}\t" +
                    $"{nameof(exception)}: {exception?.Message??"None"}";
            }

            ((ICruStorage<ReminderItem>)Storage).Update(item);

            //ReminderItem r = item;
            //Console.WriteLine($"Send remainder {r.Id}\t{r.NextReminderDate:HH:mm:ss}\t{r.ThreadGuid}\t{r.Message}"+
            //    $"\tNow {DateTimeOffset.Now:HH:mm:ss}");
        }
    }
}
