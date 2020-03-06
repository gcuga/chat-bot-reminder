using System;
using Reminder.Storage.Core;

namespace Reminder.Storage.Entity
{
    public abstract class EntityBase<T>
    {
        public long Id { get; }

        protected EntityBase(ICruStorage<T> storage) => Id = storage.GetNextId();
    }
}
