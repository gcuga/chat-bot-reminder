using System;
using Reminder.Storage.Entity;


namespace Reminder.Storage.InMemory
{
    public sealed class CategoriesInMemory : StorageInMemoryBase<Category>
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

        protected override long GetIdFromItem(Category item)
        {
            return item.Id;
        }
    }
}
