using System;
using System.Threading.Tasks;

namespace Reminder.Sender.Core
{
    public interface IReminderSender
    {
        (SendingResultTypes, AggregateException) Send(string contactId, string message);
    }
}
