using System;
using Audio.CashedSounds.Holiday;
using Core;
using Data.Calendar;
using UnityEngine;
using EventType = Core.EventType;


namespace Audio.ClipQueue
{
    public class CalendarClipQueueInfo: IClipQueueInfo
    {
        private CalendarEventData _calendarEventData;
        private readonly string _sourceId;
       
        public AudioClip Clip { get; private set; }
        public bool ClipLoaded { get; private set; }

        public CalendarClipQueueInfo(CalendarEventData eventData)
        {
            _calendarEventData = eventData;
            _sourceId = _calendarEventData.SourceId;
        }
        
        public AudioClip CheckClip(out bool success)
        {
            success = false;
            EventManager.AddHandler(EventType.HolidaySoundLoaded, OnSoundLoaded);
            HolidaySoundManager.DownloadSoundFromResource(_calendarEventData);

            return null;
        }
        
        private void OnSoundLoaded(params object[] args)
        {
            if(_sourceId != (string) args[1])
                return;

            Clip = (AudioClip) args[0];
            ClipLoaded = true;
        }
        
        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _calendarEventData = null;
                    Clip = null;
                    EventManager.RemoveHandler(EventType.HolidaySoundLoaded, OnSoundLoaded);
                }
                disposedValue = true;
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