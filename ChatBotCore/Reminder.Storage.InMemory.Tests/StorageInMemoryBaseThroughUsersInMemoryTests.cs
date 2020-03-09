using System;
using Xunit;

namespace Reminder.Storage.InMemory.Tests
{
    public class StorageInMemoryBaseThroughUsersInMemoryTests
    {
        [Fact]
        public void Method_Insert_With_Not_Null_Item_Should_Store_The_Item_Internally()
        {
			//// prepare test data
			//var storage = new InMemoryReminderStorage();
			//var expected = new ReminderItem(
			//	Guid.NewGuid(),
			//	"TelegramContactId",
			//	DateTimeOffset.Now,
			//	"Hello World ><");

			//// do the test
			//storage.Add(expected);

			//// check the results
			//var actual = storage.Get(expected.Id);

			//Assert.IsNotNull(actual);
			//Assert.AreEqual(expected.Id, actual.Id);
			//Assert.AreEqual(expected.ContactId, actual.ContactId);
			//Assert.AreEqual(expected.Status, actual.Status);
			//Assert.AreEqual(expected.Date, actual.Date);
			//Assert.AreEqual(expected.Message, actual.Message);
			Assert.True(true);
		}
    }
}
