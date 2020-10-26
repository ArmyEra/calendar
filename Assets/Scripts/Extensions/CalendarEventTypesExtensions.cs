using System;
using System.Linq;
using Calendar.InfoPanel.Tabs;
using Calendar.InfoPanel.Utils;

namespace Extensions
{
    public static class CalendarEventTypesExtensions
    {
        /// <summary>
        /// Выбирает вкладки по типам календарных событий 
        /// </summary>
        public static Func<Tab, bool> SelectByType(params CalendarEventTypes[] types) =>
            tab => types.Any(t => t == tab.calendarEventType);

        /// <summary>
        /// Проверяет, принаделжит ли тип массиву 
        /// </summary>
        public static bool OneOf(this CalendarEventTypes type, params CalendarEventTypes[] types) =>
            types.Any(t => t == type);
    }
}