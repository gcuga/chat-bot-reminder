using System;

namespace ChatBotReminder
{
    class Program
    {
        static void Main(string[] args)
        {
            Scheduler scheduler = Scheduler.Instance;
            scheduler.MainProcess();
        }
    }
}
