using System;
using System.Collections.Generic;
using Reminder.Storage.Core;
using Reminder.Storage.Entity;
using System.Linq;

namespace Reminder.Storage.InMemory
{
    class ReminderItemsInMemory : StorageInMemoryBase<ReminderItem>, IQueryableToSortedSet<ReminderItem>
    {
        private static volatile ReminderItemsInMemory _instance;
        private static object syncRoot = new Object();

        public static ReminderItemsInMemory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new ReminderItemsInMemory();
                    }
                }

                return _instance;
            }
        }

        private ReminderItemsInMemory()
        {
        }

        protected override long GetIdFromItem(ReminderItem item)
        {
            return item.Id;
        }

        public SortedSet<ReminderItem> Get(Func<ReminderItem, bool> predicate)
        {
            return ReminderItems
    .Values
    .Where((x) => x.Status == status)
    .ToList();
        }
    }
}
