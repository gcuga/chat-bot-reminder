using System;
using Reminder.Storage.Core;

namespace Reminder.Storage.Entity
{
    public class User : EntityBase<User>
    {
        public string Name { get; set; }
        public User(ICruStorage<User> storage, string name) : base(storage)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
