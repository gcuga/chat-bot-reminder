using System;
using Reminder.Storage.Core;

namespace Reminder.Storage.Entity
{
    public class Category : EntityBase<Category>
    {
        public string Name { get; set; }

        public Category(ICruStorage<Category> storage, string name) : base(storage)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
