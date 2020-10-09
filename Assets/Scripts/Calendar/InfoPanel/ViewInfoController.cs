using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Audio.CashedSounds.Default.Utils;
using Calendar.InfoPanel.Utils;
using Core;
using Data.Calendar;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using EventType = Core.EventType;

namespace Calendar.InfoPanel
{
    public class ViewInfoController : MonoBehaviour
    {
        [SerializeField] private Text title;

        [Header("Scroll view settings")] 
        [SerializeField] private RectTransform rectContainer;
        [SerializeField] private GameObject textTemplate;
        [SerializeField] private GameObject imageTemplate;
        [Space] 
        [SerializeField] private float marginItems;
        [SerializeField] private float marginPosts;

        private Vector2 _lastPosition = Vector2.zero;
        private readonly List<GameObject> _generatedObjects = new List<GameObject>();

        private InfoPanelController InfoPanelController
            => _infoPanelController == null
                ? _infoPanelController = GetComponentInParent<InfoPanelController>()
                : _infoPanelController; 
        private InfoPanelController _infoPanelController;

        private bool _startInitialized;

        public void Initialize()
        {
            OnStartInitialize();
        }

        private void OnStartInitialize()
        {
            if (_startInitialized)
                return;
            
            EventManager.AddHandler(EventType.OnDateChanged, OnDayChanged);
            EventManager.AddHandler(EventType.OnCalendarEventTypeChanged, OnCalendarTypeChanged);
            EventManager.AddHandler(EventType.CalendarEventAdd, UpdateInfo);

            _startInitialized = true;
        }

        private void OnDestroy()
        {
            EventManager.RemoveHandler(EventType.OnDateChanged, OnDayChanged);
            EventManager.RemoveHandler(EventType.OnCalendarEventTypeChanged, OnCalendarTypeChanged);
            EventManager.RemoveHandler(EventType.CalendarEventAdd, UpdateInfo);
        }

        /// <summary>
        /// Событие, вызываемое при изменении даты 
        /// </summary>
        private void OnDayChanged(params object[] args)
        {
            var date = (DateTime) args[0];
            OnDayOrCalendarTypeChanged(date, MainPageController.ActiveCalendarEventType);
            PlayDayEvents(date);
        }
        
        /// <summary>
        /// Событие, вывываемое при изменении типа вкладки 
        /// </summary>
        private void OnCalendarTypeChanged(params object[] args)
        {
            title.text = (string) args[1];
            OnDayOrCalendarTypeChanged(MainPageController.ActiveDate, (CalendarEventTypes) args[0]);
        }

        private void OnDayOrCalendarTypeChanged(DateTime date, CalendarEventTypes calendarEventType)
        {
            if(date.IsNull() || calendarEventType == CalendarEventTypes.Null)
                return;

            var calendarEvents = InfoPanelController
                .GetDayTypedEventDatas(date, calendarEventType)
                .ToArray();
            
            ShowInfo(calendarEvents);
        }
        
        #region SOUND PLAY

        private void PlayDayEvents(DateTime date)
        {
            var calendarEvents = InfoPanelController.GetDayEventDatas(date);
            
            if(calendarEvents.Any(ev => ev.calendarEventType == CalendarEventTypes.Notes))
                SoundManger.PlayQueued(DefaultSoundType.ScheduledEvent);
        }
        
        #endregion
        
        #region INFO UPDATE

        /// <summary>
        /// Выводит информацию на текущую дату по текущему типу событий 
        /// </summary>
        private void ShowInfo(IEnumerable<CalendarEventData> calendarEvents)
        {
            DropItems();
            foreach (var calendarEvent in calendarEvents)
                GenerateInfo(calendarEvent);
            
            rectContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -_lastPosition.y);
        }

        /// <summary>
        /// Добавляет новое событие на панель
        /// </summary>
        private void UpdateInfo(params object[] args)
        {
            GenerateInfo((CalendarEventData) args[0]);
            rectContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -_lastPosition.y);
        }

        /// <summary>
        /// Создание всех комопнентов события 
        /// </summary>
        private void GenerateInfo(CalendarEventData calendarEventData)
        {
            var result = false;
            result |= GenerateText(calendarEventData.textInfo);
            result |= GenerateImage(calendarEventData.sprite);
            
            if(result)
                _lastPosition -= new Vector2(0, marginPosts);
        }

        /// <summary>
        /// Создание текстового компонента
        /// </summary>
        private bool GenerateText(string text)
        {
            if(string.IsNullOrWhiteSpace(text))
                return false;

            var textItem = Instantiate(textTemplate, rectContainer);
            textItem.SetActive(true);
            textItem.GetComponent<Text>().text = text;
            textItem.GetComponent<ContentSizeFitter>().SetLayoutVertical();

            var rectTextItem = textItem.GetComponent<RectTransform>();
            rectTextItem.anchoredPosition = _lastPosition;

            _lastPosition -= new Vector2(0, rectTextItem.rect.height + marginItems);
            _generatedObjects.Add(textItem);
            return true;
        }

        /// <summary>
        /// Создание картинки 
        /// </summary>
        private bool GenerateImage(Sprite sprite)
        {
            if(sprite == null)
                return false;

            var imageItem = Instantiate(imageTemplate, rectContainer);
            imageItem.SetActive(true);
            imageItem.GetComponent<Image>().sprite = sprite;

            var textureSize = sprite.textureRect.size;
            var aspect = textureSize.y / textureSize.x; 

            var rectImageItem = imageItem.GetComponent<RectTransform>();
            rectImageItem.anchoredPosition = _lastPosition;
            rectImageItem.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectImageItem.rect.width * aspect);
            
            _lastPosition -= new Vector2(0, rectImageItem.rect.height + marginItems);
            _generatedObjects.Add(imageItem);
            return true;
        }

        /// <summary>
        /// Удаление всех элементов
        /// </summary>
        private void DropItems()
        {
            foreach (var item in _generatedObjects.ToArray())
                Destroy(item);
            _generatedObjects.Clear();
            _lastPosition = Vector2.zero;
        }

        #endregion
    }
}