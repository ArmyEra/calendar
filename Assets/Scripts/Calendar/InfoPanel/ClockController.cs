using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Calendar.InfoPanel
{
    public class ClockController : MonoBehaviour
    {
        private const int UpdateRate = 60;
        
        [SerializeField] private Text textComponent;

        private int DelayInvoking => (UpdateRate - DateTime.Now.Second) + 1;
        
        private void OnEnable()
        {
            Execute();
            InvokeRepeating(nameof(Execute), DelayInvoking, UpdateRate);
        }

        private void OnDisable()
        {
            CancelInvoke(nameof(Execute));
        }

        
        private void Execute()
        {
            var now = DateTime.Now;
            var shortTime = now.ToShortTimeString();
            var shortDate = now.ToString(DateTimeExtensions.ClockDateFormat);
            textComponent.text = $"{shortTime}\n{shortDate}";
        }
    }
}