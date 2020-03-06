using System;
using Reminder.Storage.Core;
using Reminder.Storage.Entity;
using System.Collections.Generic;

namespace Reminder.Storage.InMemory
{
    public abstract class StorageInMemoryBase
    {
        private long Sequence { get; set; }
        internal readonly Dictionary<long, EntityBase<User>> _storage;
        protected StorageInMemoryBase()
        {
            _storage = new Dictionary<long, EntityBase>();
            Sequence = 0;
        }

        public long GetNextId()
        {
            return ++Sequence;
        }

        protected EntityBase Get(long id)
        {
            return _storage[id];
        }

        protected bool TryGet(long id, out EntityBase item)
        {
            return _storage.TryGetValue(id, out item);
        }

        protected void Insert(EntityBase item)
        {
            _storage.Add(item.Id, item);
        }

        protected void Update(EntityBase item)
        {
            if (_storage.ContainsKey(item.Id))
            {
                _storage[item.Id] = item;
            }
            else
            {
                throw new KeyNotFoundException($"Record with ID = {item.Id} not found");
            }
        }

        protected void Upsert(EntityBase item)
        {
            _storage[item.Id] = item;
        }
    }
}
