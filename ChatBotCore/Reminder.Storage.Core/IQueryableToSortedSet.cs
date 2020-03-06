using System;
using System.Collections.Generic;

namespace Reminder.Storage.Core
{
    public interface IQueryableToSortedSet<T>
    {
        SortedSet<T> Get(Func<T, bool> predicate);
    }
}
