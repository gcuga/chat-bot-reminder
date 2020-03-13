using Reminder.Parameters;
using Reminder.Receiver.Core;
using Reminder.Storage.Entity;
using Reminder.Parsing;
using System;
using Reminder.Storage.Core;
using Reminder.Sender.Core;

namespace Reminder.Receiver.Domain
{
    public class ReceiverDomain
    {
        private IReminderReceiver _receiver = ServiceParameters._receiver;
        private ICruStorage<User> _userStorage = ServiceParameters._userStorage;
        private ICruStorage<Category> _categoryStorage = ServiceParameters._categoryStorage;
        private ICruStorage<ReminderItem> _reminderStorage = ServiceParameters._reminderStorage;
        private IReminderSender Sender { get; } = ServiceParameters._sender;

        public ReceiverDomain()
        {
            _receiver.MessageReceived += ReceiverOnMessageReceived;
        }

        public void Run()
        {
            _receiver.Run();
        }

        private void ReceiverOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
			// parsing of the e.Message to get
			var parsedMessage = MessageParser.ParseMessage(e.Message);
			if (parsedMessage == null)
			{
                // we can raise some MessageParsingFailed event
                SendResponse($"Parsing {e.Message} has failed", e);
				return;
			}

			string alarmMessage = parsedMessage.Message;
			DateTimeOffset alarmDate = parsedMessage.Date;

			// adding new reminder item to the storage
            // пользователя и категорию нужно проверять на наличие в таблицах
            // пока не реализовано
            User user = new User(_userStorage, "ReminderBot", e.ContactId);
            Category category = new Category(_categoryStorage, "General events");

            _reminderStorage.Insert(new ReminderItem(
                storage: _reminderStorage,
                user: user,
                category: category,
                isActive: true,
                frequencyType: FrequencyTypes.None,
                dateBegin: null,
                dateGoal: alarmDate,
                message: alarmMessage));

            // send message that reminder item was added
            SendResponse("Reminder was added", e);
        }

        private void SendResponse(string message, MessageReceivedEventArgs e)
        {
            try
            {
                (SendingResultTypes sendingResult, AggregateException aggregateException) =
                    Sender.Send(e.ContactId, message);
            }
            catch (Exception)
            {
                // Если ответ в чат о успешном или не успешном добавлении
                // напоминания не прошел, не делаем ничего
            }
        }
    }
}
