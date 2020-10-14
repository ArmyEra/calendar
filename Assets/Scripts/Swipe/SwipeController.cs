using System;
using System.Collections.Generic;
using Core;
using Swipe.Utils;
using Swipe.Utils.Send;
using Swipe.Utils.Special;
using UnityEngine;
using EventType = Core.EventType;

namespace Swipe
{
    public class SwipeController: UnityEngineSwipeController
    {
        [Header("Month change conditions")] 
        [SerializeField] private Condition monthChangeCondition;
	
        private void Start()
        {   
            var newCondition = new Condition()
            {
                gridCondition = new GridCondition(
                    new Dictionary<uint, uint[]>
                        {
                            [0] = new uint[] {0, 2},
                            [2] = new uint[] {2, 0}
                        }
                )
            };
            
            SpecialSwipes.Add(EventType.BeforeMonthChanged, newCondition);

            Swiper.onSpecialSwipe += CatchSpecialSwipe;
            Swiper.onSwipe += TestHandler;
        }

        private void OnDestroy()
        {
            SpecialSwipes.Remove(EventType.BeforeMonthChanged);
		
            Swiper.onSpecialSwipe -= CatchSpecialSwipe;
            Swiper.onSwipe -= TestHandler;
        }

        private static void CatchSpecialSwipe(object key, SwipeInfo swipeInfo)
        {
            if (key is EventType eventType)
            {
                switch (eventType)
                {
                    case EventType.BeforeMonthChanged:
                        CatchMonthChange(swipeInfo);
                        break;
                }
            }
        }

        private static void TestHandler(SwipeInfo swipeInfo)
        {
            Debug.Log(swipeInfo);
        }

        private static void CatchMonthChange(SwipeInfo swipeInfo)
        {
            switch (swipeInfo.OptionalInfo.SwipeDirection)
            {
                case SwipeDirection.Down:
                case SwipeDirection.Left:
                    EventManager.RaiseEvent(EventType.BeforeMonthChanged, -1);
                    break;
                case SwipeDirection.Up:
                case SwipeDirection.Right:
                    EventManager.RaiseEvent(EventType.BeforeMonthChanged, 1);
                    break;
                default: return;
            }
        }
    }
}