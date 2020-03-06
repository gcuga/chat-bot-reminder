using System;
using Reminder.Storage.Core;
using Reminder.Storage.Entity;

namespace Reminder.Storage.InMemory
{
    public sealed class UsersInMemory : StorageInMemoryBase, ICruStorage<User>
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

        public new User Get(long id)
        {
            return (User)base.Get(id);
        }

        public bool TryGet(long id, out User item)
        {
            bool isSuccessful = base.TryGet(id, out EntityBase entity);
            item = (User)entity;
            return isSuccessful;
        }

        public void Insert(User item)
        {
            base.Insert(item);
        }

        public void Update(User item)
        {
            base.Update(item);
        }

        public void Upsert(User item)
        {
            base.Upsert(item);
        }
    }
}
