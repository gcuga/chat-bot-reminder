using System;
using Reminder.Storage.Core;
using Reminder.Storage.Entity;


namespace Reminder.Storage.InMemory
{
    public sealed class CategoriesInMemory : StorageInMemoryBase, ICruStorage<Category>
    {
        private static volatile CategoriesInMemory _instance;
        private static object syncRoot = new Object();

        public static CategoriesInMemory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new CategoriesInMemory();
                    }
                }

                return _instance;
            }
        }

        private CategoriesInMemory()
        {
        }

        public new Category Get(long id)
        {
            return (Category)base.Get(id);
        }

        public bool TryGet(long id, out Category item)
        {
            bool isSuccessful = base.TryGet(id, out EntityBase entity);
            item = (Category)entity;
            return isSuccessful;
        }

        public void Insert(Category item)
        {
            base.Insert(item);
        }

        public void Update(Category item)
        {
            base.Update(item);
        }

        public void Upsert(Category item)
        {
            base.Upsert(item);
        }
    }
}
