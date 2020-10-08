using System.Collections.Generic;
using System.Linq;
using Calendar.InfoPanel.Utils;
using UnityEngine;

namespace Calendar.InfoPanel.Tabs
{
    public class TabsController : MonoBehaviour
    {
        [SerializeField] private Tab[] tabs;

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Инциализирует вкладки
        /// </summary>
        private void Initialize()
        {
            foreach (var tab in tabs)
                tab.Initialize();
            
            tabs.First(t => t.calendarEventType == CalendarEventTypes.MilitaryMemoryDay)
                .OnClickAction();
        }

        /// <summary>
        /// Устанавливает видимость вкладки 
        /// </summary>
        public void SetTabsVisible(IEnumerable<CalendarEventTypes> existedEventTypes)
        {
            foreach (var tab in tabs.Where(t => t.calendarEventType != CalendarEventTypes.Notes))
            {
                tab.SetVisible(existedEventTypes.Any(et => et == tab.calendarEventType));
            }
        }
    }
}