using System;

namespace Reminder.Parsing
{
    public class ParsedMessage
    {
        public string Message { get; set; }
        public DateTimeOffset Date { get; set; }

        public ParsedMessage(string message, DateTimeOffset date)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Date = date;
        }
    }
}
