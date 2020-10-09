using Data.Calendar;
using UnityEngine;
using Utils;

namespace Audio
{
    public class HolidaySoundManager : Singleton<HolidaySoundManager>
    {
        [SerializeField] private CalendarEventsContainer holidayContainer;
        
        public bool Initialize()
        {

            return true;
        }
    }
}