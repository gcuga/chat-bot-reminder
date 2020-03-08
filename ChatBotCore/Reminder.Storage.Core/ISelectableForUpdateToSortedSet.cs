using System;
using System.Collections.Generic;

namespace Reminder.Storage.Core
{
    public interface ISelectableForUpdateToSortedSet<T>
    {
        SortedSet<T> SelectForUpdateSkipLocked(Func<T, bool> predicate, out string threadName);
    }
}
