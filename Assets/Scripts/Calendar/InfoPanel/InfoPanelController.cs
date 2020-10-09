using System;
using System.Collections.Generic;
using Calendar.InfoPanel.Tabs;
using Calendar.InfoPanel.Utils;
using Data.Calendar;
using Extensions;
using UnityEngine;

namespace Calendar.InfoPanel
{
    public class InfoPanelController : MonoBehaviour
    {
        [SerializeField] private ViewInfoController viewInfoController;
        [SerializeField] private TabsController tabsController;

        private DateTime _monthDate;
        private readonly List<CalendarEventData> _monthEvents = new List<CalendarEventData>();
        
        public void Initialize(in DateTime monthDate, IEnumerable<CalendarEventData> monthEvents)
        {
            _monthDate = monthDate;
            
            _monthEvents.Clear();
            _monthEvents.AddRange(monthEvents);
            
            viewInfoController.Initialize();
            tabsController.Initialize();
        }

        public void UpdateEvents(CalendarEventData dataEvent)
        {
            _monthEvents.Add(dataEvent);
        }

        
        /// <summary>
        /// Возвращает события на конкретную дату 
        /// </summary>
        public IEnumerable<CalendarEventData> GetDayEventDatas(DateTime date)
            => _monthEvents.GetTodayEvents(date);
        
        /// <summary>
        /// Возвращает типизированные события на конкретную дату
        /// </summary>
        public IEnumerable<CalendarEventData> GetDayTypedEventDatas(DateTime date, CalendarEventTypes calendarEventType)
            => GetDayEventDatas(date).GetTypedEvents(calendarEventType);
    }
}