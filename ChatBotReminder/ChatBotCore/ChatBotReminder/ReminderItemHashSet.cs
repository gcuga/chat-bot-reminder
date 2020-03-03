using System;
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
                                           dateGoal: DateTimeOffset.Now.AddSeconds(120),
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
                                           dateGoal: DateTimeOffset.Now.AddSeconds(180),
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
                                               dateGoal: DateTimeOffset.Now.AddSeconds(random.Next(5, 200)),
                                               message: $"random {+i}",
                                               creationDate: DateTimeOffset.Now,
                                               nextReminderDate: null,
                                               false));
            }


            foreach (var item in Reminders)
            {
                if (item.Id == 60 || item.Id == 120)
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
        }

        public static SortedSet<ReminderItem> GetReminderSet()
        {
            SortedSet<ReminderItem> reminderItems = new SortedSet<ReminderItem>(Reminders);
            foreach (var item in reminderItems)
            {
                Console.WriteLine($"{item.Id} {item.Message}" +
                    $" current NextReminderDate {item.NextReminderDate} date goal {item.DateGoal}");
            }
            Console.WriteLine();

            return reminderItems;
        }
    }
}
