using System;
using System.Linq;
using Calendar.InfoPanel.Tabs;
using Calendar.InfoPanel.Utils;
using Data.Calendar;

namespace Extensions
{
    public static class CalendarEventTypesExtensions
    {
        public static Func<Tab, bool> SelectByType(params CalendarEventTypes[] types) =>
            tab => types.Any(t => t == tab.calendarEventType);
    }
}