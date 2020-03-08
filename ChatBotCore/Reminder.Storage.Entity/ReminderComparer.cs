using System;
using System.Collections.Generic;

namespace Reminder.Storage.Entity
{
    public class ReminderComparer : IComparer<ReminderItem>
    {
        int IComparer<ReminderItem>.Compare(ReminderItem x, ReminderItem y)
        {
            return x.CompareTo(y);
        }
    }
}
