using System;
using Calendar.InfoPanel.Utils;

namespace Calendar.Utils
{
    public class UniqEventInfo
    {
        public DateTime Date
        {
            get => _date;
            set => _date = value.Date;
        }

        public CalendarEventTypes CalendarEventType;

        public bool IsValidate
            => Date != DateTime.MinValue.Date && CalendarEventType != CalendarEventTypes.Null;
        
        private DateTime _date;

        public override bool Equals(object obj)
        {
            if (obj is UniqEventInfo compareValue)
                return Equals(compareValue);

            return false;
        }
        
        public bool Equals(UniqEventInfo compareValue)
        {
            return compareValue != null 
                   && Date == compareValue.Date 
                   && CalendarEventType == compareValue.CalendarEventType;
        }
    }
}