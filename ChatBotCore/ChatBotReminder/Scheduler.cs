﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;

namespace ChatBotReminder
{
    /// <summary>
    /// Планировщик выполняющий обработку расписания, формирующий локальный список
    /// напоминаний и инициирующий процесс обработки этого списка
    /// </summary>
    public sealed class Scheduler
    {
        private static volatile Scheduler _instance;
        private static object syncRoot = new Object();
        private static object syncTimedEvent = new Object();

        // Промежуток нахождения планировщика в гибернации (интервал работы)
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
        private bool IsActivity { get; set; } = false;
        // Дата запуска планировщика
        public DateTimeOffset StartDateTime { get; } = DateTimeOffset.Now;
        // Количество срабатываний с момента запуска
        public int ActivityCounter { get; private set; } = 0;
        // Время последнего сеанса работы
        public TimeSpan LastActivityTime { get; private set; } = TimeSpan.Zero;

        private Scheduler()
        {
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0} {1}", e.SignalTime, source?.ToString());
            DoJob();
        }

        private void SetTimer(System.Timers.Timer timer)
        {
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        /// <summary>
        /// Основной процесс планировщика
        /// </summary>
        public void MainProcess()
        {
            //DoJob();

            using (var timer = new System.Timers.Timer(_timeSpan.TotalMilliseconds))
            {
                SetTimer(timer);

                Console.WriteLine("\nPress the Enter key to exit the application...\n");
                Console.WriteLine("The application started at {0:HH:mm:ss.fff}", StartDateTime);

                Thread.Sleep(100000);
                timer.Stop();

                Console.ReadLine();

                timer.Stop();
            }
        }

        public void DoJob()
        {
            if (IsActivity)
            {
                Console.WriteLine($"IsActivity = {IsActivity}");
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


                    SortedSet<ReminderItem> reminderItems = ReminderItemHashSet.GetReminderSet();
                    if (reminderItems.Count > 0 )
                    {
                        // если список для отправки не пустой, то создаем асинхронный обработчик задания
                        CreateTaskHandler(reminderItems);
                    }

                    Console.WriteLine($"ActivityCounter = {ActivityCounter}, LastActivityTime = {LastActivityTime}");
                    Console.WriteLine();

                    Thread.Sleep(7000);

                    Console.WriteLine("end sleep");

                    stopWatch.Stop();
                    LastActivityTime = stopWatch.Elapsed;

                    //Console.WriteLine("Threads.Count " + Process.GetCurrentProcess().Threads.Count.ToString());
                    //Process[] processes = Process.GetProcesses();
                    //processes.ToList().ForEach(item => Console.WriteLine(item));

                }
                finally
                {
                    IsActivity = false;
                    Monitor.Exit(syncTimedEvent);
                }
            } 
        }

        public async void CreateTaskHandler(SortedSet<ReminderItem> reminderItems)
        {
            TaskHandler taskHandler = new TaskHandler(reminderItems);
            await Task.Run(taskHandler.ProcessReminders);

            //Thread newThread = new Thread(taskHandler.ProcessReminders);
            //newThread.Start();
        }

    }
}