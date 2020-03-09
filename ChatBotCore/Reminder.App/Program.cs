using System;
using Reminder.Parameters;
using Reminder.Storage;
using Reminder.Scheduler;
using System.Threading;

namespace Reminder.App
{
    class Program
    {
        static void Main(string[] args)
        {
            TestDataInMemory.Populate();

            ReminderScheduler scheduler = ReminderScheduler.Instance;
            scheduler.MainThread();
        }
    }
}
