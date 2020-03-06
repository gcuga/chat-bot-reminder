using System;
using Reminder.Storage.Entity;

namespace Reminder.Storage.InMemory
{
    public sealed class UsersInMemory : StorageInMemoryBase<User>
    {
        private static volatile UsersInMemory _instance;
        private static object syncRoot = new Object();

        public static UsersInMemory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new UsersInMemory();
                    }
                }

                return _instance;
            }
        }

        private UsersInMemory()
        {
        }

        protected override long GetIdFromItem(User item)
        {
            return item.Id;
        }
    }
}
