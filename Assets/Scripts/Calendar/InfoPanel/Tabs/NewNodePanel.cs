using System;
using Audio;
using Audio.CashedSounds.Default.Utils;
using Calendar.InfoPanel.Utils;
using Core;
using Data.Calendar;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using EventType = Core.EventType;

namespace Calendar.InfoPanel.Tabs
{
    public class NewNodePanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroupControl panelCanvasGroupControl;
        [SerializeField] private CanvasGroupControl addButtonCanvasGroupControl;
        [Space] 
        [SerializeField] private Button addButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button closeButton;
        [Space] 
        [SerializeField] private InputField inputField;

        private void OnValidate()
        {
            panelCanvasGroupControl.SetActive(false);
        }

        private void Start()
        {
            EventManager.AddHandler(EventType.OnCalendarEventTypeChanged, OnCalendarEventChange);
            
            addButton.onClick.AddListener(()=> SetPanelActive(true));
            closeButton.onClick.AddListener(()=> SetPanelActive(false));
            saveButton.onClick.AddListener(SaveNode);
        }

        private void OnDestroy()
        {
            EventManager.RemoveHandler(EventType.OnCalendarEventTypeChanged, OnCalendarEventChange);
        }
        
        /// <summary>
        /// При изменении типа календарного события
        /// </summary>
        private void OnCalendarEventChange(params object[] args)
        {
            addButtonCanvasGroupControl.SetActive(((CalendarEventTypes) args[0]) == CalendarEventTypes.Notes);
        }

        /// <summary>
        /// Активация/Деактивация панели добавления событий 
        /// </summary>
        private void SetPanelActive(bool value)
        {
            panelCanvasGroupControl.SetActive(value);    
            
            if(!value)
                Dispose();
        }

        /// <summary>
        /// Сохраняет заметку
        /// </summary>
        private void SaveNode()
        {
            var text = inputField.text.Trim();
            if(string.IsNullOrEmpty(text))
                return;

            var newCalendarEvent =
                new CalendarEventData(CalendarEventTypes.Notes, MainPageController.ActiveInfo.Date) {textInfo = text};
            
            EventManager.RaiseEvent(EventType.CalendarEventAdd, newCalendarEvent, 1);
            SoundManger.PlayQueued(DefaultSoundType.EventAdded);
            SetPanelActive(false);
        }

        private void Dispose()
        {
            inputField.text = "";
        }
    }
}