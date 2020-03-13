using Reminder.Receiver.Core;
using System;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Reminder.Receiver.Telegram
{
    public class TelegramReminderReceiver : IReminderReceiver
    {
        private TelegramBotClient _botClient;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public void Run()
        {
            _botClient.OnMessage += BotClientOnMessage;
            _botClient.StartReceiving();
        }

        private void BotClientOnMessage(object sender, global::Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Text)
            {
                OnMessageReceived(
                    this,
                    new MessageReceivedEventArgs(
                        e.Message.Text,
                        e.Message.Chat.Id.ToString()));
            }
        }

        public TelegramReminderReceiver(string token, IWebProxy proxy = null)
        {
            _botClient = (proxy != null)?
                new TelegramBotClient(token, proxy): new TelegramBotClient(token);
        }

        protected virtual void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(sender, e);
        }
    }
}
