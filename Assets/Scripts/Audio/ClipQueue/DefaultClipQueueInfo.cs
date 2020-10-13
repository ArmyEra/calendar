using System;
using Audio.CashedSounds.Default;
using Audio.CashedSounds.Default.Utils;
using Audio.FlowChart.Utils;
using Audio.Utils;
using Core;
using Extensions.AttributeExtensions;
using UnityEngine;
using EventType = Core.EventType;

namespace Audio.ClipQueue
{
    public class DefaultClipQueueInfo: IClipQueueInfo
    {
        /// <summary>
        /// Узел звукового менеджера
        /// </summary>
        public AudioFlowChartStates FlowChartState { get; }
        
        /// <summary>
        /// Текущие состояние аудио-клипа
        /// </summary>
        public QueuedClipStates ClipState { get; private set; } = QueuedClipStates.Awaiting;
        
        /// <summary>
        /// Аудио-клип
        /// </summary>
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
        
        private readonly DefaultSoundType _soundType;
        
        public DefaultClipQueueInfo(DefaultSoundType soundType)
        {
            _soundType = soundType;
            FlowChartState = _soundType.GetState();
        }

        /// <summary>
        /// Функция, возращающия аудио-клип, если его возможно проиграть 
        /// </summary>
        public AudioClip CheckClip(out bool success)
        {
            var clip = Clip;
            success = clip != null;
            ClipState = success ? QueuedClipStates.Playing : QueuedClipStates.Loading;
            
            if (!success)
                EventManager.AddHandler(EventType.DefaultSoundLoaded, OnSoundLoaded);
            return clip;
        }

        private void OnSoundLoaded(params object[] args)
        {
            if(_soundType != (DefaultSoundType)args[0])
                return;

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
                    EventManager.RemoveHandler(EventType.DefaultSoundLoaded, OnSoundLoaded);
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