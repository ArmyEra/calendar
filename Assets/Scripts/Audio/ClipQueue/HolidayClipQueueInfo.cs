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
        /// Узел звукового менеджера
        /// </summary>
        public AudioFlowChartStates FlowChartState { get; } = AudioFlowChartStates.HolidayNotification;
        
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
        
        public HolidayClipQueueInfo(CalendarEventData eventData)
        {
            _calendarEventData = eventData;
            _sourceId = _calendarEventData.SourceId;
        }
        
        /// <summary>
        /// Функция, возращающия аудио-клип, если его возможно проиграть 
        /// </summary>
        public AudioClip CheckClip(out bool success)
        {
            success = false;
            ClipState = QueuedClipStates.Loading;
            
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
            ClipState = QueuedClipStates.Playing;
        }
        
        #region IDisposable Support
        public bool IsDisposed { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    _calendarEventData = null;
                    Clip = null;
                    EventManager.RemoveHandler(EventType.HolidaySoundLoaded, OnSoundLoaded);
                }
                IsDisposed = true;
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