using System;
using System.Collections.Generic;
using Calendar.InfoPanel.Tabs;
using Calendar.InfoPanel.Utils;
using Data.Calendar;
using UnityEngine;
using Utils;

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
            
            tabsController.SetTabsVisible(_monthEvents.GetAvailableTypes());
            viewInfoController.ShowInfo(MainPageController.ActiveDate, MainPageController.ActiveCalendarEventType);
        }

        public void UpdateEvents(CalendarEventData dataEvent)
        {
            _monthEvents.Add(dataEvent);
        }

        /// <summary>
        /// Возвращает типизированные события на конкретную дату
        /// </summary>
        public IEnumerable<CalendarEventData> GetDayTypedEventDatas(DateTime date, CalendarEventTypes calendarEventType)
            => _monthEvents.GetTodayEvents(date).GetTypedEvents(calendarEventType);
    }
}