using Reminder.Storage.Entity;
using System;
using System.Collections.Generic;
using Xunit;

namespace Reminder.Storage.InMemory.Tests
{
    public class ReminderItemsInMemoryTests
    {
        [Fact]
        public void SelectForUpdateSkipLocked_Should_Return_SortedSet_For_False_isProcessing_With_isProcessing_Set_To_True_And_ThreadGuid_Not_Null()
        {
            // prepare test data
            UsersInMemory userStorage = UsersInMemory.Instance;
            CategoriesInMemory categoryStorage = CategoriesInMemory.Instance;
            ReminderItemsInMemory reminderStorage = ReminderItemsInMemory.Instance;
            userStorage._storage.Clear();
            categoryStorage._storage.Clear();
            reminderStorage._storage.Clear();
            User user = new User(userStorage, "User", "2223322");
            Category category = new Category(categoryStorage, "Category");
            ReminderItem expectedReminder1 = new ReminderItem(
                storage: reminderStorage,
                user: user,
                category: category,
                isActive: false,
                frequencyType: FrequencyTypes.None,
                dateBegin: null,
                dateGoal: DateTimeOffset.Now.AddMinutes(10),
                message: "Remainder 1");
            ReminderItem expectedReminder2 = new ReminderItem(
                storage: reminderStorage,
                user: user,
                category: category,
                isActive: true,
                frequencyType: FrequencyTypes.None,
                dateBegin: null,
                dateGoal: DateTimeOffset.Now.AddMinutes(5),
                message: "Remainder 2");
            ReminderItem expectedReminder3 = new ReminderItem(
                storage: reminderStorage,
                user: user,
                category: category,
                isActive: true,
                frequencyType: FrequencyTypes.None,
                dateBegin: null,
                dateGoal: DateTimeOffset.Now.AddMinutes(5),
                message: "Remainder 3");
            reminderStorage.Insert(expectedReminder1);
            reminderStorage.Insert(expectedReminder2);
            reminderStorage.Insert(expectedReminder3);
            reminderStorage._storage[expectedReminder1.Id].IsProcessing = false;
            reminderStorage._storage[expectedReminder2.Id].IsProcessing = true;
            reminderStorage._storage[expectedReminder3.Id].IsProcessing = false;
            Func<ReminderItem, bool> predicate = (item) => true;

            // do the test
            SortedSet<ReminderItem> actual = reminderStorage.SelectForUpdateSkipLocked(predicate, out string threadName);

            // check the results
            Assert.Equal(2, actual.Count);
            Assert.True(actual.TryGetValue(expectedReminder1, out ReminderItem actualReminder1));
            Assert.False(actual.TryGetValue(expectedReminder2, out ReminderItem actualReminder2));
            Assert.True(actual.TryGetValue(expectedReminder3, out ReminderItem actualReminder3));
            Assert.True(actualReminder1.IsProcessing);
            Assert.Null(actualReminder2);
            Assert.True(actualReminder3.IsProcessing);
            Assert.NotNull(actualReminder1.ThreadGuid);
            Assert.NotNull(actualReminder3.ThreadGuid);
            Assert.Equal(threadName, actualReminder1.ThreadGuid);
            Assert.Equal(threadName, actualReminder3.ThreadGuid);
        }
    }
}
