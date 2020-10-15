using System;
using System.Collections.Generic;
using System.Linq;
using Data.Calendar;
using Extensions;
using Newtonsoft.Json;

namespace Settings.Save.Model
{
    [Serializable]
    public class NoteInfo
    {
        [JsonRequired] public string stringDate;
        [JsonRequired] public string textInfo;

        public NoteInfo(DateTime date, string text)
        {
            stringDate = date.ToString(DateTimeExtensions.ConvertDateTimeFormat);
            textInfo = text;
        }

        public static NoteInfo[] FromCalendarEventsContainer(CalendarEventsContainer container)
            => FromCalendarEvents(container.datas.Where(ev => ev.NeedSave));

        public static NoteInfo[] FromCalendarEvents(IEnumerable<CalendarEventData> calendarEvens)
        {
            var calendarEventDatas = calendarEvens as CalendarEventData[] ?? calendarEvens.ToArray();
            return calendarEventDatas.Any()
                ? calendarEventDatas.Select(ce => new NoteInfo(ce.DateTime, ce.textInfo))
                    .ToArray()
                : new NoteInfo[0];
        }
    }
}