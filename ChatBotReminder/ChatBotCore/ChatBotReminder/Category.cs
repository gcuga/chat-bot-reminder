using System;

namespace ChatBotReminder
{
    class Category
    {
        public long Id { get; private set; }
        public string Name { get; set; }

        public Category(long id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

    }
}
