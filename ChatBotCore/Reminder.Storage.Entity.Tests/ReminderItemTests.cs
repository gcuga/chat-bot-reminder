using System;
using Xunit;
using Reminder.Storage.Entity;
using System.Globalization;

namespace Reminder.Storage.Entity.Tests
{
    public class ReminderItemTests
    {
        [Theory]
        [InlineData("09.03.2020 23:07:24.968 +03:00", "09.03.2020 23:07:24.968 +03:00")]
        [InlineData("09.03.2020 23:07:24.967 +03:00", "09.03.2020 23:07:24.968 +03:00")]
        [InlineData("09.03.2020 23:07:00.000 +03:00", "09.03.2020 23:07:24.968 +03:00")]
        public void CalculateNextReminderDate_Should_Return_Null_For_Active_Not_Null_DateGoal_Less_Or_Equal_compareWithDate(
            string stringDateGoal, string stringCompareWithDate)
        {
            // prepare test data
            DateTimeOffset.TryParseExact(stringDateGoal,
                "dd.MM.yyyy HH:mm:ss.fff zzz",
                CultureInfo.InvariantCulture.DateTimeFormat,
                DateTimeStyles.None,
                out DateTimeOffset dateGoal);
            DateTimeOffset.TryParseExact(stringCompareWithDate,
                "dd.MM.yyyy HH:mm:ss.fff zzz",
                CultureInfo.InvariantCulture.DateTimeFormat,
                DateTimeStyles.None,
                out DateTimeOffset compareWithDate);

            // do the test
            DateTimeOffset? actual = ReminderItem.CalculateNextReminderDate(true,
                FrequencyTypes.None,
                null,
                dateGoal,
                compareWithDate);

            // check the results
            Assert.Null(actual);
        }
    }
}
