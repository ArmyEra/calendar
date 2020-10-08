using Calendar.InfoPanel.Utils;
using Core;
using SpeechKitApi.Utils;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.EventType;

namespace Calendar.InfoPanel.Tabs
{
    public class Tab: MonoBehaviour
    {
        public CalendarEventTypes calendarEventType;

        private Button _button;
        private string _typeDescription;
        
        private void Start()
        {
            EventManager.AddHandler(EventType.OnCalendarEventTypeChanged, OnCalendarEventChange);
        }

        private void OnDestroy()
        {
            EventManager.RemoveHandler(EventType.OnCalendarEventTypeChanged, OnCalendarEventChange);
        }

        /// <summary>
        /// Иницицализация вкладки
        /// </summary>
        public void Initialize()
        {
            SetTitle();
            SetButtonSubscription();
        }
        
        /// <summary>
        /// Устанавливает, является ли вкладка видимой 
        /// </summary>
        public void SetVisible(bool value)
        {
            gameObject.SetActive(value);  
        }

        /// <summary>
        /// Устанавливает заголовок вкладки
        /// </summary>
        private void SetTitle()
        {
            _typeDescription = calendarEventType.GetEnumString();

            Text title;
            if ((title = GetComponentInChildren<Text>()) != null)
                title.text = _typeDescription;
        }

        /// <summary>
        /// Создает обработчик нажатия
        /// </summary>
        private void SetButtonSubscription()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClickAction);
        }

        /// <summary>
        /// Оюработчик нажатия
        /// </summary>
        public void OnClickAction()
        {
            EventManager.RaiseEvent(EventType.OnCalendarEventTypeChanged, calendarEventType, _typeDescription);
        }

        /// <summary>
        /// При изменении типа календарного события
        /// </summary>
        private void OnCalendarEventChange(params object[] args)
        {
            _button.interactable = calendarEventType != (CalendarEventTypes) args[0];
        }
    }
}