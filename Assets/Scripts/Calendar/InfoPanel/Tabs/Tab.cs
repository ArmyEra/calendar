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

        public virtual bool IsVisible { get; protected set; } = true;
        
        private Button _button;
        private string _typeDescription;
        
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
        public virtual void SetVisible(bool value)
        {
            IsVisible = value;
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
    }
}