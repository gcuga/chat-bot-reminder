using System;
using Reminder.Parameters;
using System.Diagnostics;
using System.Threading;

namespace Reminder.Scheduler
{
    /// <summary>
    /// Планировщик выполняющий обработку расписания, формирующий локальный список
    /// напоминаний и инициирующий процесс обработки этого списка
    /// </summary>
    public sealed class Scheduler
    {
        private static volatile Scheduler _instance;
        private static readonly object syncRoot = new Object();
        private static readonly object syncTimedEvent = new Object();

        // Интервал срабатывания таймера планировщика
        private static readonly TimeSpan _timeSpan = ServiceParameters._schedulerTimeSpan;

        public static Scheduler Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new Scheduler();
                    }
                }

                return _instance;
            }
        }

        // флаг работы планировщика
        public bool IsActivity { get; private set; } = false;
        // Время последнего сеанса работы
        public TimeSpan LastActivityTime { get; private set; } = TimeSpan.Zero;
        // Дата запуска планировщика
        public DateTimeOffset StartDateTime { get; } = DateTimeOffset.Now;
        // Количество срабатываний с момента запуска
        public int ActivityCounter { get; private set; } = 0;

        private Scheduler()
        {
        }

        private void OnTimedEvent(object source, EventArgs e)
        {
            StartJob();
        }

        private void OnTimedEvent()
        {
            OnTimedEvent(null, EventArgs.Empty);
        }

        /// <summary>
        /// Основной процесс планировщика
        /// </summary>
        public void MainProcess()
        {
            Console.WriteLine("The scheduler started at {0:HH:mm:ss.fff}", StartDateTime);
            using (var timer = new System.Timers.Timer(_timeSpan.TotalMilliseconds))
            {
                timer.Elapsed += OnTimedEvent;
                timer.AutoReset = true;
                timer.Enabled = true;
                // первый запуск обработки выполняется сразу
                Thread immediateRunTimedEventThread = new Thread(OnTimedEvent);
                immediateRunTimedEventThread.Start();
                Console.WriteLine("\nPress the Enter key to stop the scheduler...\n");
                Console.ReadLine();
            }
        }

        public void StartJob()
        {
            if (IsActivity)
            {
                return;
            }

            if (Monitor.TryEnter(syncTimedEvent))
            {
                try
                {
                    IsActivity = true;
                    ActivityCounter++;
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    ReminderJob reminderJob = new ReminderJob(ServiceParameters._storage);
                    Thread newThread = new Thread(reminderJob.Run);
                    newThread.Start();
                    stopWatch.Stop();
                    LastActivityTime = stopWatch.Elapsed;
                }
                finally
                {
                    IsActivity = false;
                    Monitor.Exit(syncTimedEvent);
                }
            }
        }
    }
}
