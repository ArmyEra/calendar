using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Data.Calendar;
using SpeechKitApi.Models;
using UnityAsyncHelper.Core;
using UnityEngine;
using Utils;
using AudioParams = Audio.Utils.Params;
using EventType = Core.EventType;

namespace Audio.CashedSounds.Holiday
{
    public class HolidaySoundManager : Singleton<HolidaySoundManager>
    {
#if UNITY_EDITOR
        private readonly List<CalendarEventData> _awaitGeneration = new List<CalendarEventData>();
        private bool _awaitInvoke = true;
#endif
        
        [SerializeField] private CalendarEventsContainer holidayContainer;
        
        public void Initialize()
        {
#if UNITY_EDITOR
            EventManager.AddHandler(EventType.YandexClientCreated, SpeechKitClientCallback);

            //Только в Эдиотре прогоняем звуки на существование и генерируем те, что еще не существуют
            //DownloadSoundsFromResources(true, holidayContainer.datas);
#endif
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Callback события подключение к API
        /// </summary>
        private void SpeechKitClientCallback(params object[] args)
        {
            StartCoroutine(
                DownloadSoundsFromInternet(
                    AudioParams.DefaultExternalOptions, 
                    AudioParams.SoundDirectory));
        }
#endif
        
        /// <summary>
        /// Manual Dispose OnDestroy
        /// </summary>
        public void OnDestroy()
        {
#if UNITY_EDITOR
            EventManager.RemoveHandler(EventType.YandexClientCreated, SpeechKitClientCallback);
            _awaitInvoke = false;
#endif
            StopAllCoroutines();
        }

        /// <summary>
        /// Загружает из Resources клип календарного события 
        /// </summary>
        public static void DownloadSoundFromResource(CalendarEventData eventData)
        {
            Instance.DownloadSoundsFromResources(false, eventData);
        }
        
        /// <summary>
        /// Вызывается один раз для загрузки звука из Resources 
        /// </summary>
        private void DownloadSoundsFromResources(bool onlyCheck, params CalendarEventData[] calendarEventsData)
        {
            foreach (var eventData in calendarEventsData)
            {
                var fullResourceName = AudioParams.HolidaySoundSubPath(eventData.SourceId);
#if UNITY_EDITOR
                if(onlyCheck && File.Exists(Path.Combine(AudioParams.ResourcesFolder, fullResourceName)))
                    continue;
#endif
                fullResourceName = fullResourceName.Replace('\\', '/');
                
                void SaveToDictionary(AsyncOperation asyncOperation)
                {
                    var asset = ((ResourceRequest) asyncOperation).asset;
                    var clip = asset as AudioClip;
                    
                    EventManager.RaiseEvent(EventType.HolidaySoundLoaded, clip, eventData.SourceId);
                    
#if UNITY_EDITOR
                    if (clip == null)
                        _awaitGeneration.Add(eventData);
                    else
                        Debug.Log($"Sound \"{eventData.headerInfo}\" loaded from Resources");
#endif    
                }

                var request = Resources.LoadAsync(fullResourceName);
                request.completed += SaveToDictionary;
            }
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Загрузка с Yandex.SpeechKit. Ожидает, что все что можно было, загружено, и что очередь не пустая 
        /// </summary>
        private IEnumerator DownloadSoundsFromInternet(SynthesisExternalOptions externalOptions, string saveDirectoryPath)
        {
            while (_awaitInvoke)
            {
                yield return new WaitUntil(_awaitGeneration.Any);
                _awaitGeneration.RemoveAll(ed => string.IsNullOrWhiteSpace(ed.headerInfo));

                if (!_awaitGeneration.Any()) 
                    continue;

                var headerInfos = _awaitGeneration.Select(ed => ed.headerInfo).ToArray();
                
                void Callback()
                {
                    foreach (var headerInfo in headerInfos)
                        Debug.Log($"Sound \"{headerInfo}\" downloaded from internet");
                }
                
                ThreadManager.AsyncExecute(
                    SoundManger.GenerateSounds,
                    Callback,
                    headerInfos,
                    externalOptions,
                    saveDirectoryPath,
                    _awaitGeneration.Select(ed => ed.SourceId).ToArray());
                
                _awaitGeneration.Clear();
            }
        }
#endif
    }
}