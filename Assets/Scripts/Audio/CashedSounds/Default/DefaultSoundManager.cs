using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Audio.CashedSounds.Default.Utils;
using Core;
using SpeechKitApi.Utils;
using UnityAsyncHelper.Core;
using UnityEngine;
using Utils;
using AudioParams = Audio.Utils.Params;
using EventType = Core.EventType;

namespace Audio.CashedSounds.Default
{
    public class DefaultSoundManager: Singleton<DefaultSoundManager>
    {
        public static readonly Dictionary<DefaultSoundType, AudioClip> SoundDictionary = new Dictionary<DefaultSoundType, AudioClip>();
        private readonly List<string> _awaitGeneration = new List<string>();
        private readonly List<string> _loadingFromResources = new List<string>();
        
        private bool _awaitInvoke = true;
        private bool _componentsInitialized;

        /// <summary>
        /// Инициализация. Запускет загрузку существующих айдио в словарь
        /// </summary>
        public void Initialize()
        {
#if UNITY_EDITOR
            EventManager.AddHandler(EventType.YandexClientCreated, SpeechKitClientCallback);
#endif
            
            var defaultTexts = Enum.GetValues(typeof(DefaultSoundType))
                .Cast<DefaultSoundType>()
                .Select(v => v.GetEnumString())
                .Where(v => v != null);
            DownloadSounds(defaultTexts, AudioParams.DEFAULT_SOUNDS_SUB_PATH);

            _componentsInitialized = true;
        }

        #region SOUND LOAD

#if UNITY_EDITOR
        /// <summary>
        /// Callback события подключение к API
        /// </summary>
        private void SpeechKitClientCallback(params object[] args)
        {
            StartCoroutine(DownloadSoundsFromInternet());
        }
#endif
        
        /// <summary>
        /// Manual Dispose OnDestroy
        /// </summary>
        public void OnDestroy()
        {
#if UNITY_EDITOR
            EventManager.RemoveHandler(EventType.YandexClientCreated, SpeechKitClientCallback);            
#endif
            SoundDictionary.Clear();
            _awaitInvoke = false;
            StopAllCoroutines();
        }
        
        private void DownloadSounds(IEnumerable<string> texts, string resourcesSubPath, bool needCashSpeech = true)
        {
            foreach (var text in texts)
            {
                var fullResourceName = $"{resourcesSubPath}\\{text.GetValidPathString()}";
                fullResourceName = fullResourceName.Replace('\\', '/');
                
                var defaultSoundType = text.GetEnumElementByName<DefaultSoundType>();
                if(SoundDictionary.ContainsKey(defaultSoundType))
                    continue;

                void SaveToDictionary(AsyncOperation asyncOperation)
                {
                    var asset = ((ResourceRequest) asyncOperation).asset;
                    
                    if (!asyncOperation.isDone || asset == null)
                    {
                        if (needCashSpeech)
                            _awaitGeneration.Add(text);
                        return;
                    }
                    
                    if(SoundDictionary.ContainsKey(defaultSoundType))
                        return;

                    SoundDictionary.Add(defaultSoundType, (AudioClip) asset);
                    EventManager.RaiseEvent(EventType.DefaultSoundLoaded, defaultSoundType);
                }
                
                var request = Resources.LoadAsync(fullResourceName);
                request.completed += SaveToDictionary;
            }
        }
        
        /// <summary>
        /// Загрузка звука с диска в словарь
        /// <param name="cashDisabled">Указывает, необходимо ли кидать в очередь ожидания те, что еще не загружены, или же их нуэно удалить из очереди</param>>
        /// </summary>
        // private void DownloadSounds(IEnumerable<string> texts, string path, bool cashDisabled = true)
        // {
        //     foreach (var text in texts)
        //     {
        //         var fullPath = $"{Path.Combine(path, text.GetValidPathString())}.wav";
        //         if (File.Exists(fullPath))
        //         {
        //             var defaultSoundType = text.GetEnumElementByName<DefaultSoundType>();
        //             
        //             if (SoundDictionary.ContainsKey(defaultSoundType))
        //                 continue;
        //
        //             void SaveToDictionary(AudioClip clip)
        //             {
        //                 if (SoundDictionary.ContainsKey(defaultSoundType))
        //                     return;
        //
        //                 SoundDictionary.Add(defaultSoundType, clip);
        //                 EventManager.RaiseEvent(EventType.DefaultSoundLoaded, defaultSoundType);
        //                 
        //                 if (!cashDisabled)
        //                     _awaitGeneration.Remove(text);
        //             }
        //
        //             StartCoroutine(SoundManger.LoadClip(fullPath, SaveToDictionary));
        //         }
        //         else if (cashDisabled)
        //             _awaitGeneration.Add(text);
        //     }
        // }

#if UNITY_EDITOR
        /// <summary>
        /// Загрузка с Yandex.SpeechKit. Ожидает, что все что можно было, загружено, и что очередб не пустая 
        /// </summary>
        private IEnumerator DownloadSoundsFromInternet()
        {
            yield return new WaitUntil(() => _componentsInitialized);
            
            while (_awaitInvoke)
            {
                yield return new WaitUntil(_awaitGeneration.Any);
                
                void CallBackDownload()
                    => DownloadSounds(_awaitGeneration.ToArray(), AudioParams.DEFAULT_SOUNDS_SUB_PATH, false);

                ThreadManager.AsyncExecute(
                    SoundManger.GenerateSounds,
                    CallBackDownload,
                    _awaitGeneration.ToArray(),
                    AudioParams.DefaultExternalOptions,
                    AudioParams.DefaultSoundsGenerateFolder);
                
                _awaitGeneration.Clear();
            }
        }
#endif
        #endregion
    }
}