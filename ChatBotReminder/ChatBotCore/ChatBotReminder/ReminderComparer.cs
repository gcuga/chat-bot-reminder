using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBotReminder
{
    class ReminderComparer : IComparer<ReminderItem>
    {
        int IComparer<ReminderItem>.Compare(ReminderItem x, ReminderItem y)
        {
            return x.CompareTo(y);
        }
    }
}
