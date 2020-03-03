using System;

namespace ChatBotReminder
{
    class User
    {
        public long Id { get; private set; }
        public string Name { get; set; }

        public User(long id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
