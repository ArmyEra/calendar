using System;
using System.Linq;
using Audio;
using Audio.CashedSounds.Default.Utils;
using Calendar.DatePanel;
using Calendar.InfoPanel;
using Calendar.InfoPanel.Utils;
using Calendar.Utils;
using Core;
using Data.Calendar;
using Extensions;
using OrderExecuter;
using UnityEngine;
using EventType = Core.EventType;

namespace Calendar
{
    public class MainPageController : MonoBehaviour, IStartable
    {
        public static readonly UniqEventInfo ActiveInfo = new UniqEventInfo
        {
            Date = DateTime.Today,
            CalendarEventType = CalendarEventTypes.MilitaryMemoryDay
        };
        
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

        private DateTime _shownMonth;
        
        public void OnStart()
        {
            PlayHello();
            
            holidayCalendarEvents.Initialize();
            scheduledCalendarEvents.Initialize();
            
            EventManager.AddHandler(EventType.OnDateChanged, OnDateChanged);
            EventManager.AddHandler(EventType.OnMonthChanged, OnMonthChanged);
            EventManager.AddHandler(EventType.OnCalendarEventTypeChanged, OnCalendarEventTypeChanged);
            EventManager.AddHandler(EventType.CalendarEventAdd, OnCalendarEventAdd);
            
            
            ShowMonth(DateTime.Now);
        }
        
        private void OnDestroy()
        {
            EventManager.RemoveHandler(EventType.OnDateChanged, OnDateChanged);
            EventManager.RemoveHandler(EventType.OnMonthChanged, OnMonthChanged);
            EventManager.RemoveHandler(EventType.OnCalendarEventTypeChanged, OnCalendarEventTypeChanged);
            EventManager.RemoveHandler(EventType.CalendarEventAdd, OnCalendarEventAdd);
        }

        /// <summary>
        /// Устанавливает настройки месяца на календаре и ифнормацйионной панеле
        /// </summary>
        private void ShowMonth(DateTime date)
        {
            _shownMonth = date.Date.FirstMonthDate();
            
            var monthEvents = holidayCalendarEvents.datas.GetMonthEvents(date)
                .Concat(scheduledCalendarEvents.datas.GetMonthEvents(date))
                .ToArray();
            infoPanelController.Initialize(date, monthEvents);
            
            datePanelController.Initialize(date);
        }

        /// <summary>
        /// Воспроизвести Приветствие 
        /// </summary>
        private void PlayHello()
        {
            var soundType = DateTimeSoundManager.GetGreetingSoundType();
            SoundManger.PlayQueued(soundType);
        }

        #region EVENTS
        
        /// <summary>
        /// Событие, вызываемое при изменнии даты 
        /// </summary>
        private void OnDateChanged(params object[] args)
        {
            ActiveInfo.Date = (DateTime) args[0];
        }

        /// <summary>
        /// Событие, вызываемое при изменнии типа текущей панели 
        /// </summary>
        private void OnCalendarEventTypeChanged(params object[] args)
        {
            ActiveInfo.CalendarEventType = (CalendarEventTypes) args[0];
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

        /// <summary>
        /// Событие, срабатываемое при изменении месяца
        /// </summary>
        private void OnMonthChanged(params object[] args)
        {
            var totalMonths = (int) args[0];
            var sign = Math.Sign(totalMonths);
            totalMonths = Math.Abs(totalMonths);

            var years = sign * (totalMonths / 12);
            var months = sign * (totalMonths % 12);
            
            ShowMonth(_shownMonth.AddYears(years).AddMonths(months));
        }
        
        #endregion
    }
}

