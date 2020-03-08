using System;
using System.Collections.Generic;
using Reminder.Storage.Core;
using Reminder.Storage.Entity;
using System.Linq;
using System.Threading;

namespace Reminder.Storage.InMemory
{
    public class ReminderItemsInMemory : StorageInMemoryBase<ReminderItem>, ISelectableForUpdateToSortedSet<ReminderItem>
    {
        private static volatile ReminderItemsInMemory _instance;
        private static readonly object syncRoot = new Object();

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

        public SortedSet<ReminderItem> SelectForUpdateSkipLocked(Func<ReminderItem, bool> predicate,
            out string threadName)
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = Guid.NewGuid().ToString();
            }

            threadName = Thread.CurrentThread.Name;
            IList<ReminderItem> processingList = base._storage
                .Values
                .Where(predicate)
                .OrderBy(x => x)
                .ToList();

            SortedSet<ReminderItem> result = new SortedSet<ReminderItem>();
            foreach (var item in processingList)
            {
                if (Monitor.TryEnter(item.SyncProcessing))
                {
                    try
                    {
                        if (!item.IsProcessing)
                        {
                            result.Add(item);
                            item.IsProcessing = true;
                            item.ThreadGuid = threadName;
                        }
                    }
                    finally
                    {
                        Monitor.Exit(item.SyncProcessing);
                    }
                }
            }

            return result;
        }
    }
}
