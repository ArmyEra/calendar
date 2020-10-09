using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.InfoPanel.Utils;
using Core;
using Extensions;
using UnityEngine;
using EventType = Core.EventType;

namespace Calendar.InfoPanel.Tabs
{
    public class TabsController : MonoBehaviour
    {
        [SerializeField] private Tab[] tabs;

        private InfoPanelController InfoPanelController
            => _infoPanelController == null
                ? _infoPanelController = GetComponentInParent<InfoPanelController>()
                : _infoPanelController; 
        private InfoPanelController _infoPanelController;
        
        private bool _startInitialized;

        /// <summary>
        /// Инциализирует вкладки
        /// </summary>
        public void Initialize()
        {
            InitializeOnStart();
        }

        private void InitializeOnStart()
        {
            if(_startInitialized)
                return;

            EventManager.AddHandler(EventType.OnDateChanged, OnDayChanged);
            
            foreach (var tab in tabs)
                tab.Initialize();
            
            InvokeFirstClick();
            
            _startInitialized = true;
        }

        private void OnDestroy()
        {
            EventManager.RemoveHandler(EventType.OnDateChanged, OnDayChanged);
        }

        /// <summary>
        /// Перехватывает событие при изменении дня 
        /// </summary>
        private void OnDayChanged(params object[] args)
        {
            var date = (DateTime) args[0];
            var eventTypes = InfoPanelController
                .GetDayEventDatas(date)
                .GetAvailableTypes();
            
            SetTabsVisible(eventTypes);
        }

        /// <summary>
        /// Устанавливает видимость вкладки 
        /// </summary>
        private void SetTabsVisible(IEnumerable<CalendarEventTypes> existedEventTypes)
        {
            //_monthEvents.GetAvailableTypes()
            foreach (var tab in tabs.Where(t => t.calendarEventType != CalendarEventTypes.Notes))
                tab.SetVisible(existedEventTypes.Any(et => et == tab.calendarEventType));

            InvokeFirstClick();
        }

        private void InvokeFirstClick()
        {
            tabs.First(t => t.IsVisible)
                .OnClickAction();
        }
    }
}