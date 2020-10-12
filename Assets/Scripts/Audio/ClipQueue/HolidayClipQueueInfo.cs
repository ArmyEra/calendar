using System;
using Audio.CashedSounds.Holiday;
using Audio.FlowChart.Utils;
using Audio.Utils;
using Core;
using Data.Calendar;
using UnityEngine;
using EventType = Core.EventType;


namespace Audio.ClipQueue
{
    public class HolidayClipQueueInfo: IClipQueueInfo
    {
        /// <summary>
        /// УИД потока, в котором произошел запрос на произведение
        /// </summary>
        public int FrameId { get; }

        /// <summary>
        /// Узел звукового менеджера
        /// </summary>
        public AudioFlowChartStates State { get; } = AudioFlowChartStates.HolidayNotification;
        
        /// <summary>
        /// Текущие состояние аудио-клипа
        /// </summary>
        public QueuedClipStates ClipState { get; private set; } = QueuedClipStates.Awaiting;
        
        /// <summary>
        /// Аудио-клип
        /// </summary>
        public AudioClip Clip { get; private set; }
        
        private CalendarEventData _calendarEventData;
        private readonly string _sourceId;
        
        public HolidayClipQueueInfo(int frameId, CalendarEventData eventData)
        {
            FrameId = frameId;
            _calendarEventData = eventData;
            _sourceId = _calendarEventData.SourceId;
        }
        
        /// <summary>
        /// Функция, возращающия аудио-клип, если его возможно проиграть 
        /// </summary>
        public AudioClip CheckClip(out bool success)
        {
            success = false;
            EventManager.AddHandler(EventType.HolidaySoundLoaded, OnSoundLoaded);
            HolidaySoundManager.DownloadSoundFromResource(_calendarEventData);

            return null;
        }
        
        /// <summary>
        /// Обработчик события загрущки клипа 
        /// </summary>
        private void OnSoundLoaded(params object[] args)
        {
            if(_sourceId != (string) args[1])
                return;

            Clip = (AudioClip) args[0];
            ClipState = QueuedClipStates.Loading;
        }
        
        #region IDisposable Support
        public bool DisposedValue { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    _calendarEventData = null;
                    Clip = null;
                    EventManager.RemoveHandler(EventType.HolidaySoundLoaded, OnSoundLoaded);
                }
                DisposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}