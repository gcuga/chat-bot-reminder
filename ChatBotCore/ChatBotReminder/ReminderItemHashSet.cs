using System;
using Reminder.Storage.Core;
using System.Collections.Generic;
using System.Linq;

namespace ChatBotReminder
{
    // контейнер с данными
    static class ReminderItemHashSet
    {
        public static HashSet<ReminderItem> Reminders { get; set; }

        static ReminderItemHashSet()
        {
            PopulateData();
        }

        public static void PopulateData()
        {
            User user = new User(3456436, "User");
            Category category = new Category(1, "Мероприятия");
            Reminders = new HashSet<ReminderItem>();

            Reminders.Add(new ReminderItem(id: 10,
                                           user: user,
                                           category: category,
                                           isActive: true,
                                           frequencyType: FrequencyTypes.None,
                                           dateBegin: null,
                                           dateGoal: DateTimeOffset.Now.AddSeconds(70),
                                           message: "Митап",
                                           creationDate: DateTimeOffset.Now,
                                           nextReminderDate: null,
                                           false));

            Reminders.Add(new ReminderItem(id: 20,
                                           user: user,
                                           category: category,
                                           isActive: true,
                                           frequencyType: FrequencyTypes.EveryHour,
                                           dateBegin: DateTimeOffset.Now.AddHours(3),
                                           dateGoal: DateTimeOffset.Now.AddDays(2).AddHours(-2),
                                           message: "Кадждый час",
                                           creationDate: DateTimeOffset.Now,
                                           nextReminderDate: null,
                                           false));

            Reminders.Add(new ReminderItem(id: 30,
                                           user: user,
                                           category: category,
                                           isActive: true,
                                           frequencyType: FrequencyTypes.None,
                                           dateBegin: null,
                                           dateGoal: DateTimeOffset.Now.AddSeconds(90),
                                           message: "Кинотеатр",
                                           creationDate: DateTimeOffset.Now,
                                           nextReminderDate: null,
                                           false));

            for (int i = 0; i < 9; i++)
            {
                Random random = new Random();
                Reminders.Add(new ReminderItem(id: 40 + (i * 10),
                                               user: user,
                                               category: category,
                                               isActive: true,
                                               frequencyType: FrequencyTypes.None,
                                               dateBegin: null,
                                               dateGoal: DateTimeOffset.Now.AddSeconds(random.Next(1, 60)),
                                               message: $"random {+i}",
                                               creationDate: DateTimeOffset.Now,
                                               nextReminderDate: null,
                                               false));
            }


            foreach (var item in Reminders)
            {
                if (item.Id == 60 || item.Id == 40)
                {
                    continue;
                }

                item.NextReminderDate = ReminderItem.CalculateNextReminderDate(
                    item.IsActive,
                    item.FrequencyType,
                    item.DateBegin,
                    item.DateGoal,
                    DateTimeOffset.Now);
            }

            foreach (var item in Reminders)
            {
                Console.WriteLine($"{item.Id}\t{item.Message}" +
                    $"\tcurrent NextReminderDate {item.NextReminderDate}\tdate goal {item.DateGoal}");

            }

        }

        public static SortedSet<ReminderItem> GetReminderSet()
        {
            Console.WriteLine();
            Console.WriteLine("GetReminderSet");

            SortedSet<ReminderItem> reminderItems = new SortedSet<ReminderItem>();
            //DateTimeOffset now = DateTimeOffset.Now;

            // имитируем выборку поподающих в список обработки напоминаний
            foreach (var item in Reminders)
            {
                if (item.IsProcessing == true)
                {
                    continue;
                }

                if (item.NextReminderDate == null || item.NextReminderDate <=  (now + ServiceParameters._taskHandlerTimeScope))
                {
                    item.IsProcessing = true;
                    reminderItems.Add(item);
                }
            }


            foreach (var item in reminderItems)
            {
                Console.WriteLine($"{item.Id}\t {item.Message}" +
                    $"\t current NextReminderDate {item.NextReminderDate}\t date goal {item.DateGoal}");
            }
            Console.WriteLine();

            return reminderItems;
        }
    }
}
