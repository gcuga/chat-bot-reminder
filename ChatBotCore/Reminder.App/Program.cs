using Reminder.Scheduler;

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
