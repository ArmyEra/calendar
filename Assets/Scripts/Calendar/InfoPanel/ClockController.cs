using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Calendar.InfoPanel
{
    public class ClockController : MonoBehaviour
    {
        private const int UpdateRate = 60;
        
        [SerializeField] private Text timeTextComponent;
        

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
            
            timeTextComponent.text = now.ToShortTimeString();
        }
    }
}