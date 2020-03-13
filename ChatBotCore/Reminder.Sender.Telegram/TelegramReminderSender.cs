using Reminder.Sender.Core;
using System;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Reminder.Sender.Telegram
{
    public class TelegramReminderSender : IReminderSender
    {
        private TelegramBotClient _botClient;

        public TelegramReminderSender(string token, IWebProxy proxy = null)
        {
            _botClient = (proxy != null) ?
                new TelegramBotClient(token, proxy) :
                new TelegramBotClient(token);
        }
        public (SendingResultTypes, AggregateException) Send(string contactId, string message)
        {
            SendingResultTypes result;
            AggregateException aggregateException;
            var chatId = new ChatId(long.Parse(contactId));
            using (Task<Message> task = _botClient.SendTextMessageAsync(chatId, message))
            {
                task.Wait(TimeSpan.FromSeconds(15));
                if (task.IsCompleted)
                {
                    if (task.Result.Text.ToString() == message)
                    {
                        result = SendingResultTypes.Success;
                    }
                    else
                    {
                        result = SendingResultTypes.WrongText;
                    }
                }
                else
                {
                    result = SendingResultTypes.Failed;
                }

                aggregateException = task.Exception;
            }
            return (result, aggregateException);
        }
    }
}
