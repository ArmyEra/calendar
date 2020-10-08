using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Calendar.DatePanel
{
    public class DatePanelController : MonoBehaviour
    {
        [Header("Настройки элементов панели")]
        [SerializeField] private Text yearHeader;
        [SerializeField] private Text monthHeader;
        [SerializeField] private DateContainer dateContainer;

        private DateTime _monthDate;
        
        public void Initialize(in DateTime monthDate)
        {
            _monthDate = monthDate;
            
            SetUpHeaders();
            dateContainer.Initialize(in monthDate);
        }

        private void SetUpHeaders()
        {
            yearHeader.text = _monthDate.Year.ToString();
            
            var currentCulture = CultureInfo.CurrentCulture;
            monthHeader.text = currentCulture.TextInfo.ToTitleCase(currentCulture.DateTimeFormat.GetMonthName(_monthDate.Month)); 
        }
    }
}