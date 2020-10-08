using System;
using System.IO;
using Calendar.InfoPanel.Utils;
using SpeechKitApi.Utils;
using UnityEngine;
using Utils;

using CalendarParams = Calendar.Utils.Params;

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

        public string headerInfo;
        [TextArea]
        public string textInfo;
        public Sprite sprite;
        public AudioClip clip;
        
        [SerializeField] private string stringDate;
        
        public string SourceId { get; private set; }
        public DateTime DateTime { get; private set; }
        
        public CalendarEventData(CalendarEventTypes newCalendarEventType, DateTime newDateTime)
        {
            calendarEventType = newCalendarEventType;
            DateTime = newDateTime;
            stringDate = newDateTime.ToString(DateTimeExtensions.ConvertDateTimeFormat);
            
            SourceId = GenerateSourceId(this);
        }
        
        /// <summary>
        /// Инициализация данных: формирует дату из строкового представления
        /// </summary>
        public void Initialize()
        {
            DateTime = stringDate.GetDateTime();
            
            SourceId = GenerateSourceId(this);
            Directory.CreateDirectory($"{CalendarParams.SoundGenerateFolder}\\{SourceId}");
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

        /// <summary>
        ///  
        /// </summary>
        public static string GenerateSourceId(CalendarEventData data)
            => $"{data.calendarEventType}\\{data.DateTime:dd_MM_yyyy}\\{data.headerInfo.GetValidPathString()}";
    }
}