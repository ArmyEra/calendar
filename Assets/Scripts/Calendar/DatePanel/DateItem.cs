using System;
using Calendar.DatePanel.Utils;
using Core;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using EventType = Core.EventType;

namespace Calendar.DatePanel
{
    public class DateItem : MonoBehaviour
    {
        [SerializeField] private Color currentDateColor;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color selectedColor;
        [Space]
        [SerializeField] private Text textComponent;
        [SerializeField] private Image imageComponent;
        [SerializeField] private Button buttonComponent;
        [SerializeField] private RectTransform rectTransformComponent;

        private DateTime _date;

        private void Start()
        {
            EventManager.AddHandler(EventType.OnDateChanged, OnDateChanged);
        }

        private void OnDestroy()
        {
            EventManager.RemoveHandler(EventType.OnDateChanged, OnDateChanged);
        }

        public void SetUp(in DateItemSetUpData setUpData)
        {
            rectTransformComponent.anchoredPosition = setUpData.Position;
            rectTransformComponent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, setUpData.Size.x);
            rectTransformComponent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, setUpData.Size.y);

            _date = setUpData.Date;
            textComponent.text = _date.Day.ToString();
            buttonComponent.onClick.AddListener(OnClickAction);

            if (_date.Date == DateTime.Now.Date)
            {
                imageComponent.color = currentDateColor;
                EventManager.RaiseEvent(EventType.OnDateChanged, _date.Date);
            }
            
            gameObject.SetActive(true);
        }

        private void OnClickAction()
        {
            EventManager.RaiseEvent(EventType.OnDateChanged, _date.Date);
        }

        private void OnDateChanged(params object[] args)
        {
            if (_date.Date == DateTime.Now.Date)
                return;

            imageComponent.color = (_date == (DateTime) args[0] ? selectedColor : normalColor);
        }
    }
}