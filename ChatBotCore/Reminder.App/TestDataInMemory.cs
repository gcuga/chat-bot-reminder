using System;
using System.Collections.Generic;
using Reminder.Storage.Entity;
using Reminder.Storage.Core;
using Reminder.Storage.InMemory;

namespace Reminder.App
{
    public static class TestDataInMemory
    {
        public static void Populate()
        {
            UsersInMemory userStorage = UsersInMemory.Instance;
            CategoriesInMemory categoryStorage = CategoriesInMemory.Instance;
            ReminderItemsInMemory reminderStorage = ReminderItemsInMemory.Instance;

            User user = new User(userStorage, "@DNReminderBot", "967061336");
            Category category = new Category(categoryStorage, "Мероприятия");

            reminderStorage.Insert(new ReminderItem(
                storage: reminderStorage,
                user: user,
                category: category,
                isActive: true,
                frequencyType: FrequencyTypes.None,
                dateBegin: null,
                dateGoal: DateTimeOffset.Now.AddSeconds(70),
                message: "Митап"));

            reminderStorage.Insert(new ReminderItem(
                storage: reminderStorage,
                user: user,
                category: category,
                isActive: true,
                frequencyType: FrequencyTypes.EveryHour,
                dateBegin: DateTimeOffset.Now.AddHours(3),
                dateGoal: DateTimeOffset.Now.AddDays(2).AddHours(-2),
                message: "Кадждый час"));

            reminderStorage.Insert(new ReminderItem(
                storage: reminderStorage,
                user: user,
                category: category,
                isActive: true,
                frequencyType: FrequencyTypes.None,
                dateBegin: null,
                dateGoal: DateTimeOffset.Now.AddSeconds(90),
                message: "Кинотеатр"));

            for (int i = 0; i < 30; i++)
            {
                Random random = new Random();
                reminderStorage.Insert(new ReminderItem(
                    storage: reminderStorage,
                    user: user,
                    category: category,
                    isActive: true,
                    frequencyType: FrequencyTypes.None,
                    dateBegin: null,
                    dateGoal: DateTimeOffset.Now.AddSeconds(random.Next(1, 120)),
                    message: $"random {+i}"));
            }

            ReminderItem r;
            for (int i = 1; i < 365; i++)
            {
                if (reminderStorage.TryGet(i, out r))
                {
                    Console.WriteLine($"{r.Id}\t IsActive = {r.IsActive}\t{r.NextReminderDate:HH:mm:ss}\t{r.ThreadGuid}\t{r.Message}");
                }
            }
        }
    }
}
