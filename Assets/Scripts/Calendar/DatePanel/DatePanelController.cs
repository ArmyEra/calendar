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

        #region TESTUPDATE

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                dateContainer.DestroyItems(1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                dateContainer.DestroyItems(-1);
            }
        }

        #endregion
    }
}