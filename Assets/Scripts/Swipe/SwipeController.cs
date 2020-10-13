using System.Collections.Generic;
using Swipe.Detectors;
using Swipe.Utils;
using Swipe.Utils.Input;
using Swipe.Utils.Special;
using UnityEngine;
using Utils;

namespace Swipe
{
    public class SwipeController: Singleton<SwipeController>
    {
        public ISwiperDetector Swiper { get; private set; }

        public InputGrid touchGrid;
        public DetectorSettings data;
        public readonly Dictionary<object, Condition> SpecialSwipes = new Dictionary<object, Condition>();

        private void Awake()
        {
#if (UNITY_EDITOR || UNITY_STANDALONE)
            Input.simulateMouseWithTouches = true;
            Swiper = gameObject.AddComponent<MouseSwipeDetector>();
#else
            var detectorType = !Input.touchSupported || !Input.touchPressureSupported
                ? typeof(MouseSwipeDetector)
                : typeof(ScreenTouchSwipeDetector);
            
            Swiper = gameObject.AddComponent(typeof(MouseSwipeDetector)) as ISwiperDetector;
#endif
        }

        public void Update()
        {
            Swiper?.Detect();
        }
    }
}
