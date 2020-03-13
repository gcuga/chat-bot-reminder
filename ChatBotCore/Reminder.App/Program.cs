using Reminder.Receiver.Domain;
using Reminder.Scheduler;

namespace Reminder.App
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestDataInMemory.Populate();

            ReceiverDomain receiverDomain = new ReceiverDomain();
            receiverDomain.Run();

            ReminderScheduler scheduler = ReminderScheduler.Instance;
            scheduler.Run();
        }
    }
}
