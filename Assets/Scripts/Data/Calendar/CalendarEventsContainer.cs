using System;
using Calendar.InfoPanel.Utils;
using UnityEngine;
using Utils;

namespace Data.Calendar
{
    [CreateAssetMenu(fileName = "CalendarEvents", menuName = "Data/CalendarEventsContainer", order = 0)]
    public class CalendarEventsContainer : ScriptableObject
    {
        [ArrayElementTitle(new [] {"stringDate", "calendarEventType"})]
        public CalendarEventData[] datas;

        /// <summary>
        /// Формирует DateTime из stringDate
        /// </summary>
        public void Initialize()
        {
            foreach (var data in datas)
                data.Initialize();
        }

        /// <summary>
        /// Добавляет новый элемент
        /// </summary>
        public void AddNew(CalendarEventData newEventData)
        {
            Array.Resize(ref datas, datas.Length + 1);
            datas[datas.Length - 1] = newEventData;
        }
    }

    /// <summary>
    /// Календарные данные, выводимые на информационную панель
    /// </summary>
    [Serializable]
    public class CalendarEventData
    {
        public CalendarEventTypes calendarEventType;

        [TextArea]
        public string textInfo;
        public Sprite sprite;
        
        [SerializeField] private string stringDate;
        
        public DateTime DateTime { get; private set; }
        
        public CalendarEventData(CalendarEventTypes newCalendarEventType, DateTime newDateTime)
        {
            calendarEventType = newCalendarEventType;
            DateTime = newDateTime;
            stringDate = newDateTime.ToString(DateTimeExtensions.ConvertDateTimeFormat);
        }
        
        /// <summary>
        /// Инициализация данных: формирует дату из строкового представления
        /// </summary>
        public void Initialize()
        {
            DateTime = stringDate.GetDateTime();
        }

        /// <summary>
        /// Принадлежит ли событие указанной дате 
        /// </summary>
        public bool ThisDay(DateTime compareDate)
            => DateTime.Date == compareDate.Date;

        /// <summary>
        /// Принадлежит ли событие месяцу указанной даты 
        /// </summary>
        public bool ThisMonth(DateTime compareDate)
            => DateTime.Year == compareDate.Year && DateTime.Month == compareDate.Month;
    }
}