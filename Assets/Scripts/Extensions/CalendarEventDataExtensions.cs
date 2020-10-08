using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.InfoPanel.Utils;
using Data.Calendar;

namespace Extensions
{
    public static class CalendarEventDataExtensions
    {
        /// <summary>
        /// Возвращает события на конкретную дату 
        /// </summary>
        public static IEnumerable<CalendarEventData> GetTodayEvents(this IEnumerable<CalendarEventData> datas, DateTime today)
            => datas.Where(d => d.ThisDay(today));
        
        /// <summary>
        /// Возвращет события на конкретный месяц 
        /// </summary>
        public static IEnumerable<CalendarEventData> GetMonthEvents(this IEnumerable<CalendarEventData> datas, DateTime todayMonth)
            => datas.Where(d => d.ThisMonth(todayMonth));

        /// <summary>
        /// Возвращети события конкретного типа
        /// </summary>
        public static IEnumerable<CalendarEventData> GetTypedEvents(this IEnumerable<CalendarEventData> datas, CalendarEventTypes type)
            => datas.Where(d => d.calendarEventType == type);
        
        /// <summary>
        /// Возвращает все типы событий из перечня событий
        /// </summary>
        public static IEnumerable<CalendarEventTypes> GetAvailableTypes(this IEnumerable<CalendarEventData> datas)
        {
            var typesArray = datas.Select(d => d.calendarEventType);
            if (typesArray.Any())
                return typesArray;

            return typesArray.Distinct();
        } 
    }
}