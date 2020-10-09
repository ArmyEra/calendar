using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Audio.CashedSounds.Default.Utils;
using Core;
using SpeechKitApi.Models;
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
        
        private bool _awaitInvoke = true;

        /// <summary>
        /// Инициализация. Запускет загрузку существующих айдио в словарь
        /// </summary>
        public void Initialize()
        {
#if UNITY_EDITOR
            EventManager.AddHandler(EventType.YandexClientCreated, SpeechKitClientCallback);
#endif

            var defaultTexts = Enum.GetValues(typeof(DefaultSoundType))
                .Cast<DefaultSoundType>();
            DownloadSoundsFromResources(defaultTexts, AudioParams.DEFAULT_SOUNDS_SUB_PATH);
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
                    AudioParams.DefaultSoundsFolder));
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
        
        /// <summary>
        /// Вызывается один раз для загрузки звука из Resources 
        /// </summary>
        private void DownloadSoundsFromResources(IEnumerable<DefaultSoundType> soundTypes, string resourcesSubPath)
        {
            foreach (var soundType in soundTypes)
            {
                var text = soundType.GetEnumString();
                
                var fullResourceName = $"{resourcesSubPath}\\{text.GetValidPathString()}";
                fullResourceName = fullResourceName.Replace('\\', '/');
                
                if(SoundDictionary.ContainsKey(soundType))
                    continue;

                void SaveToDictionary(AsyncOperation asyncOperation)
                {
                    var asset = ((ResourceRequest) asyncOperation).asset;
                    
                    if (!asyncOperation.isDone || asset == null)
                    {
                        _awaitGeneration.Add(text);
                        return;
                    }
                    
                    if(SoundDictionary.ContainsKey(soundType))
                        return;

                    SoundDictionary.Add(soundType, (AudioClip) asset);
                    EventManager.RaiseEvent(EventType.DefaultSoundLoaded, soundType);
                }
                
                var request = Resources.LoadAsync(fullResourceName);
                request.completed += SaveToDictionary;
            }
        }
        
        // /// <summary>
        // /// Вызывается каждый раз после создания Yandex.SpeechKit и сохранения на диск
        // /// </summary>
        // private void DownloadSoundsFromDisk(IEnumerable<string> texts, string path)
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
        //             }
        //
        //             StartCoroutine(SoundManger.LoadClip(fullPath, SaveToDictionary));
        //         }
        //     }
        // }

#if UNITY_EDITOR
        /// <summary>
        /// Загрузка с Yandex.SpeechKit. Ожидает, что все что можно было, загружено, и что очередь не пустая 
        /// </summary>
        private IEnumerator DownloadSoundsFromInternet(SynthesisExternalOptions externalOptions, string saveDirectoryPath)
        {
            while (_awaitInvoke)
            {
                yield return new WaitUntil(_awaitGeneration.Any);

                var textsToGenerate = _awaitGeneration.ToArray();
                void CallBackDownload()
                {
                    foreach (var text in textsToGenerate)
                        Debug.Log($"Sound \"{text}\" downloaded from internet");
                }

                ThreadManager.AsyncExecute(
                    SoundManger.GenerateSounds,
                    CallBackDownload,
                    textsToGenerate,
                    externalOptions,
                    saveDirectoryPath);
                
                _awaitGeneration.Clear();
            }
        }
#endif
    }
}