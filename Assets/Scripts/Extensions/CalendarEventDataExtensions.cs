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
        /// Проверяет, принадлежит ли календарное событие одному из типов
        /// </summary>
        public static bool IsOneOfType(this IEnumerable<CalendarEventData> calendarEventData,
            params CalendarEventTypes[] types) =>
            calendarEventData.Any(ev => ev.IsOneOfType(types));

        /// <summary>
        /// Проверяет, принадлежит ли календарное событие одному из типов
        /// </summary>
        private static bool IsOneOfType(this CalendarEventData calendarEventData, params CalendarEventTypes[] types) =>
            types.Any(t => t == calendarEventData.calendarEventType);

        /// <summary>
        /// Прооверяет, все ли события принадлежат конкретному типу 
        /// </summary>
        public static bool IsAllOfType(this IEnumerable<CalendarEventData> calendarEventData,
            CalendarEventTypes type) =>
            calendarEventData.GetAvailableTypes().All(t => t == type);
        
        /// <summary>
        /// Возвращает события на конкретную дату 
        /// </summary>
        public static IEnumerable<CalendarEventData> GetTodayEvents(this IEnumerable<CalendarEventData> datas, DateTime today)
            => datas.Where(d => d.ThisDayAndMonth(today));
        
        /// <summary>
        /// Возвращет события на конкретный месяц 
        /// </summary>
        public static IEnumerable<CalendarEventData> GetMonthEvents(this IEnumerable<CalendarEventData> datas, DateTime todayMonth)
            => datas.Where(d => d.ThisMonthAndYear(todayMonth));

        /// <summary>
        /// Возвращет события на конкретный месяц 
        /// </summary>
        public static IEnumerable<CalendarEventData> GetMonthHolidays(this IEnumerable<CalendarEventData> datas, DateTime todayMonth)
            => datas.Where(d => d.ThisMonthOnly(todayMonth));
        
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