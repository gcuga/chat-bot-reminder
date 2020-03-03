using System;
using System.Collections;
using System.Collections.Generic;

namespace ChatBotReminder
{
    /// <summary>
    /// Напоминание.
    /// <value><c>_isActive</c> - true напоминание активно, false неактивно</value>
    /// <value><c>_frequencyType</c> - частота напоминания, если установлено в None напоминание однократное</value>
    /// <value><c>_dateBegin</c> - дата с которой напоминание начинает отправляться, для однократных не задается,
    /// усекается до дня если периодичность месяц, неделя, день, усекается до часа если напоминание ежечасное</value>
    /// <value><c>_dateGoal</c> - целевая дата напоминания, для бесконечно повторяющихся напоминаний не задается,
    /// время периодических напоминаний расчитывается по времени целевой даты</value>
    /// <value><c>_nextReminderDate</c> - следующая дата отправки, устанавливается обработчиком напоминаний</value>
    /// <value><c>isProcessing</c> - флаг забрано в обработку обработчиком</value>
    /// </summary>
    class ReminderItem : IComparable
    {
        private long _id;
        private User _user;
        private Category _category;
        private bool _isActive;
        private FrequencyTypes _frequencyType;
        private DateTimeOffset? _dateBegin; // возможно значение null
        private DateTimeOffset? _dateGoal; // возможно значение null
        private string _message;
        private DateTimeOffset _creationDate;
        private DateTimeOffset? _nextReminderDate; // возможно значение null
        private bool _isProcessing; // пока непонятно как реализовывать согласованность на всех уровнях, между потоками и в субд

        public long Id { get => _id; private set => _id = value; }
        public DateTimeOffset? NextReminderDate { get => _nextReminderDate; set => _nextReminderDate = value; }
        public bool IsActive { get => _isActive; set => _isActive = value; }
        public FrequencyTypes FrequencyType { get => _frequencyType; set => _frequencyType = value; }
        public DateTimeOffset? DateBegin { get => _dateBegin; set => _dateBegin = value; }
        public DateTimeOffset? DateGoal { get => _dateGoal; set => _dateGoal = value; }
        public string Message { get => _message; set => _message = value; }
        public bool IsProcessing { get => _isProcessing; set => _isProcessing = value; }

        internal User User { get => _user; set => _user = value; }
        internal Category Category { get => _category; set => _category = value; }
        public DateTimeOffset CreationDate { get => _creationDate; set => _creationDate = value; }

        public long GetUserId()
        {
            return _user.Id;
        }

        public string GetCategoryName()
        {
            return _category.Name;
        }

        public static DateTimeOffset? CalculateNextReminderDate(bool isActive,
                                                                FrequencyTypes frequencyType,
                                                                DateTimeOffset? dateBegin,
                                                                DateTimeOffset? dateGoal,
                                                                DateTimeOffset compareWithDate)
        {
            // compareWithDate - предполагается что это дата текущего запуска,
            // т.е. предыдущая nextReminderDate или дата добавления напоминания,
            // если отправлений напоминания еще не было
            DateTimeOffset? nextReminderDate = null;
            if (!isActive)
            {
                // напоминание неактивно
                return nextReminderDate;
            }

            if (dateGoal != null && compareWithDate > dateGoal)
            {
                // целевая дата уже прошла, новой даты следующей отправки нет
                return nextReminderDate;
            }

            if (frequencyType == FrequencyTypes.None)
            {
                // напоминание однократное, если целевая дата не достигнута,
                // то возвращаем ее в качестве даты следующей отправки напоминания
                nextReminderDate = dateGoal;
            }
            else if (frequencyType == FrequencyTypes.EveryHour)
            {
                if (dateGoal != null)
                {
                    // если compareWithDate в том же часу что и dateGoal, то новой даты нет
                    DateTimeOffset d = compareWithDate;
                    DateTimeOffset dG = dateGoal.Value;
                    if (d.Year == dG.Year && d.Month == dG.Month && d.Day == dG.Day && d.Hour == dG.Hour)
                    {
                        return nextReminderDate;
                    }
                }

                if (dateBegin != null && dateBegin >= compareWithDate)
                {
                    // напоминание начинает отправляться после достижения даты начала
                    // по времени целевой даты, если она задана, иначе по времени даты начала
                    if (dateGoal != null)
                    {
                        DateTimeOffset dB = dateBegin.Value;
                        DateTimeOffset dG = dateGoal.Value;
                        nextReminderDate = new DateTimeOffset(dB.Year, dB.Month, dB.Day, dB.Hour, dG.Minute, dG.Second, dG.Offset);
                    }
                    else
                    {
                        nextReminderDate = dateBegin;
                    }
                }
                else
                {
                    // дата отправки в следующем часу после compareWithDate, время по целевой дате, если не задана
                    // то по дате начала, если не задана то усеченное до часа
                    DateTimeOffset d = compareWithDate.AddHours(1);
                    DateTimeOffset dtime;
                    if (dateGoal != null)
                    {
                        dtime = dateGoal.Value;
                    }
                    else if (dateBegin != null)
                    {
                        dtime = dateBegin.Value;
                    }
                    else
                    {
                        dtime = new DateTimeOffset(d.Year, d.Month, d.Day, d.Hour, 0, 0, d.Offset);
                    }

                    nextReminderDate = new DateTimeOffset(d.Year, d.Month, d.Day, d.Hour, dtime.Minute, dtime.Second, d.Offset);
                }
            }
            else if (frequencyType == FrequencyTypes.EveryDay)
            {
                if (dateGoal != null)
                {
                    // если compareWithDate в том же дне что и dateGoal, то новой даты нет
                    DateTimeOffset d = compareWithDate;
                    DateTimeOffset dG = dateGoal.Value;
                    if (d.Year == dG.Year && d.Month == dG.Month && d.Day == dG.Day)
                    {
                        return nextReminderDate;
                    }
                }

                if (dateBegin != null && dateBegin >= compareWithDate)
                {
                    // напоминание начинает отправляться после достижения даты начала
                    // по времени целевой даты, если она задана, иначе по времени даты начала
                    if (dateGoal != null)
                    {
                        DateTimeOffset dB = dateBegin.Value;
                        DateTimeOffset dG = dateGoal.Value;
                        nextReminderDate = new DateTimeOffset(dB.Year, dB.Month, dB.Day, dG.Hour, dG.Minute, dG.Second, dG.Offset);
                    }
                    else
                    {
                        nextReminderDate = dateBegin;
                    }
                }
                else
                {
                    // дата отправки в следующий день после compareWithDate, время по целевой дате, если не задана
                    // то по дате начала, если не задана то в 12 часов
                    DateTimeOffset d = compareWithDate.AddDays(1);
                    DateTimeOffset dtime;
                    if (dateGoal != null)
                    {
                        dtime = dateGoal.Value;
                    }
                    else if (dateBegin != null)
                    {
                        dtime = dateBegin.Value;
                    }
                    else
                    {
                        dtime = new DateTimeOffset(d.Year, d.Month, d.Day, 12, 0, 0, d.Offset);
                    }

                    nextReminderDate = new DateTimeOffset(d.Year, d.Month, d.Day, dtime.Hour, dtime.Minute, dtime.Second, d.Offset);
                }
            }

            return nextReminderDate;
        }

        public int CompareTo(object obj)
        {
            // nulls first
            if (obj == null) 
            {
                return 1;
            }

            ReminderItem otherReminderItem = obj as ReminderItem;
            if (otherReminderItem != null)
            {
                if (this._id == otherReminderItem._id)
                {
                    return 0;
                }

                if (this._nextReminderDate == null && otherReminderItem._nextReminderDate == null)
                {
                    return this._id.CompareTo(otherReminderItem._id);
                }

                if (otherReminderItem._nextReminderDate == null)
                {
                    return 1;
                }

                if (this._nextReminderDate == null)
                {
                    return -1;
                }

                return this._nextReminderDate.Value.CompareTo(otherReminderItem._nextReminderDate.Value);
            }
            else
            {
                throw new ArgumentException("Object is not a ReminderItem");
            }
        }

        public ReminderItem(long id,
                            User user,
                            Category category,
                            bool isActive,
                            FrequencyTypes frequencyType,
                            DateTimeOffset? dateBegin,
                            DateTimeOffset? dateGoal,
                            string message,
                            DateTimeOffset creationDate,
                            DateTimeOffset? nextReminderDate,
                            bool isProcessing)
        {
            _id = id;
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _category = category ?? throw new ArgumentNullException(nameof(category));
            _isActive = isActive;
            _frequencyType = frequencyType;
            _dateBegin = dateBegin;
            _dateGoal = dateGoal;
            _message = message ?? throw new ArgumentNullException(nameof(message));
            _creationDate = creationDate;
            _nextReminderDate = nextReminderDate;
            _isProcessing = isProcessing;
        }



    }
}
