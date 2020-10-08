using System;
using System.Linq;
using System.Threading.Tasks;
using Calendar.DatePanel;
using Calendar.InfoPanel;
using Calendar.InfoPanel.Utils;
using Core;
using Data.Calendar;
using SpeechKitApi;
using SpeechKitApi.Enums;
using SpeechKitApi.Models;
using SpeechKitApi.Models.TokenResources;
using UnityEngine;
using Utils;
using YandexSpeechKit;
using YandexSpeechKit.Utils;
using EventType = Core.EventType;

namespace Calendar
{
    public class MainPageController : MonoBehaviour
    {
        /// <summary>
        /// Текущая активная дата
        /// </summary>
        public static DateTime ActiveDate { get; private set; } = DateTime.Today;
        
        /// <summary>
        /// Текущий активный тип панели
        /// </summary>
        public static CalendarEventTypes ActiveCalendarEventType { get; private set; } = CalendarEventTypes.MilitaryMemoryDay;
        
        [Header("Контроллеры")] 
        [SerializeField] private DatePanelController datePanelController;
        [SerializeField] private InfoPanelController infoPanelController;
        
        [Header("Данные событий")] 
        [SerializeField] private CalendarEventsContainer holidayCalendarEvents;
        [SerializeField] private CalendarEventsContainer scheduledCalendarEvents;

        /// <summary>
        /// Массив данных событий
        /// </summary>
        private CalendarEventsContainer[] CalendarEventsContainers
            => new[] {holidayCalendarEvents, scheduledCalendarEvents};
        
        private void Start()
        {
            holidayCalendarEvents.Initialize();
            scheduledCalendarEvents.Initialize();
            
            EventManager.AddHandler(EventType.OnDateChanged, OnDateChanged);
            EventManager.AddHandler(EventType.OnCalendarEventTypeChanged, OnCalendarEventTypeChanged);
            EventManager.AddHandler(EventType.CalendarEventAdd, OnCalendarEventAdd);
            
            ShowMonth(DateTime.Now);

            //Task.Factory.StartNew(Execute);
        }
        
        private void OnDestroy()
        {
            EventManager.RemoveHandler(EventType.OnDateChanged, OnDateChanged);
            EventManager.RemoveHandler(EventType.OnCalendarEventTypeChanged, OnCalendarEventTypeChanged);
            EventManager.RemoveHandler(EventType.CalendarEventAdd, OnCalendarEventAdd);
        }

        /// <summary>
        /// Устанавливает настройки месяца на календаре и ифнормацйионной панеле
        /// </summary>
        private void ShowMonth(DateTime date)
        {
            datePanelController.Initialize(date);
            var monthEvents = holidayCalendarEvents.datas.GetMonthEvents(date)
                .Concat(scheduledCalendarEvents.datas.GetMonthEvents(date))
                .ToArray();
            infoPanelController.Initialize(date, monthEvents);
        }

        /// <summary>
        /// Событие, вызываемое при изменнии даты 
        /// </summary>
        private void OnDateChanged(params object[] args)
        {
            ActiveDate = (DateTime) args[0];
        }

        /// <summary>
        /// Событие, вызываемое при изменнии типа текущей панели 
        /// </summary>
        private void OnCalendarEventTypeChanged(params object[] args)
        {
            ActiveCalendarEventType = (CalendarEventTypes) args[0];
        }

        /// <summary>
        /// Событие, вызываемое при добавлении нового события 
        /// </summary>
        private void OnCalendarEventAdd(params object[] args)
        {
            var newCalendarEvent = (CalendarEventData) args[0];
            var index = (int) args[1];
            CalendarEventsContainers[index].AddNew(newCalendarEvent);
            infoPanelController.UpdateEvents(newCalendarEvent);
        }
        
        static void Execute()
        {
            var client = SpeechKitClient.Create(new OAuthToken {Key = ClientParams.OAuthKey});
            
            var options = new SynthesisOptions("Не забыть сдать Hедельный отчет", 0.75f, ClientParams.YandexCloudFolderId)
            {
                Emotion = Emotion.Good,
                Language = SynthesisLanguage.Russian,
                Quality = SynthesisQuality.High,
                Speaker = Speaker.Oksana,
                AudioFormat = SynthesisAudioFormat.Lpcm
            };
            var byteArray = client.GetSpeech(options).GetAwaiter().GetResult();
            //client.SaveSpeech(options, $"C:\\tmp").GetAwaiter().GetResult();
            
            client.Dispose();
            //ThreadManager.ExecuteOnMainThread(()=> AudioConverter.Execute(byteArray, options));
        }

        
    }
}

