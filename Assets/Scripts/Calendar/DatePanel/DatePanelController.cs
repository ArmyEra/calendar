using System;
using System.Globalization;
using Core;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.EventType;

namespace Calendar.DatePanel
{
    public class DatePanelController : MonoBehaviour
    {
        [Header("Настройки элементов панели")]
        [SerializeField] private Text yearHeader;
        [SerializeField] private Text monthHeader;
        [SerializeField] private DateContainer dateContainer;

        private void Start()
        {
            EventManager.AddHandler(EventType.BeforeMonthChanged, BeforeMonthChanged);
        }

        private void OnDestroy()
        {
            EventManager.RemoveHandler(EventType.BeforeMonthChanged, BeforeMonthChanged);
        }

        public void Initialize(in DateTime monthDate)
        {
            SetUpHeaders(in monthDate);
            dateContainer.Initialize(in monthDate);
        }

        private void SetUpHeaders(in DateTime monthDate)
        {
            yearHeader.text = monthDate.Year.ToString();
            
            var currentCulture = CultureInfo.CurrentCulture;
            monthHeader.text = currentCulture.TextInfo.ToTitleCase(currentCulture.DateTimeFormat.GetMonthName(monthDate.Month)); 
        }

        private void BeforeMonthChanged(params object[] args)
        {
            var monthIncrement = (int) args[0];
            dateContainer.DestroyItems(monthIncrement);
        }

        #region TESTUPDATE

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                BeforeMonthChanged(1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                BeforeMonthChanged(-1);
            }
        }

        #endregion
    }
}