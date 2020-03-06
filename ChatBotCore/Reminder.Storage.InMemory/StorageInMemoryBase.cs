using System;
using Reminder.Storage.Core;
using System.Collections.Generic;

namespace Reminder.Storage.InMemory
{
    public abstract class StorageInMemoryBase<T> : ICruStorage<T>
    {
        private long Sequence { get; set; }
        internal readonly Dictionary<long, T> _storage;
        protected StorageInMemoryBase()
        {
            _storage = new Dictionary<long, T>();
            Sequence = 0;
        }

        protected abstract long GetIdFromItem(T item);

        public long GetNextId()
        {
            return ++Sequence;
        }

        public T Get(long id)
        {
            return _storage[id];
        }

        public bool TryGet(long id, out T item)
        {
            return _storage.TryGetValue(id, out item);
        }

        public virtual void Insert(T item)
        {
            _storage.Add(GetIdFromItem(item), item);
        }

        public void Update(T item)
        {
            long id = GetIdFromItem(item);
            if (_storage.ContainsKey(id))
            {
                _storage[id] = item;
            }
            else
            {
                throw new KeyNotFoundException($"Record with ID = {id} not found");
            }
        }

        public void Upsert(T item)
        {
            _storage[GetIdFromItem(item)] = item;
        }
    }
}
