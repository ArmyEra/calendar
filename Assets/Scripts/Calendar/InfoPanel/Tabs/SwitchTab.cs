using Calendar.InfoPanel.Utils;
using Core;
using UnityEngine;
using Utils;
using EventType = Core.EventType;

namespace Calendar.InfoPanel.Tabs
{
    /// <summary>
    /// Кнопка выхода к выбору событий
    /// </summary>
    public class SwitchTab: Tab
    {
        [Header("Canvas group settings")] 
        [SerializeField] private CanvasGroupControl switchCanvasGroupSettings;
        [SerializeField] private CanvasGroupControl tabPanelCanvasGroupSettings;
        
        public override bool IsVisible { get; protected set; }

        /// <summary>
        /// Устанавливает, является ли вкладка видимой 
        /// </summary>
        public override void SetVisible(bool value)
        {
            //ToDo: check
            //ToDo: SetVisible with canvasGroupSettings switching
            IsVisible = value;  
            switchCanvasGroupSettings.SetActive(value);
            tabPanelCanvasGroupSettings.SetActive(value);
        }
        
        private void Start()
        {
            EventManager.AddHandler(EventType.OnCalendarEventTypeChanged, OnCalendarEventTypeChanged);
        }

        private void OnDestroy()
        {
            EventManager.RemoveHandler(EventType.OnCalendarEventTypeChanged, OnCalendarEventTypeChanged);
        }

        private void OnCalendarEventTypeChanged(params object[] args)
        {
            var isVisible = (CalendarEventTypes) args[0] == calendarEventType;//CalendarEventTypes.Null
            SetVisible(isVisible);
        }
    }
}