using System;
using Calendar.InfoPanel.Utils;
using Extensions;
using SpeechKitApi.Utils;
using UnityEngine;

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
        /// <summary>
        /// УИД, который является и относительным путем к аудио
        /// </summary>
        public string SourceId { get; private set; }
        
        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime DateTime { get; private set; }
        
        /// <summary>
        /// Необходимо ли сохранять в PlayerPrefs 
        /// </summary>
        public bool NeedSave { get; }
        
        /// <summary>
        /// Тип события
        /// </summary>
        public CalendarEventTypes calendarEventType;

        /// <summary>
        /// Заголовок
        /// </summary>
        public string headerInfo;
        
        /// <summary>
        /// Текстовая заметка 
        /// </summary>
        [TextArea]
        public string textInfo;
        
        /// <summary>
        /// Картинка
        /// </summary>
        public Sprite sprite;
        
        /// <summary>
        /// Строковое представление даты, которое сериализуется в Дату
        /// </summary>
        [SerializeField] private string stringDate;
        
        /// <summary>
        /// Инициализаця нового элемента, который будем сохранять в настройки 
        /// </summary>
        public CalendarEventData(CalendarEventTypes newCalendarEventType, DateTime newDateTime)
        {
            calendarEventType = newCalendarEventType;
            DateTime = newDateTime;
            stringDate = newDateTime.ToString(DateTimeExtensions.ConvertDateTimeFormat);
            headerInfo = "New node";
            
            SourceId = GenerateSourceId(this);
            NeedSave = true;
        }
        
        /// <summary>
        /// Инициализация данных: формирует дату из строкового представления
        /// </summary>
        public void Initialize()
        {
            DateTime = stringDate.GetDateTime();
            SourceId = GenerateSourceId(this);
        }

        /// <summary>
        /// Принадлежит ли событие указанной дате (совпадения по дню и месяцу)
        /// </summary>
        public bool ThisDayAndMonth(DateTime compareDate)
          => DateTime.Day == compareDate.Day && DateTime.Month == compareDate.Month;
        
        /// <summary>
        /// Принадлежит ли событие месяцу указанной даты 
        /// </summary>
        public bool ThisMonthAndYear(DateTime compareDate)
            => DateTime.Year == compareDate.Year && DateTime.Month == compareDate.Month;/// <summary>
        
        /// Принадлежит ли событие месяцу указанной даты 
        /// </summary>
        public bool ThisMonthOnly(DateTime compareDate)
            => DateTime.Month == compareDate.Month;
        
        /// <summary>
        /// Генерирует УИД события, который является относительным путем к аудио-сообщению
        /// </summary>
        public static string GenerateSourceId(CalendarEventData data)
            => $"{data.calendarEventType}\\{data.DateTime:dd_MM_yyyy}\\{data.headerInfo.GetValidPathString()}";
    }
}