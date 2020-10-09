using System;
using Audio.CashedSounds.Default;
using Audio.CashedSounds.Default.Utils;
using Audio.Utils;
using Core;
using UnityEngine;
using EventType = Core.EventType;

namespace Audio.ClipQueue
{
    public class DefaultClipQueueInfo: IClipQueueInfo
    {
        private readonly DefaultSoundType _soundType;
        public bool ClipLoaded { get; private set; }
        
        public AudioClip Clip
        {
            get
            {
                var dictionary = DefaultSoundManager.SoundDictionary;
                return dictionary.ContainsKey(_soundType) 
                    ? dictionary[_soundType] 
                    : null;
            }
        }

        public DefaultClipQueueInfo(DefaultSoundType soundType)
        {
            if(soundType == DefaultSoundType.Null)
                throw new Exception("It's impossible to generate ClipQueueInfo from \"DefaultSoundType.Null\"");
            
            _soundType = soundType;
        }

        public AudioClip CheckClip(out bool success)
        {
            var clip = Clip;
            success = clip != null;
            ClipLoaded = success;
            
            if (!success)
                EventManager.AddHandler(EventType.DefaultSoundLoaded, OnSoundLoaded);
            return clip;
        }

        private void OnSoundLoaded(params object[] args)
        {
            if(_soundType != (DefaultSoundType)args[0])
                return;

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
                    EventManager.RemoveHandler(EventType.DefaultSoundLoaded, OnSoundLoaded);
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