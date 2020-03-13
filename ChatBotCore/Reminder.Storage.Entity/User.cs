using System;
using Reminder.Storage.Core;

namespace Reminder.Storage.Entity
{
    public class User : EntityBase<User>
    {
        public string Name { get; set; }
        public string ChatId { get; set; }
        public User(ICruStorage<User> storage, string name, string chatId) : base(storage)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ChatId = chatId ?? throw new ArgumentNullException(nameof(chatId));
        }
    }
}
